// 檔案路徑：MainForm.Designer.cs

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
            btnOpen = new ToolStripButton();
            btnSave = new ToolStripButton();
            btnPrint = new ToolStripButton();
            btnPrintFromApi = new ToolStripButton();
            btnApiTest = new ToolStripButton();
            mainSplitContainer = new SplitContainer();
            leftSplitContainer = new SplitContainer();
            toolboxPanel = new Panel();
            toolbox = new LabelDesigner.UI.ToolboxControl();
            toolboxLabel = new Label();
            canvas = new LabelDesigner.UI.DesignerCanvas();
            propertyGrid1 = new PropertyGrid();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)mainSplitContainer).BeginInit();
            mainSplitContainer.Panel1.SuspendLayout();
            mainSplitContainer.Panel2.SuspendLayout();
            mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)leftSplitContainer).BeginInit();
            leftSplitContainer.Panel1.SuspendLayout();
            leftSplitContainer.Panel2.SuspendLayout();
            leftSplitContainer.SuspendLayout();
            toolboxPanel.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new Size(20, 20);
            toolStrip1.Items.AddRange(new ToolStripItem[] { btnOpen, btnSave, btnPrint, btnPrintFromApi, btnApiTest });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1200, 25);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
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
            // btnPrintFromApi
            // 
            btnPrintFromApi.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnPrintFromApi.Name = "btnPrintFromApi";
            btnPrintFromApi.Size = new Size(77, 22);
            btnPrintFromApi.Text = "列印（API）";
            btnPrintFromApi.Click += btnPrintFromApi_Click;
            // 
            // btnApiTest
            // 
            btnApiTest.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnApiTest.Name = "btnApiTest";
            btnApiTest.Size = new Size(35, 22);
            btnApiTest.Text = "資料";
            btnApiTest.Click += btnApiTest_Click;
            // 
            // mainSplitContainer
            // 
            mainSplitContainer.Dock = DockStyle.Fill;
            mainSplitContainer.Location = new Point(0, 25);
            mainSplitContainer.Name = "mainSplitContainer";
            // 
            // mainSplitContainer.Panel1
            // 
            mainSplitContainer.Panel1.Controls.Add(leftSplitContainer);
            // 
            // mainSplitContainer.Panel2
            // 
            mainSplitContainer.Panel2.Controls.Add(propertyGrid1);
            mainSplitContainer.Size = new Size(1200, 575);
            mainSplitContainer.SplitterDistance = 950;
            mainSplitContainer.TabIndex = 1;
            // 
            // leftSplitContainer
            // 
            leftSplitContainer.Dock = DockStyle.Fill;
            leftSplitContainer.Location = new Point(0, 0);
            leftSplitContainer.Name = "leftSplitContainer";
            // 
            // leftSplitContainer.Panel1
            // 
            leftSplitContainer.Panel1.Controls.Add(toolboxPanel);
            // 
            // leftSplitContainer.Panel2
            // 
            leftSplitContainer.Panel2.Controls.Add(canvas);
            leftSplitContainer.Size = new Size(950, 575);
            leftSplitContainer.SplitterDistance = 90;
            leftSplitContainer.TabIndex = 0;
            // 
            // toolboxPanel
            // 
            toolboxPanel.BackColor = Color.FromArgb(245, 245, 245);
            toolboxPanel.BorderStyle = BorderStyle.FixedSingle;
            toolboxPanel.Controls.Add(toolbox);
            toolboxPanel.Controls.Add(toolboxLabel);
            toolboxPanel.Location = new Point(0, 0);
            toolboxPanel.Name = "toolboxPanel";
            toolboxPanel.Size = new Size(85, 572);
            toolboxPanel.TabIndex = 0;
            // 
            // toolbox
            // 
            toolbox.AutoScroll = true;
            toolbox.BackColor = Color.FromArgb(240, 240, 240);
            toolbox.Dock = DockStyle.Fill;
            toolbox.ItemHeight = 34;
            toolbox.ItemWidth = 100;
            toolbox.Location = new Point(0, 30);
            toolbox.Name = "toolbox";
            toolbox.Padding = new Padding(8);
            toolbox.Size = new Size(83, 540);
            toolbox.TabIndex = 1;
            // 
            // toolboxLabel
            // 
            toolboxLabel.BackColor = Color.FromArgb(230, 230, 230);
            toolboxLabel.Dock = DockStyle.Top;
            toolboxLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            toolboxLabel.ForeColor = Color.FromArgb(60, 60, 60);
            toolboxLabel.Location = new Point(0, 0);
            toolboxLabel.Name = "toolboxLabel";
            toolboxLabel.Padding = new Padding(8, 5, 0, 5);
            toolboxLabel.Size = new Size(83, 30);
            toolboxLabel.TabIndex = 0;
            toolboxLabel.Text = "工具箱";
            toolboxLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // canvas
            // 
            canvas.AllowDrop = true;
            canvas.BackColor = Color.WhiteSmoke;
            canvas.Dock = DockStyle.Fill;
            labelDocument1.Dpi = 300F;
            labelDocument1.PageHeightMm = 50F;
            labelDocument1.PageSize = new SizeF(400F, 300F);
            labelDocument1.PageWidthMm = 100F;
            canvas.Document = labelDocument1;
            canvas.Location = new Point(0, 0);
            canvas.Name = "canvas";
            canvas.Size = new Size(766, 575);
            canvas.TabIndex = 0;
            canvas.SelectionChanged += canvas_SelectionChanged;
            // 
            // propertyGrid1
            // 
            propertyGrid1.Dock = DockStyle.Fill;
            propertyGrid1.Location = new Point(0, 0);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.Size = new Size(246, 575);
            propertyGrid1.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 600);
            Controls.Add(mainSplitContainer);
            Controls.Add(toolStrip1);
            Name = "MainForm";
            Text = "Label Designer - 工具箱版本";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            mainSplitContainer.Panel1.ResumeLayout(false);
            mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)mainSplitContainer).EndInit();
            mainSplitContainer.ResumeLayout(false);
            leftSplitContainer.Panel1.ResumeLayout(false);
            leftSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)leftSplitContainer).EndInit();
            leftSplitContainer.ResumeLayout(false);
            toolboxPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private ToolStrip toolStrip1;
        private ToolStripButton btnOpen;
        private ToolStripButton btnSave;
        private ToolStripButton btnPrint;
        private ToolStripButton btnPrintFromApi;
        private ToolStripButton btnApiTest;
        private SplitContainer mainSplitContainer;
        private SplitContainer leftSplitContainer;
        private Panel toolboxPanel;
        private Label toolboxLabel;
        private UI.ToolboxControl toolbox;
        private UI.DesignerCanvas canvas;
        private PropertyGrid propertyGrid1;
    }
}