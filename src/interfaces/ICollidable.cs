using Microsoft.Xna.Framework;

namespace StickFight.src.interfaces;

internal interface ICollidable
{
    Rectangle CollisionRectangle { get; }
}
