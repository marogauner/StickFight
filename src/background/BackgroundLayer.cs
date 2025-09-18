using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StickFight.src.background;

internal class BackgroundLayer(Texture2D texture, float scroll)
{
    private Vector2 _position = Vector2.Zero;
    private Vector2 _position2 = Vector2.Zero;
    private Texture2D _texture = texture;
    private float _scrollFactor = scroll;

    public void Update(float moveAmount)
    {
        _position.X -= (moveAmount * _scrollFactor) * Globals.ElapsedSeconds;
        _position.X %= _texture.Width;

        if (_position.X >= 0)
        {
            _position2.X = _position.X - Globals.ScreenDimensions.X;
        }
        else
        {
            _position2.X = _position.X + Globals.ScreenDimensions.X;
        }
    }

    public void Draw()
    {
        Globals.SpriteBatch.Draw(
            _texture,
            destinationRectangle: new Rectangle((int)_position.X, (int)_position.Y, Globals.ScreenDimensions.X, Globals.ScreenDimensions.Y),
            color: Color.White
            );
        Globals.SpriteBatch.Draw(
            _texture,
            destinationRectangle: new Rectangle((int)_position2.X, (int)_position2.Y, Globals.ScreenDimensions.X, Globals.ScreenDimensions.Y),
            color: Color.White
            );
    }
}
