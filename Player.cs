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
	private int speed = 500;
	private int gravity = 9000;
	private float friction = .2f;
	private float acceleration = .35f;
	private double fuel = 100;
	private bool is_stunned = false;
	public bool Stun{get=>is_stunned; set=>is_stunned=value;}
	private bool is_shot = false; 
	private Stopwatch jetpack_timer;
	private Stopwatch stand_timer;
	private Stopwatch shot_timer;
	private Stopwatch bomb_timer;
	private AudioStreamPlayer2D blasterSound;
	private AudioStreamPlayer2D jetpackSound;
	private AudioStreamPlayer2D JetpackFullyFunctional;
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
		progBarFuel.Value = fuel;
		progBarHealth.Value = health;
		jetpack_particles.Visible = false;
		health = 100;
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
		if(jetpack_timer.Elapsed.TotalMilliseconds>3000 && fuel == 0){
			jump_acceleration = jump_acceleration_default;
			fuel = 20;
		}else if(jetpack_timer.Elapsed.TotalMilliseconds>3000 && fuel<100){
			jump_acceleration = jump_acceleration_default;
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
		if(!jetpackSound.Playing){
			jetpackSound.Play();
		}
		stand_timer.Reset();
		stand_timer.Stop();
	}else if(IsOnFloor()&&direction!=0){
		animatedSprite.Play("Run");
		stand_timer.Reset();
		stand_timer.Stop();
		jetpack_particles.Visible = false;
		jetpackSound.Stop();
	}else{
		if(stand_timer.Elapsed.Seconds>5 && !is_shot){
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
	MoveAndSlide(velocity, Vector2.Up);
 }
    public override void Hurt(int damage)
    {
        base.Hurt(damage);
		GetNode<AudioStreamPlayer2D>("damagedSound").Play();
    }
}
