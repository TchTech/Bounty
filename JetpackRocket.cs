using Godot;
using System;

public class JetpackRocket : Projectiles
{
	public float MaxDistance = 800; // How far (in pixels) the bullet will travel before it is destroyed
	public float MaxSeconds = 1;    // How long (in seconds) before the bullet is destroyed
	public float ImpulseMag = 800;
	private Playable goal;
	private Vector2 originalPos;
    private bool boomed = false;
    private string boom_path;
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
	}
    private void DestroyRocket(){
        GetParent<KinematicBody2D>().GetParent<Node2D>().GetNode<Boom>("Boom").QueueFree();
        Console.WriteLine(GetNode(boom_path).IsQueuedForDeletion() ? 1 : 0);
        this.QueueFree();
	}
}
