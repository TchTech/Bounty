using Godot;
using System;
using System.Diagnostics;

public class Player : Playable
{
	private const int jump_acceleration_default = 800;
	private CPUParticles2D jetpack_particles;
	private PackedScene bulletScene;
	private PackedScene bombScene;
	private int last_direction = 1;
	private AnimatedSprite animatedSprite;
	private ProgressBar progBarFuel;
	private ProgressBar progBarHealth;
	private int jump_acceleration = jump_acceleration_default;
	public int money = 0;
	private int speed = 500;
	private int gravity = 9000;
	private float friction = .2f;
	private float acceleration = .35f;
	public double fuel = 100;
	private bool is_stunned = false;
	public bool Stun{get=>is_stunned; set=>is_stunned=value;}
	private bool is_shot = false; 
	private bool is_jetpack_rocket = false;
	private Stopwatch jetpack_timer;
	private Stopwatch stand_timer;
	private Stopwatch shot_timer;
	private bool is_flaming = false;
	private Stopwatch bomb_timer;
	private AudioStreamPlayer2D blasterSound;
	private AudioStreamPlayer2D jetpackSound;
	private AudioStreamPlayer2D JetpackFullyFunctional;
	private Sprite bombImage;
	private Sprite miniRocketImage;
	private Timer miniRocketTimer;
	private Timer flameTimer;
	private Timer RocketTimer;
	private FlameArea flame;
	private int flameFuel;
	public override void _Ready()
	{
		
		bulletScene = GD.Load<PackedScene>("res://Bullet.tscn");
		bombScene = GD.Load<PackedScene>("res://Bomb.tscn");
		stand_timer = new Stopwatch();
		jetpack_timer = new Stopwatch();
		shot_timer = new Stopwatch();
		bomb_timer = new Stopwatch();
		jetpack_particles = GetNode<CPUParticles2D>("CPUParticles2D");
		animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		progBarFuel = GetNode<ProgressBar>("Fuel");
		progBarHealth = GetNode<ProgressBar>("Health");
		blasterSound = GetNode<AudioStreamPlayer2D>("BlasterSound");
		jetpackSound = GetNode<AudioStreamPlayer2D>("JetpackSound");
		JetpackFullyFunctional = GetNode<AudioStreamPlayer2D>("JetpackFullyFunctional");
		bombImage = GetNode<Sprite>("BombImage");
		miniRocketImage = GetNode<Sprite>("MiniRocketImage");
		progBarFuel.Value = fuel;
		progBarHealth.Value = health;
		jetpack_particles.Visible = false;
		health = 100;
		flameFuel = 100;
		maxHealth = health;
		miniRocketTimer = GetNode<Timer>("MiniRocketTimer");
		RocketTimer = GetNode<Timer>("RocketTimer");
		flameTimer = GetNode<Timer>("FlameTimer");
		flameTimer.Connect("timeout", this, nameof(turnOffFlame));
	}

private void turnOffFlame(){
	flame.QueueFree();
	Stun = false;
	is_flaming = false;
	GetNode<AudioStreamPlayer2D>("FlameSound").Stop();
}
public override void _Process(float delta)
 {
	 Vector2 velocity = new Vector2();
	 int direction = 0;
	 if(!is_shot){
		if(Input.IsActionPressed("ui_left")){
			direction -= 1;
			last_direction = -1;
		}else if(Input.IsActionPressed("ui_right")){
			direction += 1;
			last_direction = 1;
		}
		if(direction != 0){
			velocity.x = Mathf.Lerp(velocity.x, direction * speed, acceleration);
		}else{
			velocity.x = Mathf.Lerp(velocity.x, 0, friction);
		}
		if(Input.IsActionPressed("jump") && fuel>0){
				jetpack_timer.Reset();
				jetpack_timer.Stop();
				velocity.y -= jump_acceleration;
				jetpack_timer.Start();
				if (jump_acceleration>0){
					jump_acceleration -= jump_acceleration/20;
				}
				if(fuel>0){fuel -= 1;}
		}else{
				velocity.y += gravity * delta;
		}
		if(IsOnFloor()){
			jump_acceleration = jump_acceleration_default;
		}
		if(jetpack_timer.Elapsed.TotalMilliseconds>3000 && fuel == 0){
			fuel = 10;
		}else if(jetpack_timer.Elapsed.TotalMilliseconds>3000 && fuel<100){
			fuel += 1;
			if(fuel == 100){
				JetpackFullyFunctional.Play();
			}
		}
	 }
	if (Input.IsActionPressed("bomb") && !bomb_timer.IsRunning) {
			Bomb bomb = (Bomb)bombScene.Instance();
			bomb.Position = new Vector2(animatedSprite.Position.x + (40 * last_direction), animatedSprite.Position.y+2);
			if (last_direction == 1) bomb.Rotation = Mathf.Deg2Rad(0);
			else bomb.Rotation = Mathf.Deg2Rad(180);
			this.AddChild(bomb);
			bomb.LaunchBomb();
			bomb_timer.Start();
	}else if (bomb_timer.Elapsed.Seconds > 5)
		{
			bomb_timer.Reset();
			bomb_timer.Stop();
		}
	if(bomb_timer.IsRunning){
		bombImage.Texture = GD.Load<Texture>("res://sprites/bomb-disabled-icon.png");
	}else{
		bombImage.Texture = GD.Load<Texture>("res://sprites/bomb-icon.png");
	}

	if(Input.IsActionPressed("mini_rocket") && miniRocketTimer.TimeLeft==0){
		var bodies_list = GetNode<Area2D>("MiniRocketArea").GetOverlappingBodies();
		for(int i = 0; i<bodies_list.Count; i++){
			if(bodies_list[i] is Playable && !(bodies_list[i] is Player)){
				var goal = bodies_list[i] as Playable;
				var miniRocketParticles = (CPUParticles2D)GD.Load<PackedScene>("res://MiniRocketParticles.tscn").Instance();
				goal.AddChild(miniRocketParticles);
				miniRocketParticles.OneShot = true;
				miniRocketParticles.Emitting = true;
				goal.Hurt(50);
			}
		}
		miniRocketTimer.Start();
	}
	if(miniRocketTimer.TimeLeft==0){
		miniRocketImage.Texture = GD.Load<Texture>("res://sprites/rocket-image.png");
	}else{
		miniRocketImage.Texture = GD.Load<Texture>("res://sprites/rocket-disabled-image.png");
	}
	if(Input.IsActionPressed("shot") && shot_timer.Elapsed.Milliseconds==0){
		 
		 Bullet bullet = (Bullet)bulletScene.Instance();
		 bullet.Position = new Vector2(animatedSprite.Position.x+(35*last_direction), animatedSprite.Position.y-1-(Convert.ToInt32(!IsOnFloor()) * 10));
		 if(last_direction==1) bullet.Rotation = Mathf.Deg2Rad(0);
		 else bullet.Rotation = Mathf.Deg2Rad(180);
		 this.AddChild(bullet);
		 bullet.LaunchBullet();
		 blasterSound.Play();
		 shot_timer.Start();
		 is_shot = true;
		 if(new Random().Next(0, 10) == 1){
			GetNode<AudioStreamPlayer2D>("comeCloserSound").Play();
		 }
	 }else if(shot_timer.Elapsed.Milliseconds>400){
		 shot_timer.Reset();
		 shot_timer.Stop();
		 is_shot = false;
	 }
	if(Input.IsActionPressed("flame") && !is_flaming){
		flame = (FlameArea)GD.Load<PackedScene>("res://Flame.tscn").Instance();
		if(last_direction == -1){
			flame.GetNode<AnimatedSprite>("AnimatedSprite").FlipH = false;
			flame.GetNode<CPUParticles2D>("CPUParticles2D").Position = new Vector2(74, 2);
			flame.GetNode<CPUParticles2D>("CPUParticles2D").Rotation = Mathf.Deg2Rad(180);
		}else{
			flame.GetNode<AnimatedSprite>("AnimatedSprite").FlipH = true;
		}
		flame.Position = new Vector2(animatedSprite.Position.x+(95*last_direction), animatedSprite.Position.y-4);
		AddChild(flame);
		Stun = true;
		flameTimer.Start();
		is_flaming = true;
		if(!GetNode<AudioStreamPlayer2D>("FlameSound").Playing) GetNode<AudioStreamPlayer2D>("FlameSound").Play();
	}
	
	if(Input.IsActionPressed("jetpack_rocket") && RocketTimer.TimeLeft == 0){
		JetpackRocket rocket = (JetpackRocket)GD.Load<PackedScene>("res://JetpackRocket.tscn").Instance();
		rocket.Position = new Vector2(animatedSprite.Position.x+(35*last_direction), animatedSprite.Position.y-1-(Convert.ToInt32(!IsOnFloor()) * 10));
		if(last_direction==1) rocket.Rotation = Mathf.Deg2Rad(0);
		else rocket.Rotation = Mathf.Deg2Rad(180);
		this.AddChild(rocket);
		rocket.LaunchBullet();
		RocketTimer.Start();
	}
	if(last_direction == -1){
		animatedSprite.FlipH = true;
	}else{
		animatedSprite.FlipH = false;
	}
	if(!IsOnFloor()){
		if(is_shot){
			animatedSprite.Play("FlyShot");
		}else{
			animatedSprite.Play("Fly");
		}
		if(last_direction == 1){
			jetpack_particles.RotationDegrees = 10;
		}else{
			jetpack_particles.RotationDegrees = -10;
		}
		jetpack_particles.Visible = true;
		jetpackSound.VolumeDb = (float)fuel/20;
		if(!jetpackSound.Playing){
			jetpackSound.Play();
		}
		stand_timer.Reset();
		stand_timer.Stop();
	}else if(Input.IsActionPressed("flame")){
		animatedSprite.Play("Fier");
		jetpack_particles.Visible = false;
		jetpackSound.Stop();
	}else if(IsOnFloor()&&direction!=0){
		animatedSprite.Play("Run");
		stand_timer.Reset();
		stand_timer.Stop();
		jetpack_particles.Visible = false;
		jetpackSound.Stop();
	}else{
		if(Input.IsActionPressed("mini_rocket") || Input.IsActionPressed("bomb")){
			animatedSprite.Play("Fier");
		}else if(stand_timer.Elapsed.Seconds>5 && !is_shot){
			animatedSprite.Play("Stand");
		}else if(is_shot){
			animatedSprite.Play("Shot");
		}else{
			animatedSprite.Play("Idle");
		}
		if(stand_timer.Elapsed.Milliseconds == 0){
			stand_timer.Start();
		}
		jetpack_particles.Visible = false;
		jetpackSound.Stop();
	}
	progBarFuel.Value = fuel;
	progBarHealth.Value = health;
	if(is_stunned){
		velocity = Vector2.Zero;
		velocity.y += gravity * delta * 10;
	}
	GetNode<Label>("Moneys").Text = "Credits: " + Convert.ToString(money);
	MoveAndSlide(velocity, Vector2.Up);
 }
	public override void Hurt(int damage)
	{
		base.Hurt(damage);
		GetNode<AudioStreamPlayer2D>("damagedSound").Play();
	}
}
