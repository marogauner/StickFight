using Microsoft.Xna.Framework.Input;

namespace StickFight.src.input;

public class KeyboardInfo
{
    public KeyboardState PreviousState { get; private set; }
    public KeyboardState CurrentState { get; private set; }

    public KeyboardInfo()
    {
        PreviousState = new KeyboardState();
        CurrentState = Keyboard.GetState();
    }

    public void Update()
    {
        PreviousState = CurrentState;
        CurrentState = Keyboard.GetState();
    }

    // Methods to check for key Presses
    public bool IsKeyDown(Keys key) => CurrentState.IsKeyDown(key);
    public bool IsKeyUp(Keys key) => CurrentState.IsKeyUp(key);
    public bool WasKeyJustPressed(Keys key) => CurrentState.IsKeyDown(key) && PreviousState.IsKeyUp(key);
    public bool WasKeyJustReleased(Keys key) => CurrentState.IsKeyUp(key) && PreviousState.IsKeyDown(key);
}