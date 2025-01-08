using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mineswiper
{
    internal sealed partial class MainForm : Form
    {
        private void SetToolStrip()
        {

            ToolStripContainer toolStripContainer = new(); //container setup
            toolStripContainer.Dock = DockStyle.Top;
            toolStripContainer.Height = 25;


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
            Mode_DD.Items.Add(GetButton("Play", () => MessageBox.Show("")));
            Mode_DD.Items.Add(GetButton("Analyse", () => MessageBox.Show("")));
            Mode_DD.Items.Add(GetButton("Build", () => MessageBox.Show("")));

            ToolStrip toolStrip = new(); // add main buttons
            toolStrip.Items.Add(GetButton("New", () => MessageBox.Show("")));
            toolStrip.Items.Add(File);
            toolStrip.Items.Add(Edit);
            toolStrip.Items.Add(Mode);

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
}
