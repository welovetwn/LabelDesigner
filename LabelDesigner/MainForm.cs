// 檔案路徑：MainForm.cs

using LabelDesigner.Model;
using LabelDesigner.Services;
using LabelDesigner.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace LabelDesigner
{
    public partial class MainForm : Form
    {
        private readonly JsonStorage _storage = new JsonStorage();
        private readonly PrintService _printer = new PrintService();
        private FieldResolver _resolver = new FieldResolver();

        public MainForm()
        {
            InitializeComponent();

            var doc = LabelDocument.CreateDefault(dpi: 300);
            canvas.Document = doc;

            // 工具箱事件處理
            toolbox.ItemSelected += Toolbox_ItemSelected;
        }

        private void Toolbox_ItemSelected(object? sender, ToolboxItem item)
        {
            // 設定 Canvas 的建立模式
            canvas.SetCreationMode(item.ItemType);
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
            if (f.Resolver != null)
            {
                _resolver = f.Resolver ?? new FieldResolver();
                canvas.SetResolver(_resolver);
            }
        }

        private void canvas_SelectionChanged(object? sender, EventArgs e)
        {
            if (canvas.SelectedItem != null)
                propertyGrid1.SelectedObject = canvas.SelectedItem;
            else
                propertyGrid1.SelectedObject = canvas.Document;
        }

        private async void btnPrintFromApi_Click(object sender, EventArgs e)
        {
            try
            {
                var provider = new ApiDataProvider();
                string apiUrl = "http://localhost:5210/api/labels";
                string httpMethod = "GET";
                string? payload = null;

                var fields = await provider.FetchAsync(apiUrl, httpMethod, payload);

                if (fields.ContainsKey("error"))
                {
                    MessageBox.Show("API 錯誤：" + fields["error"]);
                    return;
                }

                _resolver = new FieldResolver(fields);
                canvas.SetResolver(_resolver);
                _printer.PrintDocument(canvas.Document, this, _resolver);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "列印失敗: " + ex.Message);
            }
        }
    }
}
