using System;
using System.Collections.Generic;
using UnityEngine;



public abstract class CollidableEntity : Entity {
	protected Vector2 position = new Vector2();
	protected Vector2 velocity = new Vector2();
	protected Vector2 acceleration = Vector2.zero;
	protected int collisionOffsetX = 0;
	protected int collisionOffsetY = 0;

	public CollidableEntity() : base() {
		position.x = 0;
		position.y = 0;
		collisionBox.x = 0;
		collisionBox.y = 0;
	}
	
	public Vector2 Position	{
	    get { return position; }
	    set {
	        position = value;
	        collisionBox.x = (int)position.x;
	        collisionBox.y = (int)position.y;
	    }
	}

	public override void updateHorizontal(float dt)	{
	    velocity.x += acceleration.x * dt;
	    position.x += velocity.x * dt;            
	    collisionBox.x = (int)position.x + collisionOffsetX;
	}

	public override void updateVertical(float dt) {
		velocity.y += acceleration.y * dt;
		position.y += velocity.y * dt;
		collisionBox.y = (int)position.y + collisionOffsetY;
	}

	public override void onCollision(int pushX, int pushY) {
	    if /*^*/ (pushX != 0) {
	        position.x += (float)pushX;
	        collisionBox.x = (int)position.x + collisionOffsetX;
	    }else if (pushY != 0) {
			position.y += (float)pushY;
			collisionBox.y = (int)position.y + collisionOffsetY;
	    }
	}
}