using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StickFight.src.ui;
using System;
using System.Collections.Generic;

namespace StickFight.src.entities;

internal class Player : Entity
{
    // Attack
    private int facingDirection = 1;
    private Rectangle PunchHurtBox;
    private MouseState previousMouseState;
    private MouseState currentMouseState;
    private KeyboardState currentKeyboardState;
    private KeyboardState previousKeyboardState;

    // Healthbar
    Healthbar hpBar;

    // Animation
    AnimationManager animationManager;

    // State Machine
    const int JUMP_VELOCITY = 20;
    private int remainingJumps = 2;

    public enum PlayerStates
    {
        Idle,
        Walk,
        Jump,
        Fall,
    }

    private PlayerStates currentState = PlayerStates.Idle;
    private PlayerStates previousState = PlayerStates.Idle;

    public Player(ContentManager content)
    {
        LoadContent(content);
        width = 32;
        height = 32;
        position = new Vector2(100, 400);
        velocity = new Vector2(0, 0);
        speed = 1;
        maxSpeed = 10;
        collisionColor = Color.Blue * 0.25f;
        scale = 1f;
        maxHP = 3;
        hp = maxHP;
        hpBar = new Healthbar(maxHP, hp, content);
        previousMouseState = Mouse.GetState();
        currentMouseState = previousMouseState;
        previousKeyboardState = Keyboard.GetState();
        currentKeyboardState = previousKeyboardState;
        debug = true;
    }

    public override void LoadContent(ContentManager content)
    {
        var idle_spritesheet = content.Load<Texture2D>("hero_idle_4");
        var walk_spritesheet = content.Load<Texture2D>("hero_run_6");
        var jump_spritesheet = content.Load<Texture2D>("hero_jump_3");
        var falling_spritesheet = content.Load<Texture2D>("hero_fall_3");
        var doublejump_spritesheet = content.Load<Texture2D>("hero_doublejump_3");
        var attack_spritesheet = content.Load<Texture2D>("hero_attack_nosword_4");
        animationManager = new AnimationManager(
            idle_spritesheet,
            walk_spritesheet,
            falling_spritesheet,
            jump_spritesheet,
            doublejump_spritesheet,
            attack_spritesheet);
        base.LoadContent(content);
    }

    public void Update(int windowHeight, List<Enemy> entities, Dictionary<Vector2, int> adjacentTiles)
    {
        ApplyGravity();
        EdgeCollision(windowHeight);
        TileCollisionDetection(adjacentTiles); // CHECK ORDER 
        HandleUserInput(entities);
        SetSpriteFlip();

        position += velocity;

        UpdateAttackHurtBox();
        hpBar.Update(position, CollisionRectangle.Width);
        animationManager.Update(currentState, remainingJumps);
    }

    private void HandleUserInput(List<Enemy> entities)
    {
        currentMouseState = Mouse.GetState();
        currentKeyboardState = Keyboard.GetState();

        // Horizontal Movement Input
        if (currentKeyboardState.IsKeyDown(Keys.A))
        {
            velocity.X += speed * -1;
            velocity.X = Math.Max(velocity.X, -maxSpeed);
        }
        if (currentKeyboardState.IsKeyDown(Keys.D))
        {
            velocity.X += speed * 1;
            velocity.X = Math.Min(velocity.X, maxSpeed);
        }
        else
        {
            velocity.X = 0;
        }
        // Inputs
        bool jumpPressed = ((currentKeyboardState.IsKeyDown(Keys.W) && previousKeyboardState.IsKeyUp(Keys.W)) ||
                    (currentKeyboardState.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space)));

        bool LMBPressed = (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released);


        switch (currentState)
        {
            case PlayerStates.Idle:
                // Jump
                if (jumpPressed && remainingJumps > 0)
                {
                    currentState = PlayerStates.Jump;
                    EnterJumpState();
                }
                // Walk
                if (currentKeyboardState.IsKeyDown(Keys.A) || currentKeyboardState.IsKeyDown(Keys.D))
                {
                    currentState = PlayerStates.Walk;
                    EnterWalkState();
                }
                // Fall
                if (velocity.Y > 0)
                {
                    currentState = PlayerStates.Fall;
                    EnterFallState();
                }
                break;
            case PlayerStates.Walk:
                // Idle
                if (velocity.X == 0)
                {
                    currentState = PlayerStates.Idle;
                    EnterIdleState();
                }
                // Jump
                if (jumpPressed && remainingJumps > 0)
                {
                    currentState = PlayerStates.Jump;
                    EnterJumpState();
                }
                // Fall
                if (velocity.Y > 0)
                {
                    currentState = PlayerStates.Fall;
                    EnterFallState();
                }
                break;
            case PlayerStates.Jump:
                // Jump
                if (jumpPressed && remainingJumps > 0)
                {
                    currentState = PlayerStates.Jump;
                    EnterJumpState();
                }
                // Fall
                if (velocity.Y > 0)
                {
                    currentState = PlayerStates.Fall;
                    EnterFallState();
                }
                break;
            case PlayerStates.Fall:
                // Jump
                if (jumpPressed && remainingJumps > 0)
                {
                    currentState = PlayerStates.Jump;
                    EnterJumpState();
                }
                // Idle, Walk
                if (velocity.Y == 0)
                {
                    if (velocity.X == 0)
                    {
                        currentState = PlayerStates.Idle;
                        EnterIdleState();
                    }
                    else
                    {
                        currentState = PlayerStates.Walk;
                        EnterWalkState();
                    }
                }
                break;
            default:
                break;
        }

