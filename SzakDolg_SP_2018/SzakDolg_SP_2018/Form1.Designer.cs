namespace SzakDolg_SP_2018
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
            this.label_MySqlConn = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.nUd_HanyszorMyS = new System.Windows.Forms.NumericUpDown();
            this.button_UpdateMys = new System.Windows.Forms.Button();
            this.button_SelectMys = new System.Windows.Forms.Button();
            this.button_QueryMys = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.rTB_QueryMys = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button_DeleteMys = new System.Windows.Forms.Button();
            this.button_LoadDataMys = new System.Windows.Forms.Button();
            this.button_InsertMys = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.button_Generate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.nUD_Generate = new System.Windows.Forms.NumericUpDown();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUd_HanyszorMyS)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_Generate)).BeginInit();
            this.SuspendLayout();
            // 
            // label_MySqlConn
            // 
            this.label_MySqlConn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label_MySqlConn.AutoSize = true;
            this.label_MySqlConn.Location = new System.Drawing.Point(4, 350);
            this.label_MySqlConn.Name = "label_MySqlConn";
            this.label_MySqlConn.Size = new System.Drawing.Size(176, 13);
            this.label_MySqlConn.TabIndex = 1;
            this.label_MySqlConn.Text = "Futtatáshoz nyomj meg egy gombot.";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.79584F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.20416F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 116F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 49);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1125, 442);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.nUd_HanyszorMyS);
            this.panel1.Controls.Add(this.button_UpdateMys);
            this.panel1.Controls.Add(this.button_SelectMys);
            this.panel1.Controls.Add(this.button_QueryMys);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.rTB_QueryMys);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.button_DeleteMys);
            this.panel1.Controls.Add(this.button_LoadDataMys);
            this.panel1.Controls.Add(this.button_InsertMys);
            this.panel1.Controls.Add(this.label_MySqlConn);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(330, 376);
            this.panel1.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 310);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Hányszor fusson le:";
            // 
            // nUd_HanyszorMyS
            // 
            this.nUd_HanyszorMyS.Location = new System.Drawing.Point(115, 307);
            this.nUd_HanyszorMyS.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nUd_HanyszorMyS.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUd_HanyszorMyS.Name = "nUd_HanyszorMyS";
            this.nUd_HanyszorMyS.Size = new System.Drawing.Size(78, 20);
            this.nUd_HanyszorMyS.TabIndex = 9;
            this.nUd_HanyszorMyS.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // button_UpdateMys
            // 
            this.button_UpdateMys.Location = new System.Drawing.Point(7, 150);
            this.button_UpdateMys.Name = "button_UpdateMys";
            this.button_UpdateMys.Size = new System.Drawing.Size(180, 23);
            this.button_UpdateMys.TabIndex = 11;
            this.button_UpdateMys.Text = "Azonos email [UPDATE]";
            this.button_UpdateMys.UseVisualStyleBackColor = true;
            this.button_UpdateMys.Click += new System.EventHandler(this.button_UpdateMys_Click);
            // 
            // button_SelectMys
            // 
            this.button_SelectMys.Location = new System.Drawing.Point(6, 120);
            this.button_SelectMys.Name = "button_SelectMys";
            this.button_SelectMys.Size = new System.Drawing.Size(181, 23);
            this.button_SelectMys.TabIndex = 10;
            this.button_SelectMys.Text = "Minden adat lekérés [SELECT *]";
            this.button_SelectMys.UseVisualStyleBackColor = true;
            this.button_SelectMys.Click += new System.EventHandler(this.button_SelectMys_Click);
            // 
            // button_QueryMys
            // 
            this.button_QueryMys.Location = new System.Drawing.Point(199, 304);
            this.button_QueryMys.Name = "button_QueryMys";
            this.button_QueryMys.Size = new System.Drawing.Size(128, 23);
            this.button_QueryMys.TabIndex = 9;
            this.button_QueryMys.Text = "Futtatás";
            this.button_QueryMys.UseVisualStyleBackColor = true;
            this.button_QueryMys.Click += new System.EventHandler(this.button_QueryMys_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 186);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Egyedi query:";
            // 
            // rTB_QueryMys
            // 
            this.rTB_QueryMys.Location = new System.Drawing.Point(7, 202);
            this.rTB_QueryMys.Name = "rTB_QueryMys";
            this.rTB_QueryMys.Size = new System.Drawing.Size(321, 96);
            this.rTB_QueryMys.TabIndex = 7;
            this.rTB_QueryMys.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(131, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "MySQL szerver műveletek";
            // 
            // button_DeleteMys
            // 
            this.button_DeleteMys.Location = new System.Drawing.Point(6, 91);
            this.button_DeleteMys.Name = "button_DeleteMys";
            this.button_DeleteMys.Size = new System.Drawing.Size(181, 23);
            this.button_DeleteMys.TabIndex = 5;
            this.button_DeleteMys.Text = "Minden törlése [DELETE]";
            this.button_DeleteMys.UseVisualStyleBackColor = true;
            this.button_DeleteMys.Click += new System.EventHandler(this.button_DeleteMys_Click);
            // 
            // button_LoadDataMys
            // 
            this.button_LoadDataMys.Location = new System.Drawing.Point(6, 62);
            this.button_LoadDataMys.Name = "button_LoadDataMys";
            this.button_LoadDataMys.Size = new System.Drawing.Size(181, 23);
            this.button_LoadDataMys.TabIndex = 4;
            this.button_LoadDataMys.Text = "Adatok Feltöltése [LOAD DATA]";
            this.button_LoadDataMys.UseVisualStyleBackColor = true;
            this.button_LoadDataMys.Click += new System.EventHandler(this.button3_Click);
            // 
            // button_InsertMys
            // 
            this.button_InsertMys.Location = new System.Drawing.Point(4, 33);
            this.button_InsertMys.Name = "button_InsertMys";
            this.button_InsertMys.Size = new System.Drawing.Size(183, 23);
            this.button_InsertMys.TabIndex = 2;
            this.button_InsertMys.Text = "Adatok feltöltése [INSERT]";
            this.button_InsertMys.UseVisualStyleBackColor = true;
            this.button_InsertMys.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.richTextBox1);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Location = new System.Drawing.Point(344, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(333, 376);
            this.panel2.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(138, 274);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button_Generate
            // 
            this.button_Generate.Location = new System.Drawing.Point(208, 10);
            this.button_Generate.Name = "button_Generate";
            this.button_Generate.Size = new System.Drawing.Size(75, 23);
            this.button_Generate.TabIndex = 3;
            this.button_Generate.Text = "Generálás";
            this.button_Generate.UseVisualStyleBackColor = true;
            this.button_Generate.Click += new System.EventHandler(this.button_Generate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(96, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "db tanuló generálása";
            // 
            // nUD_Generate
            // 
            this.nUD_Generate.Location = new System.Drawing.Point(12, 13);
            this.nUD_Generate.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nUD_Generate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUD_Generate.Name = "nUD_Generate";
            this.nUD_Generate.Size = new System.Drawing.Size(78, 20);
            this.nUD_Generate.TabIndex = 8;
            this.nUD_Generate.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(3, 13);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(327, 255);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1149, 503);
            this.Controls.Add(this.nUD_Generate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.button_Generate);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUd_HanyszorMyS)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nUD_Generate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_MySqlConn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button_DeleteMys;
        private System.Windows.Forms.Button button_LoadDataMys;
        private System.Windows.Forms.Button button_Generate;
        private System.Windows.Forms.Button button_InsertMys;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nUD_Generate;
        private System.Windows.Forms.Button button_UpdateMys;
        private System.Windows.Forms.Button button_SelectMys;
        private System.Windows.Forms.Button button_QueryMys;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox rTB_QueryMys;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nUd_HanyszorMyS;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}

