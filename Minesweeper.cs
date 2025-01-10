using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mineswiper
{
    internal class Minesweeper
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
                    default: { MessageBox.Show("ERROR"); return; }
                }
            }
        }
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
            board = new Board2D();
        }
    }

    internal enum Modes
    {
        Auto = 0,
        Play = 1,
        Analyse = 2,
        Build = 3,
    }
}
