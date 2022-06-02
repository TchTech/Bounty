using Godot;
using System;

public class JetpackRocket : Projectiles
{
	public float MaxDistance = 800; // How far (in pixels) the bullet will travel before it is destroyed
	public float MaxSeconds = 2;    // How long (in seconds) before the bullet is destroyed
	public float ImpulseMag = 350;
	public float originalY = 0;
	public int rocketStep = 0;
	private int counter = 1;
	private Vector2 originalPos;
	private Random random;
	private bool boomed = false;
	public override void _Ready()
	{
		random = new Random();
	}
	private void OnTimeToDie(){
		QueueFree();
	}
	public void LaunchBullet(){
		originalPos = Position;
		originalY = GlobalPosition.y;
		ApplyCentralImpulse(Transform.x.Normalized() * ImpulseMag);
		Timer timer = new Timer();
		AddChild(timer);
		timer.WaitTime = MaxSeconds;
		timer.OneShot = true;
		timer.Connect("timeout",this,nameof(OnTimeToDie));
		timer.Start();
	}

	public override void _PhysicsProcess(float delta){
		float distanceTravelled = Position.DistanceTo(originalPos);
		if (distanceTravelled > MaxDistance) QueueFree();
		else if(GetCollidingBodies().Count>0 && !boomed){
			PackedScene boomScene = GD.Load<PackedScene>("res://Boom.tscn");
			Boom boom = (Boom)boomScene.Instance();
			boom.GlobalPosition = GetNode<Sprite>("Sprite").GlobalPosition;
			GetParent().GetParent().AddChild(boom);
			QueueFree();
		}
		
		if (rocketStep == 1) {
			SetAxisVelocity(new Vector2(0, -35+random.Next(-50,51)));
		}
		else if (rocketStep == 5)
		{
			rocketStep = 0;
			counter += 1;
		}
		rocketStep++;
	}
}
