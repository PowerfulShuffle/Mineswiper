using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mineswiper
{
    internal sealed partial class MainForm : Form
    {
        BufferedGraphics BufferedGraphics;
        Rectangle _boardSpace;
        Rectangle BoardSpace { get { return _boardSpace; } set 
            {
                if (value != new Rectangle()) _boardSpace = value;
                else _boardSpace.Size = new Size(this.Width/2, this.Height/2);
                this.Invalidate();
            } 
        }
        public MainForm()
        {
            this.Text = "Mineswiper Prototype";
            this.WindowState = FormWindowState.Maximized;
            BufferedGraphics = BufferedGraphicsManager.Current.Allocate(this.CreateGraphics(), this.DisplayRectangle);

            BoardSpace = new(50,50,50,50);
            Minesweeper m = new();

            SetToolStrip();

            this.Resize += (object? o, EventArgs ea) => BoardSpace = new();
            this.Paint += (object? o, PaintEventArgs ea) =>
            {
                BufferedGraphics.Graphics.Clear(Color.LightGray);
                BufferedGraphics.Graphics.FillRectangle(Brushes.Red, BoardSpace);
                BufferedGraphics.Render();
            };
        }
    }
}
