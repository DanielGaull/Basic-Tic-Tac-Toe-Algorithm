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
        //public const float POINTS_FOR_WIN = 5.0f;

        ComputerInformation ai;
        //List<ComAction> actions = new List<ComAction>();
        //List<BoardState> states = new List<BoardState>();

        Players player;
        ComputerType type;

        //int rotation = 0;

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
            //ComAction action = ai.GetAction(state, true);
            //actions.Add(action);
            //states.Add(state);
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

            //List<float> movePoints = new List<float>();
            //for (int i = 0; i < actions.Count; i++)
            //{
            //    float points = 0;
            //    if (winner == player)
            //    {
            //        points = ((i + 1.0f) / actions.Count) * POINTS_FOR_WIN;
            //    }
            //    else
            //    {
            //        points = ((i + 1.0f) / actions.Count) * POINTS_FOR_WIN * -1.0f;
            //    }
            //    movePoints.Add(points);
            //}

            //for (int i = 0; i < actions.Count; i++)
            //{
            //    ComAction action = new ComAction();
            //    float points = 0;

            //    if (AnyActionsMatchThisAction(ai, actions[i], states[i]))
            //    {
            //        if (Math.Abs(ai.GetPoints(states[i])) > Math.Abs(movePoints[i]) && 
            //            ai.GetAction(states[i], false) != null)
            //        {
            //            action = ai.GetAction(states[i], false);
            //            points = ai.GetPoints(states[i]);
            //        }
            //        else
            //        {
            //            action = actions[i];
            //            points = movePoints[i];
            //        }
            //    }
            //    else
            //    {
            //        points = Info.Average(movePoints[i], ai.GetPoints(states[i]));
            //        action = actions[i];
            //    }
            //    ActionPair a = new ActionPair(states[i], action, points);
            //    ai.SetActionForState(a);
            //}
            //if (player == Players.O)
            //{
            //    ai.ReverseAllStates();
            //}
            //ai.SaveFile(FILE_PATH);
        }

        #endregion

        #region Private Methods

        //private bool AnyActionsMatchThisAction(ComputerInformation ai, ComAction targetAction, BoardState state)
        //{
        //    // Rotates the board to check the state if this works
        //    rotation = 0;
        //    for (; rotation < 4; rotation++)
        //    {
        //        if (ai.GetAction(BoardState.Rotate(state, rotation), false) != targetAction)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

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

    //[Serializable]
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

        //public static ComAction ApplyRotation(ComAction action, int rotation)
        //{
        //    action.Row = rotation - action.Row - 1;
        //    return action;
        //}

        public static bool operator ==(ComAction c1, ComAction c2)
        {
            //if (c1 == null || c2 == null)
            //{
            //    return false;
            //}
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

    //[Serializable]
    //public struct ActionPair
    //{
    //    public BoardState State;
    //    public ComAction Action;
    //    public float Points;

    //    public ActionPair(BoardState state, ComAction action, float points)
    //    {
    //        State = state;
    //        Action = action;
    //        Points = points;
    //    }

    //    public void Reverse()
    //    {
    //        State.Reverse();
    //    }
    //}
    public class ComputerInformation
    {
        //public List<ActionPair> ActionPairs = new List<ActionPair>();
        public int[,] Scores = new int[Board.SQUARES_PER_ROW, Board.SQUARES_PER_ROW];
        public ComputerInformation()
        {
            LoadFile(Computer.FILE_PATH);
        }

        public int GetScore(int row, int col)
        {
            return Scores[row, col];
        }

        //public int GetScore(int row, int col)
        //{
        //    for (int i = 0; i < ActionPairs.Count; i++)
        //    {
        //        if (ActionPairs[i].State == state)
        //        {
        //            return ActionPairs[i].Action;
        //        }
        //    }
        //    if (canReturnRandom)
        //    {
        //        ComAction ret = ComAction.Random(state);
        //        return ret;
        //    }
        //    return null;
        //}
        //public ComAction GetAction(BoardState state, int rotationalOffset)
        //{
        //    for (int i = 0; i < ActionPairs.Count; i++)
        //    {
        //        if (ActionPairs[i].State == state)
        //        {
        //            return ActionPairs[i].Action;
        //        }
        //    }
        //    return null;
        //}
        //public float GetPoints(BoardState state)
        //{
        //    for (int i = 0; i < ActionPairs.Count; i++)
        //    {
        //        if (ActionPairs[i].State == state)
        //        {
        //            return ActionPairs[i].Points;
        //        }
        //    }
        //    return 0;
        //}

        //public void ReverseAllStates()
        //{
        //    for (int i = 0; i < ActionPairs.Count; i++)
        //    {
        //        //Console.WriteLine(i);
        //        ActionPairs[i].Reverse();
        //    }
        //}

        //public void SetActionForState(ActionPair actionPair)
        //{
        //    bool success = false;
        //    for (int i = 0; i < ActionPairs.Count; i++)
        //    {
        //        if (ActionPairs[i].State == actionPair.State)
        //        {
        //            ActionPairs[i] = actionPair;
        //            success = true;
        //        }
        //    }
        //    if (!success)
        //    {
        //        ActionPairs.Add(actionPair);
        //    }
        //}
        
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