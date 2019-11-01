namespace Derp_InSim
{
    partial class Form1
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
            this.chat = new System.Windows.Forms.TextBox();
            this.chatbox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // chat
            // 
            this.chat.AllowDrop = true;
            this.chat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chat.Location = new System.Drawing.Point(12, 12);
            this.chat.Multiline = true;
            this.chat.Name = "chat";
            this.chat.ReadOnly = true;
            this.chat.Size = new System.Drawing.Size(560, 446);
            this.chat.TabIndex = 0;
            this.chat.TabStop = false;
            this.chat.TextChanged += new System.EventHandler(this.chat_TextChanged);
            // 
            // chatbox
            // 
            this.chatbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chatbox.Location = new System.Drawing.Point(12, 464);
            this.chatbox.Name = "chatbox";
            this.chatbox.Size = new System.Drawing.Size(560, 20);
            this.chatbox.TabIndex = 1;
            this.chatbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.chatbox_KeyPress);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 496);
            this.Controls.Add(this.chatbox);
            this.Controls.Add(this.chat);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EC InSim";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox chat;
        private System.Windows.Forms.TextBox chatbox;
    }
}

