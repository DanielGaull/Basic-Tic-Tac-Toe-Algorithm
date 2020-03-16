using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe_Q_Learning
{
    public class BoardSquare
    {
        #region Fields & Properties

        Texture2D img;
        Rectangle rect;

        Texture2D contentImg;
        Texture2D playerXImg;
        Texture2D playerOImg;
        Rectangle contentRect;

        public BoardContent Content { get; private set; }

        const int SPACING = 20;

        public int Row { get; set; }
        public int Column { get; set; }

        Button button;
        Players currentTurn;

        const int SMALL_SPACING = 7;

        #endregion

        #region Constructors

        public BoardSquare(Texture2D img, int x, int y, int width, int height, Texture2D playerXImg, Texture2D playerOImg, 
            int row, int col)
        {
            this.playerXImg = playerXImg;
            this.playerOImg = playerOImg;

            this.img = img;
            rect = new Rectangle(x, y, width, height);

            contentImg = playerXImg;
            contentRect = new Rectangle(x + SMALL_SPACING, y + SMALL_SPACING, width - SPACING, height - SPACING);

            button = new Button(x, y, width, height);
            button.AddClickHandler(item => SetContent(ContentOf(currentTurn)));

            Row = row;
            Column = col;
        }

        #endregion

        #region Public Methods

        public void Update(GameTime gameTime, Players currentTurn)
        {
            this.currentTurn = currentTurn;
            if (Info.PlayerOf(currentTurn) == PlayerType.Human && Content == BoardContent.None)
            {
                button.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(img, rect, button.Color);
            if (Content != BoardContent.None)
            {
                spriteBatch.Draw(contentImg, contentRect, Color.White);
            }
        }

        public void SetContent(BoardContent content)
        {
            Content = content;
            if (content == BoardContent.PlayerX)
            {
                contentImg = playerXImg;
            }
            else if (content == BoardContent.PlayerO)
            {
                contentImg = playerOImg;
            }
        }

        public void AddTurnTakenHandler(Action handler)
        {
            button.AddClickHandler(x => handler());
        }

        public static BoardContent ContentOf(Players player)
        {
            if (player == Players.O)
            {
                return BoardContent.PlayerO;
            }
            else if (player == Players.X)
            {
                return BoardContent.PlayerX;
            }
            return BoardContent.None;
        }
        public static Players PlayerOf(BoardContent content)
        {
            if (content == BoardContent.PlayerO)
            {
                return Players.O;
            }
            else if (content == BoardContent.PlayerX)
            {
                return Players.X;
            }
            return Players.O;
        }

        #endregion

        #region Private Methods

        #endregion
    }

    #region Enumerations

    public enum BoardContent
    {
        None,
        PlayerX,
        PlayerO,
    }

    public enum Players
    {
        X,
        O,
    }
    public enum PlayerType
    {
        Human,
        Computer,
    }

    public enum GameType
    {
        HumanVSCom,
        ComVSCom,
        HumanVSHuman,
    }

    #endregion
}
