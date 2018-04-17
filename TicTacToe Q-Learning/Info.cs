using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe_Q_Learning
{
    public static class Info
    {
        public const int WINDOW_SIZE = 775;

        public static Random Random = new Random();
        public static int RandomNum(int min, int max)
        {
            return Random.Next(min, max);
        }

        public static List<Player> Players = new List<Player>();
        public static PlayerType PlayerOf(Players player)
        {
            return Players.Where(x => x.PlayerSquare == player).ElementAt(0).PlayerType;
        }

        public static int Average(float val1, float val2)
        {
            return (int)((val1 + val2) / 2.0f);
        }
    }

    public struct Player
    {
        public Players PlayerSquare;
        public PlayerType PlayerType;

        public Player(Players square, PlayerType type)
        {
            PlayerSquare = square;
            PlayerType = type;
        }
    }

    public static class Extentions
    {
        public static string FormatToWidth(this string str, SpriteFont font, float width)
        {
            float currentWidth = font.MeasureString(str).X;
            if (currentWidth < width)
            {
                // The string is already less than the desired max. Return it because we're already done
                return str;
            }

            string returnStr = "";
            string currentLine = "";
            string word = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == ' ')
                {
                    if (font.MeasureString(currentLine + word + " ").X > width)
                    {
                        returnStr += currentLine + "\n";
                        currentLine = word + " ";
                    }
                    else
                    {
                        currentLine += word + " ";
                    }
                    word = "";
                }
                else
                {
                    word += str[i];
                }
            }

            returnStr += currentLine + word;

            return returnStr;
        }

        public static Players Opposite(this Players p)
        {
            if (p == Players.X)
            {
                return Players.O;
            }
            return Players.X;
        }
        
        public static Texture2D CreateRectangle(this GraphicsDevice graphics, int width, int height, Color color)
        {
            Texture2D rect = new Texture2D(graphics, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Count(); i++)
            {
                data[i] = color;
            }
            rect.SetData(data);
            return rect;
        }
        public static Texture2D AddBorder(this Texture2D texture, int borderWidth, Color borderColor, Color fillColor)
        {
            // Code taken from stackoverflow.com/questions/13893959/how-to-draw-the-border-of-a-square
            Color[] colors = new Color[texture.Width * texture.Height];

            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    bool colored = false;
                    for (int i = 0; i <= borderWidth; i++)
                    {
                        if (x == i || y == i || x == texture.Width - 1 - i || y == texture.Height - 1 - i)
                        {
                            colors[x + y * texture.Width] = borderColor;
                            colored = true;
                            break;
                        }
                    }

                    if (!colored)
                    {
                        colors[x + y * texture.Width] = fillColor;
                    }
                }
            }

            texture.SetData(colors);

            return texture;
        }

        public static BoardContent Opposite(this BoardContent b)
        {
            if (b == BoardContent.PlayerO)
            {
                return BoardContent.PlayerX;
            }
            else if (b == BoardContent.PlayerX)
            {
                return BoardContent.PlayerO;
            }
            return BoardContent.None;
        }
    }
}
