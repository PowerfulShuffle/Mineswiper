using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mineswiper
{
    internal sealed partial class Minesweeper
    {
        public MineForm? Parent;
        private Modes _mode;
        public Modes Mode { get { return _mode; } set 
            {
                if (_mode == value) return;
                _mode = value; 
                switch(value)
                {
                    case Modes.Auto:
                        if (Parent != null) Parent.mainButton.Color = Color.DarkSlateGray;
                        break;
                    case Modes.Play:
                        if (Parent != null) Parent.mainButton.Color = Color.FromArgb(58, 12, 163);
                        break;
                    case Modes.Analyse:
                        if (Parent != null) Parent.mainButton.Color = Color.FromArgb(114, 9, 183);
                        break;
                    case Modes.Build:
                        if (Parent != null) Parent.mainButton.Color = Color.FromArgb(247, 37, 133);
                        break;
                    default: { MessageBox.Show($"ERROR 1a MODE {value} INVALID"); return; }
                }
            }
        }
        public Solvers SelectedSolver;
        public Generators SelectedGenerator;
        public PlayState CurrentPlayState;
        public bool MinecountEnabled;
        private Board _board;
        public Board board
        {
            get { return _board; }
            set { _board = value; Parent?.Invalidate(); }
        }

        public Minesweeper(MineForm? f)
        {
            Parent = f;
            Mode = Modes.Auto;
            SelectedSolver = Solvers.None;
            SelectedGenerator = Generators.Random;
            CurrentPlayState = PlayState.FirstClick;
            MinecountEnabled = true;
            board = new Board(new int[] { 9, 9 }, 10);

            Mode = Modes.Play;
            //MainButtonPress();
        }

        public void MainButtonPress()
        {
            switch (Mode)
            {
                case Modes.Play:
                    Generate_Random();
                    CurrentPlayState = PlayState.FirstClick;
                    Parent?.UpdateBoard();
                    break;
                case Modes.Analyse: break;
                case Modes.Build: break;
                case Modes.Auto:
                default: { MessageBox.Show($"ERROR 1b MODE {Mode} INVALID"); return; }

            }
        }

        public void LeftClick (Tile tile)
        {
            switch (Mode)
            {
                case Modes.Play:
                    if (CurrentPlayState == PlayState.Playing || CurrentPlayState == PlayState.FirstClick)
                    { 
                        if (tile.State == States.Hidden)
                        {
                            CurrentPlayState = tile.Reveal();
                            if (CurrentPlayState == PlayState.Playing && board.CheckCompletion()) CurrentPlayState = PlayState.Won;
                            if (CurrentPlayState == PlayState.Lost) foreach (Tile t in board.Grid) if (t.HasMine) t.State = States.Mine;
                            Parent?.UpdateBoard();
                            break;
                        }
                        if ((int)tile.State > 0)
                        {
                            CurrentPlayState = tile.Chord();
                            if (CurrentPlayState == PlayState.Playing && board.CheckCompletion()) CurrentPlayState = PlayState.Won;
                            if (CurrentPlayState == PlayState.Lost) foreach (Tile t in board.Grid) if (t.HasMine) t.State = States.Mine;
                            Parent?.UpdateBoard();
                            break;
                        }
                    }
                    break;
                case Modes.Analyse: break;
                case Modes.Build: break;
                case Modes.Auto:
                default: { MessageBox.Show($"ERROR 1c MODE {Mode} INVALID"); return; }
            }
        }

        public void RightClick(Tile tile)
        {
            switch (Mode)
            {
                case Modes.Play:
                    if ((CurrentPlayState == PlayState.Playing || CurrentPlayState == PlayState.FirstClick))
                    {
                        if (tile.State == States.Hidden) { tile.State = States.Flagged; Parent?.UpdateBoard(); break; }
                        if (tile.State == States.Flagged) { tile.State = States.Hidden; Parent?.UpdateBoard(); break; }
                    }

                    break;
                case Modes.Analyse: break;
                case Modes.Build: break;
                case Modes.Auto:
                default: { MessageBox.Show($"ERROR 1c MODE {Mode} INVALID"); return; }
            }
        }
    }

    internal enum Modes
    {
        Auto = 0,
        Play = 1,
        Analyse = 2,
        Build = 3,
    }

    internal enum Solvers //ort, jsm, mso, qso, pla
    {
        None = 0,
        Trivial = 1
    }

    internal enum Generators //ran, low/hi, mso, atl
    {
        None = 0,
        Random = 1
    }

    internal enum PlayState
    {
        FirstClick = 0,
        Playing = 1,
        Won = 2,
        Lost = 3,
        Review = 4
    }
}
