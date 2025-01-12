using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mineswiper
{
    internal class Tile
    {
        public bool HasMine;
        public States State;
        public List<Tile> Neighbors;
        public Color Highlight;
        public bool IsWrong { get { return (State == States.Flagged && !HasMine); } }
        public int AdjacentMines { get
            {
                int mines = 0;
                foreach (Tile t in Neighbors) if (t.HasMine) mines++;
                return mines;
            } 
        }
        public int AdjacentFlags
        {
            get
            {
                int flags = 0;
                foreach (Tile t in Neighbors) if (t.State == States.Flagged) flags++;
                return flags;
            }
        }
        public Tile() 
        {
            HasMine = false;
            State = States.Hidden;
            Neighbors = [];
        }

        public PlayState Reveal() //returns playstate after revealing 
        {
            if (State != States.Hidden && State != States.Flagged) return PlayState.Playing;
            if (HasMine) { State = States.Mine; return PlayState.Lost; }
            else 
            { 
                State = (States)AdjacentMines;
                if (State == States.Zero) foreach (Tile t in Neighbors) t.Reveal();
                return PlayState.Playing; 
            }
        }
        public PlayState Chord() //returns playstate after revealing 
        {
            if ((int)State <= 0) throw new Exception();
            if ((int)State == AdjacentFlags) foreach (Tile tile in Neighbors) if (tile.State != States.Flagged) if (tile.Reveal() == PlayState.Lost) return PlayState.Lost;
            return PlayState.Playing;
        }

        public void Splash()
        {
            MessageBox.Show("no");
            //foreach (Tile tile in Neighbors) tile.State = States.Seven;
        }
    }
    internal enum States
    {
        None = -4,
        Mine = -3,
        Flagged = -2,
        Hidden = -1,

        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,

        Nine = 9,
        Ten = 10,
        Eleven = 11,
        Twelve = 12
    }
}
