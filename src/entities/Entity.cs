using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StickFight.src.interfaces;
using System;
using System.Collections.Generic;

namespace StickFight.src.entities;

internal class Entity
{
    public Vector2 position;
    public Vector2 velocity;
    protected float speed;
    protected float maxSpeed;
    protected float maxGravity = 13f;
    protected float friction = 0.8f;
    protected bool isOnGround;
    protected Texture2D collisionTexture;
    protected float scale;
    protected SpriteEffects flip = SpriteEffects.None;
    protected bool debug = false;
    // Draw HP
    protected int maxHP;
    protected int hp;
    protected SpriteFont font;

    protected int width;
    protected int height;
    public Rectangle CollisionRectangle => new Rectangle(
        (int)position.X,
        (int)position.Y,
        (int)(width * scale),
        (int)(height * scale)
        );
    protected Color collisionColor = Color.Yellow * 0.2f;

    public virtual void LoadContent(ContentManager content)
    {
        collisionTexture = content.Load<Texture2D>("collisionRect");
        font = content.Load<SpriteFont>("score");
    }

    //public virtual void Update(int windowHeight, List<ICollidable> platforms) { }
    protected void ApplyGravity()
    {
        velocity.Y += 1f;
        velocity.Y = Math.Min(velocity.Y, maxGravity);
    }

    protected void EdgeCollision(int windowHeight)
    {
        // Bottom Screen
        if (position.Y + CollisionRectangle.Height >= windowHeight)
        {
            position.Y = windowHeight - CollisionRectangle.Height;
            velocity.Y = 0;
            isOnGround = true;
        }
    }
    protected void ApplyFriction()
    {
        if (velocity.X > 0)
        {
            velocity.X -= friction;
            if (velocity.X < 0) { velocity.X = 0; }
        }
        else if (velocity.X < 0)
        {
            velocity.X += friction;
            if (velocity.X > 0) { velocity.X = 0; }
        }
    }

    protected void SetSpriteFlip()
    {
        flip = velocity.X switch
        {
            > 0 => SpriteEffects.None,
            < 0 => SpriteEffects.FlipHorizontally,
            _ => SpriteEffects.FlipHorizontally,
        };
    }

    #region Collision
    protected bool TopCollides(Rectangle tileRect, Rectangle futureRect)
    {
        return futureRect.Top < tileRect.Bottom &&
               futureRect.Bottom > tileRect.Bottom &&
               futureRect.Right > tileRect.Left &&
               futureRect.Left < tileRect.Right;
    }

    protected bool BottomCollides(Rectangle tileRect, Rectangle futureRect)
    {
        return futureRect.Top < tileRect.Bottom &&
               futureRect.Bottom > tileRect.Top &&
               futureRect.Right > tileRect.Left &&
               futureRect.Left < tileRect.Right;
    }

    protected bool RightCollides(Rectangle tileRect, Rectangle futureRect)
    {
        return futureRect.Top < tileRect.Bottom &&
               futureRect.Bottom > tileRect.Top &&
               futureRect.Right > tileRect.Left &&
               futureRect.Left < tileRect.Left;
    }

    protected bool LeftCollides(Rectangle tileRect, Rectangle futureRect)
    {
        return futureRect.Top < tileRect.Bottom &&
               futureRect.Bottom > tileRect.Top &&
               futureRect.Right > tileRect.Right &&
               futureRect.Left < tileRect.Right;
    }
    #endregion
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (debug)
        {
            spriteBatch.Draw(collisionTexture, CollisionRectangle, collisionColor);
        }
        spriteBatch.DrawString(font, hp.ToString(), new Vector2(position.X + width * scale / 2, position.Y - 20), Color.LightGreen);
    }
}
