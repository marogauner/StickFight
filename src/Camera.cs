using Microsoft.Xna.Framework;

namespace StickFight.src;

internal class Camera
{
    public Matrix Translation {  get; private set; }

    private void CalculateTranslationMatrix(Vector2 playerPosition)
    {
        var dx = (Globals.ScreenDimensions.X / 2) - playerPosition.X;
        var dy = (Globals.ScreenDimensions.Y / 2) - playerPosition.Y;
        Translation = Matrix.CreateTranslation(dx, dy, 0f);
    }

    public void Update(Vector2 playerPosition)
    {
        CalculateTranslationMatrix(playerPosition);
    }
}
