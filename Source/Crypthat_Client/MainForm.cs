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
        GestoreLogicoClient clientManager;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            clientManager = new GestoreLogicoClient(ModalitaOperativa.Rs232, new Identity("Mr. Artum", null), "COM1");
            clientManager.OnMessaggioRicevuto += clientManager_OnMessaggioRicevuto;
        }

        void clientManager_OnMessaggioRicevuto(object sender, InterLevelArgs args)
        {
            txtDisplay.Text += args.Subject.Name + ") " + args.Data + "\n\r";
        }

        private void btn_Click(object sender, EventArgs e)
        {
            if(clientManager != null)
            {
                clientManager.InviaMessaggio(txtScrivi.Text, clientManager.Destinatari[0]);
            }
        }

        
    }
}
