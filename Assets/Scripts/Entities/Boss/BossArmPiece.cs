using System;
using System.Collections.Generic;
using UnityEngine;



	public class BossArmPiece : EnemyEntity {
		static readonly Rect[] BossArmPieceSource = new Rect[] { new Rect(5 * 16, 8 * 16, 16, 16) };

        public BossArmPiece(Vector2 position, Player player)
            : base(position, parentRoom, player)
        {
            collisionOffsetX = 0;
            collisionOffsetY = 0;
            collisionBox.x = (int)position.X + collisionOffsetX;
            collisionBox.y = (int)position.Y + collisionOffsetY;
            collisionBox.width = 12;
            collisionBox.height = 12;
            currentSourceRect = BossArmPieceSource;
            countTowardsEnemyCount = false;

            animCount = 1;
            animSpeed = 0.1f;
            isHurtingOnContact = true;
            isHittable = false;
            health = 3;
            maxHealth = 3;
            maxSpeed = 1.1f;
        }

        int level = 0;

        public override void updateHorizontal(float dt)
        {
            base.updateHorizontal(dt);
        }

        public override void draw() {
            
        }

        public void specialDraw() {
            base.draw();
        }

        public override void handleInput()
        {
        }

        public override void setKnockback(Vector2 velocity)
        {
        }

        public override void onCollision(int pushX, int pushY)
        {
        }

        public override void onDie()
        {
            base.onDie();
            parentRoom.addEnemy(new EnemyExplosion(screenManager, position, parentRoom, player, spriteSheet));
        }
    }