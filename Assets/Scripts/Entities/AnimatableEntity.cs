using System;

public abstract class AnimatableEntity : CollidableEntity {
	protected AABB spriteRect;

	protected int currentAnim = 0;
	protected int animCount = 2;
	protected float animTimer = 0f;
	protected float animSpeed = 0.1f;

	public AnimatableEntity() : base() {
	}
}
