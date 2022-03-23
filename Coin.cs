using Godot;
using System;

public class Coin : Area2D
{
    private void _on_Area2D_body_entered(object body){
		if(body is KinematicBody2D){
			var kbody = body as KinematicBody2D;
            if(kbody.Name == "Player"){
                GetNode<AudioStreamPlayer2D>("Sound").Play();
                var player = kbody as Player;
                player.money += 1;
			    QueueFree();
            }
		}
        Console.WriteLine(body);
	}
}
