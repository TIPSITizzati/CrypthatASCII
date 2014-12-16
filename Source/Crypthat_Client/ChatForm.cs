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
using Crypthat_Common.Crittografia;
using System.Runtime.InteropServices;

namespace Crypthat_Client
{
    public partial class ChatForm : Form
    {
        private Identity dest;  // Utente di destinazione a cui è legato il form
        private GestoreLogicoClient gestoreClient; //Riferimento al GestoreLogico
        private UserList parent;    // Riferimento alla lista utenti, utilizzato per rimuovere la chat dalle chat attive in caso di chiusura

        public ChatForm(UserList parent, GestoreLogicoClient currentGestore, Identity dest)
        {
            InitializeComponent();

            this.dest = dest;
            this.gestoreClient = currentGestore;
            this.parent = parent;
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            // Imposta testi di default
            lblName.Text = dest.Name;
            txtSend.Select();
            txtChat.ReadOnly = true;

            // Imposta l'evento per bloccare l'Invio
            txtSend.KeyPress += txtSend_KeyPress;

            this.Text = "Crypthat - Chat ( " + dest.Name + " )";
        }

        // Evento che intercetta l'Enter premuto nella textBox dell'invio
        private void txtSend_KeyPress(object sender, KeyPressEventArgs e)
        {
            if((Keys)e.KeyChar == Keys.Enter)
            {
                InviaMessaggio();
                e.Handled = true;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            InviaMessaggio();
        }

        private void btnASCIIArt_Click(object sender, EventArgs e)
        {
            InviaMessaggio(true);
        }

        // Se la Chat si sta chiudendo, allora si rimuove dalla lista delle chat attive
        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.chatAttive.Remove(dest);
        }

        // Metodo per l'invio del messaggio al destinatario
        private void InviaMessaggio(bool ASCII = false)
        {
            string Messaggio = txtSend.Text.Trim('\n', '\r', ' ');
            // Se si ha del testo da inviare
            if(!String.IsNullOrEmpty(Messaggio))
            {
                if (!ASCII)
                {
                    // Invia il messaggio sotto forma di test
                    gestoreClient.InviaMessaggio(Messaggio, dest, chkEncrypt.Checked);
                    ScriviMessaggio(Messaggio, gestoreClient.Me, chkEncrypt.Checked ? Color.DarkGreen : txtChat.ForeColor);
                    txtSend.Clear();
                }
                else
                {
                    // Trasfroma il messaggio in ASCII Art
                    Messaggio = "\n" + ASCIIArtCipher.GenerateASCIIArt(Messaggio, 12);

                    // Invia il messaggio sotto forma di ASCII Art
                    gestoreClient.InviaMessaggio(Messaggio, dest, chkEncrypt.Checked);
                    ScriviMessaggio(Messaggio, gestoreClient.Me, chkEncrypt.Checked ? Color.DarkGreen : txtChat.ForeColor);
                    txtSend.Clear();
                }
            }
        }


        public void ScriviMessaggio(string Messaggio, Identity Mittente, Color Colore)
        {
            // Colora il testo ed effettua l'autoscroll
            txtChat.SelectionStart = txtChat.TextLength;
            txtChat.SelectionLength = 0;
            txtChat.SelectionColor = Colore;
            txtChat.AppendText(String.Format("[{0}] {1} - {2}\n", DateTime.Now.ToShortTimeString(), Mittente.Name, Messaggio));
            txtChat.SelectionColor = txtChat.ForeColor;
            txtChat.ScrollToCaret();

            // Fa lampeggiare la chat
            if(!this.Focused)
            {
                FlashWindowEx(this);
            }

        }

        // Metodo chiamato dalla lista utenti se la chat corrente è legata ad un utente che si è appena disconnesso
        // Impedisce l'invio di ulteriori messaggi
        public void DisabilitaChat()
        {
            btnSend.Enabled = false;
            txtSend.Enabled = false;
            ScriviMessaggio("Offline", dest, Color.Red);
        }

        // Parte aggiunta per avere l'icona lampeggiante copiata da http://stackoverflow.com/questions/11309827/window-application-flash-like-orange-on-taskbar-when-minimize
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        //Flash both the window caption and taskbar button.
        //This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags. 
        public const UInt32 FLASHW_ALL = 3;

        // Flash continuously until the window comes to the foreground. 
        public const UInt32 FLASHW_TIMERNOFG = 12;

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }

        // Do the flashing - this does not involve a raincoat.
        public static bool FlashWindowEx(Form form)
        {
            IntPtr hWnd = form.Handle;
            FLASHWINFO fInfo = new FLASHWINFO();

            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hWnd;
            fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;
            fInfo.uCount = UInt32.MaxValue;
            fInfo.dwTimeout = 0;

            return FlashWindowEx(ref fInfo);
        }
    }
}
