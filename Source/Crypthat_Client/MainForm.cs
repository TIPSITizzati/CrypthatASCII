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
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Random rnd = new Random();

            // Impedisce la modifica delle dimensioni del form
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;

            // Imposta i valori di default della schermata iniziale
            txtNome.MaxLength = 18;
            txtNome.Text = "Utente_" + rnd.Next(10000); //Nome utente random
            gbSockets.Enabled = true;
            gbRs232.Enabled = false;
            cbNomePorta.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            txtIpAddress.Text = "127.0.0.1";
            nPorta.Maximum = 65535;
            nPorta.Minimum = 1;
            nPorta.Value = 11000;
            rbSockets.Checked = true;
        }

        private void rbSockets_CheckedChanged(object sender, EventArgs e)
        {
            gbSockets.Enabled = rbSockets.Checked;
        }

        private void rbRs232_CheckedChanged(object sender, EventArgs e)
        {
            gbRs232.Enabled = rbRs232.Checked;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            // In caso di connessione, controlla che tutti i parametri inseriti siano validi
            if (String.IsNullOrEmpty(txtNome.Text) || !ConvalidaNome(txtNome.Text)) // Il nome deve essere presente e non deve contenere determinati caratteri
            {
                MessageBox.Show("Nome inserito non valido. Controlla di avere inserito un nome che non contenga caratteri speciali (\" :;?~ \")", "Errore");
                return;
            }

            if(rbRs232.Checked && !cbNomePorta.Items.Contains(cbNomePorta.Text))    // Il nome della porta seriale deve essere corretto (in modalità Rs232)
            {
                MessageBox.Show("Nome porta invalido, selezionare una porta dall'elenco.", "Errore");
                return;
            }

            System.Net.IPAddress address = null;
            if (rbSockets.Checked && !System.Net.IPAddress.TryParse(txtIpAddress.Text, out address))    // L'indirizzo IP inserito deve essere valido (in modalità Sockets)
            {
                MessageBox.Show("Indirizzo IP invalido!", "Errore");
                return;
            }
            
            // Inizializza il form della lista utenti a seconda della modalità aporativa
            UserList list = null;
            if(rbSockets.Checked)
            {
                System.Net.IPEndPoint endPoint = new System.Net.IPEndPoint(address, (int)nPorta.Value);
                list = new UserList(txtNome.Text, ModalitaOperativa.Sockets, endPoint);
            }
            else if(rbRs232.Checked)
                list = new UserList(txtNome.Text, ModalitaOperativa.Sockets, cbNomePorta.Text);

            // Mostra il form della lista utenti
            list.Show();

            // Chiude il form corrente (senza terminare l'apllicazione)
            this.Close();
        }

        // Il nome non deve contenere caratteri di controllo come ":", ";", "?", "~"
        private bool ConvalidaNome(string Nome)
        {
            foreach (char i in Nome)
                if (":;?~".Contains(i))
                    return false;
            return true;
        }

        // Se è l'unico Form aperto chiude l'applicazione
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Application.OpenForms.Count == 1)
                Application.Exit();
        }
    }
}
