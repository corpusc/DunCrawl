//using System;
//using System.Collections.Generic;
//
//
//public class Player___MinimalisticDungeonCrawler : EnemyEntity
//{
//	//Texture2D spriteSheet;
//	
//	static readonly Rectangle[] WalkingUp = new Rectangle[] { new Rectangle(7 * 16, 0, 16, 16), new Rectangle(8 * 16, 0, 16, 16) };
//	static readonly Rectangle[] WalkingRight = new Rectangle[] { new Rectangle(7 * 16, 16, 16, 16), new Rectangle(8 * 16, 16, 16, 16) };
//	static readonly Rectangle[] WalkingDown = new Rectangle[] { new Rectangle(7 * 16, 32, 16, 16), new Rectangle(8 * 16, 32, 16, 16) };
//	static readonly Rectangle[] WalkingLeft = new Rectangle[] { new Rectangle(7 * 16, 48, 16, 16), new Rectangle(8 * 16, 48, 16, 16) };
//	
//	static readonly Rectangle[] SwordUp = new Rectangle[] { new Rectangle(1 * 16, 6 * 16, 16, 16), new Rectangle(0 * 16, 6 * 16, 16, 16) };
//	static readonly Rectangle[] SwordRight = new Rectangle[] { new Rectangle(3 * 16, 6 * 16, 16, 16), new Rectangle(2 * 16, 6 * 16, 16, 16) };
//	static readonly Rectangle[] SwordDown = new Rectangle[] { new Rectangle(5 * 16, 6 * 16, 16, 16), new Rectangle(4 * 16, 6 * 16, 16, 16) };
//	static readonly Rectangle[] SwordLeft = new Rectangle[] { new Rectangle(7 * 16, 6 * 16, 16, 16), new Rectangle(6 * 16, 6 * 16, 16, 16) };
//	
//	SoundEffect enemyHit;
//	SoundEffect playerHit;
//	
//	// protected Rectangle[] currentSourceRect;
//	
//	private enum FaceDirection
//	{
//		Left,
//		Up,
//		Right,
//		Down
//	}
//	
//	public Map.Room ParentRoom
//	{
//		get { return parentRoom; }
//		set { parentRoom = value; }
//	}
//	
//	FaceDirection faceDir = FaceDirection.Down;
//	
//	private enum AttackState
//	{
//		Diagonal,
//		Straight
//	}
//	
//	Rectangle attackSourceRect;
//	
//	float stateTimer = 0f;
//	readonly float stateTimerSpeed = 0.1f;//0.15f;
//	
//	//State currentState = State.Neutral;
//	AttackState attackState = AttackState.Diagonal;
//	
//	public Player(Screens.ScreenManager screenManager)
//		: base(screenManager, Vector2.Zero, null, null)
//	{
//		position.X = 0;
//		position.Y = 0;
//		collisionOffsetX = 8;
//		collisionOffsetY = 16;
//		collisionBox.width = 16;
//		collisionBox.height = 16;
//		spriteRect.width = 32;
//		spriteRect.height = 32;
//		currentSourceRect = WalkingDown;
//		attackRect.width = 32;
//		attackRect.height = 32;
//		
//		health = 6;
//		maxHealth = 6;
//		Attack = 1;
//		
//		player = this;
//		countTowardsEnemyCount = false;
//	}
//	
//	public override void updateHorizontal(float dt)
//	{
//		if( currentState == State.Neutral )
//			acceleration = Vector2.Zero;
//		
//		base.updateHorizontal(dt);
//		
//		if (currentState == State.Attacking)
//		{
//			stateTimer += stateTimerSpeed * dt;
//			if (attackState == AttackState.Diagonal)
//			{
//				if (stateTimer >= 1f)
//				{
//					stateTimer = 0f;
//					attackState = AttackState.Straight;
//				}
//				else
//				{
//					switch (faceDir)
//					{
//					case FaceDirection.Up:
//						attackRect.x = (int)position.X + 4 * 6;
//						attackRect.y = (int)position.Y - 4 * 6;
//						attackSourceRect = SwordUp[0];
//						break;
//						
//					case FaceDirection.Right:
//						attackRect.x = (int)position.X + 4 * 4;
//						attackRect.y = (int)position.Y + 4 * 6;
//						attackSourceRect = SwordRight[0];
//						break;
//						
//					case FaceDirection.Down:
//						attackRect.x = (int)position.X - 4 * 6;
//						attackRect.y = (int)position.Y + 4 * 4;
//						attackSourceRect = SwordDown[0];
//						break;
//						
//					case FaceDirection.Left:
//						attackRect.x = (int)position.X - 4 * 4;
//						attackRect.y = (int)position.Y - 4 * 6;
//						attackSourceRect = SwordLeft[0];
//						break;
//						
//					default:
//						currentState = State.Neutral;
//						break;
//					}
//				}
//			}
//			
//			if (attackState == AttackState.Straight)
//			{
//				if (stateTimer >= 1f)
//				{
//					stateTimer = 0f;
//					currentState = State.Neutral;
//				}
//				else
//				{
//					switch (faceDir)
//					{
//					case FaceDirection.Up:
//						attackRect.x = (int)position.X;
//						attackRect.y = (int)position.Y - 4 * 8;
//						attackSourceRect = SwordUp[1];
//						break;
//						
//					case FaceDirection.Right:
//						attackRect.x = (int)position.X + 4 * 6;
//						attackRect.y = (int)position.Y;
//						attackSourceRect = SwordRight[1];
//						break;
//						
//					case FaceDirection.Down:
//						attackRect.x = (int)position.X;
//						attackRect.y = (int)position.Y + 4 * 8;
//						attackSourceRect = SwordDown[1];
//						break;
//						
//					case FaceDirection.Left:
//						attackRect.x = (int)position.X - 4 * 6;
//						attackRect.y = (int)position.Y + 4;
//						attackSourceRect = SwordLeft[1];
//						break;
//						
//					default:
//						currentState = State.Neutral;
//						break;
//					}
//				}     
//			}
//		}
//		
//		if (Math.Abs(velocity.X * velocity.X + velocity.Y * velocity.Y) <= 0.000001f)
//			return;
//		
//		animTimer += animSpeed * dt;
//		if (animTimer >= 1f)
//		{
//			animTimer = 0f;
//			currentAnim = (currentAnim + 1) % animCount;
//		}
//	}
//	
//	public override void loadContent(ContentManager content)
//	{
//		spriteSheet = content.Load<Texture2D>(@"data\Spritesheet");
//		enemyHit = content.Load<SoundEffect>(@"sounds\Hit");
//		playerHit = content.Load<SoundEffect>(@"sounds\\Hit2");
//	}
//	
//	public override void draw(GameTime gameTime)
//	{
//		base.draw(gameTime);
//		
//		SpriteBatch spriteBatch = screenManager.SpriteBatch;
//		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, parentRoom.TransitionMatrix);
//		
//		if (currentState == State.Attacking)
//		{
//			spriteBatch.Draw(spriteSheet, attackRect.getScaled(scale), attackSourceRect, Color.White);
//		}
//		
//		spriteBatch.End();
//	}
//	
//	public override void handleInput(Input.InputState inputState)
//	{
//		if( currentState == State.Attacking )
//			return;
//		
//		if (currentState == State.Neutral && (inputState.IsNewButtonPress(Buttons.A) || inputState.IsNewKeyPress(Keys.Space)))
//		{
//			currentState = State.Attacking;
//			attackState = AttackState.Diagonal;
//			velocity = Vector2.Zero;
//			stateTimer = 0;
//			return;
//		}
//		
//		if (currentState == State.Neutral)
//		{
//			velocity = inputState.currentGamePadState.ThumbSticks.Left;
//			velocity.Y *= -1f;
//			acceleration = Vector2.Zero;
//		}
//		else
//		{
//			acceleration = inputState.currentGamePadState.ThumbSticks.Left * 0.1f;
//			acceleration.Y *= -1f;
//			return;
//		}
//		
//		if (velocity.X < 0f && Math.Abs(velocity.X) > Math.Abs(velocity.Y) )
//		{
//			currentSourceRect = WalkingLeft;
//			faceDir = FaceDirection.Left;
//		}
//		else if (velocity.X > 0f && Math.Abs(velocity.X) > Math.Abs(velocity.Y))
//		{
//			currentSourceRect = WalkingRight;
//			faceDir = FaceDirection.Right;
//		}
//		else if (velocity.Y < 0f )
//		{
//			currentSourceRect = WalkingUp;
//			faceDir = FaceDirection.Up;
//		}
//		else if (velocity.Y > 0f)
//		{
//			currentSourceRect = WalkingDown;
//			faceDir = FaceDirection.Down;
//		}
//		
//		if (Keyboard.GetState().GetPressedKeys().Length > 0)
//		{
//			velocity = Vector2.Zero;
//			
//			if( inputState.currentKeyboardState.IsKeyDown(Keys.W) )
//			{
//				currentSourceRect = WalkingUp;
//				faceDir = FaceDirection.Up;
//				velocity.Y = -1;
//			}
//			
//			if (inputState.currentKeyboardState.IsKeyDown(Keys.A))
//			{
//				currentSourceRect = WalkingLeft;
//				faceDir = FaceDirection.Left;
//				velocity.X = -1;
//			}
//			
//			if (inputState.currentKeyboardState.IsKeyDown(Keys.S))
//			{
//				currentSourceRect = WalkingDown;
//				faceDir = FaceDirection.Down;
//				velocity.Y = 1;
//			}
//			
//			if (inputState.currentKeyboardState.IsKeyDown(Keys.D))
//			{
//				currentSourceRect = WalkingRight;
//				faceDir = FaceDirection.Right;
//				velocity.X = 1;
//			}
//			
//			if (velocity != Vector2.Zero)
//				velocity.Normalize();
//		}
//		
//		//velocity.Normalize();
//		velocity *= maxSpeed;
//		
//		animSpeed = velocity.Length() / 18f;
//	}
//	
//	public void onHitting(EnemyEntity other)
//	{
//		other.Health -= Attack;
//		Vector2 knockbackVelocity = other.Position - position;
//		knockbackVelocity.Normalize();
//		knockbackVelocity *= 4f;
//		other.setKnockback(knockbackVelocity);
//		enemyHit.Play();
//	}
//	
//	public void onGettingHit(EnemyEntity other)
//	{
//		Health -= other.Attack;
//		Vector2 knockbackVelocity = position - other.Position;
//		knockbackVelocity.Normalize();
//		knockbackVelocity *= 4f;
//		setKnockback(knockbackVelocity);
//		playerHit.Play();
//	}
//}
