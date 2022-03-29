using Godot;
using System;

public class Bomb : RigidBody2D
{
	private Sprite sprite;
	private CPUParticles2D bombParticles;
	private PackedScene boomScene;
	public float MaxSeconds = 1.5f;
	public float ImpulseMag = 200;
	public override void _Ready()
	{
		boomScene = GD.Load<PackedScene>("res://Boom.tscn");
		sprite = GetNode<Sprite>("Sprite");
		bombParticles = GetNode<CPUParticles2D>("BombCPUParticles2D");
		bombParticles.Visible = true;
	}
	private void Destroy(){
		QueueFree();
	}
	public void OnTimeToDie(){
		Boom boom = (Boom)boomScene.Instance();
		boom.Position = sprite.GlobalPosition;
		GetParent<KinematicBody2D>().GetParent<Node2D>().AddChild(boom);
		Sleeping = true;
		bombParticles.Visible = false;
		sprite.Visible = false;
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
}
