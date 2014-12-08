namespace Crypthat_Client
{
    partial class ChatForm
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
            this.txtChat = new System.Windows.Forms.RichTextBox();
            this.txtSend = new System.Windows.Forms.RichTextBox();
            this.barraStato = new System.Windows.Forms.ToolStrip();
            this.lblName = new System.Windows.Forms.ToolStripLabel();
            this.btnSend = new System.Windows.Forms.Button();
            this.chkEncrypt = new System.Windows.Forms.CheckBox();
            this.barraStato.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtChat
            // 
            this.txtChat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChat.Location = new System.Drawing.Point(12, 28);
            this.txtChat.Name = "txtChat";
            this.txtChat.Size = new System.Drawing.Size(449, 210);
            this.txtChat.TabIndex = 0;
            this.txtChat.Text = "";
            // 
            // txtSend
            // 
            this.txtSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSend.Location = new System.Drawing.Point(12, 244);
            this.txtSend.Name = "txtSend";
            this.txtSend.Size = new System.Drawing.Size(285, 77);
            this.txtSend.TabIndex = 1;
            this.txtSend.Text = "";
            // 
            // barraStato
            // 
            this.barraStato.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblName});
            this.barraStato.Location = new System.Drawing.Point(0, 0);
            this.barraStato.Name = "barraStato";
            this.barraStato.Size = new System.Drawing.Size(473, 25);
            this.barraStato.TabIndex = 2;
            // 
            // lblName
            // 
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(40, 22);
            this.lblName.Text = "Nome";
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Location = new System.Drawing.Point(303, 244);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(158, 77);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "Invia";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // chkEncrypt
            // 
            this.chkEncrypt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkEncrypt.AutoSize = true;
            this.chkEncrypt.Location = new System.Drawing.Point(12, 324);
            this.chkEncrypt.Name = "chkEncrypt";
            this.chkEncrypt.Size = new System.Drawing.Size(53, 17);
            this.chkEncrypt.TabIndex = 4;
            this.chkEncrypt.Text = "Cripta";
            this.chkEncrypt.UseVisualStyleBackColor = true;
            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 342);
            this.Controls.Add(this.chkEncrypt);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.barraStato);
            this.Controls.Add(this.txtSend);
            this.Controls.Add(this.txtChat);
            this.Name = "ChatForm";
            this.Text = "ChatForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChatForm_FormClosing);
            this.Load += new System.EventHandler(this.ChatForm_Load);
            this.barraStato.ResumeLayout(false);
            this.barraStato.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtChat;
        private System.Windows.Forms.RichTextBox txtSend;
        private System.Windows.Forms.ToolStrip barraStato;
        private System.Windows.Forms.ToolStripLabel lblName;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.CheckBox chkEncrypt;
    }
}