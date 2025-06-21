using Godot;
using System;

public class VaderHologramme : Playable
{
	private int run_speed = 100;
	private Vector2 velocity;
	private Player player;
	private PackedScene deadScene;
	private AnimatedSprite animatedSprite;
	private CPUParticles2D redParticles;
	private ProgressBar progBarHealth;
	private bool allow_attack = true;
	private bool is_stun = false;
	private bool player_entered = false;
	public bool Stun{get=>is_stun;set=>is_stun=value;}
	private CPUParticles2D particles;
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		redParticles = GetNode<CPUParticles2D>("RedParticles");
		deadScene = GD.Load<PackedScene>("res://DeadParticles.tscn");
		redParticles.Visible = false;
		progBarHealth = GetNode<ProgressBar>("Health");
		health = 100;
		maxHealth = health;
		progBarHealth.Visible = false;
	}
	public override void _Process(float delta){
		velocity = Vector2.Zero;
		try{
		if(Math.Abs(player.Position.x - Position.x) > 40 && !Stun){
			velocity = Position.DirectionTo(player.Position) * run_speed;
			animatedSprite.Play("Go");
		}else if(allow_attack && !Stun){
			Timer timer = new Timer();
			this.AddChild(timer);
			timer.WaitTime = 0.5f;
			timer.OneShot = true;
			timer.Connect("timeout", this, nameof(Attack));
			timer.Start();
		}
		if(Position.DirectionTo(player.Position).x<0){
			animatedSprite.FlipH = false;
		}else{
			animatedSprite.FlipH = true;
		}
		if(velocity.x == 0 && allow_attack && !Stun){
			animatedSprite.Play("Idle");
		}
		}catch{
			Console.WriteLine("fff");
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
	public void _on_Area2D_body_entered(object other){
		if(other is Player){
			player = other as Player;
			player_entered = true;
		}
		//Console.WriteLine(Math.Abs(player.Position.x - Position.x));
	}
	public void _on_Area2D_body_exited(object other){
		try{
			if(!player.Stun && other is Player){
				GetNode<AudioStreamPlayer2D>("noescape").Play();
				player.Stun = true;
				animatedSprite.Play("Stun");
				Timer timer = new Timer();
				this.AddChild(timer);
				timer.Connect("timeout", this, nameof(turnOffStun));
				timer.WaitTime = 2.5f;
				timer.OneShot = true;
				timer.Start();
			}
		}catch{

		}
	}
	public void turnOffStun(){
		try{
		player.Stun = false;
		Stun = false;
		}catch{

		}
	}
	public override void Die(){
		animatedSprite.Visible = false;
		redParticles.Visible = false;
		GetNode<CPUParticles2D>("CPUParticles2D").QueueFree();
		GetNode<CPUParticles2D>("RedParticles").QueueFree();
		GetNode<Area2D>("Area2D").QueueFree();
		particles = (CPUParticles2D)deadScene.Instance();
		particles.Position = animatedSprite.Position;
		particles.Visible = true;
		Stun = true;
		allow_attack = false;
		this.AddChild(particles);
		Timer timer = new Timer();
		this.AddChild(timer);
		timer.WaitTime = 2.5f;
		timer.OneShot = true;
		timer.Connect("timeout", this, nameof(DestroyParticles));
		timer.Start();
		GetNode<AudioStreamPlayer2D>("deadsound").Play();
	}
	public void DestroyParticles(){
		QueueFree();
		try{
		player.Stun = false;
		}catch{
			
		}
		//QueueFree();
	}
	public void Attack(){
		if(allow_attack && Math.Abs(player.Position.y - Position.y)<40 && Math.Abs(player.Position.x - Position.x) < 40){
			player.Hurt(30);
			animatedSprite.Play("Attack");
			redParticles.Visible = true;
			GetNode<AudioStreamPlayer2D>("lightsabersound").Play();
		}
		Timer timer = new Timer();
		this.AddChild(timer);
		timer.WaitTime = 1.0f;
		timer.OneShot = true;
		timer.Connect("timeout", this, nameof(SwitchAttack));
		timer.Start();
		allow_attack = false;
	}
	public override void Hurt(int damage){
		base.Hurt(damage);
		animatedSprite.Play("Hurt");
		Stun = true;
		Timer timer = new Timer();
		this.AddChild(timer);
		timer.WaitTime = 0.5f;
		timer.OneShot = true;
		timer.Connect("timeout", this, nameof(turnOffStun));
		timer.Start();
	}
	private void SwitchAttack(){
		allow_attack = true;
	}
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
