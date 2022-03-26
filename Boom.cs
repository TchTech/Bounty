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
}
