using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StickFight.src;

internal class Animation(int numFrames, int width, int height, Texture2D texture, int animationSpeed = 30, bool loop = true)
{
    private Texture2D texture = texture;
    private int activeFrame = 0;
    private int numFrames = numFrames;
    private int counter = 0;
    private int animationSpeed = animationSpeed;
    private int width = width;
    private int height = height;
    public bool animationFinished = false;

    public void Update()
    {
        counter++;
        if (counter >= animationSpeed)
        {
            counter = 0;
            activeFrame++;

            if (activeFrame == numFrames)
            {
                activeFrame = 0;
                animationFinished = true;
            }
        }
    }

    public Rectangle GetFrame()
    {
        return new Rectangle(activeFrame * width, 0, width, height);
    }

    public void ResetAnimation()
    {
        counter = 0;
        animationFinished = false;
        activeFrame = 0;
    }
}
