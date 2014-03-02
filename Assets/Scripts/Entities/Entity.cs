using System;
using UnityEngine;



public abstract class Entity {
	protected AABB collisionBox = new AABB();
	public static Vector2 scale;

	public Rect CollisionRect {
		get { return collisionBox.Rect; }
	}

	public AABB CollisionBox {
	    get { return collisionBox; }
	}

	public abstract void updateHorizontal(float dt);
	public abstract void updateVertical(float dt);
	public abstract void draw();
	public abstract void onCollision(int pushX, int pushY);
	public abstract void handleInput();
}
