using System;
using System.Collections.Generic;
using UnityEngine;



	public class Boss : EnemyEntity {
        static readonly Rect[] BossHead = new Rect[] { new Rect(9 * 16, 7 * 16, 16, 16), new Rect(3 * 16, 8 * 16, 16, 16) };
        static readonly Rect[] BossBody = new Rect[] { new Rect(6 * 16, 7 * 16, 16, 16), new Rect(7 * 16, 7 * 16, 16, 16), 
			new Rect(8 * 16, 7 * 16, 16, 16), new Rectangle(6 * 16, 7 * 16, 16, 16) };
        static readonly Rect[] BossArms = new Rect[] { new Rect(5 * 16, 8 * 16, 16, 16), new Rect(6 * 16, 8 * 16, 16, 16), 
			new Rect(7 * 16, 8 * 16, 16, 16) };

        BossArm leftArm, rightArm;

        public Boss(Vector2 position, Player player, int level)
            : base(screenManager, position, parentRoom, player)
        {
            collisionOffsetX = 0;
            collisionOffsetY = 0;
            collisionBox.x = (int)position.X + collisionOffsetX;
            collisionBox.y = (int)position.Y + collisionOffsetY;
            collisionBox.width = 32;
            collisionBox.height = 32;
            currentSourceRect = BossBody;

            animCount = 4;
            animSpeed = 0.1f;
            isHurtingOnContact = true;
            health = 6;
            maxHealth = 6;
            maxSpeed = 1.1f;
            //invincibleCountdownSpeed = -0.02f;

            if (screenManager.RNG.Next(100) < 50)
                velocity.X = maxSpeed;
            else
                velocity.X = -maxSpeed;

            this.level = level;

            leftArm = new BossArm(screenManager, position, parentRoom, player, level, true);
            rightArm = new BossArm(screenManager, position, parentRoom, player, level, false);
            parentRoom.addEnemy(leftArm);
            parentRoom.addEnemy(rightArm);
        }

        float timer = 0.0f;
        readonly float timerSpeed = 0.01f;
        int level = 0;
        int headAnim = 0;

        float shootTimer = 0.0f;

        bool bothAttacked = false;

        public override void updateHorizontal(float dt) {
            base.updateHorizontal(dt);

            if (!leftArm.isAttacking)
                leftArm.Position = position + new Vector2(-4f, 8f);

            if (!rightArm.isAttacking)
                rightArm.Position = position + new Vector2(24f, 8f);

            //do some ai stuff
            if (currentState == State.Neutral) {
                shootTimer += timerSpeed * dt;
                if (shootTimer >= 1f && level >= 3) {
                    BossProjectile projectile = new BossProjectile(screenManager, position, parentRoom, player, screenManager.SpriteSheet);
                    parentRoom.addEnemy(projectile);
                    shootTimer = 0f;
                }

                timer += timerSpeed * dt;

                if (timer >= 1f) {
                    if (!bothAttacked)
                    {
                        if (!leftArm.isAttacking)
                            leftArm.doAttack();
                        else if (!rightArm.isAttacking)
                            rightArm.doAttack();
                        else
                            bothAttacked = true;
                        velocity = Vector2.Zero;
                    }else if(!leftArm.isAttacking && !rightArm.isAttacking) {
                        if (screenManager.RNG.Next(100) < 50)
                            velocity.X = maxSpeed;
                        else
                            velocity.X = -maxSpeed;
                        bothAttacked = false;
                    }

                    timer = 0f;
                }
            }

            /*if (Math.Abs(velocity.X * velocity.X + velocity.Y * velocity.Y) <= 0.000001f)
                return;*/

            animTimer += animSpeed * dt;
            if (animTimer >= 1f) {
                animTimer = 0f;
                currentAnim = (currentAnim + 1) % animCount;
                headAnim = (headAnim + 1) % 2;
            }
        }

        public override void draw() {
            base.draw(gameTime);

            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            spriteRect.x = (int)position.X;
            spriteRect.y = (int)position.Y - 8;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, parentRoom.TransitionMatrix);
            Color color = Color.White;
            if (IsInvincible) {
                if (invincibleAlternate)
                    color.A = 128;
            }
            spriteBatch.Draw(screenManager.SpriteSheet, spriteRect.getScaled(scale), BossHead[headAnim], color);
            spriteBatch.End();
        }

        public override void handleInput() {
        }

        public override void setKnockback(Vector2 velocity) {
            currentState = State.Neutral;
            if (health <= 0) {
                onDie();
            }
        }

        public override void onCollision(int pushX, int pushY) {
            base.onCollision(pushX, pushY);

            if (pushX != 0) {
                velocity.X = -velocity.X;
            }
        }

        public override void onDie() {
            base.onDie();
            leftArm.onDie();
            rightArm.onDie();
            parentRoom.addEnemy(new EnemyExplosion(screenManager, position, parentRoom, player, spriteSheet));
        }
    }