using Godot;
using System;

public class DarkDroid : Playable
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	private AnimatedSprite animatedSprite;
	private Player player;
	private int runSpeed = 60;
	private Vector2 velocity;
	private bool isPlayerEntered = false;
	private ProgressBar progBarHealth;
	private PackedScene bulletScene;
	private Timer shotTimer;
	private bool makeShot = true;
	private RayCast2D rcast0;
	private RayCast2D rcast1;
	public override void _Ready()
	{
		health = 300;
		progBarHealth = GetNode<ProgressBar>("Health");
		animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		animatedSprite.Play("Idle");
		bulletScene = GD.Load<PackedScene>("res://Bullet.tscn");
		shotTimer = GetNode<Timer>("ShotTimer");
		shotTimer.Connect("timeout", this, nameof(ResetShot));
		rcast0 = GetNode<RayCast2D>("RayCast2D");
		rcast1 = GetNode<RayCast2D>("RayCast2D2");
	}
	public override void _Process(float delta)
	{
		velocity = Vector2.Zero;
		if(isPlayerEntered){
			 //Position.DirectionTo(player.Position)
			if(Position.DirectionTo(player.Position).x < 0) animatedSprite.FlipH = true;
			else animatedSprite.FlipH = false;
			if((rcast0.IsColliding() && rcast1.IsColliding()) || (!rcast0.IsColliding() && Position.DirectionTo(player.Position).x>0) || (!rcast1.IsColliding() && Position.DirectionTo(player.Position).x<0)){
				animatedSprite.Play("Go");
				velocity = Position.DirectionTo(player.Position) * runSpeed;
			}else{
				animatedSprite.Play("StandShot");
			}
			if(makeShot){
				Bullet bullet = (Bullet)bulletScene.Instance();
				if(Position.DirectionTo(player.Position).x>0){
					bullet.Rotation = Mathf.Deg2Rad(0);
					bullet.Position = new Vector2(animatedSprite.Position.x+45, animatedSprite.Position.y-15);
				}else {
					bullet.Rotation = Mathf.Deg2Rad(180);
					bullet.Position = new Vector2(animatedSprite.Position.x-45, animatedSprite.Position.y-15);
				}
			 	this.AddChild(bullet);
			 	bullet.LaunchBullet(33);
				makeShot = false;
				shotTimer.Start();
			}
		}
		rcast1.ForceRaycastUpdate();
		rcast0.ForceRaycastUpdate();
		velocity.y += 15000 * delta;
		velocity = MoveAndSlide(velocity, Vector2.Up);
		progBarHealth.Value = health;
	}
	private void ResetShot(){
		makeShot = true;
	}
	private void _on_Area2D_body_entered(object body)
	{
		if(body is Player){
			player = body as Player;
			isPlayerEntered = true;
		}
	}
}
