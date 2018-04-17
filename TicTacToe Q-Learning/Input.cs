using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe_Q_Learning
{
    public static class Input
    {
        static MouseState mouse;

        public static int MouseX
        {
            get
            {
                return mouse.X;
            }
        }
        public static int MouseY
        {
            get
            {
                return mouse.Y;
            }
        }
        public static Rectangle MouseRect
        {
            get
            {
                return new Rectangle(mouse.X, mouse.Y, 1, 1);
            }
        }
        public static Point MouseLoc
        {
            get
            {
                return new Point(mouse.X, mouse.Y);
            }
        }

        public static bool LeftMouseDown
        {
            get
            {
                return mouse.LeftButton == ButtonState.Pressed;
            }
        }
        public static bool RightMouseDown
        {
            get
            {
                return mouse.RightButton == ButtonState.Pressed;
            }
        }

        public static void Update()
        {
            mouse = Mouse.GetState();
        }
    }
}
