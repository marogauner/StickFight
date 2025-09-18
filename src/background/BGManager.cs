using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace StickFight.src.background;

internal class BGManager
{
    private List<BackgroundLayer> backgroundLayers;

    public BGManager()
    {
        AddLayers();
    }

    public void AddLayers()
    {
        var content = Globals.Content;
        backgroundLayers = new List<BackgroundLayer>{
            new BackgroundLayer(content.Load<Texture2D>("forest_sky"), 0.1f),
            new BackgroundLayer(content.Load<Texture2D>("forest_mountain"), 0.2f),
            new BackgroundLayer(content.Load<Texture2D>("forest_back"), 0.4f),
            new BackgroundLayer(content.Load<Texture2D>("forest_mid"), 0.6f),
            new BackgroundLayer(content.Load<Texture2D>("forest_long"), 0.8f),
            new BackgroundLayer(content.Load<Texture2D>("forest_short"), 1.0f)
        };
    }

    public void Update(float moveAmount)
    {
        foreach (var layer in backgroundLayers)
        {
            layer.Update(moveAmount);
        }
    }

    public void Draw()
    {
        foreach(var layer in backgroundLayers)
        {
            layer.Draw();
        }
    }
}
