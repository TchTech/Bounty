using Godot;
using System;

public class MainMenu : Control
{
	private PackedScene mainScene;
	private PackedScene infoScene;
	public override void _Ready()
	{
		mainScene = GD.Load<PackedScene>("res://Main.tscn");
		infoScene = GD.Load<PackedScene>("res://InfoMenu.tscn");
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	private void _on_Start_pressed()
{
	GetParent().AddChild((Node2D)mainScene.Instance());
	QueueFree();
}
private void _on_Info_pressed()
{
	GetParent().AddChild((Control)infoScene.Instance());
	QueueFree();
}

}
