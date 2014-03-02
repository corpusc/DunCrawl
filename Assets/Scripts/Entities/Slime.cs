using System;
using System.Collections.Generic;
using UnityEngine;



public class Slime : EnemyEntity {
	static readonly Rect[] SlimeRect = new Rect[] { new Rect(2 * 16, 4 * 16, 16, 16), new Rect(3 * 16, 4 * 16, 16, 16) };

	float timer = 0f;
	float timerSpeed = 0.01f;
	bool shoot = true;

	public Slime(Vector2 position, Player player )
	    : base(position, parentRoom, player)
	{
	    collisionOffsetX = 4 * 2;
	    collisionOffsetY = 5 * 2;
	    collisionBox.x = (int)position.X + collisionOffsetX;
	    collisionBox.y = (int)position.Y + collisionOffsetY;
	    collisionBox.width = 16;
	    collisionBox.height = 12;
	    currentSourceRect = SlimeRect;

	    animCount = 2;
	    animSpeed = 0.02f;
	    isHurtingOnContact = true;

	    timer = (float)screenManager.RNG.NextDouble();
	}

	public override void updateHorizontal(float dt) {
	    base.updateHorizontal(dt);

	    // ai stuff
	    if (currentState == State.Neutral) {
	        if (!shoot) {
	            velocity = player.Position - position;
	            velocity.Normalize();
	            velocity *= 0.3f;
	        }
	        else
	            velocity = Vector2.Zero;

	        timer += timerSpeed * dt;
	        if ((timer >= 1f && !shoot) || (timer >= 0.3f && shoot)) {
	            timer = 0f;

	            if (shoot) {
	                Projectile proj = new Projectile(screenManager, position, parentRoom, player, spriteSheet);
	                parentRoom.addEnemy(proj);
	            }

	            shoot = !shoot;
	        }
	    }           

	    /*if (Math.Abs(velocity.X * velocity.X + velocity.Y * velocity.Y) <= 0.000001f)
	        return;*/

	    animTimer += animSpeed * dt;
	    if (animTimer >= 1f) {
	        animTimer = 0f;
	        currentAnim = (currentAnim + 1) % animCount;
	    }
	}

	public override void draw() {
	    base.draw();
	}

	public override void handleInput() {
	}

	public override void onDie() {
	    base.onDie();
	    parentRoom.addEnemy(new EnemyExplosion(screenManager, position, parentRoom, player, spriteSheet));
	}
}
