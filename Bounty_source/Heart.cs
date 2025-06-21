using Godot;
using System;

public class Heart : Area2D
{
	private void _on_Area2D_body_entered(object body){
		if(body is KinematicBody2D){
			var kbody = body as KinematicBody2D;
			if(kbody.Name == "Player"){
				GetNode<AudioStreamPlayer2D>("Sound").Play();
				var player = kbody as Player;
				player.Heal(10);
				Timer timer = new Timer();
				this.AddChild(timer);
				timer.WaitTime = 2.0f;
				timer.OneShot = true;
				timer.Connect("timeout", this, nameof(Destroy));
				timer.Start();
				Visible = false;
				if(player.fuel<100)player.fuel += 20;
			}
		}
		Console.WriteLine(body);
	}
	public void Destroy(){
		QueueFree();
	}
}
