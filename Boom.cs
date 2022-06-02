using Godot;
using System;

public class Boom : Projectiles
{
	public void _on_Area2D_body_entered(object other){
		if(other is Playable){
			var pl = other as Playable;
			pl.Hurt(50);
		}
	}
	public override void _Process(float delta)
	{
		Timer timer = new Timer();
		timer.WaitTime = 1;
		timer.OneShot = true;
		AddChild(timer);
		timer.Connect("timeout",this,nameof(Destroy));
		timer.Start();
	}
	private void Destroy(){
		QueueFree();
	}
}
