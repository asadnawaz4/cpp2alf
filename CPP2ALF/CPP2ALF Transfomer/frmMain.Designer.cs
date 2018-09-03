namespace CPP2ALF_Transfomer
{
    partial class frmMain
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
            this.btnViewSrcML = new System.Windows.Forms.Button();
            this.btnGenerateALF = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.rtbCPP = new System.Windows.Forms.RichTextBox();
            this.rtbALF = new System.Windows.Forms.RichTextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtOutputDirectory = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.clbCppFilesList = new System.Windows.Forms.CheckedListBox();
            this.lbl_SrcMLFiles = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.trackBarCpp = new System.Windows.Forms.TrackBar();
            this.trackBarALF = new System.Windows.Forms.TrackBar();
            this.lbl_topLeft = new System.Windows.Forms.Label();
            this.gp_bottom = new System.Windows.Forms.GroupBox();
            this.cbCustomCode = new System.Windows.Forms.CheckBox();
            this.btn_GenerateAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarCpp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarALF)).BeginInit();
            this.gp_bottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnViewSrcML
            // 
            this.btnViewSrcML.Location = new System.Drawing.Point(12, 571);
            this.btnViewSrcML.Name = "btnViewSrcML";
            this.btnViewSrcML.Size = new System.Drawing.Size(130, 33);
            this.btnViewSrcML.TabIndex = 0;
            this.btnViewSrcML.Text = "View SrcML";
            this.btnViewSrcML.UseVisualStyleBackColor = true;
            this.btnViewSrcML.Click += new System.EventHandler(this.btnViewSrcML_Click);
            // 
            // btnGenerateALF
            // 
            this.btnGenerateALF.Location = new System.Drawing.Point(521, 19);
            this.btnGenerateALF.Name = "btnGenerateALF";
            this.btnGenerateALF.Size = new System.Drawing.Size(130, 33);
            this.btnGenerateALF.TabIndex = 0;
            this.btnGenerateALF.Text = "Generate ALF";
            this.btnGenerateALF.UseVisualStyleBackColor = true;
            this.btnGenerateALF.Click += new System.EventHandler(this.btnGenerateALF_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(12, 30);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(130, 33);
            this.btnBrowse.TabIndex = 0;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // rtbCPP
            // 
            this.rtbCPP.Location = new System.Drawing.Point(164, 88);
            this.rtbCPP.Name = "rtbCPP";
            this.rtbCPP.Size = new System.Drawing.Size(612, 500);
            this.rtbCPP.TabIndex = 1;
            this.rtbCPP.Text = "";
            this.rtbCPP.WordWrap = false;
            // 
            // rtbALF
            // 
            this.rtbALF.Location = new System.Drawing.Point(782, 88);
            this.rtbALF.Name = "rtbALF";
            this.rtbALF.Size = new System.Drawing.Size(585, 500);
            this.rtbALF.TabIndex = 1;
            this.rtbALF.Text = "";
            this.rtbALF.WordWrap = false;
            this.rtbALF.TextChanged += new System.EventHandler(this.rtbALF_TextChanged);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(1008, 34);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(139, 45);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtOutputDirectory
            // 
            this.txtOutputDirectory.Location = new System.Drawing.Point(432, 47);
            this.txtOutputDirectory.Name = "txtOutputDirectory";
            this.txtOutputDirectory.Size = new System.Drawing.Size(408, 20);
            this.txtOutputDirectory.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(872, 44);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Browse";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(288, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Select Output Directory";
            // 
            // clbCppFilesList
            // 
            this.clbCppFilesList.FormattingEnabled = true;
            this.clbCppFilesList.Location = new System.Drawing.Point(12, 88);
            this.clbCppFilesList.Name = "clbCppFilesList";
            this.clbCppFilesList.Size = new System.Drawing.Size(130, 454);
            this.clbCppFilesList.TabIndex = 6;
            this.clbCppFilesList.SelectedIndexChanged += new System.EventHandler(this.clbCppFilesList_SelectedIndexChanged);
            // 
            // lbl_SrcMLFiles
            // 
            this.lbl_SrcMLFiles.AutoSize = true;
            this.lbl_SrcMLFiles.Location = new System.Drawing.Point(12, 545);
            this.lbl_SrcMLFiles.Name = "lbl_SrcMLFiles";
            this.lbl_SrcMLFiles.Size = new System.Drawing.Size(91, 13);
            this.lbl_SrcMLFiles.TabIndex = 7;
            this.lbl_SrcMLFiles.Text = "Source Files (0/0)";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(199, 28);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 8;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // trackBarCpp
            // 
            this.trackBarCpp.Location = new System.Drawing.Point(210, 14);
            this.trackBarCpp.Maximum = 50;
            this.trackBarCpp.Minimum = 10;
            this.trackBarCpp.Name = "trackBarCpp";
            this.trackBarCpp.Size = new System.Drawing.Size(104, 45);
            this.trackBarCpp.TabIndex = 9;
            this.trackBarCpp.Value = 10;
            this.trackBarCpp.Scroll += new System.EventHandler(this.trackBarCpp_Scroll);
            // 
            // trackBarALF
            // 
            this.trackBarALF.Location = new System.Drawing.Point(855, 14);
            this.trackBarALF.Maximum = 50;
            this.trackBarALF.Minimum = 10;
            this.trackBarALF.Name = "trackBarALF";
            this.trackBarALF.Size = new System.Drawing.Size(104, 45);
            this.trackBarALF.TabIndex = 9;
            this.trackBarALF.Value = 10;
            this.trackBarALF.Scroll += new System.EventHandler(this.trackBarALF_Scroll);
            // 
            // lbl_topLeft
            // 
            this.lbl_topLeft.AutoSize = true;
            this.lbl_topLeft.Location = new System.Drawing.Point(161, 88);
            this.lbl_topLeft.Name = "lbl_topLeft";
            this.lbl_topLeft.Size = new System.Drawing.Size(0, 13);
            this.lbl_topLeft.TabIndex = 10;
            // 
            // gp_bottom
            // 
            this.gp_bottom.Controls.Add(this.trackBarCpp);
            this.gp_bottom.Controls.Add(this.btnGenerateALF);
            this.gp_bottom.Controls.Add(this.trackBarALF);
            this.gp_bottom.Location = new System.Drawing.Point(164, 751);
            this.gp_bottom.Name = "gp_bottom";
            this.gp_bottom.Size = new System.Drawing.Size(1098, 70);
            this.gp_bottom.TabIndex = 11;
            this.gp_bottom.TabStop = false;
            // 
            // cbCustomCode
            // 
            this.cbCustomCode.AutoSize = true;
            this.cbCustomCode.Location = new System.Drawing.Point(291, 12);
            this.cbCustomCode.Name = "cbCustomCode";
            this.cbCustomCode.Size = new System.Drawing.Size(89, 17);
            this.cbCustomCode.TabIndex = 12;
            this.cbCustomCode.Text = "Custom Code";
            this.cbCustomCode.UseVisualStyleBackColor = true;
            // 
            // btn_GenerateAll
            // 
            this.btn_GenerateAll.Location = new System.Drawing.Point(1178, 34);
            this.btn_GenerateAll.Name = "btn_GenerateAll";
            this.btn_GenerateAll.Size = new System.Drawing.Size(139, 45);
            this.btn_GenerateAll.TabIndex = 13;
            this.btn_GenerateAll.Text = "Generate All";
            this.btn_GenerateAll.UseVisualStyleBackColor = true;
            this.btn_GenerateAll.Click += new System.EventHandler(this.btn_GenerateAll_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 749);
            this.Controls.Add(this.btn_GenerateAll);
            this.Controls.Add(this.cbCustomCode);
            this.Controls.Add(this.gp_bottom);
            this.Controls.Add(this.lbl_topLeft);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.lbl_SrcMLFiles);
            this.Controls.Add(this.clbCppFilesList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtOutputDirectory);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.rtbALF);
            this.Controls.Add(this.rtbCPP);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnViewSrcML);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CPP2ALF Transfomer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.SizeChanged += new System.EventHandler(this.frmMain_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarCpp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarALF)).EndInit();
            this.gp_bottom.ResumeLayout(false);
            this.gp_bottom.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnViewSrcML;
        private System.Windows.Forms.Button btnGenerateALF;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.RichTextBox rtbCPP;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtOutputDirectory;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckedListBox clbCppFilesList;
        private System.Windows.Forms.RichTextBox rtbALF;
        private System.Windows.Forms.Label lbl_SrcMLFiles;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TrackBar trackBarCpp;
        private System.Windows.Forms.TrackBar trackBarALF;
        private System.Windows.Forms.Label lbl_topLeft;
        private System.Windows.Forms.GroupBox gp_bottom;
        private System.Windows.Forms.CheckBox cbCustomCode;
        private System.Windows.Forms.Button btn_GenerateAll;
    }
}

