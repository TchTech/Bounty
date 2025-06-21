using Godot;
using System;

public class JetpackRocket : Projectiles
{
	public float MaxDistance = 800; // How far (in pixels) the bullet will travel before it is destroyed
<<<<<<< Updated upstream
<<<<<<< Updated upstream
	public float MaxSeconds = 1;    // How long (in seconds) before the bullet is destroyed
	public float ImpulseMag = 800;
	private Playable goal;
	private Vector2 originalPos;
    private bool boomed = false;
    private string boom_path;
=======
	public float MaxSeconds = 2;    // How long (in seconds) before the bullet is destroyed
	public float ImpulseMag = 350;
	public float originalY = 0;
	public int rocketStep = 0;
	private int counter = 1;
	private Vector2 originalPos;
=======
	public float MaxSeconds = 2;    // How long (in seconds) before the bullet is destroyed
	public float ImpulseMag = 350;
	public float originalY = 0;
	public int rocketStep = 0;
	private int counter = 1;
	private Vector2 originalPos;
>>>>>>> Stashed changes
	private Random random;
	private bool boomed = false;
>>>>>>> Stashed changes
	public override void _Ready()
	{
		random = new Random();
	}
	private void OnTimeToDie(){
		QueueFree();
	}
	public void LaunchBullet(){
<<<<<<< Updated upstream
<<<<<<< Updated upstream
		this.originalPos = this.Position; // save the position the bullet was launched from
		this.ApplyCentralImpulse(this.Transform.x.Normalized() * this.ImpulseMag); // apply an impulse in the same direction that the bullet is facing
		// start a timer that will delete the bullet when it has lived long enough
=======
		originalPos = Position;
		originalY = GlobalPosition.y;
		ApplyCentralImpulse(Transform.x.Normalized() * ImpulseMag);
>>>>>>> Stashed changes
=======
		originalPos = Position;
		originalY = GlobalPosition.y;
		ApplyCentralImpulse(Transform.x.Normalized() * ImpulseMag);
>>>>>>> Stashed changes
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
            boom.Position = GetNode<Sprite>("Sprite").GlobalPosition;
            GetParent<KinematicBody2D>().GetParent<Node2D>().AddChild(boom);
            boom_path = boom.GetPath();
            Timer timer = new Timer();
		    timer.WaitTime = 1;
		    timer.OneShot = true;
            AddChild(timer);
		    timer.Connect("timeout",this,nameof(DestroyRocket));
		    timer.Start();
			GetNode<Sprite>("Sprite").Visible=false;
            boomed = true;
		}
		
		if (rocketStep == 1) {
			SetAxisVelocity(new Vector2(0, -35+random.Next(-50,51)));
			Console.WriteLine(Convert.ToString(LinearVelocity.y));
			//GravityScale = 10;
		}
		else if (rocketStep == 5)
		{
			rocketStep = 0;
			counter += 1;
			//GravityScale = 5;
		}
<<<<<<< Updated upstream
=======
		
		if (rocketStep == 1) {
			SetAxisVelocity(new Vector2(0, -35+random.Next(-50,51)));
			Console.WriteLine(Convert.ToString(LinearVelocity.y));
			//GravityScale = 10;
		}
		else if (rocketStep == 5)
		{
			rocketStep = 0;
			counter += 1;
			//GravityScale = 5;
		}
>>>>>>> Stashed changes
		rocketStep++;
		//SetAxisVelocity(new Vector2(0, -200));


<<<<<<< Updated upstream
	}
    private void DestroyRocket(){
        GetParent<KinematicBody2D>().GetParent<Node2D>().GetNode<Boom>("Boom").QueueFree();
        Console.WriteLine(GetNode(boom_path).IsQueuedForDeletion() ? 1 : 0);
        this.QueueFree();
=======
>>>>>>> Stashed changes
	}
}
