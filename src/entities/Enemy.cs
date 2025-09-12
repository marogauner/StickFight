using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StickFight.src.interfaces;
using StickFight.src.ui;
using System.Collections.Generic;

namespace StickFight.src.entities;

internal class Enemy : Entity
{
    private Texture2D walk_spritesheet;
    public bool isAlive;
    private Healthbar hpBar;
    public Enemy(ContentManager content)
    {
        LoadContent(content);
        position = new Vector2(100, 600);
        velocity = new Vector2(0, 0);
        width = 60;
        height = 100;
        speed = 1;
        maxSpeed = 10;
        collisionColor = Color.Purple * 0.25f;
        scale = 1f;
        maxHP = 3;
        hp = maxHP;
        hpBar = new Healthbar(maxHP, hp, content);
        isAlive = true;
    }
    public override void LoadContent(ContentManager content)
    {
        walk_spritesheet = content.Load<Texture2D>("stickman_punch");
        base.LoadContent(content);
    }

    public void Update(int windowHeight)
    {
        ApplyGravity();
        EdgeCollision(windowHeight);

        position += velocity;
        ApplyFriction();
        hpBar.Update(position, CollisionRectangle.Width);
    }

    public void TakeDamage()
    {
        if (!isAlive) return;

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

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            walk_spritesheet,
            new Rectangle((int)position.X, (int)position.Y, width, height),
            new Rectangle(0, 0, walk_spritesheet.Width, walk_spritesheet.Height),
            Color.White,
            0f,
            Vector2.Zero,
            flip,
            0f);
        hpBar.Draw(spriteBatch);
        base.Draw(spriteBatch);
    }
}
