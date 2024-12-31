using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System;

namespace Navy
{
    public enum MouseButton
    {
        None,
        Left,
        Middle,
        Right,
        Button1,
        Button2
    }

    public static class InputManager
    {
        public static class Mouse
        {
            private static readonly Dictionary<MouseButton, ButtonState> currentButtonStates = new Dictionary<MouseButton, ButtonState>()
            {
                {MouseButton.Left, 0 },
                {MouseButton.Middle, 0 },
                {MouseButton.Right, 0 },
                {MouseButton.Button1, 0 },
                {MouseButton.Button2, 0 },
            };

            private static Dictionary<MouseButton, ButtonState> previousButtonStates = new Dictionary<MouseButton, ButtonState>();
            
            private static MouseState currentMouseState;
            private static MouseState previousMouseState;

            public static event EventHandler<MouseButton> ButtonPressed;
            public static event EventHandler<MouseButton> ButtonReleased;

            public static event EventHandler<int> Scrolled;
            public static Vector2 MouseScreenPosition 
            {
                get => new Vector2(Microsoft.Xna.Framework.Input.Mouse.GetState().Position.X / ResolutionHandler.GetVirtualClientRatio().X, Microsoft.Xna.Framework.Input.Mouse.GetState().Position.Y / ResolutionHandler.GetVirtualClientRatio().Y);
            }

            public static Vector2 GetMouseWorldPosition(Camera camera) => camera.ScreenToWorld(MouseScreenPosition);

            internal static void Update()
            {
                previousMouseState = currentMouseState;
                currentMouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();

                if (previousMouseState.ScrollWheelValue != currentMouseState.ScrollWheelValue)
                {
                    Scrolled?.Invoke(null, currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue);
                }
                #region ButtonStateValueSetting

                // Sets the value in the button state dictionaries
                previousButtonStates.Clear();
                foreach (KeyValuePair<MouseButton, ButtonState> kvp in currentButtonStates)
                {
                    previousButtonStates.Add(kvp.Key, kvp.Value);
                }
                currentButtonStates[MouseButton.Left] = currentMouseState.LeftButton;
                currentButtonStates[MouseButton.Middle] = currentMouseState.MiddleButton;
                currentButtonStates[MouseButton.Right] = currentMouseState.RightButton;
                currentButtonStates[MouseButton.Button1] = currentMouseState.XButton1;
                currentButtonStates[MouseButton.Button2] = currentMouseState.XButton2;
                #endregion

                // Checks for event calling
                foreach (var button in currentButtonStates)
                {
                    if (button.Value == ButtonState.Pressed && previousButtonStates[button.Key] == ButtonState.Released)
                    {
                        ButtonPressed?.Invoke(null, button.Key);
                    }

                    else if (button.Value == ButtonState.Released && previousButtonStates[button.Key] == ButtonState.Pressed)
                    {
                        ButtonReleased?.Invoke(null, button.Key);
                    }
                }               
            }

            public static MouseState GetCurrentState() => currentMouseState;
            public static MouseState GetPreviousState() => previousMouseState;

            public static bool IsButtonDown(MouseButton button) => currentButtonStates[button] == ButtonState.Pressed;

            public static bool IsButtonUp(MouseButton button) => currentButtonStates[button] == ButtonState.Released;
        }

        public static class Keyboard
        {
            private static KeyboardState currentKeyboardState;
            private static KeyboardState previousKeyboardState;
            public static Keys[] CurrentKeys { get; private set; }
            private static Keys[] previousKeys;

            public static KeyboardState GetCurrentState() => currentKeyboardState;
            public static KeyboardState GetPreviousState() => previousKeyboardState;
            public static bool IsKeyDown(Keys key) => currentKeyboardState.IsKeyDown(key);
            public static bool IsKeyUp(Keys key) => currentKeyboardState.IsKeyUp(key);

            /// <summary>
            /// Returns whether or not all of the specified keys are down
            /// </summary>
            public static bool AreAllKeysDown(params Keys[] keys)
            {
                foreach (Keys key in keys)
                {
                    if (currentKeyboardState.IsKeyUp(key))
                    {
                        return false;
                    }
                }

                return true;
            }

            /// <summary>
            /// Returns whether or not all of the specified keys are released
            /// </summary>
            public static bool AreAllKeysUp(params Keys[] keys)
            {
                foreach (Keys key in keys)
                {
                    if (currentKeyboardState.IsKeyDown(key))
                    {
                        return false;
                    }
                }

                return true;
            }
            
            /// <summary>
            /// Returns whether or not one of the specified keys is released
            /// </summary>
            public static bool IsOneKeyUp(params Keys[] keys)
            {
                foreach (Keys key in keys)
                {
                    if (currentKeyboardState.IsKeyUp(key))
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// Returns whether or not one of the specified keys is down
            /// </summary>
            public static bool IsOneKeyDown(params Keys[] keys)
            {
                foreach (Keys key in keys)
                {
                    if (currentKeyboardState.IsKeyDown(key))
                    {
                        return true;
                    }
                }

                return false;
            }

            public static event EventHandler<Keys> KeyPressed;
            public static event EventHandler<Keys> KeyReleased;

            public static event EventHandler<TextInputEventArgs> TextInput;

            // Handles special text input for input field - includes proper capitalization spaces and other stuff
            public static void TextInputHandler(object sender, TextInputEventArgs args)
            {
                TextInput?.Invoke(null, args);
            }

            internal static void Update()
            {
                previousKeyboardState = currentKeyboardState;
                currentKeyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();

                previousKeys = CurrentKeys;
                CurrentKeys = currentKeyboardState.GetPressedKeys();

                // Checking for calling of events
                if (CurrentKeys.Length > 0)
                {
                    if (previousKeys.Length < CurrentKeys.Length)
                    {

                        if (KeyPressed != null)
                        {
                            foreach (var key in CurrentKeys)
                            {
                                if (!previousKeyboardState.IsKeyDown(key))
                                {
                                    KeyPressed?.Invoke(null, key);
                                }
                            }
                        }

                    }

                    else if (previousKeys.Length > CurrentKeys.Length)
                    {
                        if (KeyReleased != null)
                        {
                            foreach (var key in previousKeys)
                            {
                                if (!currentKeyboardState.IsKeyDown(key))
                                {
                                    KeyReleased?.Invoke(null, key);
                                }
                            }
                        }

                    }
                }

                else
                {
                    if (previousKeys != CurrentKeys)
                    {
                        if (KeyReleased != null)
                        {
                            foreach (var key in previousKeys)
                            {
                                KeyReleased?.Invoke(null, key);
                            }
                        }

                    }
                }
            }
        }

        public static void Update()
        {
            Mouse.Update();
            Keyboard.Update();
        }
    }
}
