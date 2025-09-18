using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StickFight.src.ui;

internal class Healthbar
{
    private int maxHealth;
    private int health;
    private int maxWidth;
    private int width;
    private int height;
    private Vector2 position;
    private Rectangle rect;
    private Texture2D texture;
    private Color color;
    public Healthbar(int maxHealth, int health)
    {
        this.maxHealth = maxHealth;
        this.health = health;
        this.maxWidth = 100;
        this.width = maxWidth;
        this.height = 10;
        color = Color.Green;
        LoadContent();
    }
    
    public void LoadContent()
    {
        var content = Globals.Content;
        texture = content.Load<Texture2D>("collisionRect");
    }

    public void Update(Vector2 entityPosition, int entityWidth)
    {
        UpdateHpBar();
        rect = new Rectangle(
            (int)entityPosition.X - (width / 2) + entityWidth/2,
            (int)entityPosition.Y - height * 2,
            width,
            height);
    }

    private void UpdateHpBar()
    {
        // Calculate width based on health ratio
        width = (int)(maxWidth * ((float)health / maxHealth));
    }

    public void SetHealth(int newHealth)
    {
        health = newHealth;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, rect, color);
    }
}
