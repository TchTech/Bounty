using Godot;
using System;

public class DieScene : Control
{
	private PackedScene mainScene;
	public override void _Ready()
	{
		mainScene = GD.Load<PackedScene>("res://Main.tscn");
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	private void _on_Button_pressed()
{
	GetParent().AddChild((Node2D)mainScene.Instance());
	QueueFree();
}
}
