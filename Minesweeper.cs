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
            MinecountEnabled = true;
            board = new Board(new int[] { 9, 9 }, 10);
            Mode = Modes.Play;
        }

        public void MainButtonPress()
        {
            switch (Mode)
            {
                case Modes.Play:
                    Generate_Random();
                    Parent?.UpdateBoard();
                    break;
                case Modes.Analyse: break;
                case Modes.Build: break;
                case Modes.Auto:
                default: { MessageBox.Show($"ERROR 1b MODE {Mode} INVALID"); return; }

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
}
