using Godot;
using System;

public class Playable : KinematicBody2D
{
    public int health;
    public virtual void Hurt(int damage){
		health -= damage;
        Console.WriteLine(health.ToString());
		if(health == 0){
			QueueFree();
		}
	}
}