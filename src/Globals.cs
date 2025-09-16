using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StickFight.src.input;

namespace StickFight.src;

// class for storing global variables IDK
public static class Globals
{
    public static SpriteBatch SpriteBatch { get; set; }
    public static ContentManager Content { get; set; }
    public static InputManager Input { get; set; }

    public static Point ScreenDimensions { get; set; }
}
