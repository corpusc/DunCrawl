using System;
using System.Collections.Generic;
using UnityEngine;



public abstract class CollidableEntity : Entity {
	protected Vector2 position = new Vector2();
	protected Vector2 velocity = new Vector2();
	protected Vector2 acceleration = Vector2.Zero;

	protected int collisionOffsetX = 0;
	protected int collisionOffsetY = 0;

	public Vector2 Position	{
	    get { return position; }
	    set
	    {
	        position = value;
	        collisionBox.x = (int)position.X;
	        collisionBox.y = (int)position.Y;
	    }
	}

	public CollidableEntity()
	    : base()
	{
	    position.X = 0;
	    position.Y = 0;
	    collisionBox.x = 0;
	    collisionBox.y = 0;
	}

	public override void updateHorizontal(float dt)	{
	    velocity.X += acceleration.X * dt;
	    position.X += velocity.X * dt;            
	    collisionBox.x = (int)position.X + collisionOffsetX;
	}

	public override void updateVertical(float dt) {
	    velocity.Y += acceleration.Y * dt;
	    position.Y += velocity.Y * dt;
	    collisionBox.y = (int)position.Y + collisionOffsetY;
	}

	public override void onCollision(int pushX, int pushY) {
	    if (pushX != 0) {
	        position.X += (float)pushX;
	        collisionBox.x = (int)position.X + collisionOffsetX;
	    }else if (pushY != 0) {
	        position.Y += (float)pushY;
	        collisionBox.y = (int)position.Y + collisionOffsetY;
	    }
	}
}