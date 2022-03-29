using Godot;
using System;
using System.Diagnostics;

public class Player : Playable
{
	private const int JumpAccelerationDefault = 800;
	private CPUParticles2D JetpackParticles;
	private PackedScene BulletScene;
	private PackedScene BombScene;
	private int LastDirection = 1;
	private AnimatedSprite AnimatedSprite;
	private ProgressBar ProgBarFuel;
	private ProgressBar ProgBarHealth;
	private int JumpAcceleration = JumpAccelerationDefault;
	public int Money = 0;
	private int Speed = 500;
	private int Gravity = 9000;
	private float Friction = .2f;
	private float Acceleration = .35f;
	public double Fuel = 100;
	private bool IsStunned = false;
	public bool Stun{get=>IsStunned; set=>IsStunned=value;}
	private bool IsShot = false;
	private Stopwatch JetpackTimer;
	private Stopwatch StandTimer;
	private Stopwatch ShotTimer;
	private bool IsFlaming = false;
	private Stopwatch BombTimer;
	private AudioStreamPlayer2D BlasterSound;
	private AudioStreamPlayer2D JetpackSound;
	private AudioStreamPlayer2D JetpackFullyFunctional;
	private Sprite BombImage;
	private Sprite MiniRocketImage;
	private Sprite RocketImage;
	private Timer MiniRocketTimer;
	private Timer FlameTimer;
	private Timer RocketTimer;
	private FlameArea Flame;
	public override void _Ready()
	{
		
		BulletScene = GD.Load<PackedScene>("res://Bullet.tscn");
		BombScene = GD.Load<PackedScene>("res://Bomb.tscn");
		StandTimer = new Stopwatch();
		JetpackTimer = new Stopwatch();
		ShotTimer = new Stopwatch();
		BombTimer = new Stopwatch();
		JetpackParticles = GetNode<CPUParticles2D>("CPUParticles2D");
		AnimatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		ProgBarFuel = GetNode<ProgressBar>("Fuel");
		ProgBarHealth = GetNode<ProgressBar>("Health");
		BlasterSound = GetNode<AudioStreamPlayer2D>("BlasterSound");
		JetpackSound = GetNode<AudioStreamPlayer2D>("JetpackSound");
		JetpackFullyFunctional = GetNode<AudioStreamPlayer2D>("JetpackFullyFunctional");
		BombImage = GetNode<Sprite>("BombImage");
		MiniRocketImage = GetNode<Sprite>("MiniRocketImage");
		RocketImage = GetNode<Sprite>("RocketImage");
		ProgBarFuel.Value = Fuel;
		ProgBarHealth.Value = health;
		JetpackParticles.Visible = false;
		health = 100;
		maxHealth = health;
		MiniRocketTimer = GetNode<Timer>("MiniRocketTimer");
		RocketTimer = GetNode<Timer>("RocketTimer");
		FlameTimer = GetNode<Timer>("FlameTimer");
		FlameTimer.Connect("timeout", this, nameof(turnOffFlame));
	}

private void turnOffFlame(){
	Flame.QueueFree();
	Stun = false;
	IsFlaming = false;
	GetNode<AudioStreamPlayer2D>("FlameSound").Stop();
}
public void AddFuel(int addfuel){
	if(addfuel + Fuel > 100) Fuel = 100;
	else Fuel += addfuel;
}
public override void _Process(float delta)
 {
	 Vector2 velocity = new Vector2();
	 int direction = 0;
	 if(!IsShot){
		if(Input.IsActionPressed("ui_left")){
			direction -= 1;
			LastDirection = -1;
		}else if(Input.IsActionPressed("ui_right")){
			direction += 1;
			LastDirection = 1;
		}
		if(direction != 0){
			velocity.x = Mathf.Lerp(velocity.x, direction * Speed, Acceleration);
		}else{
			velocity.x = Mathf.Lerp(velocity.x, 0, Friction);
		}
		if(Input.IsActionPressed("jump") && Fuel>0){
				JetpackTimer.Reset();
				JetpackTimer.Stop();
				velocity.y -= JumpAcceleration;
				JetpackTimer.Start();
				if (JumpAcceleration>0){
					JumpAcceleration -= JumpAcceleration/20;
				}
				if(Fuel>0){Fuel -= 1;}
		}else{
				velocity.y += Gravity * delta;
		}
		if(IsOnFloor()){
			JumpAcceleration = JumpAccelerationDefault;
		}
		if(JetpackTimer.Elapsed.TotalMilliseconds>3000 && Fuel == 0){
			Fuel = 10;
		}else if(JetpackTimer.Elapsed.TotalMilliseconds>3000 && Fuel<100){
			Fuel += 1;
			if(Fuel == 100){
				JetpackFullyFunctional.Play();
			}
		}
	 }
	if (Input.IsActionPressed("bomb") && !BombTimer.IsRunning) {
			Bomb bomb = (Bomb)BombScene.Instance();
			bomb.Position = new Vector2(AnimatedSprite.Position.x + (40 * LastDirection), AnimatedSprite.Position.y+2);
			if (LastDirection == 1) bomb.Rotation = Mathf.Deg2Rad(0);
			else bomb.Rotation = Mathf.Deg2Rad(180);
			this.AddChild(bomb);
			bomb.LaunchBomb();
			BombTimer.Start();
	}else if (BombTimer.Elapsed.Seconds > 5)
		{
			BombTimer.Reset();
			BombTimer.Stop();
		}
	if(BombTimer.IsRunning){
		BombImage.Texture = GD.Load<Texture>("res://sprites/bomb-disabled-icon.png");
	}else{
		BombImage.Texture = GD.Load<Texture>("res://sprites/bomb-icon.png");
	}

	if(Input.IsActionPressed("mini_rocket") && MiniRocketTimer.TimeLeft==0){
		var bodies_list = GetNode<Area2D>("MiniRocketArea").GetOverlappingBodies();
		for(int i = 0; i<bodies_list.Count; i++){
			if(bodies_list[i] is Playable && !(bodies_list[i] is Player)){
				var goal = bodies_list[i] as Playable;
				var miniRocketParticles = (CPUParticles2D)GD.Load<PackedScene>("res://MiniRocketParticles.tscn").Instance();
				goal.AddChild(miniRocketParticles);
				miniRocketParticles.OneShot = true;
				miniRocketParticles.Emitting = true;
				goal.Hurt(60);
			}
		}
		MiniRocketTimer.Start();
	}
	if(MiniRocketTimer.TimeLeft==0){
		MiniRocketImage.Texture = GD.Load<Texture>("res://sprites/rocket-image.png");
	}else{
		MiniRocketImage.Texture = GD.Load<Texture>("res://sprites/rocket-disabled-image.png");
	}
	if(RocketTimer.TimeLeft==0){
		RocketImage.Texture = GD.Load<Texture>("res://sprites/rocket-icon.png");
	}else{
		RocketImage.Texture = GD.Load<Texture>("res://sprites/rocket-disabled-icon.png");
	}
	if(Input.IsActionPressed("shot") && ShotTimer.Elapsed.Milliseconds==0){
		 
		 Bullet bullet = (Bullet)BulletScene.Instance();
		 bullet.Position = new Vector2(AnimatedSprite.Position.x+(35*LastDirection), AnimatedSprite.Position.y-1-(Convert.ToInt32(!IsOnFloor()) * 10));
		 if(LastDirection==1) bullet.Rotation = Mathf.Deg2Rad(0);
		 else bullet.Rotation = Mathf.Deg2Rad(180);
		 this.AddChild(bullet);
		 bullet.LaunchBullet();
		 BlasterSound.Play();
		 ShotTimer.Start();
		 IsShot = true;
		 if(new Random().Next(0, 10) == 1){
			GetNode<AudioStreamPlayer2D>("comeCloserSound").Play();
		 }
	 }else if(ShotTimer.Elapsed.Milliseconds>400){
		 ShotTimer.Reset();
		 ShotTimer.Stop();
		 IsShot = false;
	 }
	if(Input.IsActionPressed("flame") && !IsFlaming){
		Flame = (FlameArea)GD.Load<PackedScene>("res://Flame.tscn").Instance();
		if(LastDirection == -1){
			Flame.GetNode<AnimatedSprite>("AnimatedSprite").FlipH = false;
			Flame.GetNode<CPUParticles2D>("CPUParticles2D").Position = new Vector2(74, 2);
			Flame.GetNode<CPUParticles2D>("CPUParticles2D").Rotation = Mathf.Deg2Rad(180);
		}else{
			Flame.GetNode<AnimatedSprite>("AnimatedSprite").FlipH = true;
		}
		Flame.Position = new Vector2(AnimatedSprite.Position.x+(95*LastDirection), AnimatedSprite.Position.y-4);
		AddChild(Flame);
		Stun = true;
		FlameTimer.Start();
		IsFlaming = true;
		if(!GetNode<AudioStreamPlayer2D>("FlameSound").Playing) GetNode<AudioStreamPlayer2D>("FlameSound").Play();
	}
	
	if(Input.IsActionPressed("jetpack_rocket") && RocketTimer.TimeLeft == 0){
		JetpackRocket rocket = (JetpackRocket)GD.Load<PackedScene>("res://JetpackRocket.tscn").Instance();
		rocket.Position = new Vector2(AnimatedSprite.Position.x+(35*LastDirection), AnimatedSprite.Position.y-1-(Convert.ToInt32(!IsOnFloor()) * 10));
		if(LastDirection==1) rocket.Rotation = Mathf.Deg2Rad(0);
		else rocket.Rotation = Mathf.Deg2Rad(180);
		this.AddChild(rocket);
		rocket.LaunchBullet();
		RocketTimer.Start();
	}
	if(LastDirection == -1){
		AnimatedSprite.FlipH = true;
	}else{
		AnimatedSprite.FlipH = false;
	}
	if(!IsOnFloor()){
		if(IsShot){
			AnimatedSprite.Play("FlyShot");
		}else{
			AnimatedSprite.Play("Fly");
		}
		if(LastDirection == 1){
			JetpackParticles.RotationDegrees = 10;
		}else{
			JetpackParticles.RotationDegrees = -10;
		}
		JetpackParticles.Visible = true;
		JetpackSound.VolumeDb = (float)Fuel/20;
		if(!JetpackSound.Playing){
			JetpackSound.Play();
		}
		StandTimer.Reset();
		StandTimer.Stop();
	}else if(Input.IsActionPressed("flame")){
		AnimatedSprite.Play("Fier");
		JetpackParticles.Visible = false;
		JetpackSound.Stop();
	}else if(IsOnFloor()&&direction!=0){
		AnimatedSprite.Play("Run");
		StandTimer.Reset();
		StandTimer.Stop();
		JetpackParticles.Visible = false;
		JetpackSound.Stop();
	}else{
		if(Input.IsActionPressed("mini_rocket") || Input.IsActionPressed("bomb") || Input.IsActionPressed("jetpack_rocket")){
			AnimatedSprite.Play("Fier");
		}else if(StandTimer.Elapsed.Seconds>5 && !IsShot){
			AnimatedSprite.Play("Stand");
		}else if(IsShot){
			AnimatedSprite.Play("Shot");
		}else{
			AnimatedSprite.Play("Idle");
		}
		if(StandTimer.Elapsed.Milliseconds == 0){
			StandTimer.Start();
		}
		JetpackParticles.Visible = false;
		JetpackSound.Stop();
	}
	ProgBarFuel.Value = Fuel;
	ProgBarHealth.Value = health;
	if(IsStunned){
		velocity = Vector2.Zero;
		velocity.y += Gravity * delta * 10;
	}
	GetNode<Label>("Moneys").Text = "Credits: " + Convert.ToString(Money);
	MoveAndSlide(velocity, Vector2.Up);
 }
	public override void Hurt(int damage)
	{
		base.Hurt(damage);
		GetNode<AudioStreamPlayer2D>("damagedSound").Play();
	}
}
