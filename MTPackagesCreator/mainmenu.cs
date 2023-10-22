using DiscordRPC.Logging;
using DiscordRPC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MTPackagesCreator
{
    public partial class mainmenu : Form
    {
        public mainmenu()
        {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(onClose);            
        }

        public DiscordRpcClient client;



        private void onClose(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to log out of MTPC? All unsaved data will be deleted!", "Exit?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Form1 f = new Form1();
                f.client.Dispose();
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void sourceToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void mainmenu_Load(object sender, EventArgs e)
        {
            
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newP newForm = new newP();
            newForm.ShowDialog();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to log out of MTPC? All unsaved data will be deleted!", "Exit?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            creatingProject c = new creatingProject();
            c.ShowDialog();
        }
    }
}
