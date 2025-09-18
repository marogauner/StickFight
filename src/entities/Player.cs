using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StickFight.src.ui;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static StickFight.src.input.MouseButtons;

namespace StickFight.src.entities;

public class Player : Entity
{
    // Attack
    private int facingDirection = 1;
    private Rectangle PunchHurtBox;

    // Healthbar
    private Healthbar hpBar;

    // Animation
    private AnimationManager animationManager;

    public PlayerIndex playerIndex;

    public bool isAlive;

    // Knockback
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private const float knockbackDuration = 0.15f;

    //
    public int tilemapDisplaySize;
    public Player(PlayerIndex index, int tileDisplaySize)
    {
        LoadContent();
        width = 32;
        height = 32;
        position = new Vector2(100, 400);
        velocity = new Vector2(0, 0);
        speed = 1.5f;
        maxSpeed = 10;
        collisionColor = Color.Blue * 0.25f;
        scale = 1f;
        maxHP = 3;
        hp = maxHP;
        hpBar = new Healthbar(maxHP, hp);
        debug = true;
        playerIndex = index;
        isAlive = true;
        tilemapDisplaySize = tileDisplaySize;
        SetPlayerPositions();
    }

    public void LoadContent()
    {
        var content = Globals.Content;
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

    public void Update(GameTime gameTime, int windowHeight, List<Player> players, Dictionary<Vector2, int> adjacentTiles)
    {
        // Knockback
        if (isKnockedBack)
        {
            knockbackTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }
            gravityAmount = 0.25f;
        }
        else
        {
            gravityAmount = 1f;
        }

        ApplyGravity();
        //EdgeCollision(windowHeight);
        TileCollisionDetection(adjacentTiles); // CHECK ORDER

        if (playerIndex == PlayerIndex.One)
        {
            DecideInputForPlayerOne(players);
        }
        else
        {
            HandleGamepadInput(players);
        }
        SetSpriteFlip();
        ApplyFriction();

        position += velocity;

        UpdateAttackHurtBox();
        hpBar.Update(position, CollisionRectangle.Width);
        animationManager.Update(currentState, remainingJumps);
    }

