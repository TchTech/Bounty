using Godot;
using System;

public class VaderHologramme : KinematicBody2D
{
    private int run_speed = 100;
    private Vector2 velocity;
    private Area2D area2D;
    private Area2D player;
    private AnimatedSprite animatedSprite;
    private bool should_run = false;
    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
    }
    public override void _Process(float delta){
        velocity = Vector2.Zero;
        try{
            if(player.Name == "Player" && Math.Abs(Position.DirectionTo(player.Position).x) > 0.7f){
            velocity = Position.DirectionTo(player.Position) * run_speed;
            animatedSprite.Play("Go");
        }
        }catch{
            player = new Area2D();
        }
        if(Position.DirectionTo(player.Position).x<0){
            animatedSprite.FlipH = false;
        }else{
            animatedSprite.FlipH = true;
        }
        velocity.y += 9000 * delta;
        velocity = MoveAndSlide(velocity, Vector2.Up);
    }
    public void _on_Area2D_body_entered(Area2D other){
        player = other;
        Console.WriteLine(Position.DirectionTo(player.Position).x);
    }
    public void _on_Area2D_body_exited(Area2D other){
        Console.WriteLine("aaaa");
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
