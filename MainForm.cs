using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mineswiper
{
    public sealed partial class MainForm : Form
    {

        BufferedGraphics bufferedGraphics;
        Rectangle _boardSpace;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle BoardSpace {  get { return _boardSpace; } set 
            {
                if (value != new Rectangle()) _boardSpace = value;
                else _boardSpace.Size = new Size(this.Width - 80, this.Height - 160);
                this.Invalidate(); 
            }
        }
        Point _camPos;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Point CamPos { get { return _camPos; } set { _camPos = value; this.Invalidate(); } }
        double _camZoom;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double CamZoom { get { return _camZoom; } set { _camZoom = value; this.Invalidate(); } }
        Size _tileSize;
        public Rectangle CurrentSpace; //Denotes last drawn position of board
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size TileSize
        {
            get { return _tileSize; }
            set
            {
                if (value != _tileSize) TexturesResized_Reset(value);
                _tileSize = value;
            }
        }
        public Dictionary<int, Image> TexturesBase;
        public Dictionary<int, Image> TexturesResized;
        Minesweeper minesweeper;

        public MainForm()
        {
            this.Text = "Mineswiper Prototype";
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new(230, 100);
            BoardSpace = new Rectangle(30, 80, 50, 50);
            CamPos = new Point(0, 0);
            CamZoom = 1;
            bufferedGraphics = BufferedGraphicsManager.Current.Allocate(this.CreateGraphics(), this.DisplayRectangle);
            //bufferedGraphics.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            TexturesBase = [];
            TexturesResized = [];
            TexturesBase_Reset("Textures");

            mainButton = new();
            minesweeper = new(this);
            SetToolStrip();

            this.Resize += (object? o, EventArgs ea) => BoardSpace = new();
            this.Paint += (object? o, PaintEventArgs pea) => UpdateBoard();
            this.MouseClick += (object? o, MouseEventArgs mea) =>
            {
                if (mea.Button != MouseButtons.Left) return;
                Tile? tile = TryPointToTupleToIndexToTile(mea.Location);
                if (tile != null) minesweeper.LeftClick(tile);
            };
            this.MouseDown += (object? o, MouseEventArgs mea) =>
            {
                if (mea.Button != MouseButtons.Right) return;
                Tile? tile = TryPointToTupleToIndexToTile(mea.Location);
                if (tile != null) minesweeper.RightClick(tile);
            };

            BoardSpace = new();
        }
        public void UpdateBoard()
        {
            bufferedGraphics.Graphics.Clear(Color.Lime);
            bufferedGraphics.Graphics.FillRectangle(Brushes.LimeGreen, BoardSpace);
            CurrentSpace = FixRectangle(BoardSpace, minesweeper.board.Dimensions);
            Draw();
            bufferedGraphics.Render();

            Rectangle FixRectangle(Rectangle rect, int[] dims)
            {
                if (dims.Length != 2) MessageBox.Show("WARNING BOARD MAY NOT DRAW CORRECTLY");
                int targettilelength = Math.Min(rect.Width / dims[0], rect.Height / dims[1]);
                rect.Size = new Size(targettilelength * dims[0], targettilelength * dims[1]);
                return rect;
            }

        }

        public void Draw()
        {
            TileSize = new Size(Math.Max(1, CurrentSpace.Width / minesweeper.board.Dimensions[0]), Math.Max(1, CurrentSpace.Height / minesweeper.board.Dimensions[1]));

            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            for (int x = 0; x < minesweeper.board.Dimensions[0]; x++) for (int y = 0; y < minesweeper.board.Dimensions[1]; y++)
                {
                    Rectangle tilespace = new(CurrentSpace.Location + new Size(x * TileSize.Width, y * TileSize.Height), TileSize);
                    int state = (int)minesweeper.board[x, y].State;
                    if (TexturesResized.TryGetValue(state, out Image? texture)) //checks if texture for value of tile exists
                    {
                        bufferedGraphics.Graphics.DrawImage(texture, tilespace.Location);
                    }
                    else //fallback tile drawing method
                    {
                        bufferedGraphics.Graphics.FillRectangle(Brushes.LightGray, tilespace);
                        bufferedGraphics.Graphics.DrawRectangle(Pens.Gray, tilespace);
                        bufferedGraphics.Graphics.DrawString(state.ToString(),
                            new Font("Verdana", (int)((double)Math.Min(TileSize.Width, TileSize.Height) * 0.4), FontStyle.Bold),
                            Brushes.Black,
                            CurrentSpace.Location + new Size(
                                (int)(((double)x + 0.52) * TileSize.Width),
                                (int)(((double)y + 0.54) * TileSize.Height)),
                            sf);
                    }
                    if (minesweeper.PlayState == PlayStates.Lost && minesweeper.Mode == Modes.Play && minesweeper.board[x, y].IsWrong)
                    {
                        bufferedGraphics.Graphics.DrawLine(new Pen(color: Color.Red,
                            (int)((double)Math.Min(TileSize.Width, TileSize.Height) * 0.1)),
                            tilespace.Location + TileSize / 10,
                            tilespace.Location + tilespace.Size - TileSize / 10);
                        bufferedGraphics.Graphics.DrawLine(new Pen(color: Color.Red,
                            (int)((double)Math.Min(TileSize.Width, TileSize.Height) * 0.1)),
                            new Point(tilespace.X + TileSize.Width / 10, tilespace.Y + tilespace.Height - TileSize.Height / 10),
                            new Point(tilespace.X + tilespace.Height - TileSize.Width / 10, tilespace.Y + TileSize.Height / 10));
                    }

                    /*bufferedGraphics.Graphics.DrawImage(TexturesBase[(int)((Tile)Grid.GetValue(x, y)).State],
                    new Rectangle(r.Location + new Size(x * TileSize.Width, y * TileSize.Height), TileSize));*/
                }
        }

        public Tile? TryPointToTupleToIndexToTile(Point p)
        {
            if (!CurrentSpace.Contains(p)) return null;
            switch (minesweeper.board.Dimensions.Length)
            {
                case 1:
                    return null;
                case 2:
                    int x = (p.X - CurrentSpace.X) / TileSize.Width;
                    int y = (p.Y - CurrentSpace.Y) / TileSize.Height;
                    if (x >= 0 && x < minesweeper.board.Dimensions[0] && y >= 0 && y < minesweeper.board.Dimensions[1]) return minesweeper.board.Grid[x + minesweeper.board.Dimensions[0] * y];
                    return null;
                case 3:
                    return null;
                default: throw new Exception();
            }
        }

        public void TexturesBase_Reset(string foldername)
        {
            TexturesBase.Clear();
            for (int i = -3; i <= 8; i++) TexturesBase.Add(i, GetImage(i.ToString() + ".png"));

            Image GetImage(string filename)
            {
                Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Mineswiper." + foldername + "." + filename);
                if (stream == null) { MessageBox.Show($"ERROR 2 CANT LOCATE <{filename}> IN FOLDER <{foldername}>"); throw new Exception(); }
                Image image = Image.FromStream(stream);
                stream.Dispose();
                return image;
            }
        }

        public void TexturesResized_Reset(Size s)
        {
            TexturesResized.Clear();
            foreach (var entry in TexturesBase) TexturesResized.Add(entry.Key, ResizeImage(entry.Value));

            Image ResizeImage(Image image)
            {
                var destRect = new Rectangle(0, 0, s.Width, s.Height);
                var destImage = new Bitmap(s.Width, s.Height);

                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }
                return destImage;
            }
        }
    }
}
