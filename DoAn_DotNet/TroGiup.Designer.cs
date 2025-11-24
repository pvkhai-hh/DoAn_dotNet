namespace DoAn_DotNet
{
    partial class TroGiup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TroGiup));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDong = new System.Windows.Forms.Button();
            this.btnGui = new System.Windows.Forms.Button();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.rtbChat = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnDong);
            this.panel1.Controls.Add(this.btnGui);
            this.panel1.Controls.Add(this.txtInput);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 467);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(598, 105);
            this.panel1.TabIndex = 1;
            // 
            // btnDong
            // 
            this.btnDong.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnDong.Font = new System.Drawing.Font("Times New Roman", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnDong.Location = new System.Drawing.Point(483, 18);
            this.btnDong.Name = "btnDong";
            this.btnDong.Size = new System.Drawing.Size(106, 65);
            this.btnDong.TabIndex = 1;
            this.btnDong.Text = "Đóng";
            this.btnDong.UseVisualStyleBackColor = false;
            this.btnDong.Click += new System.EventHandler(this.btnDong_Click);
            // 
            // btnGui
            // 
            this.btnGui.BackColor = System.Drawing.Color.PapayaWhip;
            this.btnGui.Font = new System.Drawing.Font("Times New Roman", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnGui.Image = ((System.Drawing.Image)(resources.GetObject("btnGui.Image")));
            this.btnGui.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGui.Location = new System.Drawing.Point(371, 18);
            this.btnGui.Name = "btnGui";
            this.btnGui.Size = new System.Drawing.Size(106, 65);
            this.btnGui.TabIndex = 1;
            this.btnGui.Text = "Gửi";
            this.btnGui.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGui.UseVisualStyleBackColor = false;
            this.btnGui.Click += new System.EventHandler(this.BtnGui_Click);
            // 
            // txtInput
            // 
            this.txtInput.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtInput.Location = new System.Drawing.Point(9, 18);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(356, 65);
            this.txtInput.TabIndex = 0;
            this.txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.rtbChat, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80.69566F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.30435F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(604, 575);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // rtbChat
            // 
            this.rtbChat.BackColor = System.Drawing.Color.Honeydew;
            this.rtbChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbChat.Location = new System.Drawing.Point(3, 3);
            this.rtbChat.Name = "rtbChat";
            this.rtbChat.ReadOnly = true;
            this.rtbChat.Size = new System.Drawing.Size(598, 458);
            this.rtbChat.TabIndex = 2;
            this.rtbChat.Text = "";
            // 
            // TroGiup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(604, 575);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TroGiup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Trợ giúp";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Button btnGui;
        private System.Windows.Forms.RichTextBox rtbChat;
        private System.Windows.Forms.Button btnDong;
    }
}