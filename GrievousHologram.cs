using Godot;
using System;

public class GrievousHologram : Playable
{
	private AnimatedSprite Sprites;
	private Vector2 velocity;
	private Player player;
	private Timer spinTimer;
	private CollisionPolygon2D collisionPolygon;
	private Line2D electricity;
	private int mode = 0;
	private int dashDirection = 1;
	private int LastHealth;
	private ProgressBar HealthBar;
	private CPUParticles2D HitParticles;
	private PackedScene WinScene;
	private AudioStreamPlayer2D HitSound;
	
	private void SwitchToMode0(){
		mode = 0;
		electricity.Visible = false;
		player.Hurt(1200/Convert.ToInt32(Position.DistanceTo(player.Position)));
	}
	
	public override void _Ready()
	{
		Health = 2000;
		MaxHealth = 2000;
		Sprites = GetNode<AnimatedSprite>("AnimatedSprite");
		player = GetParent().GetNode<Player>("Player");
		spinTimer = GetNode<Timer>("SpinTimer");
		collisionPolygon = GetNode<CollisionPolygon2D>("CollisionPolygon2D");
		spinTimer.Connect("timeout", this, nameof(SwitchToMode0));
		electricity = GetNode<Line2D>("Line2D");
		LastHealth = Health;
		HealthBar = player.GetNode<ProgressBar>("BossHealth");
		HitParticles = GetNode<CPUParticles2D>("HitParticles");
		WinScene = GD.Load<PackedScene>("res://WinScene.tscn");
		HitSound = GetNode<AudioStreamPlayer2D>("HitSound");
	}
	public override void _Process(float delta)
	{
		switch(mode){
			case 0:
				
				Sprites.Play("Go");
				velocity.x = Position.DirectionTo(player.Position).x*50;
				if(Math.Abs(player.Position.x - Position.x) < 200){
					mode = 1;
					if(player.Position.x - Position.x >= 0) dashDirection = 1;
					else dashDirection  = -1;
				}
				break;
			case 1:
				Sprites.Play("Dash");
				velocity.x = 200 * dashDirection;
				if((Math.Abs(player.Position.x - Position.x) > 300) || IsOnWall())
				{
					mode = 2;
					spinTimer.Start();
				}
				break;
			case 2:
				electricity.Visible = true;
				Sprites.Play("Spin");
				velocity.x = 0;
				electricity.Points = new Vector2[2]{ToLocal(player.GlobalPosition), new Vector2(0,0)};
				if(LastHealth>Health){
					spinTimer.Stop();
					SwitchToMode0();
				}
				break;
		}
		if(LastHealth>Health){
			HitParticles.Emitting = true;
			HitSound.Play();
		}
		if(Position.DirectionTo(player.Position).x>0){
			Sprites.FlipH = true;
		}else{
			Sprites.FlipH = false;
		}
		velocity.y += 9000 * delta;
		velocity = MoveAndSlide(velocity, Vector2.Up);
		HealthBar.Value = Health;
		LastHealth = Health;
		if(Health <= 0){
			GetParent().QueueFree();
			GetParent().GetParent().AddChild((Control)WinScene.Instance());
		}
	}
}
