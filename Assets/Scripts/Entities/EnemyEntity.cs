using System;
using System.Collections.Generic;
using UnityEngine;



public abstract class EnemyEntity : AnimatableEntity {
	//protected PlayerMINIMAL player;
	protected float maxSpeed = 2.5f;

	protected int health = 2;
	protected int maxHealth = 2;
	protected int attack = 1;

	protected bool countTowardsEnemyCount = true;
	protected bool invincibleAlternate = false;
	protected float invincibleAlternateTimer = 0f;
	protected readonly float invincibleAlternateTimerSpeed = 0.15f;

	protected Vector2 knockbackFriction = Vector2.zero;
	protected readonly float knockbackFrictionSpeed = 0.1f;
	protected float knockbackTimer = 0f;
	protected readonly float knockbackTimerSpeed = 0.03f;

	protected Rect[] currentSourceRect;
	protected State currentState = State.Neutral;
	protected float invincibleTimer = 0f;
	protected float invincibleCountdownSpeed = -0.01f;
	
	protected bool isHittable = true;
	protected bool isHurtingOnContact = false;

	protected enum State {
		Neutral,
		Attacking,
		Knockback
	}
	
	public EnemyEntity(Vector2 position)
		: base()
	{
		this.position = position;
		spriteRect.x = 0;
		spriteRect.y = 0;
		spriteRect.width = 32;
		spriteRect.height = 32;
		
		//this.parentRoom = parentRoom;
	}

	public bool IsHittable {
	    get { return isHittable; }
	}

	public bool IsHurtingOnContact {
	    get { return isHurtingOnContact; }
	}

	public bool CountTowardsEnemyCount {
	    get { return countTowardsEnemyCount; }
	}

	public int Health {
	    get { return health; }
	    set {
	        if (value < health) {
	            invincibleTimer = 1f;
	        }

	        health = value;

	        if (health > maxHealth)
	            health = maxHealth;
	        /*if (health <= 0 && this != player) {
	            parentRoom.removeEnemy(this);
	        }*/
	    }
	}

	public int MaxHealth {
	    get { return maxHealth; }
	    set {
	        maxHealth = value;
	        if (maxHealth < health)
	            health = maxHealth;
	    }
	}

	public int Attack {
	    get { return attack; }
	    set { attack = value; }
	}

	public float Speed {
	    get { return maxSpeed; }
	    set { maxSpeed = value; }
	}

	public bool IsInvincible {
	    get { return invincibleTimer > 0f; }
	}

	public bool IsAttacking {
	    get { return currentState == State.Attacking; }
	}

	protected AABB attackRect = new AABB(0, 0, 0, 0);
	public AABB AttackRect {
	    get { return attackRect; }
	}

	public override void updateHorizontal(float dt) {
	    if (currentState == State.Knockback) {
	        velocity.x += knockbackFriction.x * dt;
	    }

	    base.updateHorizontal(dt);

	    if (invincibleTimer > 0f) {
	        invincibleTimer += invincibleCountdownSpeed * dt;
	        invincibleAlternateTimer += invincibleAlternateTimerSpeed * dt;

	        if (invincibleAlternateTimer >= 1f) {
	            invincibleAlternateTimer = 0f;
	            invincibleAlternate = !invincibleAlternate;
	        }
	    }else{
	        invincibleTimer = 0f;
	        invincibleAlternateTimer = 0f;
	    }
	}

	public override void updateVertical(float dt) {
	    if (currentState == State.Knockback) {
			velocity.y += knockbackFriction.y * dt;

	        knockbackTimer += knockbackTimerSpeed * dt;
	        if (knockbackTimer >= 1f) {
	            currentState = State.Neutral;
	            knockbackTimer = 0f;

	            if (health <= 0) {
	                onDie();
	            }
	        }
	    }

	    base.updateVertical(dt);
	}

	public override void draw() {
	    spriteRect.x = (int)position.x;
		spriteRect.y = (int)position.y;
	    //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, parentRoom.TransitionMatrix);
	    
		//Color color = Color.white;	    
		if (IsInvincible) {
	        if (invincibleAlternate)
				;//color.A = 128;
	    }

	    //spriteBatch.Draw(screenManager.SpriteSheet, spriteRect.getScaled(scale), currentSourceRect[currentAnim], color);
	    //spriteBatch.End();
	}

	virtual public void setKnockback(Vector2 veloc) {
	    currentState = State.Knockback;
	    this.velocity = veloc;
	    knockbackFriction = -veloc;
	    knockbackFriction.Normalize();
	    knockbackFriction *= knockbackFrictionSpeed;
	    knockbackTimer = 0f;
	}

	public virtual void onDie() {
	    //parentRoom.removeEnemy(this);
	}
}