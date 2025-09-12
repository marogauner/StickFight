using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StickFight.src.interfaces;

namespace StickFight.src.objects;

internal class Platform : ICollidable
{
    private Texture2D texture;
    private Vector2 position;
    private int width;
    private int height;
    public Rectangle CollisionRectangle => new Rectangle((int)position.X, (int)position.Y, width, height);

    public Platform(int x, int y, int w, int h, ContentManager content)
    {
        LoadContent(content);
        position = new Vector2(x, y);
        width = w;
        height = h;
    }

    public void LoadContent(ContentManager content)
    {
        texture = content.Load<Texture2D>("collisionRect");
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        //spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, width, height), Color.White);
        spriteBatch.Draw(texture, CollisionRectangle, Color.Red * 0.2f);
    }
}
