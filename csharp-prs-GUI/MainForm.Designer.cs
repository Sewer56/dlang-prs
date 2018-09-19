namespace csharp_prs_GUI
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
            this.nud_SearchBufferSize = new System.Windows.Forms.NumericUpDown();
            this.lbl_SearchBufferSize = new System.Windows.Forms.Label();
            this.btn_CompressDragDrop = new System.Windows.Forms.Button();
            this.btn_DecompressDragDrop = new System.Windows.Forms.Button();
            this.lbl_SearchBufferSizeDescription = new System.Windows.Forms.Label();
            this.lbl_SearchBufferSizeDescription2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nud_SearchBufferSize)).BeginInit();
            this.SuspendLayout();
            // 
            // nud_SearchBufferSize
            // 
            this.nud_SearchBufferSize.Location = new System.Drawing.Point(184, 176);
            this.nud_SearchBufferSize.Maximum = new decimal(new int[] {
            8191,
            0,
            0,
            0});
            this.nud_SearchBufferSize.Name = "nud_SearchBufferSize";
            this.nud_SearchBufferSize.Size = new System.Drawing.Size(121, 20);
            this.nud_SearchBufferSize.TabIndex = 0;
            this.nud_SearchBufferSize.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // lbl_SearchBufferSize
            // 
            this.lbl_SearchBufferSize.AutoSize = true;
            this.lbl_SearchBufferSize.Location = new System.Drawing.Point(6, 178);
            this.lbl_SearchBufferSize.Name = "lbl_SearchBufferSize";
            this.lbl_SearchBufferSize.Size = new System.Drawing.Size(167, 13);
            this.lbl_SearchBufferSize.TabIndex = 1;
            this.lbl_SearchBufferSize.Text = "Search Buffer Size (Compression):";
            // 
            // btn_CompressDragDrop
            // 
            this.btn_CompressDragDrop.AllowDrop = true;
            this.btn_CompressDragDrop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_CompressDragDrop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_CompressDragDrop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_CompressDragDrop.Location = new System.Drawing.Point(8, 16);
            this.btn_CompressDragDrop.Name = "btn_CompressDragDrop";
            this.btn_CompressDragDrop.Size = new System.Drawing.Size(144, 155);
            this.btn_CompressDragDrop.TabIndex = 2;
            this.btn_CompressDragDrop.Text = "Drag and Drop Files Here to Compress";
            this.btn_CompressDragDrop.UseVisualStyleBackColor = true;
            this.btn_CompressDragDrop.DragDrop += new System.Windows.Forms.DragEventHandler(this.btn_CompressDragDrop_DragDrop);
            this.btn_CompressDragDrop.DragEnter += new System.Windows.Forms.DragEventHandler(this.btn_CompressDragDrop_DragEnter);
            // 
            // btn_DecompressDragDrop
            // 
            this.btn_DecompressDragDrop.AllowDrop = true;
            this.btn_DecompressDragDrop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_DecompressDragDrop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_DecompressDragDrop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_DecompressDragDrop.Location = new System.Drawing.Point(160, 16);
            this.btn_DecompressDragDrop.Name = "btn_DecompressDragDrop";
            this.btn_DecompressDragDrop.Size = new System.Drawing.Size(144, 155);
            this.btn_DecompressDragDrop.TabIndex = 3;
            this.btn_DecompressDragDrop.Text = "Drag and Drop Files Here to Decompress";
            this.btn_DecompressDragDrop.UseVisualStyleBackColor = true;
            this.btn_DecompressDragDrop.DragDrop += new System.Windows.Forms.DragEventHandler(this.btn_DecompressDragDrop_DragDrop);
            this.btn_DecompressDragDrop.DragEnter += new System.Windows.Forms.DragEventHandler(this.btn_DecompressDragDrop_DragEnter);
            // 
            // lbl_SearchBufferSizeDescription
            // 
            this.lbl_SearchBufferSizeDescription.AutoSize = true;
            this.lbl_SearchBufferSizeDescription.Location = new System.Drawing.Point(6, 200);
            this.lbl_SearchBufferSizeDescription.Name = "lbl_SearchBufferSizeDescription";
            this.lbl_SearchBufferSizeDescription.Size = new System.Drawing.Size(297, 13);
            this.lbl_SearchBufferSizeDescription.TabIndex = 4;
            this.lbl_SearchBufferSizeDescription.Text = "Lower values = Less compressed files but faster compression.";
            // 
            // lbl_SearchBufferSizeDescription2
            // 
            this.lbl_SearchBufferSizeDescription2.AutoSize = true;
            this.lbl_SearchBufferSizeDescription2.Location = new System.Drawing.Point(6, 224);
            this.lbl_SearchBufferSizeDescription2.Name = "lbl_SearchBufferSizeDescription2";
            this.lbl_SearchBufferSizeDescription2.Size = new System.Drawing.Size(284, 13);
            this.lbl_SearchBufferSizeDescription2.TabIndex = 5;
            this.lbl_SearchBufferSizeDescription2.Text = "Range: 0-8191 | See Github for Search Buffer Benchmarks\r\n";
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(313, 249);
            this.Controls.Add(this.lbl_SearchBufferSizeDescription2);
            this.Controls.Add(this.lbl_SearchBufferSizeDescription);
            this.Controls.Add(this.btn_DecompressDragDrop);
            this.Controls.Add(this.btn_CompressDragDrop);
            this.Controls.Add(this.lbl_SearchBufferSize);
            this.Controls.Add(this.nud_SearchBufferSize);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Basic Compressor GUI";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.nud_SearchBufferSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nud_SearchBufferSize;
        private System.Windows.Forms.Label lbl_SearchBufferSize;
        private System.Windows.Forms.Button btn_CompressDragDrop;
        private System.Windows.Forms.Button btn_DecompressDragDrop;
        private System.Windows.Forms.Label lbl_SearchBufferSizeDescription;
        private System.Windows.Forms.Label lbl_SearchBufferSizeDescription2;
    }
}

