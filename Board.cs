using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mineswiper
{
    internal class Board
    {
        public readonly int[] Dimensions;
        public readonly Tile[] Grid;
        public int Minecount;


        public Board(int[] dim, int m)
        {
            Dimensions = dim;
            Grid = new Tile[Product(Dimensions)];
            Minecount = m;
            for(int i = 0; i < Grid.Length; i++) Grid[i] = new Tile();
            SetNeighbors();

            static int Product(int[] arr)
            {
                int res = 1;
                foreach (int i in arr) res *= i;
                return res;
            }
        }

        public Tile this[int x] //indexers throw an exception if dimension doesnt match input
        {
            get => Dimensions.Length != 1 ? throw new Exception() : Grid[x]; 
            set { if (Dimensions.Length != 1) throw new Exception(); Grid[x] = value; }
        }
        public Tile this[int x, int y]
        {
            get => Dimensions.Length != 2 ? throw new Exception() : Grid[x + Dimensions[0] * y];
            set { if (Dimensions.Length != 2) throw new Exception(); Grid[x + Dimensions[0] * y] = value; }
        }
        public Tile this[int x, int y, int z]
        {
            get => Dimensions.Length != 3 ? throw new Exception() : Grid[x + Dimensions[0] * (y + Dimensions[1] * z)];
            set { if (Dimensions.Length != 3) throw new Exception(); Grid[x + Dimensions[0] * y] = value; }
        }

        public virtual void SetNeighbors()
        {
            switch (Dimensions.Length)
            {
                case 1: break;

                case 2:
                    for(int x = 0; x < Dimensions[0]; x++) for(int y = 0; y < Dimensions[1]; y++)
                        {
                            Tile tile = this[x, y];
                            if (x != 0 && x != Dimensions[0] - 1 && y != 0 && y != Dimensions[1] - 1) //case central cells
                            {
                                for (int i = -1; i < 2; i++) for (int j = -1; j < 2; j++)
                                    {
                                        tile.Neighbors.Add(this[x+i, y+j]);
                                    }
                            }
                            else //case edge cells
                            {
                                for (int i = -1; i < 2; i++) for (int j = -1; j < 2; j++)
                                    {
                                        if (x + i >= 0 && x + i < Dimensions[0] && y + j >= 0 && y + j < Dimensions[1]) tile.Neighbors.Add(this[x + i, y + j]);
                                    }
                            }
                        }
                    break;

                case 3: break;
            }
        }

        public virtual bool CheckCompletion()
        {
            foreach (Tile t in Grid) if (t.State == States.Hidden && !t.HasMine) return false;
            foreach (Tile t in Grid) if (t.State == States.Hidden) t.State = States.Flagged;
            return true;
        }
    }
}
