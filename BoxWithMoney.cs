using Godot;
using System;

public class BoxWithMoney : StaticBody2D
{
	private void _on_Area2D_body_entered(object body)
{
	if(body is Projectiles){
		PackedScene scene = GD.Load<PackedScene>("res://Coin.tscn"); 
		Node2D money = (Node2D)scene.Instance();
		money.Position = GetNode<Sprite>("Sprite").GlobalPosition;
		GetParent<Node2D>().GetParent<Node2D>().AddChild(money);
		QueueFree();
		CPUParticles2D brokenParticles = (CPUParticles2D)GD.Load<PackedScene>("res://BrokenBoxParticles.tscn").Instance();
		GetParent<Node2D>().GetParent<Node2D>().AddChild(brokenParticles);
		brokenParticles.Position = GlobalPosition;
		brokenParticles.OneShot = true;
	}
}
}
