using Godot;
using System;

public partial class TopDownMovement : CharacterBody2D
{
	[Export] private float speed = 15f;
	[Export] private AnimatedSprite2D animatedSprite;
	[Export] private PackedScene testNode;

	// Called every frame. 'delta' is the elapsed time since the previous frame.

	Vector2 latestDirection;
	float maxRayDistance = 30f;

    public override void _Process(double delta)
    {
		if (Input.IsActionJustPressed("action"))
		{
			Ray2D ray = new Ray2D(GlobalPosition, latestDirection);
			HitInfo2D hitInfo = new HitInfo2D();
			Raycast.Raycast2D(ray, out hitInfo, GetWorld2D().DirectSpaceState, maxRayDistance, GetRid());

            #region raycast_debugging
            //Node2D testNode1 = testNode.Instantiate() as Node2D;
            //Node2D testNode2 = testNode.Instantiate() as Node2D;
            //testNode1.GlobalPosition = GlobalPosition;
            //testNode2.GlobalPosition = ray.GetPoint(maxRayDistance);
            //GetTree().Root.AddChild(testNode1);
            //GetTree().Root.AddChild(testNode2);
            #endregion

            if (hitInfo != null) {
				Node node = hitInfo.collider as Node;
				if(node is NPC NPCNode)
				{
					NPCNode.StartDialogue();
				}
			}
		}
    }

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector("left", "right", "forward", "backward").Normalized();
		Vector2 oldDir = Vector2.Down;

		if (direction == Vector2.Left)
		{
			animatedSprite.Frame = 2;
            latestDirection = direction;

        }
        else if (direction == Vector2.Right) 
		{
			animatedSprite.Frame = 1;
            latestDirection = direction;
        }
        else if (direction == Vector2.Up)
		{
			animatedSprite.Frame = 3;
            latestDirection = direction;
        }
        else if(direction == Vector2.Down)
		{
			animatedSprite.Frame = 0;
            latestDirection = direction;
        }

        Velocity = direction * speed;
		MoveAndSlide();
	}
}
