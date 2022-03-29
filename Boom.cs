using Godot;
using System;

public class Boom : Projectiles
{
	public override void _Ready()
	{
		Timer timer = new Timer();
		timer.WaitTime = 0.5f;
		timer.OneShot = true;
		AddChild(timer);
		timer.Connect("timeout",this,nameof(Destroy));
		timer.Start();
	}
	public void _on_Area2D_body_entered(object other){
		if(other is Playable){
			var pl = other as Playable;
			pl.Hurt(75);
		}
	}
	private void Destroy(){
		QueueFree();
	}
}
