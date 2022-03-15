using Godot;
using System;

public class VaderHologramme : Playable
{
	private int run_speed = 150;
	private Vector2 velocity;
	private Player player;
	private AnimatedSprite animatedSprite;
	private CPUParticles2D redParticles;
	private bool allow_go = true;
	private bool allow_attack = true;
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		redParticles = GetNode<CPUParticles2D>("RedParticles");
		redParticles.Visible = false;
		health = 100;
	}
	public override void _Process(float delta){
		velocity = Vector2.Zero;
		try{
		if(Math.Abs(player.Position.x - Position.x) > 50 && allow_go){
			velocity = Position.DirectionTo(player.Position) * run_speed;
			animatedSprite.Play("Go");
		}else if(allow_go){
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
		}catch{
			Console.WriteLine("fff");
		}
		velocity.y += 9000 * delta;
		velocity = MoveAndSlide(velocity, Vector2.Up);
	}
	public void _on_Area2D_body_entered(object other){
		if(other is Player){
			player = other as Player;
		}
		//Console.WriteLine(Math.Abs(player.Position.x - Position.x));
	}
	public void _on_Area2D_body_exited(Area2D other){
		player = new Player();
	}
	public void Die(){}
	public void Attack(){
		if(allow_attack && Math.Abs(player.Position.y - Position.y)<50 && Math.Abs(player.Position.x - Position.x) < 50){
			player.Hurt(2);
			animatedSprite.Play("Attack");
			redParticles.Visible = true;
		}
		Timer timer = new Timer();
		this.AddChild(timer);
		timer.WaitTime = 2.5f;
		timer.OneShot = true;
		timer.Connect("timeout", this, nameof(SwitchAttack));
		timer.Start();
		allow_attack = false;
	}
	public override void Hurt(int damage){
		base.Hurt(damage);
		animatedSprite.Play("Hurt");
		allow_go = false;
		Timer timer = new Timer();
		this.AddChild(timer);
		timer.WaitTime = 1.5f;
		timer.OneShot = true;
		timer.Connect("timeout", this, nameof(SwitchGoing));
		timer.Start();
	}
	private void SwitchGoing(){
		allow_go = true;
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
