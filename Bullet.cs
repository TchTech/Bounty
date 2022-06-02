using Godot;
using System;

public class Bullet : Projectiles
{
	public float MaxDistance = 800;
	public float MaxSeconds = 1;
	public float ImpulseMag = 2000;
	private Playable goal;
	private Vector2 originalPos;
	private int Damage;
	public override void _Ready()
	{
	}
	private void OnTimeToDie(){
		this.QueueFree();
	}
	public void LaunchBullet(int damage){
		Damage = damage;
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
		else if(GetCollidingBodies().Count>0){
			if(GetCollidingBodies()[0] is Playable){
				goal = GetCollidingBodies()[0] as Playable;
				goal.Hurt(Damage);
			}
			this.QueueFree();
		}
	}
}
