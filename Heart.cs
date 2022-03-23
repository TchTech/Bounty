using Godot;
using System;

public class Heart : Area2D
{
    private void _on_Area2D_body_entered(object body){
		if(body is KinematicBody2D){
			var kbody = body as KinematicBody2D;
            if(kbody.Name == "Player"){
                var player = kbody as Player;
                player.Heal(10);
			    QueueFree();
            }
		}
        Console.WriteLine(body);
	}
}
