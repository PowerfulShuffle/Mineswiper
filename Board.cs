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
        public Dictionary<int, Image> TexturesBase;
        public Dictionary<int, Image> TexturesResized;
        public Array Grid;
        public int Minecount;
        public Rectangle CurrentSpace {  get; protected set; } //Denotes last drawn position of board

        public Board(int x = 30, int y = 16, int m = 99)
        {
            Grid = new Tile[x, y];
            Minecount = m;
            for (int i = 0; i < Grid.GetLength(0); i++) for (int j = 0; j < Grid.GetLength(1); j++) Grid.SetValue(new Tile(), i, j);
            TexturesBase = new();
            TexturesResized = new();
            TexturesBase_Reset("Textures");
        }

        public virtual void SetNeighbors()
        {

        }

        public virtual void SetMines(Array Mine)
        {

        }
        public virtual void Draw(Graphics gr, Rectangle r, Point p, double z)
        {
            CurrentSpace = r;
            Size TileSize = new Size(Math.Max(1, r.Width / Grid.GetLength(0)), Math.Max(1, r.Height / Grid.GetLength(1)));
            TexturesResized_Reset(TileSize);

            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            for (int x = 0; x < Grid.GetLength(0); x++) for (int y = 0; y < Grid.GetLength(1); y++)
                {
                    Rectangle tilespace = new(r.Location + new Size(x * TileSize.Width, y * TileSize.Height), TileSize);
                    int state = (int)((Tile)Grid.GetValue(x, y)).State;
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
