﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mineswiper
{
    public sealed partial class MineForm : Form
    {

        BufferedGraphics bufferedGraphics;
        Rectangle _boardSpace;
        Rectangle BoardSpace {  get { return _boardSpace; } set 
            {
                if (value != new Rectangle()) _boardSpace = value;
                else _boardSpace.Size = new Size(this.Width - 80, this.Height - 160);
                this.Invalidate(); 
            }
        }
        Point _camPos;
        Point CamPos { get { return _camPos; } set { _camPos = value; this.Invalidate(); } }
        double _camZoom;
        double CamZoom { get { return _camZoom; } set { _camZoom = value; this.Invalidate(); } }
        Minesweeper minesweeper;
        public MineForm()
        {
            this.Text = "Mineswiper Prototype";
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new(230, 100);
            BoardSpace = new Rectangle(30, 80, 50, 50);
            CamPos = new Point(0, 0);
            CamZoom = 1;
            bufferedGraphics = BufferedGraphicsManager.Current.Allocate(this.CreateGraphics(), this.DisplayRectangle);
            //bufferedGraphics.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            mainButton = new();
            minesweeper = new(this);
            SetToolStrip();

            minesweeper = new(this);
            this.Resize += (object? o, EventArgs ea) => BoardSpace = new();
            this.Paint += (object? o, PaintEventArgs ea) =>
            {
                bufferedGraphics.Graphics.Clear(Color.Lime);
                bufferedGraphics.Graphics.FillRectangle(Brushes.LimeGreen, BoardSpace);
                minesweeper.board.Draw(bufferedGraphics.Graphics, BoardSpace, CamPos, CamZoom);
                bufferedGraphics.Render();
            };

            BoardSpace = new();
        }
    }
}
