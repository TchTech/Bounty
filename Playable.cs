using Godot;
using System;

public class Playable : KinematicBody2D
{
	public int MaxHealth;
	public int Health;
	public virtual void Die(){
		QueueFree();
	}
	public virtual void Heal(int heal){
		if(MaxHealth>=heal+Health){
			Health += heal;
		}else{
			Health = MaxHealth;
		}
		
	}
	public virtual void Hurt(int damage){
		Health -= damage;
		Console.WriteLine(Health.ToString());
		if(Health <= 0){
			Die();
		}
	}
}
