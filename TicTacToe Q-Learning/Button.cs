using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe_Q_Learning
{
    public class Button
    {
        #region Fields & Properties

        Texture2D img;
        Rectangle rect;
        public Color Color { get; private set; }

        SpriteFont font;
        string text = "";
        Vector2 textLoc;
        Color textColor;

        public int X
        {
            get
            {
                return rect.X;
            }
            set
            {
                rect.X = value;
                textLoc.X = value + SPACING / 2;
            }
        }
        public int Y
        {
            get
            {
                return rect.Y;
            }
            set
            {
                rect.Y = value;
                textLoc.Y = value + SPACING / 2;
            }
        }
        public int Width
        {
            get
            {
                return rect.Width;
            }
        }
        public int Height
        {
            get
            {
                return rect.Height;
            }
        }

        const int SPACING = 10;

        bool clickStarted = false;
        bool clickReleased = false;
        public bool Hovering { get; private set; }

        event Action<object> OnClick;

        #endregion

        #region Constructors

        public Button(string text, SpriteFont font, int x, int y, Color textColor, GraphicsDevice graphics, Color color)
        {
            // Drawable button
            this.text = text;
            this.font = font;
            this.textColor = textColor;

            rect = new Rectangle(x, y, (int)font.MeasureString(text).X + SPACING, (int)font.MeasureString(text).Y + SPACING);
            textLoc = new Vector2(x + SPACING / 2, y + SPACING / 2);
            Color borderColor = new Color(color.R / 3, color.G / 3, color.B / 3);
            img = graphics.CreateRectangle(rect.Width, rect.Height, Color.White).AddBorder(SPACING / 2,
                borderColor, color);

            Color = Color.White;
        }
        public Button(int x, int y, int width, int height)
        {
            // Click detector button; not for drawing
            rect = new Rectangle(x, y, width, height);

            Color = Color.White;
        }

        #endregion

        #region Public Methods

        public void Update(GameTime gameTime)
        {
            Hovering = Input.MouseRect.Intersects(rect);
            if (Input.LeftMouseDown && Hovering)
            {
                clickStarted = true;
                clickReleased = false;
                Color = new Color(75, 75, 75);
            }
            else if (!Input.LeftMouseDown)
            {
                clickReleased = true;
                if (Hovering)
                {
                    Color = new Color(150, 150, 150);
                }
                else
                {
                    Color = Color.White;
                }
            }
            else
            {
                clickStarted = false;
            }

            if (clickStarted && clickReleased && Hovering)
            {
                Reset();
                OnClick?.Invoke(this);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(img, rect, Color);
            spriteBatch.DrawString(font, text, textLoc, textColor);
        }

        public void AddClickHandler(Action<object> handler)
        {
            OnClick += handler;
        }

        public void Reset()
        {
            Color = Color.White;
            Hovering = false;
            clickStarted = false;
            clickReleased = false;
        }

        #endregion
    }
}
