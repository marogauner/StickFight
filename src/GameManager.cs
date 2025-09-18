using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StickFight.src.background;
using StickFight.src.entities;
using StickFight.src.input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;

namespace StickFight.src;

public class GameManager
{
    // Declarations
    public List<Player> players;
    private List<Player> deadPlayers;
    private float respawntimer = 10f;

    private Tilemap tilemap;
    private Texture2D rectTexture;
    private BGManager bgManager = new();
    private Vector2 previousCameraPosition;
    private Vector2 currentCameraPosition;

    public GameManager()
    {
        Globals.Input = new InputManager();
    }

    public void Initialize()
    {

    }

    public void LoadContent()
    {
        // Map
        Texture2D mapTextureAtlas = Globals.Content.Load<Texture2D>("world_tileset");
        rectTexture = Globals.Content.Load<Texture2D>("collisionRect");
        tilemap = new("../../../data/stickfight_map1.csv", mapTextureAtlas, Globals.ScreenDimensions.X, Globals.ScreenDimensions.Y);

        // Initializing
        players = new List<Player>  {
            new Player(PlayerIndex.One, tilemap.DisplayTileSize),
            new Player(PlayerIndex.Two, tilemap.DisplayTileSize),
            new Player(PlayerIndex.Three, tilemap.DisplayTileSize),
            new Player(PlayerIndex.Four, tilemap.DisplayTileSize)
        };
        deadPlayers = [];
    }

    public void Update(GameTime gameTime, Vector2 cameraPos)
    {
        Globals.Update(gameTime);

        foreach (var player in players)
        {
            player.Update(gameTime, Globals.ScreenDimensions.X, players, tilemap.GetAdjacentTiles(player.position));
        }

        currentCameraPosition = cameraPos;
        bgManager.Update(GetMoveAmount());

        PlayerDeath();
        RespawnPlayers(gameTime);
        //Debug.WriteLine($"Respawning in: {(int)respawntimer}");
        //Debug.WriteLine($"tilemap: {tilemap.DisplayTileSize}, player: {players[0].tilemapDisplaySize});");
        //Debug.WriteLine($"{players[0].velocity}");
        previousCameraPosition = currentCameraPosition;
    }

    private float GetMoveAmount()
    {
        float maxSpeed = 0.15f;
        float cameraDelta = currentCameraPosition.X - previousCameraPosition.X;
        if (cameraDelta > 0)
        {
            return Math.Min(cameraDelta, maxSpeed);
        }
        else if (cameraDelta < 0)
        {
            return Math.Max(cameraDelta, -maxSpeed);
        }
        else
        {
            return 0;
        }
    }

    public bool quitGame()
    {
        var input = Globals.Input;
        return (input.GetGamePad(PlayerIndex.One).WasButtonJustReleased(Buttons.Back) || 
                input.Keyboard.WasKeyJustReleased(Keys.Escape));
    }

    private void PlayerDeath()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].isAlive == false)
            {
                deadPlayers.Add(players[i]);
                players.RemoveAt(i);
            }
        }
    }

    private void RespawnPlayers(GameTime gameTime)
    {
        respawntimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (respawntimer <= 0)
        {
            respawntimer = 10f;
            for (int i = deadPlayers.Count - 1; i >= 0; i--)
            {
                var p = deadPlayers[i];
                p.Respawn();
                players.Add(p);
                deadPlayers.RemoveAt(i);
            }
        }
    }

    public void Draw()
    {
        var spriteBatch = Globals.SpriteBatch;

        // Background
        bgManager.Draw();
        
        // TileMap
        tilemap.Draw(spriteBatch);

        foreach (var player in players)
        {
            player.Draw(spriteBatch);
        }

        // Draw Tile Collision
        foreach (var tile in tilemap.GetAdjacentTiles(players[0].position))
        {
            Rectangle tileCollisionRectangle = new(
                (int)tile.Key.X,
                (int)tile.Key.Y,
                tilemap.DisplayTileSize,
                tilemap.DisplayTileSize
                );
            spriteBatch.Draw(rectTexture, tileCollisionRectangle, Color.Purple * 0.5f);
        }


    }
}
