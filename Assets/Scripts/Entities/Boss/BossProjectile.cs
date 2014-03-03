using System;
using System.Collections.Generic;
using UnityEngine;



    public class BossProjectile : EnemyEntity
    {
	static readonly Rect[] BossProjectileRect = new Rect[] { new Rect(4 * 16, 4 * 16, 16, 16), new Rect(5 * 16, 4 * 16, 16, 16) };
        
        public BossProjectile(Vector2 position) : base(position) {
            collisionOffsetX = 0;
            collisionOffsetY = 0;
            collisionBox.x = (int)position.x + collisionOffsetX;
            collisionBox.y = (int)position.y + collisionOffsetY;
            collisionBox.width = 7 * 2;
            collisionBox.height = 8 * 2;
            currentSourceRect = BossProjectileRect;

            animCount = 2;
            animSpeed = 0.2f;

            velocity = Player.Pos2D - position;
            velocity.Normalize();
            velocity *= 2f;

            currentState = State.Attacking;
            attackRect.width = collisionBox.width;
            attackRect.height = collisionBox.height;

            countTowardsEnemyCount = false;
            health = 2;
        }

        public override void updateHorizontal(float dt) {
            base.updateHorizontal(dt);
            attackRect.x = collisionBox.x;

            if (Math.Abs(velocity.x * velocity.x + velocity.y * velocity.y) <= 0.000001f)
                return;

            animTimer += animSpeed * dt;
            if (animTimer >= 1f) {
                animTimer = 0f;
                currentAnim = (currentAnim + 1) % animCount;
            }
        }

        public override void updateVertical(float dt) {
            base.updateVertical(dt);
            attackRect.y = collisionBox.y;
        }

        public override void onCollision(int pushX, int pushY) {
            base.onCollision(pushX, pushY);

            if (pushX != 0)
                velocity.x = -velocity.x;

            if (pushY != 0)
                velocity.y = -velocity.y;
        }

        public override void draw() {
            base.draw();
        }

        public override void handleInput() {
        }
    }