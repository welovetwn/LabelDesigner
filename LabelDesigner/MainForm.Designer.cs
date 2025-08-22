namespace LabelDesigner
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            Model.LabelDocument labelDocument1 = new Model.LabelDocument();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            toolStrip1 = new ToolStrip();
            btnAddText = new ToolStripButton();
            btnAddImage = new ToolStripButton();
            btnAddBarcode = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            btnOpen = new ToolStripButton();
            btnSave = new ToolStripButton();
            btnPrint = new ToolStripButton();
            splitContainer1 = new SplitContainer();
            canvas = new LabelDesigner.UI.DesignerCanvas();
            propertyGrid1 = new PropertyGrid();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new Size(20, 20);
            toolStrip1.Items.AddRange(new ToolStripItem[] { btnAddText, btnAddImage, btnAddBarcode, toolStripSeparator1, btnOpen, btnSave, btnPrint });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1050, 25);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // btnAddText
            // 
            btnAddText.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnAddText.Name = "btnAddText";
            btnAddText.Size = new Size(59, 22);
            btnAddText.Text = "新增文字";
            btnAddText.Click += btnAddText_Click;
            // 
            // btnAddImage
            // 
            btnAddImage.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnAddImage.Name = "btnAddImage";
            btnAddImage.Size = new Size(59, 22);
            btnAddImage.Text = "新增圖片";
            btnAddImage.Click += btnAddImage_Click;
            // 
            // btnAddBarcode
            // 
            btnAddBarcode.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnAddBarcode.Name = "btnAddBarcode";
            btnAddBarcode.Size = new Size(59, 22);
            btnAddBarcode.Text = "新增條碼";
            btnAddBarcode.Click += btnAddBarcode_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // btnOpen
            // 
            btnOpen.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(35, 22);
            btnOpen.Text = "開啟";
            btnOpen.Click += btnOpen_Click;
            // 
            // btnSave
            // 
            btnSave.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(35, 22);
            btnSave.Text = "儲存";
            btnSave.Click += btnSave_Click;
            // 
            // btnPrint
            // 
            btnPrint.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(35, 22);
            btnPrint.Text = "列印";
            btnPrint.Click += btnPrint_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 25);
            splitContainer1.Margin = new Padding(3, 2, 3, 2);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(canvas);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(propertyGrid1);
            splitContainer1.Size = new Size(1050, 575);
            splitContainer1.SplitterDistance = 787;
            splitContainer1.TabIndex = 1;
            // 
            // canvas
            // 
            canvas.BackColor = Color.WhiteSmoke;
            canvas.Dock = DockStyle.Fill;
            labelDocument1.Dpi = 300F;
            labelDocument1.PageHeightMm = 50F;
            labelDocument1.PageWidthMm = 100F;
            canvas.Document = labelDocument1;
            canvas.Location = new Point(0, 0);
            canvas.Margin = new Padding(3, 2, 3, 2);
            canvas.Name = "canvas";
            canvas.Size = new Size(787, 575);
            canvas.TabIndex = 0;
            canvas.SelectionChanged += canvas_SelectionChanged;
            // 
            // propertyGrid1
            // 
            propertyGrid1.Dock = DockStyle.Fill;
            propertyGrid1.Location = new Point(0, 0);
            propertyGrid1.Margin = new Padding(3, 2, 3, 2);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.Size = new Size(259, 575);
            propertyGrid1.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1050, 600);
            Controls.Add(splitContainer1);
            Controls.Add(toolStrip1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "MainForm";
            Text = "Label Designer - WinForms (.NET 8)";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnAddText;
        private System.Windows.Forms.ToolStripButton btnAddImage;
        private System.Windows.Forms.ToolStripButton btnAddBarcode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnPrint;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private UI.DesignerCanvas canvas;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
    }
}
