using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiscordRPC;
using DiscordRPC.Logging;

namespace MTPackagesCreator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Initialize();
            FormClosing += Form1_FormClosing;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mainmenu f = new mainmenu();
            f.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure you don't want to accept the license agreement and want out?", "Exit?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public DiscordRpcClient client;

        void Initialize()
        {
            client = new DiscordRpcClient("1165395682769780807");
            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };
            client.Initialize();
                client.SetPresence(new RichPresence()
                {
                    Details = "-= MultiTerminal Project Creator [IDE] =-",
                    State = "Creates a new project...",
                    Assets = new Assets()
                    {
                        LargeImageKey = "p",
                        LargeImageText = "MultiTerminal Package Creator [IDE]",
                        SmallImageKey = "s"
                    },
                    Timestamps = new Timestamps()
                    {
                        Start = DateTime.UtcNow,
                    },
                    Buttons = new DiscordRPC.Button[]
                    {
                        new DiscordRPC.Button() { Label = "Download MTPC", Url = "https://google.ru" },
                        new DiscordRPC.Button() { Label = "Download MultiTerminal", Url = "https://google.ru" },
                    }

                });
            
            
            
        }

    }
}
