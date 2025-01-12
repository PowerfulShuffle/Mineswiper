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
        public bool IsWrong { get { return (State == States.Flagged && !HasMine) || (State == States.Mine); } }
        public int AdjacentMines { get
            {
                int mines = 0;
                foreach (Tile t in Neighbors) if (t.HasMine) mines++;
                return mines;
            } 
        }
        public Tile() 
        {
            HasMine = false;
            State = States.Hidden;
            Neighbors = [];
        }

        public PlayState Reveal() //returns false iff reveals bomb
        {
            if (State != States.Hidden) return PlayState.Playing;
            if (HasMine) { State = States.Mine; return PlayState.Done; }
            else 
            { 
                State = (States)AdjacentMines;
                if (State == States.Zero) foreach (Tile t in Neighbors) t.Reveal();
                return PlayState.Playing; 
            }

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
