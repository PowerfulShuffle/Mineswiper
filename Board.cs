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
        Size _tileSize;
        public Size TileSize { get { return _tileSize; } set 
            {
                if (value != _tileSize) TexturesResized_Reset(value);
                _tileSize = value;
            } 
        }
        public Dictionary<int, Image> TexturesBase;
        public Dictionary<int, Image> TexturesResized;
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
            TexturesBase = [];
            TexturesResized = [];
            TexturesBase_Reset("Textures");

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
        public virtual void Draw(Graphics gr, Rectangle r, Point p, double z)
        {
            TileSize = new Size(Math.Max(1, r.Width / Dimensions[0]), Math.Max(1, r.Height / Dimensions[1]));

            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            for (int x = 0; x < Dimensions[0]; x++) for (int y = 0; y < Dimensions[1]; y++)
                {
                    Rectangle tilespace = new(r.Location + new Size(x * TileSize.Width, y * TileSize.Height), TileSize);
                    int state = (int)this[x,y].State;
                    if (TexturesResized.TryGetValue(state, out Image? texture)) //checks if texture for value of tile exists
                    {
                        gr.DrawImage(texture, tilespace.Location);
                    }
                    else //fallback tile drawing method
                    {
                        gr.FillRectangle(Brushes.LightGray, tilespace);
                        gr.DrawRectangle(Pens.Gray, tilespace);
                        gr.DrawString(state.ToString(),
                            new Font("Verdana", (int)((double)Math.Min(TileSize.Width, TileSize.Height) * 0.4), FontStyle.Bold),
                            Brushes.Black,
                            r.Location + new Size(
                                (int)(((double)x + 0.52) * TileSize.Width),
                                (int)(((double)y + 0.54) * TileSize.Height)),
                            sf);
                    }

                    /*gr.DrawImage(TexturesBase[(int)((Tile)Grid.GetValue(x, y)).State],
                    new Rectangle(r.Location + new Size(x * TileSize.Width, y * TileSize.Height), TileSize));*/
                }
        }

        public virtual int? TryPointToTupleToIndex(Point p, Rectangle r, Point camPos, double camZoom)
        {
            if (!r.Contains(p)) return null;
            switch (Dimensions.Length)
            {
                case 1:
                    return null;
                case 2:
                    int x = (p.X - r.X) / TileSize.Width;
                    int y = (p.Y - r.Y) / TileSize.Height;
                    if (x >= 0 && x < Dimensions[0] && y >= 0 && y < Dimensions[1]) return (x + Dimensions[0] * y);
                    return null;
                case 3:
                    return null;
                default: throw new Exception();
            }
        }

        public virtual void TexturesBase_Reset(string foldername)
        {
            TexturesBase.Clear();
            for (int i = -2; i <= 8; i++) TexturesBase.Add(i, GetImage(i.ToString() + ".png"));

            Image GetImage(string filename)
            {
                Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Mineswiper." + foldername + "." + filename);
                if (stream == null) { MessageBox.Show($"ERROR 2 CANT LOCATE <{filename}> IN FOLDER <{foldername}>"); throw new Exception(); }
                Image image = Image.FromStream(stream);
                stream.Dispose();
                return image;
            }
        }

        public virtual void TexturesResized_Reset(Size s)
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
