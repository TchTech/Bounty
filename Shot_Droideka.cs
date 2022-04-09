using Godot;
using System;

public class Shot_Droideka : Playable
{
	
	private Player player;
	private AnimatedSprite animatedSprite;
	private int run_speed = 120;
	private ProgressBar progBarHealth;
	private bool is_stun = false;
	public bool Stun{get=>is_stun;set=>is_stun=value;}
	private bool is_player_in_area = false;
	private bool allow_attack = true;
	private Vector2 velocity;
	private bool is_protecting = false;
	private Timer protect_timer;
	private Timer turn_off_protect_timer;
	private PackedScene bulletScene;
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		progBarHealth = GetNode<ProgressBar>("Health");
		player = new Player();
		animatedSprite.Play();
		health = 100;
		progBarHealth.Visible = false;
		maxHealth = health;
		protect_timer = GetNode<Timer>("ProtectTimer");
		turn_off_protect_timer = GetNode<Timer>("TurnOffProtectTimer");
		protect_timer.Connect("timeout", this, nameof(Protect));
		turn_off_protect_timer.Connect("timeout", this, nameof(turnOffProtect));
		protect_timer.Start();
		bulletScene = GD.Load<PackedScene>("res://Bullet.tscn");
	}
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public void Protect(){
		GetNode<CPUParticles2D>("ShieldParticles").Emitting = true;
		is_protecting = true;
		turn_off_protect_timer.Start();
	}
	public void turnOffProtect(){
		GetNode<CPUParticles2D>("ShieldParticles").Emitting = false;
		is_protecting = false;
		protect_timer.Start();
	}
	public override void _Process(float delta)
	{
		velocity = Vector2.Zero;
		if(is_player_in_area && !Stun){
			if((Math.Abs(player.Position.x - Position.x) > 250 || Math.Abs(player.Position.y - Position.y) > 10) && !Stun){
				velocity = Position.DirectionTo(player.Position) * run_speed;
				animatedSprite.Play("Spin");
			}else if(allow_attack && !Stun){
				Timer timer = new Timer();
				this.AddChild(timer);
				timer.WaitTime = 0.2f;
				timer.OneShot = true;
				timer.Connect("timeout", this, nameof(Attack));
				timer.Start();
			}
			if(Position.DirectionTo(player.Position).x<0){
				animatedSprite.FlipH = false;
			}else{
				animatedSprite.FlipH = true;
			}
		}
		if(velocity.x == 0 && allow_attack && !Stun){
			animatedSprite.Play("Idle");
		}
		if(is_stun){
			velocity = Vector2.Zero;
		}
		if(health < 100){
			progBarHealth.Visible = true;
		}
		velocity.y += 9000 * delta;
		velocity = MoveAndSlide(velocity, Vector2.Up);
		progBarHealth.Value = health;
	}
	private void _on_Area2D_body_entered(object body)
	{
		if(body is Player){
			player = body as Player;
			is_player_in_area = true;
		}
	}
	private void _on_Area2D_body_exited(object body)
	{
		if(body is Player){
			is_player_in_area = false;
		}
	}
	public override void Die(){
		GetNode<CPUParticles2D>("DiedParticles").Emitting = true;
		GetNode<AudioStreamPlayer2D>("DiedSound").Play();
		Stun = true;
		Timer timer = new Timer();
		this.AddChild(timer);
		timer.WaitTime = 2.0f;
		timer.OneShot = true;
		timer.Connect("timeout", this, nameof(Destroy));
		timer.Start();
	}
	private void Destroy()
	{
		var scene = GetParent();
		PackedScene prize_scene = GD.Load<PackedScene>(new string[]{"res://Coin.tscn", "res://Heart.tscn"}[new Random().Next(0,2)]);
		Node2D prize = (Node2D)prize_scene.Instance();
		prize.Position = Position;
		scene.AddChild(prize);
		QueueFree();
	}
	public void Attack(){
		if(allow_attack && Math.Abs(player.Position.y - Position.y)<10 && Math.Abs(player.Position.x - Position.x) < 250){
			Bullet bullet = (Bullet)bulletScene.Instance();
			if(Position.DirectionTo(player.Position).x>0){
				bullet.Rotation = Mathf.Deg2Rad(0);
				bullet.Position = new Vector2(animatedSprite.Position.x+55, animatedSprite.Position.y);
			}else {
				bullet.Rotation = Mathf.Deg2Rad(180);
				bullet.Position = new Vector2(animatedSprite.Position.x-55, animatedSprite.Position.y);
			}
		 	this.AddChild(bullet);
		 	bullet.LaunchBullet(10);
			GetNode<AudioStreamPlayer2D>("ShotSound").Play();
			animatedSprite.Play("Attack");
		}
		Timer timer = new Timer();
		this.AddChild(timer);
		timer.WaitTime = 0.5f;
		timer.OneShot = true;
		timer.Connect("timeout", this, nameof(SwitchAttack));
		timer.Start();
		allow_attack = false;
	}
	public override void Hurt(int damage){
		if(!is_protecting){
			base.Hurt(damage);
			Stun = true;
			Timer timer = new Timer();
			this.AddChild(timer);
			timer.WaitTime = 0.5f;
			timer.OneShot = true;
			timer.Connect("timeout", this, nameof(turnOffStun));
			timer.Start();
		}
	}
	private void SwitchAttack(){
		allow_attack = true;
	}
	public void turnOffStun(){
		Stun = false;
	}
}
