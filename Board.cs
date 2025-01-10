using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mineswiper
{
    internal abstract class Board
    {
        public abstract void SetNeighbors();
        public abstract void Draw(Graphics gr, Rectangle r);
    }

    internal class Board2D : Board
    {
        public Tile[,] Grid;
        public Board2D(int x = 6, int y = 6)
        {
            Grid = new Tile[x,y];
        }
        public override void SetNeighbors() { }
        public override void Draw(Graphics gr, Rectangle r)
        {
            gr.FillRectangle(Brushes.Blue, r);
            MessageBox.Show($"{r}");
        }
    }
}
