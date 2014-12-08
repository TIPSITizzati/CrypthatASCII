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
    public partial class UserList : Form
    {
        private GestoreLogicoClient gestoreClient;
        public Dictionary<Identity, ChatForm> chatAttive; // <Persona, Form>

        public UserList(string Me, ModalitaOperativa opMode, object param)
        {
            InitializeComponent();

            // Impedisce la modifica del form
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;

            // Inizializza la lista di chat attive
            chatAttive = new Dictionary<Identity, ChatForm>();

            switch (opMode)
            {
                case ModalitaOperativa.Rs232:
                    gestoreClient = new GestoreLogicoClient(opMode, Me, (string)param);
                    break;
                case ModalitaOperativa.Sockets:
                    gestoreClient = new GestoreLogicoClient(opMode, Me, (System.Net.IPEndPoint)param);
                    break;
            }

            gestoreClient.OnMessaggioRicevuto += gestoreClient_OnMessaggioRicevuto;
            gestoreClient.OnUtenteRegistrato += gestoreClient_OnUtenteRegistrato;
            gestoreClient.OnUtenteDisconnesso += gestoreClient_OnUtenteDisconnesso;

            lblUtenti.Text = "0 Utenti Connessi";
            lblNomeUtente.Text = Me;
        }

        void gestoreClient_OnUtenteDisconnesso(object sender, InterLevelArgs args)
        {
            // Previene azione cross-thread
            Invoke(new Action(() =>
            {
                // Controlla che non sia terminata la connessione con il server
                if((int)args.Data == -1)
                {
                    MessageBox.Show("Sei stato disconnesso forzatamente dal server!\nL'applicazione sarà chiusa.", "Errore di Connessione", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }

                // Rimuove l'utente dalla lista utenti
                lsUtenti.Items.RemoveAt((int)args.Data - 1);
                lblUtenti.Text = String.Format("{0} Utenti Connessi", lsUtenti.Items.Count);

                // Se è attiva una chat con l'utente in questione
                if(chatAttive.ContainsKey(args.Subject))
                {
                    chatAttive[args.Subject].DisabilitaChat();  // Disabilita la sessione
                }
            }));
        }

        private void gestoreClient_OnUtenteRegistrato(object sender, InterLevelArgs args)
        {
            // Previene azione cross-thread
            Invoke(new Action(() =>
            {
                // Se l'utente aggiunto non è il server
                if (gestoreClient.Destinatari.IndexOf(args.Subject) != 0)
                {
                    lsUtenti.Items.Add(args.Subject.Name);
                    lblUtenti.Text = String.Format("{0} Utenti Connessi", lsUtenti.Items.Count);
                }
            }));
        }

        private void gestoreClient_OnMessaggioRicevuto(object sender, InterLevelArgs args)
        {
            // Previene azione cross-thread
            Invoke(new Action(() =>
            {
                // Visualizza il messaggio nella rispettiva chat
                
                // Se la chat con l'utente da cui arriva il messaggio è già attiva
                if(chatAttive.ContainsKey(args.Subject))
                    chatAttive[args.Subject].ScriviMessaggio(args.Data.ToString(), args.Subject, (bool)sender == false ? Color.Black : Color.DarkGreen);
                else  // Sennò crea la chat relativa
                {
                    ChatForm nuovaChat = new ChatForm(this, gestoreClient, args.Subject);
                    nuovaChat.Show();
                    chatAttive.Add(args.Subject, nuovaChat);
                    nuovaChat.ScriviMessaggio(args.Data.ToString(), args.Subject, (bool)sender == false ? Color.Black : Color.DarkGreen);
                }
            }));
        }

        private void UserList_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Esce sempre dall'applicazione
            //TODO: Aggiungere chiusura concordata della connessione
            Application.Exit();
        }

        private void lsUtenti_DoubleClick(object sender, EventArgs e)
        {
            if(lsUtenti.SelectedIndex != -1)
            {
                if (!chatAttive.ContainsKey(gestoreClient.Destinatari[lsUtenti.SelectedIndex + 1]))
                {
                    ChatForm nuovaChat = new ChatForm(this, gestoreClient, gestoreClient.Destinatari[lsUtenti.SelectedIndex + 1]);
                    nuovaChat.Show();
                    chatAttive.Add(gestoreClient.Destinatari[lsUtenti.SelectedIndex + 1], nuovaChat);
                }
                else
                    chatAttive[gestoreClient.Destinatari[lsUtenti.SelectedIndex + 1]].Activate();
            }
        }
    }
}
