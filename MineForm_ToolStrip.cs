using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;

namespace Mineswiper
{
    public sealed partial class MineForm : Form
    {
        public ToolStripButton_Adjusted mainButton;
        private void SetToolStrip()
        {
            ToolStripContainer toolStripContainer = new(); //container setup
            toolStripContainer.Dock = DockStyle.Top;
            toolStripContainer.Height = 50;

            ToolStripDropDownButton File = new(); //File
            File.Text = "File";
            ToolStripDropDown File_DD = new();
            File.DropDown = File_DD;
            File_DD.Items.Add(GetButton("Open File...", () => MessageBox.Show("")));
            File_DD.Items.Add(GetButton("Save As...", () => MessageBox.Show("")));
            File_DD.Items.Add(GetButton("Import Line...", () => MessageBox.Show("")));
            File_DD.Items.Add(GetButton("Export Line...", () => MessageBox.Show("")));
            File_DD.Items.Add(GetButton("Print...", () => MessageBox.Show("")));
            File_DD.Items.Add(GetButton("Save As Image...", () => MessageBox.Show("")));

            ToolStripDropDownButton Edit = new(); //Edit
            Edit.Text = "Edit";
            ToolStripDropDown Edit_DD = new();
            Edit.DropDown = Edit_DD;
            Edit_DD.Items.Add(GetButton("Undo", () => MessageBox.Show("")));
            Edit_DD.Items.Add(GetButton("Redo", () => MessageBox.Show("")));
            Edit_DD.Items.Add(GetButton("Restart", () => MessageBox.Show("")));

            ToolStripDropDownButton Mode = new(); //Mode
            Mode.Text = "Mode";
            ToolStripDropDown Mode_DD = new();
            Mode.DropDown = Mode_DD;
            Mode_DD.Items.Add(GetButton("Play", () => minesweeper.Mode = Modes.Play));
            Mode_DD.Items.Add(GetButton("Analyse", () => minesweeper.Mode = Modes.Analyse));
            Mode_DD.Items.Add(GetButton("Build", () => minesweeper.Mode = Modes.Build));

            mainButton.AutoSize = false; //mainButton
            mainButton.Size = new Size(45, 45);
            mainButton.TextAlign = ContentAlignment.MiddleCenter;
            mainButton.Font = new Font("", 25, FontStyle.Bold);
            mainButton.Click += (object? o, EventArgs mea) => minesweeper.MainButtonPress();

            ToolStripDropDownButton Generator = new(); //Engine
            Generator.ForeColor = Color.FromArgb(58, 12, 163);
            Generator.Text = "Random";
            ToolStripDropDown Generator_DD = new();
            Generator.DropDown = Generator_DD;
            Generator_DD.Items.Add(GetButton("None", () => { minesweeper.SelectedGenerator = Generators.None; Generator.Text = "None"; }));
            Generator_DD.Items.Add(GetButton("Random", () => { minesweeper.SelectedGenerator = Generators.Random; Generator.Text = "Random"; }));


            ToolStrip toolStrip = new(); // add main buttons
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.Items.Add(File);
            toolStrip.Items.Add(Edit);
            toolStrip.Items.Add(Mode);
            toolStrip.Items.Add(mainButton);
            toolStrip.Items.Add(Generator);

            toolStripContainer.TopToolStripPanel.Controls.Add(toolStrip);//finish stuff
            Controls.Add(toolStripContainer);

            ToolStripButton GetButton(string Text, Action Function)
            {
                ToolStripButton res = new();
                res.ForeColor = Color.Black;
                res.Text = Text;
                res.Click += (object? o, EventArgs mea) => Function();
                return res;
            }
        }
    }

    public class ToolStripButton_Adjusted : ToolStripButton
    {
        private Color _color;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color Color
        {
            get { return _color; }
            set { _color = value; this.Invalidate(); }
        }
        public ToolStripButton_Adjusted()
        {
            Color = Color.DarkSlateGray;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            StringFormat sf = new();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString("⯄", new Font("Verdana", 30, FontStyle.Bold), new SolidBrush(Color), new Rectangle(this.Height/24, this.Height/18, this.Width, this.Height), sf);
        }
    }
}
