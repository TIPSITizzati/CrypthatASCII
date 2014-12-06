namespace Crypthat_Client
{
    partial class MainForm
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
            this.rbSockets = new System.Windows.Forms.RadioButton();
            this.rbRs232 = new System.Windows.Forms.RadioButton();
            this.gbOpMode = new System.Windows.Forms.GroupBox();
            this.gbInfoUtente = new System.Windows.Forms.GroupBox();
            this.txtNome = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gbRs232 = new System.Windows.Forms.GroupBox();
            this.gbSockets = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtIpAddress = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.nPorta = new System.Windows.Forms.NumericUpDown();
            this.cbNomePorta = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.gbOpMode.SuspendLayout();
            this.gbInfoUtente.SuspendLayout();
            this.gbRs232.SuspendLayout();
            this.gbSockets.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nPorta)).BeginInit();
            this.SuspendLayout();
            // 
            // rbSockets
            // 
            this.rbSockets.AutoSize = true;
            this.rbSockets.Location = new System.Drawing.Point(10, 19);
            this.rbSockets.Name = "rbSockets";
            this.rbSockets.Size = new System.Drawing.Size(64, 17);
            this.rbSockets.TabIndex = 0;
            this.rbSockets.TabStop = true;
            this.rbSockets.Text = "Sockets";
            this.rbSockets.UseVisualStyleBackColor = true;
            this.rbSockets.CheckedChanged += new System.EventHandler(this.rbSockets_CheckedChanged);
            // 
            // rbRs232
            // 
            this.rbRs232.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.rbRs232.AutoSize = true;
            this.rbRs232.Location = new System.Drawing.Point(273, 19);
            this.rbRs232.Name = "rbRs232";
            this.rbRs232.Size = new System.Drawing.Size(56, 17);
            this.rbRs232.TabIndex = 1;
            this.rbRs232.TabStop = true;
            this.rbRs232.Text = "Rs232";
            this.rbRs232.UseVisualStyleBackColor = true;
            this.rbRs232.CheckedChanged += new System.EventHandler(this.rbRs232_CheckedChanged);
            // 
            // gbOpMode
            // 
            this.gbOpMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.gbOpMode.Controls.Add(this.rbSockets);
            this.gbOpMode.Controls.Add(this.rbRs232);
            this.gbOpMode.Location = new System.Drawing.Point(12, 73);
            this.gbOpMode.Name = "gbOpMode";
            this.gbOpMode.Size = new System.Drawing.Size(347, 46);
            this.gbOpMode.TabIndex = 2;
            this.gbOpMode.TabStop = false;
            this.gbOpMode.Text = "Modalità Operativa";
            // 
            // gbInfoUtente
            // 
            this.gbInfoUtente.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.gbInfoUtente.Controls.Add(this.label1);
            this.gbInfoUtente.Controls.Add(this.txtNome);
            this.gbInfoUtente.Location = new System.Drawing.Point(12, 11);
            this.gbInfoUtente.Name = "gbInfoUtente";
            this.gbInfoUtente.Size = new System.Drawing.Size(347, 56);
            this.gbInfoUtente.TabIndex = 3;
            this.gbInfoUtente.TabStop = false;
            this.gbInfoUtente.Text = "Informazioni Utente";
            // 
            // txtNome
            // 
            this.txtNome.Location = new System.Drawing.Point(57, 19);
            this.txtNome.MaxLength = 18;
            this.txtNome.Name = "txtNome";
            this.txtNome.Size = new System.Drawing.Size(272, 20);
            this.txtNome.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Nome";
            // 
            // gbRs232
            // 
            this.gbRs232.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.gbRs232.Controls.Add(this.label4);
            this.gbRs232.Controls.Add(this.cbNomePorta);
            this.gbRs232.Location = new System.Drawing.Point(12, 180);
            this.gbRs232.Name = "gbRs232";
            this.gbRs232.Size = new System.Drawing.Size(347, 53);
            this.gbRs232.TabIndex = 4;
            this.gbRs232.TabStop = false;
            this.gbRs232.Text = "Impostazioni Rs232";
            // 
            // gbSockets
            // 
            this.gbSockets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSockets.Controls.Add(this.nPorta);
            this.gbSockets.Controls.Add(this.label3);
            this.gbSockets.Controls.Add(this.txtIpAddress);
            this.gbSockets.Controls.Add(this.label2);
            this.gbSockets.Location = new System.Drawing.Point(12, 125);
            this.gbSockets.Name = "gbSockets";
            this.gbSockets.Size = new System.Drawing.Size(347, 49);
            this.gbSockets.TabIndex = 5;
            this.gbSockets.TabStop = false;
            this.gbSockets.Text = "Impostazioni Sockets";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Indirizzo:";
            // 
            // txtIpAddress
            // 
            this.txtIpAddress.Location = new System.Drawing.Point(57, 17);
            this.txtIpAddress.Name = "txtIpAddress";
            this.txtIpAddress.Size = new System.Drawing.Size(148, 20);
            this.txtIpAddress.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(211, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Porta:";
            // 
            // nPorta
            // 
            this.nPorta.Location = new System.Drawing.Point(252, 17);
            this.nPorta.Name = "nPorta";
            this.nPorta.Size = new System.Drawing.Size(77, 20);
            this.nPorta.TabIndex = 3;
            // 
            // cbNomePorta
            // 
            this.cbNomePorta.FormattingEnabled = true;
            this.cbNomePorta.Location = new System.Drawing.Point(97, 17);
            this.cbNomePorta.Name = "cbNomePorta";
            this.cbNomePorta.Size = new System.Drawing.Size(232, 21);
            this.cbNomePorta.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Porta del server:";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(100, 239);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(167, 63);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 314);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.gbRs232);
            this.Controls.Add(this.gbInfoUtente);
            this.Controls.Add(this.gbSockets);
            this.Controls.Add(this.gbOpMode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = "Crypthat - Connessione";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.gbOpMode.ResumeLayout(false);
            this.gbOpMode.PerformLayout();
            this.gbInfoUtente.ResumeLayout(false);
            this.gbInfoUtente.PerformLayout();
            this.gbRs232.ResumeLayout(false);
            this.gbRs232.PerformLayout();
            this.gbSockets.ResumeLayout(false);
            this.gbSockets.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nPorta)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rbSockets;
        private System.Windows.Forms.RadioButton rbRs232;
        private System.Windows.Forms.GroupBox gbOpMode;
        private System.Windows.Forms.GroupBox gbInfoUtente;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNome;
        private System.Windows.Forms.GroupBox gbRs232;
        private System.Windows.Forms.GroupBox gbSockets;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbNomePorta;
        private System.Windows.Forms.NumericUpDown nPorta;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtIpAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnConnect;

    }
}