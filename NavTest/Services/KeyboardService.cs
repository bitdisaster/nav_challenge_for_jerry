using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace NavTest.Services
{
    namespace Template10.Services.KeyboardService
    {
        public class KeyboardHelper
        {
            CoreWindow _win = Window.Current.CoreWindow;
            public KeyboardHelper()
            {
                _win.Dispatcher.AcceleratorKeyActivated += CoreDispatcher_AcceleratorKeyActivated;
                _win.PointerPressed += CoreWindow_PointerPressed;
            }

            public void Cleanup()
            {
                _win.Dispatcher.AcceleratorKeyActivated -= CoreDispatcher_AcceleratorKeyActivated;
                _win.PointerPressed -= CoreWindow_PointerPressed;
            }

            private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
            {
                if (e.EventType.ToString().Contains("Down") && !e.Handled)
                {
                    var args = KeyboardEventArgs(e.VirtualKey);
                    args.EventArgs = e;

                    try { KeyDown?.Invoke(args); }
                    finally
                    {
                        e.Handled = e.Handled;
                    }
                }
            }

            public Action<KeyboardEventArgs> KeyDown { get; set; }

            private KeyboardEventArgs KeyboardEventArgs(VirtualKey key)
            {
                var alt = (_win.GetKeyState(VirtualKey.Menu) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
                var shift = (_win.GetKeyState(VirtualKey.Shift) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
                var control = (_win.GetKeyState(VirtualKey.Control) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
                var windows = ((_win.GetKeyState(VirtualKey.LeftWindows) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down)
                    || ((_win.GetKeyState(VirtualKey.RightWindows) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down);
                return new KeyboardEventArgs
                {
                    AltKey = alt,
                    ControlKey = control,
                    ShiftKey = shift,
                    WindowsKey = windows,
                    VirtualKey = key,
                    Character = ToChar(key, shift),
                };
            }

            /// <summary>
            /// Invoked on every mouse click, touch screen tap, or equivalent interaction when this
            /// page is active and occupies the entire window.  Used to detect browser-style next and
            /// previous mouse button clicks to navigate between pages.
            /// </summary>
            /// <param name="sender">Instance that triggered the event.</param>
            /// <param name="e">Event data describing the conditions that led to the event.</param>
            private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs e)
            {
                var properties = e.CurrentPoint.Properties;

                // Ignore button chords with the left, right, and middle buttons
                if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed ||
                    properties.IsMiddleButtonPressed)
                    return;

                // If back or foward are pressed (but not both) navigate appropriately
                bool backPressed = properties.IsXButton1Pressed;
                bool forwardPressed = properties.IsXButton2Pressed;
                if (backPressed ^ forwardPressed)
                {
                    e.Handled = true;
                    if (backPressed) RaisePointerGoBackGestured();
                    if (forwardPressed) RaisePointerGoForwardGestured();
                }
            }

            public Action PointerGoForwardGestured { get; set; }
            protected void RaisePointerGoForwardGestured()
            {
                try { PointerGoForwardGestured?.Invoke(); }
                catch { }
            }

            public Action PointerGoBackGestured { get; set; }
            protected void RaisePointerGoBackGestured()
            {
                try { PointerGoBackGestured?.Invoke(); }
                catch { }
            }

            private static char? ToChar(VirtualKey key, bool shift)
            {
                // convert virtual key to char
                if (32 == (int)key)
                    return ' ';

                VirtualKey search;

                // look for simple letter
                foreach (var letter in "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
                {
                    if (Enum.TryParse<VirtualKey>(letter.ToString(), out search) && search.Equals(key))
                        return (shift) ? letter : letter.ToString().ToLower()[0];
                }

                // look for simple number0
                foreach (var number in "1234567890")
                {
                    if (Enum.TryParse<VirtualKey>("Number" + number.ToString(), out search) && search.Equals(key))
                        return number;
                }

                // not found
                return null;
            }
        }

        enum VKeyClass_EnUs
        {
            Control, // 0-31, 33-47, 91-95, 144-165
            Character, // 32, 48-90
            NumPad, // 96-111
            Function // 112 - 135
        }

        public enum VKeyCharacterClass
        {
            Space,
            Numeric,
            Alphabetic
        }

        public class KeyboardService
        {
            KeyboardHelper _helper;

            public static KeyboardService Instance { get; private set; } = new KeyboardService();

            private KeyboardService()
            {
                _helper = new KeyboardHelper();
                _helper.KeyDown = async (e) =>
                {
                    e.Handled = true;

                    // use this to place focus in search box
                    if (e.OnlyControl && e.Character.ToString().ToLower().Equals("e"))
                    {
                        AfterControlEGesture?.Invoke();
                    }

                    // use this to nav back
                    else if (e.VirtualKey == Windows.System.VirtualKey.GoBack)
                    {
                        Debug.WriteLine("GoBack:AfterBackGesture");
                        AfterBackGesture?.Invoke();
                    }

                    else if (e.VirtualKey == Windows.System.VirtualKey.NavigationLeft)
                    {
                        Debug.WriteLine("NavigationLeft:AfterBackGesture");
                        AfterBackGesture?.Invoke();
                    }

                    else if (e.VirtualKey == Windows.System.VirtualKey.GamepadMenu)
                    {
                        Debug.WriteLine("GamepadMenu:AfterMenuGesture");
                        AfterMenuGesture?.Invoke();
                    }

                    else if (e.VirtualKey == Windows.System.VirtualKey.GamepadLeftShoulder)
                    {
                        Debug.WriteLine("GamepadLeftShoulder:AfterBackGesture");
                        AfterBackGesture?.Invoke();
                    }

                    else if (e.OnlyAlt && e.VirtualKey == Windows.System.VirtualKey.Back)
                    {
                        Debug.WriteLine("Alt+Back:AfterBackGesture");
                        AfterBackGesture?.Invoke();
                    }

                    else if (e.OnlyAlt && e.VirtualKey == Windows.System.VirtualKey.Left)
                    {
                        Debug.WriteLine("Alt+Left:AfterBackGesture");
                        AfterBackGesture?.Invoke();
                    }

                    // use this to nav forward
                    else if (e.VirtualKey == Windows.System.VirtualKey.GoForward)
                    {
                        Debug.WriteLine("GoForward:AfterForwardGesture");
                        AfterForwardGesture?.Invoke();
                    }
                    else if (e.VirtualKey == Windows.System.VirtualKey.NavigationRight)
                    {
                        Debug.WriteLine("NavigationRight:AfterForwardGesture");
                        AfterForwardGesture?.Invoke();
                    }
                    else if (e.VirtualKey == Windows.System.VirtualKey.GamepadRightShoulder)
                    {
                        Debug.WriteLine("GamepadRightShoulder:AfterForwardGesture");
                        AfterForwardGesture?.Invoke();
                    }
                    else if (e.OnlyAlt && e.VirtualKey == Windows.System.VirtualKey.Right)
                    {
                        Debug.WriteLine("Alt+Right:AfterForwardGesture");
                        AfterForwardGesture?.Invoke();
                    }

                    // anything else
                    else
                        e.Handled = false;
                };
                _helper.PointerGoBackGestured = () =>
                {
                    Debug.WriteLine("PointerGoBackGestured");
                    AfterBackGesture?.Invoke();
                };
                _helper.PointerGoForwardGestured = () =>
                {
                    Debug.WriteLine("PointerGoForwardGestured");
                    AfterForwardGesture?.Invoke();
                };
            }

            public Action AfterBackGesture { get; set; }
            public Action AfterForwardGesture { get; set; }
            public Action AfterControlEGesture { get; set; }
            public Action AfterMenuGesture { get; set; }
        }

        public class KeyboardEventArgs : EventArgs
        {
            public bool Handled { get; set; } = false;
            public bool AltKey { get; set; }
            public bool ControlKey { get; set; }
            public bool ShiftKey { get; set; }
            public VirtualKey VirtualKey { get; set; }
            public AcceleratorKeyEventArgs EventArgs { get; set; }
            public char? Character { get; set; }
            public bool WindowsKey { get; internal set; }

            public bool OnlyWindows => WindowsKey & !AltKey & !ControlKey & !ShiftKey;
            public bool OnlyAlt => !WindowsKey & AltKey & !ControlKey & !ShiftKey;
            public bool OnlyControl => !WindowsKey & !AltKey & ControlKey & !ShiftKey;
            public bool OnlyShift => !WindowsKey & !AltKey & !ControlKey & ShiftKey;
            public bool Combo => new[] { AltKey, ControlKey, ShiftKey }.Any(x => x) & Character.HasValue;

            public override string ToString()
            {
                return $"KeyboardEventArgs = Handled {Handled}, AltKey {AltKey}, ControlKey {ControlKey}, ShiftKey {ShiftKey}, VirtualKey {VirtualKey}, Character {Character}, WindowsKey {WindowsKey}, OnlyWindows {OnlyWindows}, OnlyAlt {OnlyAlt}, OnlyControl {OnlyControl}, OnlyShift {OnlyShift}";
            }
        }
    }

}
