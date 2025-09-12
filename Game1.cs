using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StickFight.src;
using StickFight.src.entities;
using System.Collections.Generic;

namespace StickFight;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    // Declarations
    Player player;
    List<Enemy> enemies;
    Tilemap tilemap;

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
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();
        base.Initialize();
    }
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Initializing
        player = new Player(Content);
        enemies = new List<Enemy>();
        SpawnEnemies();

        // Map
        Texture2D groundTextureAtlas = Content.Load<Texture2D>("grass");
        rectTexture = Content.Load<Texture2D>("collisionRect");
        tilemap = new("../../../data/map2.csv", groundTextureAtlas, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        player.Update(GraphicsDevice.Viewport.Height, enemies, tilemap.GetAdjacentTiles(player.position));

        foreach (var e in enemies)
        {
            e.Update(GraphicsDevice.Viewport.Height);
        }
        UpdateEnemies();
        base.Update(gameTime);
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < 10; i++)
        {
            var enemy = new Enemy(Content);
            enemy.position = new Vector2(100 * i, 0);
            enemies.Add(enemy);
        }
    }

    private void UpdateEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].isAlive == false)
            {
                enemies.RemoveAt(i);
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Draw
        tilemap.Draw(_spriteBatch);
        player.Draw(_spriteBatch);
        
        foreach (var e in enemies)
        {
            e.Draw(_spriteBatch);
        }

        foreach (var tile in tilemap.GetAdjacentTiles(player.position))
        {
            Rectangle tileCollisionRectangle = new(
                (int)tile.Key.X,
                (int)tile.Key.Y,
                32,
                32
                );
            _spriteBatch.Draw(rectTexture, tileCollisionRectangle, Color.Purple * 0.5f);
        }


        base.Draw(gameTime);
        _spriteBatch.End();
    }
}
