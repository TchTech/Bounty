using Godot;
using System;

public class Playable : KinematicBody2D
{
	public int maxHealth;
	public int health;
	public virtual void Die(){
		QueueFree();
	}
	public virtual void Heal(int heal){
		if(maxHealth>=heal+health){
			health += heal;
		}else{
			health = maxHealth;
		}
		
	}
	public virtual void Hurt(int damage){
		health -= damage;
		Console.WriteLine(health.ToString());
		if(health <= 0){
			Die();
		}
	}
}
