using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StickFight.src;
using StickFight.src.entities;
using StickFight.src.input;
using System.Collections.Generic;
using System.Diagnostics;

namespace StickFight;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    //private SpriteBatch _spriteBatch;

    // Declarations
    private List<Player> players;
    private List<Player> deadPlayers;
    private int maxPlayers = 4;
    private Tilemap tilemap;
    private Camera camera;

    Texture2D rectTexture;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Set Resolution
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();
        Globals.ScreenDimensions = new Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

        Globals.Input = new InputManager();
        base.Initialize();
    }
    protected override void LoadContent()
    {
        Globals.SpriteBatch = new SpriteBatch(GraphicsDevice);
        //_spriteBatch = new SpriteBatch(GraphicsDevice);

        // Initializing
        players = new List<Player>  {
            new Player(Content, PlayerIndex.One),
            new Player(Content, PlayerIndex.Two),
            new Player(Content, PlayerIndex.Three),
            new Player(Content, PlayerIndex.Four)
        };
        deadPlayers = [];

        // Map
        Texture2D mapTextureAtlas = Content.Load<Texture2D>("world_tileset");
        rectTexture = Content.Load<Texture2D>("collisionRect");
        tilemap = new("../../../data/stickfight_map1.csv", mapTextureAtlas, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

        // Camera
        camera = new Camera();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Globals.Input.Update(gameTime);

        foreach (var player in players)
        {
            player.Update(gameTime, GraphicsDevice.Viewport.Height, players, tilemap.GetAdjacentTiles(player.position));
        }
        PlayerDeath();
        RespawnPlayers(gameTime);
        camera.Update(players[0].position);
        //Debug.WriteLine($"Respawning in: {(int)respawntimer}");
        base.Update(gameTime);
    }

    //private void SpawnEnemies()
    //{
    //    for (int i = 0; i < 10; i++)
    //    {
    //        var enemy = new Enemy(Content);
    //        enemy.position = new Vector2(100 * i, 100);
    //        entities.Add(enemy);
    //    }
    //}

    //private void UpdateEnemies()
    //{
    //    for (int i = 0; i < entities.Count; i++)
    //    {
    //        if (entities[i].isAlive == false)
    //        {
    //            entities.RemoveAt(i);
    //        }
    //    }
    //}

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

    private float respawntimer = 10f;
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

    protected override void Draw(GameTime gameTime)
    {
        var spritebatch = Globals.SpriteBatch;
        spritebatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.Translation);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Draw
        tilemap.Draw(spritebatch);
        
        foreach(var player in players)
        {
            player.Draw(spritebatch);
        }

        // Draw Tile Collision
        //foreach (var tile in tilemap.GetAdjacentTiles(player.position))
        //{
        //    Rectangle tileCollisionRectangle = new(
        //        (int)tile.Key.X,
        //        (int)tile.Key.Y,
        //        32,
        //        32
        //        );
        //    _spriteBatch.Draw(rectTexture, tileCollisionRectangle, Color.Purple * 0.5f);
        //}


        base.Draw(gameTime);
        spritebatch.End();
    }
}
