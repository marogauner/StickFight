using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace StickFight.src;

internal sealed class Tilemap
{
    private Dictionary<Vector2, int> ground;
    private Texture2D spritesheet;
    private const int tileSize = 16;

    private int numTilesPerRow = 1;
    private int displayTileSize = 32;


    public Tilemap(string filepath, Texture2D texture, int screenWidth, int screenHeight)
    {
        ground = ParseCSV(filepath);
        spritesheet = texture;

        // Calculate displayTileSize
        int tilesHorizontally = 60;
        displayTileSize = screenWidth / tilesHorizontally;
    }

    // Parse csv file into <coordinates, tileType>
    public Dictionary<Vector2, int> ParseCSV(string filepath)
    {
        Dictionary<Vector2, int> res = [];
        StreamReader reader = new(filepath);
        int y = 0;
        string line;
        
        while ((line = reader.ReadLine()) != null)
        {
            string[] items = line.Split(',');
            for (int x = 0; x < items.Length; x++)
            {
                if (int.TryParse(items[x], out int value))
                {
                    if (value >= 0)
                    {
                        res[new Vector2(x, y)] = value;
                    }
                }
            }
            y++;
        }
        return res;
    }

    public Dictionary<Vector2, int> GetAdjacentTiles(Vector2 playerPosition)
    {
        Dictionary<Vector2, int> res = [];
        
        foreach (var item in ground)
        {
            if (Math.Abs((item.Key.X * displayTileSize) - playerPosition.X) <= tileSize * 8 &&
               (Math.Abs((item.Key.Y * displayTileSize) - playerPosition.Y) <= tileSize * 8))
            {
                res.Add(item.Key * displayTileSize, item.Value);
            }
            //Debug.WriteLine($"{item.Key.X * displayTileSize}, {item.Key.Y * displayTileSize}");
        }
        return res;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach(var item in ground)
        {
            Rectangle drect = new(
                (int)item.Key.X * displayTileSize,
                (int)item.Key.Y * displayTileSize,
                displayTileSize,
                displayTileSize
                );

            int x = item.Value % numTilesPerRow;
            int y = item.Value / numTilesPerRow;

            Rectangle srect = new(
                    x * tileSize,
                    y * tileSize,
                    tileSize,
                    tileSize
                );
            spriteBatch.Draw(spritesheet, drect, srect, Color.White);
        }
    }
}
