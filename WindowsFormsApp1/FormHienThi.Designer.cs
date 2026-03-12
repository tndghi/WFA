namespace WindowsFormsApp1
{
    partial class panelMain
    {
        private void InitializeComponent()
        {
            this.cbLop = new System.Windows.Forms.ComboBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.dgvSinhVien = new System.Windows.Forms.DataGridView();
            this.dgvDIem = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSinhVien)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDIem)).BeginInit();
            this.SuspendLayout();
            // 
            // cbLop
            // 
            this.cbLop.FormattingEnabled = true;
            this.cbLop.Location = new System.Drawing.Point(56, 25);
            this.cbLop.Name = "cbLop";
            this.cbLop.Size = new System.Drawing.Size(131, 24);
            this.cbLop.TabIndex = 0;
            this.cbLop.SelectedIndexChanged += new System.EventHandler(this.cbLop_SelectedIndexChanged);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(230, 16);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 40);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // dgvSinhVien
            // 
            this.dgvSinhVien.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSinhVien.Location = new System.Drawing.Point(56, 85);
            this.dgvSinhVien.Name = "dgvSinhVien";
            this.dgvSinhVien.RowHeadersWidth = 51;
            this.dgvSinhVien.RowTemplate.Height = 24;
            this.dgvSinhVien.Size = new System.Drawing.Size(605, 184);
            this.dgvSinhVien.TabIndex = 2;
            this.dgvSinhVien.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSinhVien_CellClick);
            // 
            // dgvDIem
            // 
            this.dgvDIem.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDIem.Location = new System.Drawing.Point(56, 326);
            this.dgvDIem.Name = "dgvDIem";
            this.dgvDIem.RowHeadersWidth = 51;
            this.dgvDIem.RowTemplate.Height = 24;
            this.dgvDIem.Size = new System.Drawing.Size(605, 119);
            this.dgvDIem.TabIndex = 3;
            // 
            // panelMain
            // 
            this.ClientSize = new System.Drawing.Size(861, 522);
            this.Controls.Add(this.dgvDIem);
            this.Controls.Add(this.dgvSinhVien);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.cbLop);
            this.Name = "panelMain";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSinhVien)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDIem)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.ComboBox cbLop;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.DataGridView dgvSinhVien;
        private System.Windows.Forms.DataGridView dgvDIem;
    }
}