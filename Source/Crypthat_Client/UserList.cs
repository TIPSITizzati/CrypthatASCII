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

        /* Un Dictionary è una lista organizzata dove sono presenti:
         *      - object Indice
         *      - object Valore
         * Nel nostro caso vengono utilizzati come indici le Identity (invece che 0,1,2,3,...) e come valore una Form che gli corrisponde.
         * Così facendo per verificare se una chat con Identity(PincoPallino) è aperta, si fa Form = Dictionary[Identity(PincoPallino)]
         */
        public Dictionary<Identity, ChatForm> chatAttive; // Lista delle chat attualmente aperte, legate ad ogni persona Dictionary<Identity, Form>

        public UserList(string Me, ModalitaOperativa opMode, object param)
        {
            InitializeComponent();

            // Impedisce la modifica del form
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;

            // Inizializza la lista di chat attive
            chatAttive = new Dictionary<Identity, ChatForm>();

            try
            {
                // Cerca di inizializzare il GestoreLogico nella modalità selezionata
                switch (opMode)
                {
                    case ModalitaOperativa.Rs232:
                        gestoreClient = new GestoreLogicoClient(opMode, Me, (string)param);
                        break;
                    case ModalitaOperativa.Sockets:
                        gestoreClient = new GestoreLogicoClient(opMode, Me, (System.Net.IPEndPoint)param);
                        break;
                }
            }
            catch (Exception ex)
            {
                // In caso di errore mostra un corretto messaggio di eccezione
                MessageBox.Show("Si è verificato un errore in fase di connessione.\n" + ex.Message, "Errore");
                Application.Exit();
                return;
            }

            // Imposta i vari eventi del GestoreLogicoClient
            gestoreClient.OnMessaggioRicevuto += gestoreClient_OnMessaggioRicevuto; // Un messaggio viene ricevuto
            gestoreClient.OnUtenteRegistrato += gestoreClient_OnUtenteRegistrato;   // Un utente si registra
            gestoreClient.OnUtenteDisconnesso += gestoreClient_OnUtenteDisconnesso; // Un utente si è disconnesso

            // Imposta i parametri di default dell'interfaccia grafica
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
                    // Registra l'utente alla lista degli utenti connessi ed aggiorna il contatore
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
                    chatAttive.Add(args.Subject, nuovaChat);    // Collega al mittente la nuova chat aperta

                    // Chiama il metodo per scrivere il messaggio, che appartiene a ChatForm, se il messaggio è cifrato scrive in verde
                    // In "sender" è contenuto il booleano che indica se il messaggio è cifrato
                    // (bool)sender == false ? Color.Black : Color.DarkGreen è una operazione logica su una righa che equivale a:
                    // se sender(castato a booleano) è false, allora usa il colore nero, sennò usa il colore verde. quindi
                    // se il messaggio non è cifrato, scrivi in nero, sennò scrivi in verde
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
                // Se non esiste una chat aperta per l'utente selezionato
                if (!chatAttive.ContainsKey(gestoreClient.Destinatari[lsUtenti.SelectedIndex + 1]))
                {
                    // Crea una nuova form per la chat con l'utente specificato
                    ChatForm nuovaChat = new ChatForm(this, gestoreClient, gestoreClient.Destinatari[lsUtenti.SelectedIndex + 1]);
                    nuovaChat.Show();

                    // Lega all'utente selezionato la form aperta
                    chatAttive.Add(gestoreClient.Destinatari[lsUtenti.SelectedIndex + 1], nuovaChat);
                }
                else
                    chatAttive[gestoreClient.Destinatari[lsUtenti.SelectedIndex + 1]].Activate();
            }
        }
    }
}