        if (LMBPressed)
        {
            animationManager.StartAttackAnimation();
            Attack(entities);
        }
        previousMouseState = currentMouseState;
        previousKeyboardState = currentKeyboardState;
    }

    // THIS CODE SUX
    private void EnterJumpState()
    {
        velocity.Y = -JUMP_VELOCITY;
        remainingJumps -= 1;
    }
    private void EnterIdleState()
    {
        remainingJumps = 2;
    }
    private void EnterWalkState()
    {
        remainingJumps = 2;
    }
    private void EnterFallState()
    {
        if (remainingJumps == 2)
        {
            remainingJumps = 1;
        }
    }

    void UpdateAttackHurtBox()
    {
        if (velocity.X > 0) { facingDirection = 1; }
        else if (velocity.X < 0) { facingDirection = -1; }

        var HurtBoxWidth = 100;
        var HurtBoxHeight = 50;

        if (facingDirection < 0)
        {
            PunchHurtBox = new Rectangle(
            CollisionRectangle.X - HurtBoxWidth,
            CollisionRectangle.Y,
            HurtBoxWidth,
            HurtBoxHeight
        );
        }
        else
        {
            PunchHurtBox = new Rectangle(
            CollisionRectangle.X + CollisionRectangle.Width,
            CollisionRectangle.Y,
            HurtBoxWidth,
            HurtBoxHeight
        );
        }
    }

    void Attack(List<Enemy> entities)
    {
        foreach(var entity in entities)
        {
            if (PunchHurtBox.Intersects(entity.CollisionRectangle))
            {
                entity.TakeDamage();
            }
        }
    }

    private void TileCollisionDetection(Dictionary<Vector2, int> adjacentTiles)
    {
        // Horizontal collision
        Vector2 newPosition = position;
        newPosition.X += velocity.X;
        Rectangle futureRectX = new Rectangle(
            (int)newPosition.X,
            (int)position.Y,
            CollisionRectangle.Width,
            CollisionRectangle.Height
            );

        foreach (var tile in adjacentTiles)
        {
            Rectangle tileCollisionRectangle = new(
                (int)tile.Key.X,
                (int)tile.Key.Y,
                32,
                32
            );

            if (velocity.X > 0 && RightCollides(tileCollisionRectangle, futureRectX))
            {
                position.X = tileCollisionRectangle.Left - CollisionRectangle.Width;
                velocity.X = 0;
            }
            if (velocity.X < 0 && LeftCollides(tileCollisionRectangle, futureRectX))
            {
                position.X = tileCollisionRectangle.Right;
                velocity.X = 0;
            }
        }

        // Vertical collision
        newPosition = position;
        newPosition.Y += velocity.Y;
        Rectangle futureRectY = new Rectangle(
            (int)position.X,
            (int)newPosition.Y,
            CollisionRectangle.Width,
            CollisionRectangle.Height
            );

        foreach (var tile in adjacentTiles)
        {
            Rectangle tileCollisionRectangle = new(
                (int)tile.Key.X,
                (int)tile.Key.Y,
                32,
                32
            );

            if (velocity.Y < 0 && TopCollides(tileCollisionRectangle, futureRectY))
            {
                position.Y = tileCollisionRectangle.Bottom;
                velocity.Y = 0;
            }
            if (velocity.Y > 0 && BottomCollides(tileCollisionRectangle, futureRectY))
            {
                position.Y = tileCollisionRectangle.Top - CollisionRectangle.Height;
                velocity.Y = 0;
                isOnGround = true;
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // Draw Attack
        if (debug)
        {
            spriteBatch.Draw(collisionTexture, PunchHurtBox, Color.Red);
        }
        // State
        spriteBatch.DrawString(font, currentState.ToString(), new Vector2(position.X + width * scale / 2 - font.MeasureString(currentState.ToString()).X / 2, position.Y - 40), Color.White);
        spriteBatch.DrawString(font, remainingJumps.ToString(), new Vector2(position.X + width * scale / 2 - font.MeasureString(remainingJumps.ToString()).X / 2, position.Y - 80), Color.White);
        hpBar.Draw(spriteBatch);
        spriteBatch.Draw(
            animationManager.GetTexture(),
            new Rectangle((int)position.X, (int)position.Y, width, height),
            //new Rectangle((int)position.X, (int)position.Y, width, height),
            animationManager.GetFrame(),
            Color.White,
            0f,
            Vector2.Zero,
            flip,
            0f);
        base.Draw(spriteBatch);
    }
}
