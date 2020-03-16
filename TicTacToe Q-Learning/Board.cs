using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe_Q_Learning
{
    public class Board
    {
        #region Fields & Properties

        List<BoardSquare> squares = new List<BoardSquare>();
        const int SQUARE_SIZE = 75;
        public const int SIZE = SQUARE_SIZE * SQUARES_PER_ROW;
        public const int SQUARES_PER_ROW = 3;

        Players currentTurn = Players.O;
        Players firstTurn = Players.X;

        Computer computer1;
        Computer computer2;
        Timer computerTimer = new Timer(0.5f, TimerUnits.Seconds);

        SpriteFont font;
        string winText = "";
        bool gameOver = false;
        Color winColor;
        Vector2 winLoc;

        int x;
        int y;

        Button resetButton;
        const int SPACING = 5;

        Button playerVComButton;
        Button trainAgainstSelfButton;
        Button pvpButton;

        bool optimalSpd = false;
        Button fastSpdButton;
        Button slowSpdButton;

        GameType gameType = GameType.HumanVSCom;

        #endregion

        #region Constructors

        public Board(Texture2D xImg, Texture2D oImg, Texture2D squareImg, int x, int y, SpriteFont font, GraphicsDevice graphics)
        {
            this.font = font;
            this.x = x;
            this.y = y;

            for (int i = 0; i < SQUARES_PER_ROW * SQUARES_PER_ROW; i++)
            {
                int row = (int)Math.Floor(i / SQUARES_PER_ROW * 1.0);
                int col = i - row * SQUARES_PER_ROW;
                BoardSquare square = new BoardSquare(squareImg, x + SQUARE_SIZE * col, y + SQUARE_SIZE * row, SQUARE_SIZE,
                    SQUARE_SIZE, xImg, oImg, row, col);
                square.AddTurnTakenHandler(TurnOver);
                squares.Add(square);
            }

            resetButton = new Button("Reset", font, 0, y + SIZE + SPACING, Color.White, graphics, Color.Blue);
            resetButton.X = Info.WINDOW_SIZE / 2 - resetButton.Width / 2;
            resetButton.AddClickHandler(item => Reset());

            Initialize(GameType.HumanVSCom);

            computer1 = new Computer(Players.X, ComputerType.Smart);
            computer2 = new Computer(Players.O, ComputerType.Random);

            playerVComButton = new Button("Human vs. Computer", font, 0, resetButton.Y + resetButton.Height + SPACING,
                Color.White, graphics, Color.Gold);
            playerVComButton.X = Info.WINDOW_SIZE / 2 - playerVComButton.Width / 2;
            playerVComButton.AddClickHandler(item => Initialize(GameType.HumanVSCom));

            pvpButton = new Button("Human vs. Human", font, 0, playerVComButton.Y + playerVComButton.Height + SPACING,
                Color.White, graphics, Color.DarkRed);
            pvpButton.X = Info.WINDOW_SIZE / 2 - pvpButton.Width / 2;
            pvpButton.AddClickHandler(item => Initialize(GameType.HumanVSHuman));

            trainAgainstSelfButton = new Button("Computer vs. Computer", font, 0, pvpButton.Y + pvpButton.Height +
                SPACING, Color.White, graphics, Color.Lime);
            trainAgainstSelfButton.X = Info.WINDOW_SIZE / 2 - trainAgainstSelfButton.Width / 2;
            trainAgainstSelfButton.AddClickHandler(item => Initialize(GameType.ComVSCom));

            fastSpdButton = new Button("Optimal Speed", font, 0, trainAgainstSelfButton.Y + trainAgainstSelfButton.Height + SPACING,
                Color.White, graphics, Color.Gray);
            slowSpdButton = new Button("Visible Gameplay", font, 0, fastSpdButton.Y, Color.White, graphics, Color.Gray);
            fastSpdButton.X = Info.WINDOW_SIZE / 2 - fastSpdButton.Width / 2;
            slowSpdButton.X = Info.WINDOW_SIZE / 2 - slowSpdButton.Width / 2;
            fastSpdButton.AddClickHandler(item => EnableOptimalSpd());
            slowSpdButton.AddClickHandler(item => DisableOptimalSpd());
        }

        #endregion

        #region Public Methods

        public void Update(GameTime gameTime)
        {
            if (gameType != GameType.ComVSCom)
            {
                optimalSpd = false;
            }
            if (!gameOver)
            {
                if (Info.PlayerOf(currentTurn) == PlayerType.Human)
                {
                    for (int i = 0; i < squares.Count; i++)
                    {
                        squares[i].Update(gameTime, currentTurn);
                    }
                }
                else if (Info.PlayerOf(currentTurn) == PlayerType.Computer)
                {
                    if (!optimalSpd)
                    {
                        computerTimer.Update(gameTime);
                    }
                    if (computerTimer.QueryWaitTime(gameTime) || optimalSpd)
                    {
                        BoardState state = GetState();
                        ComAction computerMove;
                        if (currentTurn == Players.X)
                        {
                            computerMove = computer1.MakeMove(state);
                        }
                        else
                        {
                            computerMove = computer2.MakeMove(state);
                        }
                        state.SetCell(computerMove.Row, computerMove.Column, BoardSquare.ContentOf(currentTurn));
                        SetSquaresToState(state);
                        TurnOver();
                    }
                }
            }
            resetButton.Update(gameTime);
            playerVComButton.Update(gameTime);
            trainAgainstSelfButton.Update(gameTime);
            pvpButton.Update(gameTime);

            if (gameType == GameType.ComVSCom)
            {
                if (optimalSpd)
                {
                    slowSpdButton.Update(gameTime);
                }
                else
                {
                    fastSpdButton.Update(gameTime);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (optimalSpd && gameType == GameType.ComVSCom)
            {
                slowSpdButton.Draw(spriteBatch);
                spriteBatch.DrawString(font, winText, winLoc, Color.White);
            }
            else if (gameType == GameType.ComVSCom)
            {
                fastSpdButton.Draw(spriteBatch);
            }
            
            for (int i = 0; i < squares.Count; i++)
            {
                squares[i].Draw(spriteBatch);
            }
            if (gameOver)
            {
                spriteBatch.DrawString(font, winText, winLoc, winColor);
            }
            resetButton.Draw(spriteBatch);
            playerVComButton.Draw(spriteBatch);
            trainAgainstSelfButton.Draw(spriteBatch);
            pvpButton.Draw(spriteBatch);
        }

        public BoardState GetState()
        {
            BoardState state = new BoardState();
            for (int i = 0; i < squares.Count; i++)
            {
                state.SetCell(squares[i].Row, squares[i].Column, squares[i].Content);
            }
            return state;
        }

        #endregion

        #region Private Methods

        private void EnableOptimalSpd()
        {
            optimalSpd = true;
            winText = "Training...";
            winLoc.X = Info.WINDOW_SIZE / 2 - font.MeasureString(winText).X / 2;
            winLoc.Y = y - font.MeasureString(winText).Y - SPACING;
        }
        private void DisableOptimalSpd()
        {
            optimalSpd = false;
        }

        private void TurnOver()
        {
            currentTurn = currentTurn.Opposite();
            BoardState state = GetState();
            if (state.CheckWin() != BoardContent.None)
            {
                Players winner = BoardSquare.PlayerOf(state.CheckWin());
                if (!optimalSpd)
                {
                    if (Info.PlayerOf(winner) == PlayerType.Human && gameType == GameType.HumanVSCom)
                    {
                        winText = "You Win!";
                    }
                    else
                    {
                        winText = "Player " + winner.ToString() + " Wins!";
                    }
                    winLoc = new Vector2(Info.WINDOW_SIZE / 2 - (font.MeasureString(winText).X / 2),
                        y - font.MeasureString(winText).Y);
                    if (winner == Players.O)
                    {
                        winColor = new Color(255, 216, 0);
                    }
                    else
                    {
                        winColor = new Color(212, 0, 0);
                    }
                }
                gameOver = true;
                computer1.GameResult(winner, GetState());
                if (gameType == GameType.ComVSCom)
                {
                    computer2.GameResult(winner, GetState());
                }
                if (optimalSpd)
                {
                    Reset();
                }
            }
            else if (state.IsFull())
            {
                gameOver = true;
                if (!optimalSpd)
                {
                    winColor = Color.LightGray;
                    winText = "It's a Tie!";
                    winLoc = new Vector2(Info.WINDOW_SIZE / 2 - (font.MeasureString(winText).X / 2),
                        y - font.MeasureString(winText).Y);
                }
                if (optimalSpd)
                {
                    Reset();
                }
            }
        }

        private void SetSquaresToState(BoardState state)
        {
            for (int i = 0; i < squares.Count; i++)
            {
                squares[i].SetContent(state.GetCell(squares[i].Row, squares[i].Column));
            }
        }

        private void Reset()
        {
            for (int i = 0; i < squares.Count; i++)
            {
                squares[i].SetContent(BoardContent.None);
            }

            gameOver = false;
            firstTurn = firstTurn.Opposite();
            currentTurn = firstTurn;
        }

        private void Initialize(GameType game)
        {
            gameType = game;
            Info.Players.Clear();
            if (game == GameType.ComVSCom)
            {
                Info.Players.Add(new Player(Players.O, PlayerType.Computer));
                Info.Players.Add(new Player(Players.X, PlayerType.Computer));
            }
            else if (game == GameType.HumanVSCom)
            {
                Info.Players.Add(new Player(Players.O, PlayerType.Human));
                Info.Players.Add(new Player(Players.X, PlayerType.Computer));
            }
            else if (game == GameType.HumanVSHuman)
            {
                Info.Players.Add(new Player(Players.O, PlayerType.Human));
                Info.Players.Add(new Player(Players.X, PlayerType.Human));
            }
            Reset();
        }

        #endregion
    }

    [Serializable]
    public class BoardState
    {
        public List<BoardContent> State = new List<BoardContent>();

        public BoardState()
        {
            State.Clear();
            for (int i = 0; i < Board.SQUARES_PER_ROW * Board.SQUARES_PER_ROW; i++)
            {
                State.Add(BoardContent.None);
            }
        }

        public void SetCell(int row, int col, BoardContent content)
        {
            int index = (row * Board.SQUARES_PER_ROW) + col;
            State[index] = content;
        }
        public BoardContent GetCell(int row, int col)
        {
            int index = (row * Board.SQUARES_PER_ROW) + col;
            return State[index];
        }

        public bool IsFull()
        {
            for (int i = 0; i < State.Count; i++)
            {
                if (State[i] == BoardContent.None)
                {
                    return false;
                }
            }
            return true;
        }

        public void ConvertTo(BoardContent find, BoardContent replace)
        {
            for (int i = 0; i < State.Count; i++)
            {
                if (State[i] == find)
                {
                    State[i] = replace;
                }
            }
        }
        public BoardState Reverse()
        {
            BoardState board = new BoardState();
            board.State.Clear();
            for (int i = 0; i < State.Count; i++)
            {
                board.State.Add(State[i].Opposite());
            }
            return board;
        }

        public BoardContent CheckWin()
        {
            if (CheckHorizontalWin() != BoardContent.None)
            {
                return CheckHorizontalWin();
            }
            else if (CheckVerticalWin() != BoardContent.None)
            {
                return CheckVerticalWin();
            }
            else if (CheckDiagonalUpWin() != BoardContent.None)
            {
                return CheckDiagonalUpWin();
            }
            else if (CheckDiagonalDownWin() != BoardContent.None)
            {
                return CheckDiagonalDownWin();
            }

            return BoardContent.None;
        }
        public BoardContent CheckHorizontalWin()
        {
            BoardContent currentVal = BoardContent.None;
            for (int row = 0; row < Board.SQUARES_PER_ROW; row++)
            {
                currentVal = GetCell(row, 0);
                for (int col = 0; col < Board.SQUARES_PER_ROW; col++)
                {
                    if (currentVal != GetCell(row, col))
                    {
                        currentVal = BoardContent.None;
                    }
                }
                if (currentVal != BoardContent.None)
                {
                    return currentVal;
                }
            }
            return currentVal;
        }
        public BoardContent CheckVerticalWin()
        {
            BoardContent currentVal = BoardContent.None;
            for (int col = 0; col < Board.SQUARES_PER_ROW; col++)
            {
                currentVal = GetCell(0, col);
                for (int row = 0; row < Board.SQUARES_PER_ROW; row++)
                {
                    if (currentVal != GetCell(row, col))
                    {
                        currentVal = BoardContent.None;
                    }
                }
                if (currentVal != BoardContent.None)
                {
                    return currentVal;
                }
            }
            return currentVal;
        }
        public BoardContent CheckDiagonalUpWin()
        {
            BoardContent currentVal = GetCell(0, 0);
            for (int val = 0; val < Board.SQUARES_PER_ROW; val++)
            {
                if (currentVal != GetCell(val, val))
                {
                    currentVal = BoardContent.None;
                }
            }
            return currentVal;
        }
        public BoardContent CheckDiagonalDownWin()
        {
            BoardContent currentVal = GetCell(0, Board.SQUARES_PER_ROW - 1);
            int col = 0;
            int row = Board.SQUARES_PER_ROW - 1;
            for (int i = 0; i < Board.SQUARES_PER_ROW; i++)
            {
                if (currentVal != GetCell(row, col))
                {
                    currentVal = BoardContent.None;
                }
                col++;
                row--;
            }
            return currentVal;
        }

        public static BoardState Rotate(BoardState state, int n)
        {
            BoardState ret = new BoardState();
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    ret.SetCell(i, j, state.GetCell(n - j - 1, i));
                }
            }
            return ret;
        }
        public static int FindHeaviestSide(BoardState state)
        {
            List<int> heavySides = new List<int>();
            int count = 0;
            for (int i = 0; i < 4; i++)
            {
                count = 0;
                for (int j = 0; j < Board.SQUARES_PER_ROW; j++)
                {
                    if (i == 0)
                    {
                        if (state.GetCell(0, j) != BoardContent.None)
                        {
                            count++;
                        }
                    }
                    else if (i == 1)
                    {
                        if (state.GetCell(j, 0) != BoardContent.None)
                        {
                            count++;
                        }
                    }
                    else if (i == 2)
                    {
                        if (state.GetCell(Board.SQUARES_PER_ROW - 1, j) != BoardContent.None)
                        {
                            count++;
                        }
                    }
                    else if (i == 3)
                    {
                        if (state.GetCell(j, Board.SQUARES_PER_ROW - 1) != BoardContent.None)
                        {
                            count++;
                        }
                    }
                }
            }
            return heavySides.IndexOf(heavySides.Max());
        }
    }
}
