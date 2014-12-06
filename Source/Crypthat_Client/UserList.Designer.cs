namespace Crypthat_Client
{
    partial class UserList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lsUtenti = new System.Windows.Forms.ListBox();
            this.lblUtenti = new System.Windows.Forms.Label();
            this.barraStato = new System.Windows.Forms.StatusStrip();
            this.lblNomeUtente = new System.Windows.Forms.ToolStripStatusLabel();
            this.barraStato.SuspendLayout();
            this.SuspendLayout();
            // 
            // lsUtenti
            // 
            this.lsUtenti.FormattingEnabled = true;
            this.lsUtenti.Location = new System.Drawing.Point(12, 23);
            this.lsUtenti.Name = "lsUtenti";
            this.lsUtenti.Size = new System.Drawing.Size(249, 355);
            this.lsUtenti.TabIndex = 0;
            this.lsUtenti.DoubleClick += new System.EventHandler(this.lsUtenti_DoubleClick);
            // 
            // lblUtenti
            // 
            this.lblUtenti.AutoSize = true;
            this.lblUtenti.Location = new System.Drawing.Point(13, 4);
            this.lblUtenti.Name = "lblUtenti";
            this.lblUtenti.Size = new System.Drawing.Size(34, 13);
            this.lblUtenti.TabIndex = 1;
            this.lblUtenti.Text = "Users";
            // 
            // barraStato
            // 
            this.barraStato.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblNomeUtente});
            this.barraStato.Location = new System.Drawing.Point(0, 391);
            this.barraStato.Name = "barraStato";
            this.barraStato.Size = new System.Drawing.Size(273, 22);
            this.barraStato.TabIndex = 2;
            this.barraStato.Text = "statusStrip1";
            // 
            // lblNomeUtente
            // 
            this.lblNomeUtente.Name = "lblNomeUtente";
            this.lblNomeUtente.Size = new System.Drawing.Size(40, 17);
            this.lblNomeUtente.Text = "Nome";
            // 
            // UserList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 413);
            this.Controls.Add(this.barraStato);
            this.Controls.Add(this.lblUtenti);
            this.Controls.Add(this.lsUtenti);
            this.Name = "UserList";
            this.Text = "UserList";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UserList_FormClosing);
            this.barraStato.ResumeLayout(false);
            this.barraStato.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lsUtenti;
        private System.Windows.Forms.Label lblUtenti;
        private System.Windows.Forms.StatusStrip barraStato;
        private System.Windows.Forms.ToolStripStatusLabel lblNomeUtente;
    }
}