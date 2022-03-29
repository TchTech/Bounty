using Godot;
using System;

public class JetpackRocket : Projectiles
{
	public float MaxDistance = 800; // How far (in pixels) the bullet will travel before it is destroyed
	public float MaxSeconds = 1;    // How long (in seconds) before the bullet is destroyed
	public float ImpulseMag = 600;
	private Vector2 originalPos;
	private bool boomed = false;
	public override void _Ready()
	{
	}
	private void OnTimeToDie(){
		this.QueueFree();
	}
	public void LaunchBullet(){
		this.originalPos = this.Position;
		this.ApplyCentralImpulse(this.Transform.x.Normalized() * this.ImpulseMag);
		Timer timer = new Timer();
		this.AddChild(timer);
		timer.WaitTime = this.MaxSeconds;
		timer.OneShot = true;
		timer.Connect("timeout",this,nameof(OnTimeToDie));
		timer.Start();
	}

	public override void _PhysicsProcess(float delta){
		float distanceTravelled = this.Position.DistanceTo(this.originalPos);
		if (distanceTravelled > this.MaxDistance) this.QueueFree();
		else if(GetCollidingBodies().Count>0 && !boomed){
			PackedScene boomScene = GD.Load<PackedScene>("res://Boom.tscn");
			Boom boom = (Boom)boomScene.Instance();
			boom.Position = GetNode<Sprite>("Sprite").GlobalPosition;
			GetParent<KinematicBody2D>().GetParent<Node2D>().AddChild(boom);
			QueueFree();
		}
	}
}
