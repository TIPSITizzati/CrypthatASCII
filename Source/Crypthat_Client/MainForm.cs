using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Crypthat_Common;

namespace Crypthat_Client
{
    public partial class MainForm : Form
    {
        int chiave=0;
        Identity i; 
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            i = new Identity(i.Name, i.SessionKey);

            txtDisplay.Text = i.ToString();
        }

        private int CreaChiave(int chiave)
        {
            Random rnd = new Random();
            chiave += rnd.Next(1, 1000);

            chiave.ToString();

            return chiave;
        }

        
    }
}
