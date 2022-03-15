using Godot;
using System;

public class Bullet : RigidBody2D
{
	public float MaxDistance = 800; // How far (in pixels) the bullet will travel before it is destroyed
	public float MaxSeconds = 1;    // How long (in seconds) before the bullet is destroyed
	public float ImpulseMag = 2000;
	private Playable goal;
	private Vector2 originalPos;
	public override void _Ready()
	{
	}
	private void OnTimeToDie(){
		this.QueueFree();
	}
	public void LaunchBullet(){
		this.originalPos = this.Position; // save the position the bullet was launched from
		this.ApplyCentralImpulse(this.Transform.x.Normalized() * this.ImpulseMag); // apply an impulse in the same direction that the bullet is facing
		// start a timer that will delete the bullet when it has lived long enough
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
				goal.Hurt(20);
			}
			this.QueueFree();
		}
	}
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
