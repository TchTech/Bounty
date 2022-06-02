using Godot;
using System;

public class InfoMenu : Control
{
	private PackedScene menuScene;
	public override void _Ready()
	{
		menuScene = GD.Load<PackedScene>("res://MainMenu.tscn");
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	private void _on_Button_pressed()
{
	GetParent().AddChild((Node2D)menuScene.Instance());
	QueueFree();
}
}

