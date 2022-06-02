using Godot;
using System;

public class ToGrievousBattle : Node2D
{
	private Player pl;
	private PackedScene GrievousScene;
	public override void _Ready()
	{
		GrievousScene = GD.Load<PackedScene>("res://GrievousBattle.tscn");
	}
	private void _on_Area2D_body_entered(object body)
	{
		if(body is Player){
			pl = body as Player;
			GetParent().GetParent().AddChild((Node2D)GrievousScene.Instance());
			GetParent().QueueFree();
		}
	}
}
