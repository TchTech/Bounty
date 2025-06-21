using Godot;
using System;

public class FlameArea : Area2D
{
	public override void _Process(float delta){
		for(int i = 0; i<GetOverlappingBodies().Count; i++){
			var body = GetOverlappingBodies()[i];
			if(body is Playable){
				Playable goal = body as Playable;
				goal.Hurt(1);
			}
		}
	}
}
