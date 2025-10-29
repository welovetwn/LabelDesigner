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
            var doc = LabelDocument.CreateDefault(dpi: 300);
            canvas.Document = doc;
        }
        
        // ✅ 新增文字
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

        // ✅ 新增圖片
        private void btnAddImage_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp";

            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                var itemSize = new SizeF(150, 100);

                var item = new Items.ImageItem
                {
                    Name = Path.GetFileNameWithoutExtension(ofd.FileName), // 改用名稱，不儲存路徑
                    Bounds = canvas.GetCenteredBounds(itemSize),
                    MaintainAspect = true
                };

                try
                {
                    item.LoadImageAndConvertToBase64(ofd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"圖片讀取失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                canvas.AddItem(item);
            }
        }

        // ✅ 新增條碼
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

        // ✅ 新增矩形
        private void btnAddRectangle_Click(object sender, EventArgs e)
        {
            var itemSize = new SizeF(200, 150);
            var item = new Items.RectangleItem
            {
                Name = "Rectangle",
                Bounds = canvas.GetCenteredBounds(itemSize),
                BorderColor = Color.Black,
                FillColor = Color.LightBlue,
                LineWidth = 2f,
                IsFilled = true
            };
            canvas.AddItem(item);
        }

        // ✅ 新增直線
        private void btnAddLine_Click(object sender, EventArgs e)
        {
            var itemSize = new SizeF(250, 80);
            var item = new Items.LineItem
            {
                Name = "Line",
                Bounds = canvas.GetCenteredBounds(itemSize),
                LineColor = Color.Black,
                LineWidth = 3f,
                DashStyle = System.Drawing.Drawing2D.DashStyle.Solid
            };
            canvas.AddItem(item);
        }

        // ✅ 新增圓形
        private void btnAddCircle_Click(object sender, EventArgs e)
        {
            var itemSize = new SizeF(180, 180);
            var item = new Items.CircleItem
            {
                Name = "Circle",
                Bounds = canvas.GetCenteredBounds(itemSize),
                BorderColor = Color.DarkRed,
                FillColor = Color.Yellow,
                LineWidth = 2f,
                IsFilled = true
            };
            canvas.AddItem(item);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Label JSON|*.label;*.json";
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
            sfd.Filter = "Label JSON|*.label";
            sfd.FileName = "MyLabel.label";
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
        // /MainForm.cs
        private async void btnPrintFromApi_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. 建立 API 提供者
                var provider = new ApiDataProvider();

                // 2. 設定 API URL 與方法（請改成你的 API）
                string apiUrl = "http://localhost:5210/api/labels"; // 改成你本機 API URL
                string httpMethod = "GET"; // 或 "POST"
                string? payload = null;    // 如果是 POST，放 JSON 字串

                // 3. 呼叫 API 並解析為欄位字典
                var fields = await provider.FetchAsync(apiUrl, httpMethod, payload);

                // 4. 錯誤檢查
                if (fields.ContainsKey("error"))
                {
                    MessageBox.Show("API 錯誤：" + fields["error"]);
                    return;
                }

                // 5. 直接用新的欄位字典建立 FieldResolver
                _resolver = new FieldResolver(fields);

                // 6. 套用欄位資料到 Canvas
                canvas.SetResolver(_resolver);

                // 7. 執行列印
                _printer.PrintDocument(canvas.Document, this, _resolver);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "列印失敗: " + ex.Message);
            }
        }


    }
}
