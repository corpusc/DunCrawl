using System;
using System.Collections.Generic;
using UnityEngine;



	public class BossArm : EnemyEntity {
		static readonly Rect[] BossArmSource = new Rect[] { new Rect(6 * 16, 8 * 16, 16, 16), new Rect(7 * 16, 8 * 16, 16, 16) };

        public BossArm(Vector2 position, Player player, int level, bool left)
            : base(position, parentRoom, player)
        {
            collisionOffsetX = 0;
            collisionOffsetY = 0;
            collisionBox.x = (int)position.X + collisionOffsetX;
            collisionBox.y = (int)position.Y + collisionOffsetY;
            collisionBox.width = 32;
            collisionBox.height = 32;
            currentSourceRect = BossArmSource;
            countTowardsEnemyCount = false;

            if (left)
                currentAnim = 0;
            else
                currentAnim = 1;

            animCount = 2;
            animSpeed = 0f;
            isHittable = false;
            isHurtingOnContact = true;
            health = 3;
            maxHealth = 3;
            maxSpeed = 3f;

            this.level = level;
        }

        float timer = 0.0f;
        readonly float timerSpeed = 0.01f;
        int level = 0;

        float pieceTimer = 0.0f;
        readonly float pieceTimerSpeed = 0.5f;

        List<BossArmPiece> pieces = new List<BossArmPiece>();
        public bool isAttacking = false;
        bool isRetracting = false;
        bool isWaiting = false;
        Vector2 attackPos = Vector2.Zero;
        Vector2 oldPos = Vector2.Zero;

        public override void updateHorizontal(float dt)
        {
            base.updateHorizontal(dt);

            //do some ai stuff
            if (currentState == State.Neutral)
            {
                if (!isAttacking)
                    return;

                Vector2 dist = attackPos - position;
                if (!isWaiting && (dist.X * dist.X + dist.Y * dist.Y >= 0.00001f) && dist.Length() < 16f)
                {
                    velocity = Vector2.Zero;
                    isWaiting = true;
                    timer = 0;
                }

                timer += timerSpeed * dt;
                if (timer >= 1f)
                {
                    if (isWaiting)
                    {
                        if (velocity == Vector2.Zero)
                        {
                            velocity = oldPos - position;
                            velocity.Normalize();
                            velocity *= maxSpeed;
                        }
                        isRetracting = true;
                        isWaiting = false;
                    }
                    timer = 0f;
                }

                pieceTimer += pieceTimerSpeed * dt;
                if (pieceTimer >= 1f)
                {
                    if (!isRetracting && !isWaiting)
                    {
                        BossArmPiece piece = new BossArmPiece(screenManager, position, parentRoom, player);
                        pieces.Add(piece);
                        parentRoom.addEnemy(piece);
                    }
                    
                    if( isRetracting )
                    {
                        if (pieces.Count > 0)
                        {
                            BossArmPiece piece = pieces[pieces.Count - 1];
                            pieces.Remove(piece);
                            parentRoom.removeEnemy(piece);
                        }
                        else
                        {
                            isRetracting = false;
                            isWaiting = false;
                            isAttacking = false;
                        }
                    }

                    pieceTimer = 0f;
                }
            }
        }

        public override void draw()
        {
            foreach (BossArmPiece piece in pieces) {
                piece.specialDraw();
            }

            base.draw();
        }

        public override void handleInput()
        {
        }

        public override void setKnockback(Vector2 velocity)
        {
            currentState = State.Neutral;
            if (health <= 0)
            {
                onDie();
            }
        }

        public override void onCollision(int pushX, int pushY)
        {
            base.onCollision(pushX, pushY);
        }

        public override void onDie()
        {
            base.onDie();
            parentRoom.addEnemy(new EnemyExplosion(screenManager, position, parentRoom, player, spriteSheet));
            foreach (BossArmPiece piece in pieces)
                parentRoom.removeEnemy(piece);
        }

        public void doAttack()
        {
            isAttacking = true;
            attackPos = player.Position;
            oldPos = position;
            velocity = player.Position - position;
            velocity.Normalize();
            velocity *= maxSpeed;
        }
    }