using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StickFight.src;

namespace StickFight;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private GameManager gameManager;
    private Camera camera;

    public bool quitGame = false;
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        Globals.Content = Content;
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Set Resolution
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();
        Globals.ScreenDimensions = new Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

        gameManager = new();
        gameManager.Initialize();
        // Camera
        camera = new Camera();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        Globals.SpriteBatch = new SpriteBatch(GraphicsDevice);
        gameManager.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (gameManager.quitGame()) Exit();

        camera.Update(gameManager.players[0].position);
        gameManager.Update(gameTime, camera.Position);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        var spriteBatch = Globals.SpriteBatch;
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.Translation);
        //spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        gameManager.Draw();
        base.Draw(gameTime);

        spriteBatch.End();
    }
}
