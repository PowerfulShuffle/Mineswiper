using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mineswiper
{
    internal sealed partial class Minesweeper
    {
        static readonly Random Rand = new();
        public void Generate_Random()
        {
            foreach (Tile tile in board.Grid) tile.State = States.Hidden;
            if (MinecountEnabled)
            {
                if (board.Minecount > board.Grid.Length) { MessageBox.Show($"WARNING 1 MINECOUNT ({board.Minecount}) EXCEEDS AVAILABLE TILES ({board.Grid.Length})"); return; }
                List<int> remainingtileindexes = [];
                for (int i = 0; i < board.Grid.Length; i++)
                {
                    remainingtileindexes.Add(i);
                    board.Grid[i].HasMine = false;
                }
                for (int i = 0; i < board.Minecount; i++)
                {
                    int tileindex = remainingtileindexes[Rand.Next(remainingtileindexes.Count)];
                    remainingtileindexes.Remove(tileindex);
                    board.Grid[tileindex].HasMine = true;
                }

            }
            else
            {
                double density = (double)board.Minecount / (double)board.Grid.Length;
                for (int i = 0; i < board.Grid.Length; i++)
                {
                    if (Rand.Next(1000) < (int)(density * 1000))
                    {
                        board.Grid[i].HasMine = true;
                    }
                    else
                    {
                        board.Grid[i].HasMine = false;
                    }
                }
            }
        }
    }
}
