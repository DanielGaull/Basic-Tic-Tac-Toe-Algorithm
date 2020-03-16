using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe_Q_Learning
{
    public class Computer
    {
        #region Fields & Properties

        public const string FILE_PATH = "computerlearning.comlrn";

        ComputerInformation ai;

        Players player;
        ComputerType type;

        #endregion

        #region Constructors

        public Computer(Players player, ComputerType type)
        {
            this.player = player;
            this.type = type;
            ai = new ComputerInformation();
        }

        #endregion

        #region Public Methods

        public ComAction MakeMove(BoardState state)
        {
            if (player == Players.O)
            {
                state = state.Reverse();
            }
            Point play = new Point();
            play = IndicesOfMax(state);
            ComAction action = null;
            if (type == ComputerType.Smart)
            {
                action = new ComAction(play.X, play.Y);
            }
            else
            {
                action = ComAction.Random(state);
            }
            return action;
        }

        public void GameResult(Players winner, BoardState board)
        {
            for (int i = 0; i < board.State.Count; i++)
            {
                int row = (int)Math.Floor(i / Board.SQUARES_PER_ROW * 1.0d);
                int col = i - row * Board.SQUARES_PER_ROW;
                if (BoardSquare.PlayerOf(board.State[i]) == winner)
                {
                    if (ai.Scores[row, col] < 5)
                    {
                        ai.Scores[row, col]++;
                    }
                }
                else if (BoardSquare.PlayerOf(board.State[i]) == winner.Opposite())
                {
                    if (ai.Scores[row, col] > -5)
                    {
                        ai.Scores[row, col]--;
                    }
                }
            }
            ai.SaveFile(FILE_PATH);
        }

        #endregion

        #region Private Methods

        private Point IndicesOfMax(BoardState state)
        {
            int[,] scores = new int[Board.SQUARES_PER_ROW, Board.SQUARES_PER_ROW];
            for (int i = 0; i < ai.Scores.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < ai.Scores.GetUpperBound(1) + 1; j++)
                {
                    scores[i, j] = ai.Scores[i, j];
                }
            }
            for (int i = 0; i < state.State.Count; i++)
            {
                if (state.State[i] != BoardContent.None)
                {
                    int r = (int)Math.Floor(i / Board.SQUARES_PER_ROW * 1.0d);
                    int c = i - r * Board.SQUARES_PER_ROW;
                    scores[r, c] = -1000;
                }
            }

            int row = 0;
            int col = 0;
            int max = 0;
            max = scores.Cast<int>().Max();
            for (int i = 0; i <= scores.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= scores.GetUpperBound(1); j++)
                {
                    if (scores[i, j] == max)
                    {
                        row = i;
                        col = j;
                    }
                }
            }
            return new Point(row, col);
        }

        #endregion
    }

    public class ComAction
    {
        public int Row;
        public int Column;

        public ComAction()
        {
            Row = Column = 0;
        }
        public ComAction(int row, int col)
        {
            Row = row;
            Column = col;
        }

        public static ComAction Random(BoardState state)
        {
            int row = Info.RandomNum(0, Board.SQUARES_PER_ROW);
            int col = Info.RandomNum(0, Board.SQUARES_PER_ROW);
            //int runCount = 0;
            while (state.GetCell(row, col) != BoardContent.None)
            {
                row = Info.RandomNum(0, Board.SQUARES_PER_ROW);
                col = Info.RandomNum(0, Board.SQUARES_PER_ROW);
                //runCount++;
                //if (runCount > 1000)
                //{
                //    Console.WriteLine("CRASH TICK: " + Info._Tick);
                //}
            }
            return new ComAction(row, col);
        }

        public static bool operator ==(ComAction c1, ComAction c2)
        {
            return (c1?.Column == c2?.Column) && (c1?.Row == c2?.Row);
        }
        public static bool operator !=(ComAction c1, ComAction c2)
        {
            return !(c1 == c2);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public class ComputerInformation
    {
        public int[,] Scores = new int[Board.SQUARES_PER_ROW, Board.SQUARES_PER_ROW];
        public ComputerInformation()
        {
            LoadFile(Computer.FILE_PATH);
        }

        public int GetScore(int row, int col)
        {
            return Scores[row, col];
        }
        
        public void LoadFile(string file)
        {
            FileStream stream = null;
            try
            {
                stream = File.Open(file, FileMode.Open);
                StreamReader reader = new StreamReader(stream);
                List<int> values = new List<int>();
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    List<string> strings = line.Split(',').ToList();
                    strings.RemoveAt(strings.Count - 1);
                    for (int i = 0; i < strings.Count; i++)
                    {
                        values.Add(int.Parse(strings[i]));
                    }
                }
                for (int i = 0; i < values.Count; i++)
                {
                    int row = (int)Math.Floor(i / Board.SQUARES_PER_ROW * 1.0d);
                    int col = i - row * Board.SQUARES_PER_ROW;
                    Scores[row, col] = values[i];
                }
            }
            finally
            {
                stream?.Close();
            }
        }
        public void SaveFile(string file)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < Scores.GetUpperBound(0)+1; i++)
            {
                for (int j = 0; j < Scores.GetUpperBound(1)+1; j++)
                {
                    builder.Append(Scores[i, j] + ",");
                }
                builder.AppendLine();
            }
            File.WriteAllText(file, builder.ToString());
        }
    }

    public enum ComputerType
    {
        Smart,
        Random
    }
}
