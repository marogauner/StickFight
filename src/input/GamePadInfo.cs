using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace StickFight.src.input;

public class GamePadInfo
{
    #region Properties
    private TimeSpan vibrationTimeRemaining = TimeSpan.Zero;

    // Get the Index of the player this GamePad is for
    public PlayerIndex PlayerIndex { get; }
    public GamePadState PreviousState { get; private set; }
    public GamePadState CurrentState { get; private set; }
    public bool IsConnected => CurrentState.IsConnected;

    public Vector2 LeftThumbstick => CurrentState.ThumbSticks.Left;
    public Vector2 RightThumbstick => CurrentState.ThumbSticks.Right;
    public float LeftTrigger => CurrentState.Triggers.Left;
    public float RightTrigger => CurrentState.Triggers.Right;
    #endregion

    #region Methods
    public GamePadInfo(PlayerIndex playerIndex)
    {
        PlayerIndex = playerIndex;
        PreviousState = GamePad.GetState(playerIndex);
        CurrentState = new GamePadState();
    }

    public void Update(GameTime gameTime)
    {
        PreviousState = CurrentState;
        CurrentState = GamePad.GetState(PlayerIndex);

        if (vibrationTimeRemaining > TimeSpan.Zero)
        {
            vibrationTimeRemaining -= gameTime.ElapsedGameTime;
            if (vibrationTimeRemaining <= TimeSpan.Zero)
            {
                StopVibration();
            }
        }
    }

    // Check Button States
    public bool IsButtonDown(Buttons button) => CurrentState.IsButtonDown(button);
    public bool IsButtonUp(Buttons button) => CurrentState.IsButtonUp(button);
    public bool WasButtonJustPressed(Buttons button) => CurrentState.IsButtonDown(button) && PreviousState.IsButtonUp(button);
    public bool WasButtonJustReleased(Buttons button) => CurrentState.IsButtonUp(button) && PreviousState.IsButtonDown(button);

    // GamePad Vibration
    public void SetVibration(float strength, TimeSpan time)
    {
        vibrationTimeRemaining = time;
        GamePad.SetVibration(PlayerIndex, strength, strength);
    }

    public void StopVibration()
    {
        GamePad.SetVibration(PlayerIndex, 0.0f, 0.0f);
    }
    #endregion
}