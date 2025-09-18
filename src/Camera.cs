using Microsoft.Xna.Framework;
using System;

namespace StickFight.src;

public class Camera
{
    public Vector2 Position { get; private set; }
    public Matrix Translation {  get; private set; }
    public float Zoom { get; private set; } = 1.5f;

    public void Update(Vector2 playerPosition)
    {
        Position = playerPosition;
        Position = ClampToMapBounds(new Rectangle(0, 0, Globals.ScreenDimensions.X, Globals.ScreenDimensions.Y));
        CalculateTranslationMatrix(playerPosition);
    }

    private void CalculateTranslationMatrix(Vector2 playerPosition)
    {
        var position = Matrix.CreateTranslation(-Position.X, -Position.Y, 0);
        var offset = Matrix.CreateTranslation(Globals.ScreenDimensions.X / 2f, Globals.ScreenDimensions.Y / 2f, 0);
        var zoom = Matrix.CreateScale(Zoom);

        Translation = position * zoom * offset;
    }

    private Vector2 ClampToMapBounds(Rectangle mapBounds)
    {
        float visibleWidth = Globals.ScreenDimensions.X / Zoom;
        float visibleHeight = Globals.ScreenDimensions.Y / Zoom;

        // the min and max values for the Camera-Position so that it does not show the outside of the map.
        float minX = mapBounds.Left + visibleWidth / 2;
        float maxX = Math.Max(mapBounds.Left + visibleWidth / 2, mapBounds.Right - visibleWidth / 2);
        float minY = mapBounds.Top + visibleHeight / 2;
        float maxY = Math.Max(mapBounds.Top + visibleHeight / 2, mapBounds.Bottom - visibleHeight / 2);

        var clampedPosition = new Vector2(
            MathHelper.Clamp(Position.X, minX, maxX),
            MathHelper.Clamp(Position.Y, minY, maxY));

        return clampedPosition;
    }
}
