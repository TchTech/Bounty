using Godot;
using System;

public class Coin : Area2D
{
	private void _on_Area2D_body_entered(object body){
		if(body is KinematicBody2D){
			var kbody = body as KinematicBody2D;
			if(kbody.Name == "Player" && Visible){
				GetNode<AudioStreamPlayer2D>("Sound").Play();
				var player = kbody as Player;
				player.Money += 1;
				Timer timer = new Timer();
				this.AddChild(timer);
				timer.WaitTime = 2.0f;
				timer.OneShot = true;
				timer.Connect("timeout", this, nameof(Destroy));
				timer.Start();
				Visible = false;
				player.AddFuel(20);
			}
		}
		Console.WriteLine(body);
	}
	public void Destroy(){
		QueueFree();
	}
}
