using Godot;
using System;

public class Bullet : Projectiles
{
	public float MaxDistance = 800; // How far (in pixels) the bullet will travel before it is destroyed
	public float MaxSeconds = 1;    // How long (in seconds) before the bullet is destroyed
	public float ImpulseMag = 2000;
	public int Damage; 
	private Playable goal;
	private Vector2 originalPos;
	public override void _Ready()
	{
	}
	private void OnTimeToDie(){
		this.QueueFree();
	}
	public void LaunchBullet(int dmg){
		originalPos = Position; // save the position the bullet was launched from
		ApplyCentralImpulse(Transform.x.Normalized() * ImpulseMag); // apply an impulse in the same direction that the bullet is facing
		// start a timer that will delete the bullet when it has lived long enough
		Damage = dmg;
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
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
