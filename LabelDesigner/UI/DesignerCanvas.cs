// 檔案路徑：UI/DesignerCanvas.cs

using LabelDesigner.Items;
using LabelDesigner.Model;
using LabelDesigner.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
// ❌ 移除這行,避免與 LabelDesigner.UI.ToolboxItem 衝突
// using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LabelDesigner.UI
{
    public class DesignerCanvas : Control
    {
        public event EventHandler? SelectionChanged;
        private LabelDocument _document = LabelDocument.CreateDefault();
        public LabelDocument Document
        {
            get => _document;
            set
            {
                _document = value;
                _history.Clear();
                _history.PushState(_document);
                Invalidate();
            }
        }

        public CanvasItem? SelectedItem { get; private set; }
        public FieldResolver Resolver => _resolver;

        private enum DragMode { None, Move, Resize }
        private DragMode _dragMode = DragMode.None;
        private int _resizeHandleIndex = -1;

        private PointF _dragStart;
        private RectangleF _originalBounds;

        // 對齊線相關
        private List<(PointF start, PointF end)> _snapLines = new();
        private const float _snapThreshold = 5f;

        // Undo/Redo 管理器
        private readonly UndoRedoManager<LabelDocument> _history;

        // 內部剪貼簿 (只存 CanvasItem)
        private List<CanvasItem> _clipboard = new();
        private FieldResolver _resolver = new FieldResolver();

        // 工具箱建立模式支援
        private Type? _creationMode = null;
        private PointF _creationStartPoint;
        private bool _isCreating = false;

        public DesignerCanvas()
        {
            this.DoubleBuffered = true;
            this.BackColor = Color.WhiteSmoke;
            this.SetStyle(ControlStyles.Selectable, true);

            // 初始化 Undo/Redo
            _history = new UndoRedoManager<LabelDocument>(doc => doc.Clone());
            _history.PushState(_document);

            // 啟用拖放支援
            AllowDrop = true;
            DragEnter += DesignerCanvas_DragEnter;
            DragDrop += DesignerCanvas_DragDrop;
        }

        // 外部設定資料來源
        public void SetResolver(FieldResolver resolver)
        {
            _resolver = resolver;
            Invalidate();
        }

        // 設定建立物件的模式
        public void SetCreationMode(Type? itemType)
        {
            _creationMode = itemType;
            _isCreating = false;
            Cursor = _creationMode == null ? Cursors.Default : Cursors.Cross;
        }

        // 拖放事件處理
        private void DesignerCanvas_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(typeof(ToolboxItem)) == true)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void DesignerCanvas_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(typeof(ToolboxItem)) is ToolboxItem toolboxItem)
            {
                if (toolboxItem.ItemType == null)
                {
                    SetCreationMode(null);
                    return;
                }

                var pt = PointToClient(new Point(e.X, e.Y));
                var canvasPt = ClientToPage(pt);
                CreateItemAt(toolboxItem.ItemType, canvasPt);
                SetCreationMode(null);
            }
        }

        public void AddItem(CanvasItem item)
        {
            _document.Items.Add(item);
            SelectItem(item);
            PushHistory();
            Invalidate();
        }

        private void PushHistory()
        {
            if (_document != null)
                _history.PushState(_document);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Page rect centered
            var pagePx = _document.PagePixelSize;
            var page = GetCenteredPageRect(pagePx);
            using (var bg = new SolidBrush(Color.White))
                e.Graphics.FillRectangle(bg, page);
            using (var pen = new Pen(Color.LightGray, 1))
                e.Graphics.DrawRectangle(pen, page.X, page.Y, page.Width, page.Height);

            // grid
            DrawGrid(e.Graphics, page, 10);

            // Transform to page coords
            var state = e.Graphics.Save();
            e.Graphics.TranslateTransform(page.X, page.Y);

            foreach (var item in _document.Items)
            {
                var stateItem = e.Graphics.Save();
                item.Draw(e.Graphics, _resolver);

                // 每個物件都有灰色虛線框
                item.DrawOutline(e.Graphics);

                // 被選取的再畫藍色框 + Resize 控制點
                if (item == SelectedItem)
                    item.DrawSelection(e.Graphics);

                e.Graphics.Restore(stateItem);
            }

            // 畫出對齊輔助線
            if (_snapLines.Any())
            {
                using var pen = new Pen(Color.Red, 1) { DashStyle = DashStyle.Dash };
                foreach (var line in _snapLines)
                    e.Graphics.DrawLine(pen, line.start, line.end);
            }

            // 繪製建立中的物件預覽
            if (_isCreating && _creationMode != null)
            {
                var currentPt = ClientToPage(PointToClient(MousePosition));
                var rect = GetRectFromTwoPoints(_creationStartPoint, currentPt);

                using var pen = new Pen(Color.DodgerBlue, 2) { DashStyle = DashStyle.Dash };
                e.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
            }

            e.Graphics.Restore(state);
        }

        private RectangleF GetCenteredPageRect(SizeF pagePx)
        {
            var x = (this.ClientSize.Width - pagePx.Width) / 2f;
            var y = (this.ClientSize.Height - pagePx.Height) / 2f;
            return new RectangleF(x, y, pagePx.Width, pagePx.Height);
        }

        private void DrawGrid(Graphics g, RectangleF page, int step)
        {
            using var pen = new Pen(Color.Gainsboro, 1);
            for (float x = page.Left; x <= page.Right; x += step)
                g.DrawLine(pen, x, page.Top, x, page.Bottom);
            for (float y = page.Top; y <= page.Bottom; y += step)
                g.DrawLine(pen, page.Left, y, page.Right, y);
        }

        private PointF ClientToPage(PointF clientPt)
        {
            var page = GetCenteredPageRect(_document.PagePixelSize);
            return new PointF(clientPt.X - page.X, clientPt.Y - page.Y);
        }

        private PointF PageToClient(PointF pagePt)
        {
            var page = GetCenteredPageRect(_document.PagePixelSize);
            return new PointF(page.X + pagePt.X, page.Y + pagePt.Y);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.Focus();

            var p = ClientToPage(e.Location);

            // 建立模式
            if (e.Button == MouseButtons.Left && _creationMode != null)
            {
                _creationStartPoint = p;
                _isCreating = true;
                return;
            }

            SelectedItem = _document.Items.LastOrDefault(it => it.HitTest(p));
            SelectionChanged?.Invoke(this, EventArgs.Empty);

            if (SelectedItem != null)
            {
                // 檢查是否點到 Resize Handle
                var handles = SelectedItem.GetResizeHandles();
                for (int i = 0; i < handles.Count; i++)
                {
                    if (handles[i].Contains(p))
                    {
                        _dragMode = DragMode.Resize;
                        _resizeHandleIndex = i;
                        _dragStart = p;
                        _originalBounds = SelectedItem.Bounds;
                        return;
                    }
                }

                // 否則就是拖曳移動
                _dragMode = DragMode.Move;
                _dragStart = p;
                _originalBounds = SelectedItem.Bounds;
            }
            else
            {
                _dragMode = DragMode.None;
            }

            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var p = ClientToPage(e.Location);

            // 如果正在建立物件
            if (_isCreating && _creationMode != null)
            {
                Invalidate();
                return;
            }

            // 如果正在拖曳或縮放
            if (_dragMode == DragMode.Move)
            {
                var dx = p.X - _dragStart.X;
                var dy = p.Y - _dragStart.Y;

                SelectedItem!.Bounds = new RectangleF(
                    _originalBounds.X + dx,
                    _originalBounds.Y + dy,
                    _originalBounds.Width,
                    _originalBounds.Height);

                _snapLines = FindSnapLines(SelectedItem);
                Invalidate();
                return;
            }
            else if (_dragMode == DragMode.Resize)
            {
                var dx = p.X - _dragStart.X;
                var dy = p.Y - _dragStart.Y;
                var b = _originalBounds;

                switch (_resizeHandleIndex)
                {
                    case 0: SelectedItem!.Bounds = new RectangleF(b.X + dx, b.Y + dy, b.Width - dx, b.Height - dy); break;
                    case 1: SelectedItem!.Bounds = new RectangleF(b.X, b.Y + dy, b.Width, b.Height - dy); break;
                    case 2: SelectedItem!.Bounds = new RectangleF(b.X, b.Y + dy, b.Width + dx, b.Height - dy); break;
                    case 3: SelectedItem!.Bounds = new RectangleF(b.X, b.Y, b.Width + dx, b.Height); break;
                    case 4: SelectedItem!.Bounds = new RectangleF(b.X, b.Y, b.Width + dx, b.Height + dy); break;
                    case 5: SelectedItem!.Bounds = new RectangleF(b.X, b.Y, b.Width, b.Height + dy); break;
                    case 6: SelectedItem!.Bounds = new RectangleF(b.X + dx, b.Y, b.Width - dx, b.Height + dy); break;
                    case 7: SelectedItem!.Bounds = new RectangleF(b.X + dx, b.Y, b.Width - dx, b.Height); break;
                }

                _snapLines = FindSnapLines(SelectedItem!);
                Invalidate();
                return;
            }

            // ✅ 修正：只在非拖曳狀態才檢查游標變化
            // 移除 _creationMode == null 的檢查,因為 SetCreationMode(null) 會在建立完成後被呼叫
            if (SelectedItem != null && _dragMode == DragMode.None)
            {
                var handles = SelectedItem.GetResizeHandles();
                Cursor = Cursors.Default;

                for (int i = 0; i < handles.Count; i++)
                {
                    if (handles[i].Contains(p))
                    {
                        Cursor = i switch
                        {
                            0 => Cursors.SizeNWSE,
                            1 => Cursors.SizeNS,
                            2 => Cursors.SizeNESW,
                            3 => Cursors.SizeWE,
                            4 => Cursors.SizeNWSE,
                            5 => Cursors.SizeNS,
                            6 => Cursors.SizeNESW,
                            7 => Cursors.SizeWE,
                            _ => Cursors.Default
                        };
                        break;
                    }
                }
            }
            // ✅ 如果在建立模式,強制顯示十字游標
            else if (_creationMode != null && !_isCreating)
            {
                Cursor = Cursors.Cross;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            // 如果正在建立物件
            if (_isCreating && _creationMode != null)
            {
                var p = ClientToPage(e.Location);
                var rect = GetRectFromTwoPoints(_creationStartPoint, p);

                // 防止建立太小的物件
                if (rect.Width > 5 && rect.Height > 5)
                {
                    CreateItemAt(_creationMode, rect);
                }

                _isCreating = false;
                SetCreationMode(null);
                return;
            }

            if (_dragMode == DragMode.Move || _dragMode == DragMode.Resize)
            {
                PushHistory();
            }

            _dragMode = DragMode.None;
            _resizeHandleIndex = -1;

            // 放開時清除輔助線
            _snapLines.Clear();
            Invalidate();
        }

        protected override bool IsInputKey(Keys keyData) => true;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Control && e.KeyCode == Keys.Z)
            {
                var prev = _history.Undo(_document);
                if (prev != null) { _document = prev; Invalidate(); }
            }
            else if (e.Control && e.KeyCode == Keys.Y)
            {
                var next = _history.Redo(_document);
                if (next != null) { _document = next; Invalidate(); }
            }
            else if (e.KeyCode == Keys.Delete && SelectedItem != null)
            {
                DeleteSelection();
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                CopySelection();
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                PasteSelection();
            }
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            if (SelectedItem is TextItem txt)
            {
                var input = Microsoft.VisualBasic.Interaction.InputBox(
                    "編輯文字 (支援 {{FIELD:Name}}、{{DATE}})",
                    "文字編輯", txt.Text);
                if (!string.IsNullOrEmpty(input))
                {
                    txt.Text = input;
                    PushHistory();
                    Invalidate();
                }
            }
        }

        private void SelectItem(CanvasItem? item)
        {
            SelectedItem = item;
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public RectangleF GetCenteredBounds(SizeF itemSize)
        {
            var page = GetCenteredPageRect(Document.PagePixelSize);
            float x = (page.Width - itemSize.Width) / 2f;
            float y = (page.Height - itemSize.Height) / 2f;
            return new RectangleF(x, y, itemSize.Width, itemSize.Height);
        }

        // 從兩點建立矩形
        private RectangleF GetRectFromTwoPoints(PointF p1, PointF p2)
        {
            return new RectangleF(
                Math.Min(p1.X, p2.X),
                Math.Min(p1.Y, p2.Y),
                Math.Abs(p2.X - p1.X),
                Math.Abs(p2.Y - p1.Y)
            );
        }

        // 在指定中心點建立物件（預設大小）
        private void CreateItemAt(Type itemType, PointF centerPoint)
        {
            var defaultSize = new SizeF(150, 100);
            var bounds = new RectangleF(
                centerPoint.X - defaultSize.Width / 2,
                centerPoint.Y - defaultSize.Height / 2,
                defaultSize.Width,
                defaultSize.Height
            );
            CreateItemAt(itemType, bounds);
        }

        // 建立物件的核心方法
        private void CreateItemAt(Type itemType, RectangleF bounds)
        {
            CanvasItem? item = null;

            if (itemType == typeof(TextItem))
            {
                item = new TextItem
                {
                    Name = "Text",
                    Text = "雙擊編輯文字",
                    Bounds = bounds,
                    FontFamily = "Segoe UI",
                    FontSize = 14,
                    Color = Color.Black
                };
            }
            else if (itemType == typeof(ImageItem))
            {
                using var ofd = new OpenFileDialog { Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp" };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var imgItem = new ImageItem
                    {
                        Name = Path.GetFileNameWithoutExtension(ofd.FileName),
                        Bounds = bounds,
                        MaintainAspect = true
                    };
                    try
                    {
                        imgItem.LoadImageAndConvertToBase64(ofd.FileName);
                        item = imgItem;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"圖片讀取失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else return;
            }
            else if (itemType == typeof(BarcodeItem))
            {
                item = new BarcodeItem
                {
                    Name = "Barcode",
                    Value = "123456789012",
                    Symbology = "Code128",
                    ShowText = true,
                    Bounds = bounds
                };
            }
            else if (itemType == typeof(RectangleItem))
            {
                item = new RectangleItem
                {
                    Name = "Rectangle",
                    Bounds = bounds,
                    BorderColor = Color.Black,
                    FillColor = Color.FromArgb(128, 173, 216, 230),
                    LineWidth = 2f,
                    IsFilled = true
                };
            }
            else if (itemType == typeof(LineItem))
            {
                item = new LineItem
                {
                    Name = "Line",
                    Bounds = bounds,
                    LineColor = Color.Black,
                    LineWidth = 3f,
                    DashStyle = DashStyle.Solid
                };
            }
            else if (itemType == typeof(CircleItem))
            {
                item = new CircleItem
                {
                    Name = "Circle",
                    Bounds = bounds,
                    BorderColor = Color.DarkRed,
                    FillColor = Color.FromArgb(128, 255, 255, 200),
                    LineWidth = 2f,
                    IsFilled = true
                };
            }

            if (item != null)
            {
                AddItem(item);
            }
        }

        // 計算對齊線
        private List<(PointF start, PointF end)> FindSnapLines(CanvasItem moving)
        {
            var lines = new List<(PointF, PointF)>();
            var m = moving.Bounds;
            var pageSize = _document.PagePixelSize;

            float mcx = m.Left + m.Width / 2f;
            float mcy = m.Top + m.Height / 2f;

            // 頁面中心
            float pageCx = pageSize.Width / 2f;
            float pageCy = pageSize.Height / 2f;

            if (Math.Abs(mcx - pageCx) <= _snapThreshold)
                lines.Add((new PointF(pageCx, 0), new PointF(pageCx, pageSize.Height)));
            if (Math.Abs(mcy - pageCy) <= _snapThreshold)
                lines.Add((new PointF(0, pageCy), new PointF(pageSize.Width, pageCy)));

            // 頁面邊界
            if (Math.Abs(m.Left - 0f) <= _snapThreshold)
                lines.Add((new PointF(0, 0), new PointF(0, pageSize.Height)));
            if (Math.Abs(m.Right - pageSize.Width) <= _snapThreshold)
                lines.Add((new PointF(pageSize.Width, 0), new PointF(pageSize.Width, pageSize.Height)));
            if (Math.Abs(m.Top - 0f) <= _snapThreshold)
                lines.Add((new PointF(0, 0), new PointF(pageSize.Width, 0)));
            if (Math.Abs(m.Bottom - pageSize.Height) <= _snapThreshold)
                lines.Add((new PointF(0, pageSize.Height), new PointF(pageSize.Width, pageSize.Height)));

            // 其他物件
            foreach (var item in _document.Items)
            {
                if (item == moving) continue;
                var b = item.Bounds;
                float bcx = b.Left + b.Width / 2f;
                float bcy = b.Top + b.Height / 2f;

                if (Math.Abs(mcx - bcx) <= _snapThreshold)
                    lines.Add((new PointF(bcx, 0), new PointF(bcx, pageSize.Height)));
                if (Math.Abs(mcy - bcy) <= _snapThreshold)
                    lines.Add((new PointF(0, bcy), new PointF(pageSize.Width, bcy)));

                if (Math.Abs(m.Left - b.Left) <= _snapThreshold)
                    lines.Add((new PointF(b.Left, 0), new PointF(b.Left, pageSize.Height)));
                if (Math.Abs(m.Right - b.Right) <= _snapThreshold)
                    lines.Add((new PointF(b.Right, 0), new PointF(b.Right, pageSize.Height)));
                if (Math.Abs(m.Top - b.Top) <= _snapThreshold)
                    lines.Add((new PointF(0, b.Top), new PointF(pageSize.Width, b.Top)));
                if (Math.Abs(m.Bottom - b.Bottom) <= _snapThreshold)
                    lines.Add((new PointF(0, b.Bottom), new PointF(pageSize.Width, b.Bottom)));
            }

            return lines;
        }

        // Copy / Paste / Delete
        private void CopySelection()
        {
            _clipboard.Clear();
            if (SelectedItem != null)
            {
                _clipboard.Add(SelectedItem.Clone());
            }
        }

        private void PasteSelection()
        {
            if (_clipboard.Count > 0)
            {
                var clone = _clipboard[0].Clone();
                clone.Bounds = new RectangleF(
                    clone.Bounds.X + 10,
                    clone.Bounds.Y + 10,
                    clone.Bounds.Width,
                    clone.Bounds.Height);

                _document.Items.Add(clone);
                SelectItem(clone);
                PushHistory();
                Invalidate();
            }
        }

        private void DeleteSelection()
        {
            if (SelectedItem != null)
            {
                _document.Items.Remove(SelectedItem);
                SelectItem(null);
                PushHistory();
                Invalidate();
            }
        }
    }
}