    private void DecideInputForPlayerOne(List<Player> players)
    {
        if (playerIndex != PlayerIndex.One) return;
        var gamepad_one = Globals.Input.GetGamePad(playerIndex);
        if (gamepad_one.IsConnected)
        {
            HandleGamepadInput(players);
        }
        else
        {
            HandleMouseAndKeyboardInput(players);
        }
    }
    private void HandleMouseAndKeyboardInput(List<Player> players)
    {
        if (isKnockedBack) return;
        if (playerIndex != 0) return;

        var mouse = Globals.Input.Mouse;
        var keyboard = Globals.Input.Keyboard;

        // Horizontal Movement Input
        if (keyboard.IsKeyDown(Keys.A) && keyboard.IsKeyUp(Keys.D))
        {
            velocity.X += speed * -1;
            velocity.X = Math.Max(velocity.X, -maxSpeed);
            isFacingRight = false;
        }
        else if (keyboard.IsKeyDown(Keys.D) && keyboard.IsKeyUp(Keys.A))
        {
            velocity.X += speed * 1;
            velocity.X = Math.Min(velocity.X, maxSpeed);
            isFacingRight = true;
        }
        else
        {
            velocity.X = 0;
        }

        // Inputs
        bool jumpPressed = (keyboard.WasKeyJustPressed(Keys.W) || keyboard.WasKeyJustPressed(Keys.Space));
        bool attackPressed = (mouse.WasButtonJustPressed(MouseButton.Left));
        
        HandleStates(jumpPressed);

        if (attackPressed)
        {
            animationManager.playerIsAttacking = true;
            animationManager.StartAttackAnimation();
            Attack(players);
        }
    }
    private void HandleGamepadInput(List<Player> players)
    {
        if (isKnockedBack) return;
        var input = Globals.Input.GetGamePad(playerIndex);

        // Deadzone
        const float deadzone = 0;

        // Horizontal Movement Input
        if (input.LeftThumbstick.X < -deadzone || input.IsButtonDown(Buttons.DPadLeft))

        {
            velocity.X += speed * -1;
            velocity.X = Math.Max(velocity.X, -maxSpeed);
            isFacingRight = false;
        }
        else if (input.LeftThumbstick.X > deadzone)
        {
            velocity.X += speed * 1;
            velocity.X = Math.Min(velocity.X, maxSpeed);
            isFacingRight = true;
        }
        else
        {
            velocity.X = 0;
        }

        // Inputs
        bool jumpPressed = input.WasButtonJustPressed(Buttons.A);
        bool attackPressed = input.WasButtonJustPressed(Buttons.RightShoulder);

        HandleStates(jumpPressed);

        if (attackPressed)
        {
            animationManager.playerIsAttacking = true;
            animationManager.StartAttackAnimation();
            Attack(players);
        }
    }
    #region STATE TRANSITIONS
    private void HandleStates(bool jumpPressed)
    {
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
                if (velocity.X != 0) // TEMP, switch back to input check
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
                if (velocity.Y >= 0)
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
    #endregion

    void UpdateAttackHurtBox()
    {
        if (velocity.X > 0) { facingDirection = 1; }
        else if (velocity.X < 0) { facingDirection = -1; }

        var HurtBoxWidth = 64;
        var HurtBoxHeight = 32;

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

    void Attack(List<Player> players)
    {
        foreach(var p in players)
        {
            if (p.playerIndex == playerIndex) continue;
            if (PunchHurtBox.Intersects(p.CollisionRectangle))
            {
                p.TakeDamage(position);
                Debug.WriteLine("HIT");
            }
        }
    }

    public void TakeDamage(Vector2 incomingPosition)
    {
        if (!isAlive) return;

        ApplyKnockback(incomingPosition);
        if (hp > 0)
        {
            hp -= 1;
        }
        if (hp <= 0)
        {
            isAlive = false;
        }
        hpBar.SetHealth(hp);
    }
    private void ApplyKnockback(Vector2 incomingPosition)
    {
        int knockbackAmount = 20;
        Vector2 knockbackdirection = (position - incomingPosition);
        knockbackdirection.Normalize();
        velocity += knockbackdirection * knockbackAmount;
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
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
                tilemapDisplaySize,
                tilemapDisplaySize
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
                tilemapDisplaySize,
                tilemapDisplaySize
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

    private void SetPlayerPositions()
    {
        position = playerIndex switch
        {
            PlayerIndex.One => new Vector2(100, 400),
            PlayerIndex.Two => new Vector2(300, 400),
            PlayerIndex.Three => new Vector2(500, 400),
            PlayerIndex.Four => new Vector2(700, 400),
            _ => new Vector2(900, 400)
        };
    }

    public void Respawn()
    {
        isAlive = true;
        hp = maxHP;
        hpBar.SetHealth(hp);
        SetPlayerPositions();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // Draw Attack
        if (debug)
        {
            spriteBatch.Draw(collisionTexture, PunchHurtBox, Color.Red * 0.2f);
        }
        // State
        spriteBatch.DrawString(font, currentState.ToString(), new Vector2(position.X + width * scale / 2 - font.MeasureString(currentState.ToString()).X / 2, position.Y - 40), Color.White);
        spriteBatch.DrawString(font, remainingJumps.ToString(), new Vector2(position.X + width * scale / 2 - font.MeasureString(remainingJumps.ToString()).X / 2, position.Y - 80), Color.White);
        hpBar.Draw(spriteBatch);
        spriteBatch.Draw(
            animationManager.GetTexture(),
            new Rectangle((int)position.X, (int)position.Y, width, height),
            animationManager.GetFrame(),
            Color.White,
            0f,
            Vector2.Zero,
            flip,
            0f);
        base.Draw(spriteBatch);
    }
}
