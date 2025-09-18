using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StickFight.src.ui;
using System.Collections.Generic;

namespace StickFight.src.entities;

internal class Enemy : Entity
{
    private Texture2D walk_spritesheet;
    public bool isAlive;
    private Healthbar hpBar;

    // Knockback
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private const float knockbackDuration = 0.15f;

    // Animation
    private AnimationManager animationManager;
    public Enemy(ContentManager content)
    {
        LoadContent(content);
        position = new Vector2(100, 600);
        velocity = new Vector2(0, 0);
        width = 32;
        height = 32;
        speed = 1;
        maxSpeed = 10;
        collisionColor = Color.Purple * 0.25f;
        scale = 1f;
        maxHP = 3;
        hp = maxHP;
        hpBar = new Healthbar(maxHP, hp);
        isAlive = true;
    }
    public override void LoadContent(ContentManager content)
    {
        var idle_spritesheet = content.Load<Texture2D>("hero_idle_4");
        animationManager = new AnimationManager(idle: idle_spritesheet);
        base.LoadContent(content);
    }

    public void Update(GameTime gameTime, int windowHeight, Dictionary<Vector2, int>  adjacentTiles)
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
        EdgeCollision(windowHeight);
        TileCollisionDetection(adjacentTiles);

        position += velocity;
        ApplyFriction();
        hpBar.Update(position, CollisionRectangle.Width);
        animationManager.Update(currentState, remainingJumps);
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
        int knockbackAmount = 8;
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
        spriteBatch.Draw(
            animationManager.GetTexture(),
            new Rectangle((int)position.X, (int)position.Y, width, height),
            animationManager.GetFrame(),
            Color.White,
            0f,
            Vector2.Zero,
            flip,
            0f);
        hpBar.Draw(spriteBatch);
        base.Draw(spriteBatch);
    }
}
