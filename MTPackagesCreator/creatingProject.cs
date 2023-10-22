using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace MTPackagesCreator
{

    public partial class creatingProject : Form
    {
        private string _name;
        private string _location;
        private string _type;
        public creatingProject()
        {
            InitializeComponent();
            FormClosing += CreatingProject_FormClosing;
        }

        private void CreatingProject_FormClosing(object sender, FormClosingEventArgs e)
        {
            Thread t = new Thread(() => { MessageBox.Show("Sorry, but you can't close the window while creating a project! Wait a little longer, it will be ready soon!"); });
            t.Start();
            e.Cancel = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void start(string NameProject, string LocationProject, string TypeProject)
        {
            _name = NameProject;
            _location = LocationProject;
            _type = TypeProject;
            ShowDialog();
            
            
        }

        private void STOP(string reason)
        {
            MessageBox.Show(reason, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            newP f = new newP();
            f.Close();
            this.Close();
        }

        private void creatingProject_Load(object sender, EventArgs e)
        {
            try
            {
                Directory.CreateDirectory(_location + "\\build");
                File.WriteAllText(_location + "\\" + _name + ".mproject", $@"{_name}
{_type}");
            }
            catch (Exception ex)
            {
                STOP(ex.Message);
            }
        }
    }
}
