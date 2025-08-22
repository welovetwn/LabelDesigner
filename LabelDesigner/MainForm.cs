using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using LabelDesigner.Model;
using LabelDesigner.Services;
using LabelDesigner.UI;

namespace LabelDesigner
{
    public partial class MainForm : Form
    {
        private readonly JsonStorage _storage = new JsonStorage();
        private readonly PrintService _printer = new PrintService();
        private FieldResolver _resolver = new FieldResolver(); // ← 新增這行

        public MainForm()
        {
            InitializeComponent();

            // default doc (100x50 mm converted to pixels at 300 dpi)
            var doc = LabelDocument.CreateDefault(dpi:300);
            canvas.Document = doc;
        }

        private void btnAddText_Click(object sender, EventArgs e)
        {
            var itemSize = new SizeF(300, 60); // 預設大小
            var item = new Items.TextItem
            {
                Name = "Text",
                Text = "雙擊編輯文字，支援變數：{{DATE}}、{{FIELD:Name}}",
                Bounds = canvas.GetCenteredBounds(itemSize),
                FontFamily = "Segoe UI",
                FontSize = 14,
                Color = Color.Black
            };
            canvas.AddItem(item);
        }

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp";
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                var itemSize = new SizeF(150, 100); // 預設大小
                var item = new Items.ImageItem
                {
                    Name = Path.GetFileName(ofd.FileName),
                    ImagePath = ofd.FileName,
                    Bounds = canvas.GetCenteredBounds(itemSize),
                    MaintainAspect = true
                };
                canvas.AddItem(item);
            }
        }

        private void btnAddBarcode_Click(object sender, EventArgs e)
        {
            var itemSize = new SizeF(250, 80); // 預設大小
            var item = new Items.BarcodeItem
            {
                Name = "Barcode",
                Value = "123456789012",
                Symbology = "Code128",
                ShowText = true,
                Bounds = canvas.GetCenteredBounds(itemSize)
            };
            canvas.AddItem(item);
        }


        private void btnOpen_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Label JSON|*.label.json;*.json";
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    var json = File.ReadAllText(ofd.FileName);
                    var doc = _storage.FromJson(json);
                    canvas.Document = doc;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "開啟失敗: " + ex.Message);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog();
            sfd.Filter = "Label JSON|*.label.json";
            sfd.FileName = "MyLabel.label.json";
            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    var json = _storage.ToJson(canvas.Document);
                    File.WriteAllText(sfd.FileName, json);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "儲存失敗: " + ex.Message);
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // 🔑 確保使用最新的 Resolver
                canvas.SetResolver(_resolver);
                _printer.PrintDocument(canvas.Document, this, _resolver);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "列印失敗: " + ex.Message);
            }
        }
        
        private void btnApiTest_Click(object sender, EventArgs e)
        {
            using var f = new LabelDesigner.UI.ApiTestForm();
            f.ShowDialog();
            // 🔑 不管 DialogResult，直接取 Resolver
            if (f.Resolver != null)
            {
                _resolver = f.Resolver ?? new FieldResolver();
                // ✅ 把 Resolver 傳進 Canvas
                canvas.SetResolver(_resolver);
                //string result = resolver.Resolve("Hello, {{Name}}! City = {{City}}");
                //MessageBox.Show(result);
            }
        }

        private void canvas_SelectionChanged(object? sender, EventArgs e)
        {
            if (canvas.SelectedItem != null)
                propertyGrid1.SelectedObject = canvas.SelectedItem;
            else
                propertyGrid1.SelectedObject = canvas.Document;
        }
    }
}
