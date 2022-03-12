using Godot;
using System;

public class Bomb : RigidBody2D
{
	private AnimatedSprite animatedSprite;
	private Sprite sprite;
	private CPUParticles2D bombParticles;
	private CPUParticles2D explParticles;
	private AudioStreamPlayer2D explSound;
	public float MaxSeconds = 1.5f;
	public float ImpulseMag = 160;
	//private Vector2 originalPos;
	public override void _Ready()
	{
		explSound = GetNode<AudioStreamPlayer2D>("ExplSound");
		animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		sprite = GetNode<Sprite>("Sprite");
		explParticles = GetNode<CPUParticles2D>("ExplCPUParticles2D");
		bombParticles = GetNode<CPUParticles2D>("BombCPUParticles2D");
		explParticles.Visible = false;
		bombParticles.Visible = true;
	}
	private void Destroy(){
		this.QueueFree();
	}
	private void OnTimeToDie(){
		Sleeping = true;
		bombParticles.Visible = false;
		explParticles.Visible = true;
		sprite.Visible = false;
		animatedSprite.Visible = true;
		animatedSprite.Play("Explosion");
		if(!explSound.Playing){
			explSound.Play();
		}
		Timer timer = new Timer();
		this.AddChild(timer);
		timer.WaitTime = 0.5f;
		timer.OneShot = true;
		timer.Connect("timeout", this, nameof(Destroy));
		timer.Start();
	}
	public void LaunchBomb(){
		this.ApplyCentralImpulse(this.Transform.x.Normalized() * this.ImpulseMag); // apply an impulse in the same direction that the bullet is facing
		Timer timer = new Timer();
		this.AddChild(timer);
		timer.WaitTime = this.MaxSeconds;
		timer.OneShot = true;
		timer.Connect("timeout", this, nameof(OnTimeToDie));
		timer.Start();
	}

	//public override void _PhysicsProcess(float delta){}
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
