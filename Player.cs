using Godot;
using System;
using System.Diagnostics;

public class Player : KinematicBody2D
{
	private const int jump_acceleration_default = 800;
	private PackedScene bulletScene;
	private int last_direction = 1;
	private AnimatedSprite _animatedSprite;
	private int jump_acceleration = jump_acceleration_default;
	private int speed = 500;
	private int gravity = 9000;
	private float friction = .2f;
	private float acceleration = .35f;
	private double fuel = 100;
	private bool is_shot = false; 
	private Stopwatch jetpack_timer;
	private Stopwatch stand_timer;
	private Stopwatch shot_timer;
	public override void _Ready()
	{
		
		bulletScene = GD.Load<PackedScene>("res://Bullet.tscn");
		stand_timer = new Stopwatch();
		jetpack_timer = new Stopwatch();
		shot_timer = new Stopwatch();
		_animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
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
		if(Input.IsActionPressed("jump") && jetpack_timer.Elapsed.TotalMilliseconds<1){
				velocity.y -= jump_acceleration;
				if(jump_acceleration>0){
					jump_acceleration -= jump_acceleration/20;
				}
				if(fuel>0){fuel -= 1;}
		}else{
				velocity.y += gravity * delta;
		}
		if(fuel==0){
			jetpack_timer.Start();
			if(jetpack_timer.Elapsed.TotalMilliseconds>3000){
				jetpack_timer.Reset();
				jetpack_timer.Stop();
				jump_acceleration = jump_acceleration_default;
				fuel = 100;
			}
		}
	 }
	 if(Input.IsActionPressed("shot") && IsOnFloor() && shot_timer.Elapsed.Milliseconds==0){
		 
		 Bullet bullet = (Bullet)bulletScene.Instance();
		 bullet.Position = new Vector2(_animatedSprite.Position.x+(35*last_direction), _animatedSprite.Position.y-1);
		 if(last_direction==1) bullet.Rotation = Mathf.Deg2Rad(0);
		 else bullet.Rotation = Mathf.Deg2Rad(180);
		 this.AddChild(bullet);
		 bullet.LaunchBullet();
		 shot_timer.Start();
		 is_shot = true;
	 }else if(shot_timer.Elapsed.Milliseconds>400){
		 shot_timer.Reset();
		 shot_timer.Stop();
		 is_shot = false;
	 }
	if(last_direction == -1){
		_animatedSprite.FlipH = true;
	}else{
		_animatedSprite.FlipH = false;
	}
	if(!IsOnFloor()){
		_animatedSprite.Play("Fly");
		stand_timer.Reset();
		stand_timer.Stop();
	}else if(IsOnFloor()&&direction!=0){
		_animatedSprite.Play("Run");
		stand_timer.Reset();
		stand_timer.Stop();
	}else{
		if(stand_timer.Elapsed.Seconds>5 && !is_shot){
			_animatedSprite.Play("Stand");
		}else if(is_shot){
			_animatedSprite.Play("Shot");
		}else{
			_animatedSprite.Play("Idle");
		}
		if(stand_timer.Elapsed.Milliseconds == 0){
			stand_timer.Start();
		}
	}
	
	MoveAndSlide(velocity, Vector2.Up);
 }
}
