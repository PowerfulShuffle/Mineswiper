using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mineswiper
{
    public sealed partial class MineForm : Form
    {
        BufferedGraphics bufferedGraphics;
        Label BoardSpace;
        Minesweeper minesweeper;
        public MineForm()
        {
            this.Text = "Mineswiper Prototype";
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new(230, 100);
            BoardSpace = new();
            BoardSpace.BackColor = Color.White;
            BoardSpace.Bounds = new Rectangle(30, 80, 50, 50);
            bufferedGraphics = BufferedGraphicsManager.Current.Allocate(this.BoardSpace.CreateGraphics(), this.DisplayRectangle);
            this.Controls.Add(BoardSpace);
            minesweeper = new(this);

            mainButton = new();
            SetToolStrip();

            this.Resize += (object? o, EventArgs ea) => { BoardSpace.Size = new Size(this.Width / 2, this.Height / 2); this.BoardSpace.Invalidate(); };
            this.BoardSpace.Paint += (object? o, PaintEventArgs ea) =>
            {
                bufferedGraphics.Graphics.Clear(Color.Pink);
                minesweeper.board.Draw(bufferedGraphics.Graphics, BoardSpace.Bounds);
                bufferedGraphics.Graphics.FillRectangle(Brushes.Yellow, 100, 100, 100, 100);
                bufferedGraphics.Render();
            };
        }
    }
}
