using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace MTPackagesCreator
{
    public partial class newP : Form
    {
        private bool doneName = false;
        private bool doneLocation = false;
        private bool doneSelected = false;
        private string selItem = "";

        protected bool debug = false; 

        public newP()
        {
            InitializeComponent();
            name.KeyPress += Name_KeyPress;
            if (debug)
            {
                _nameTEST.Visible = true;
                _locationTEST.Visible = true;
                _listTEST.Visible = true;
            }
            else
            {
                _nameTEST.Visible = false;
                _locationTEST.Visible = false;
                _listTEST.Visible = false;
            }
        }

        private bool checking()
        {
            /* debug */
            if (debug)
            {
                _nameTEST.Invoke(new Action(() => _nameTEST.Text = Convert.ToString(doneName)));
                _locationTEST.Invoke(new Action(() => _locationTEST.Text = Convert.ToString(doneLocation)));
                _listTEST.Invoke(new Action(() => _listTEST.Text = Convert.ToString(doneSelected)));
            }
            /* END */

            if (doneLocation && doneName && doneSelected)
            {
                createButton.Invoke(new Action(() => createButton.Enabled = true));
                return true;
            }
            else 
            { 
                createButton.Invoke(new Action(() => createButton.Enabled = false));
                return false;
            }
        }

        private void Name_KeyPress(object sender, KeyPressEventArgs e)
        {
            var regex = new Regex(@"[^a-zA-Z0-9\b]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                ToolTip t = new ToolTip();
                t.Show("I'm sorry, but that symbol is forbidden! Allowed characters: a-z; A-Z; 0-9.", name);
                e.Handled = true;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            
            if(treeView1.SelectedNode.Index == 0)
            {
                richTextBox1.Text = "(Select type)";
                doneSelected = false;
                checking();
                listView1.Items.Clear();
                ListViewItem newItem = new ListViewItem("Program Package");
                ListViewItem newItem2 = new ListViewItem("Service Pack");
                newItem.ImageIndex = 1;
                newItem2.ImageIndex = 1;
                listView1.Items.Add(newItem);
                listView1.Items.Add(newItem2);
                listView1.TileSize = new Size(300, 40);
            }
            else if (treeView1.SelectedNode.Index == 1)
            {
                richTextBox1.Text = "(Select type)";
                doneSelected = false;
                checking();
                listView1.Items.Clear();
                ListViewItem newItem = new ListViewItem("Firmware package");
                ListViewItem newItem2 = new ListViewItem("Service Pack");
                newItem.ImageIndex = 1;
                newItem2.ImageIndex = 1;
                listView1.Items.Add(newItem);
                listView1.TileSize = new Size(300, 40);
                listView1.Items.Add(newItem2);
            }
            else if (treeView1.SelectedNode.Index == 2)
            {
                richTextBox1.Text = "(Select type)";
                doneSelected = false;
                checking();
                listView1.Items.Clear();
                ListViewItem newItem = new ListViewItem("Source code");
                newItem.ImageIndex = 2;
                listView1.TileSize = new Size(300, 40);
                listView1.Items.Add(newItem);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string selectedName = listView1.SelectedItems[0].Text.ToLower();
                if (selectedName == "program package")
                {
                    selItem = "pp";
                    richTextBox1.Text = "This is a package for your programs under MultiTerminal. Due to maximum compatibility of programs under MT, we ask you to create programs on C# .Net Framework 4.8.2 (optional), but if you want the program to work on another platform, it is your choice. We also ask you to release programs under MTFXP (MultiTerminal For XP) (you can use .net framework 4.0 for this).";
                    doneSelected = true;
                    checking();
                }
                else if (selectedName == "service pack")
                {
                    selItem = "sp";
                    richTextBox1.Text = "This package is for upgrades. In the official environment it is used to update MT to new versions, in the environment of third-party developers, as an update package for their programs.";
                    doneSelected = true;
                    checking();
                }
                else if (selectedName == "source code")
                {
                    selItem = "sc";
                    richTextBox1.Text = @"With this module you can download the source code of MultiTerminal, for later editing it in VisualStudio.
Mandatory requirements for working with the source code:
- VisualStudio 2019
- .Net Framework 4.0
- .Net Framework 4.8.2";
                    doneSelected = true;
                    checking();
                }
                else if (selectedName == "firmware package")
                {
                    selItem = "fp";
                    richTextBox1.Text = "This module helps you to create your own firmware for MT.What is the difference between a service pack and a firmware pack ? A service pack updates some files from MultiTerminal or third - party programs.A firmware package is for MT only. In it you can load your version of MultiTerminal that you have made and compiled in VisualStudio.";
                    doneSelected = true;
                    checking();
                }
                else
                {
                    doneSelected = false;
                    selItem = "";
                    richTextBox1.Text = "(Select type)";
                    checking();
                }
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void name_TextChanged(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(name.Text))
            {
                doneName = false;
                checking();
            }
            else
            {
                if (name.TextLength == 32)
                {
                    ToolTip t = new ToolTip();
                    t.Show("Sorry, but the maximum name value is 32 characters!", name);
                    doneName = false;
                    checking();
                }
                else
                {
                    doneName = true;
                    checking();
                }
            }
            
        }

        private void locationButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            location.Text = folderBrowserDialog1.SelectedPath;
        }

        private void location_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(location.Text, @"\s"))
            {
                ToolTip t = new ToolTip();
                t.Show("Your path contains gaps!", location);
                doneLocation = false;
                checking();
            }
            else
            {
                if (string.IsNullOrEmpty(location.Text))
                {
                    doneLocation = false;
                    checking();
                }
                else
                {
                    if (System.IO.Directory.Exists(location.Text))
                    {
                        string[] files = System.IO.Directory.GetFiles(location.Text);
                        string[] subdirectories = System.IO.Directory.GetDirectories(location.Text);
                        if (files.Length > 0 && subdirectories.Length > 0)
                        {
                            doneLocation = false;
                            ToolTip t = new ToolTip();
                            t.Show("The folder is not empty", location);
                            checking();
                        }
                        else if (files.Length > 0 || subdirectories.Length > 0)
                        {
                            doneLocation = false;
                            ToolTip t = new ToolTip();
                            t.Show("The folder is not empty", location);
                            checking();
                        }
                        else
                        {
                            doneLocation = true;
                            checking();
                        }
                    }
                    else
                    {
                        doneLocation = false;
                        checking();
                    }
                }
            }
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            if (checking())
            {
                MessageBox.Show(location.Text);
                creatingProject f = new creatingProject();
                this.Hide();
                f.start(name.Text, location.Text, selItem);
            }
        }
    }
}
