using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using LabelDesigner.Items;
using LabelDesigner.Model;
using LabelDesigner.Services;
using System.Collections.Generic;   // 🔹 for List<>

namespace LabelDesigner.UI
{
    public class DesignerCanvas : Control
    {
        public event EventHandler? SelectionChanged;

        private LabelDocument _document = LabelDocument.CreateDefault();
        public LabelDocument Document
        {
            get => _document;
            set { _document = value; Invalidate(); }
        }

        public CanvasItem? SelectedItem { get; private set; }

        private enum DragMode { None, Move, Resize }
        private DragMode _dragMode = DragMode.None;
        private int _resizeHandleIndex = -1;

        private PointF _dragStart;
        private RectangleF _originalBounds;

        // 🔹 對齊線相關
        private List<(PointF start, PointF end)> _snapLines = new();
        private const float _snapThreshold = 5f;

        private readonly FieldResolver _resolver = new FieldResolver(new()
        {
            ["Name"] = "測試姓名",
            ["Code"] = "A001"
        });

        public DesignerCanvas()
        {
            this.DoubleBuffered = true;
            this.BackColor = Color.WhiteSmoke;
            this.SetStyle(ControlStyles.Selectable, true);
        }

        public void AddItem(CanvasItem item)
        {
            _document.Items.Add(item);
            SelectItem(item);
            Invalidate();
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

                // 🔹 每個物件都有灰色虛線框
                item.DrawOutline(e.Graphics);

                // 🔹 被選取的再畫藍色框 + Resize 控制點
                if (item == SelectedItem)
                    item.DrawSelection(e.Graphics);

                e.Graphics.Restore(stateItem);
            }

            // 🔹 畫出對齊輔助線
            if (_snapLines.Any())
            {
                using var pen = new Pen(Color.Red, 1) { DashStyle = DashStyle.Dash };
                foreach (var line in _snapLines)
                    e.Graphics.DrawLine(pen, line.start, line.end);
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

        // ✅ Client <-> Page 轉換
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
            SelectedItem = _document.Items.LastOrDefault(it => it.HitTest(p));
            SelectionChanged?.Invoke(this, EventArgs.Empty);

            if (SelectedItem != null)
            {
                // 🔹 檢查是否點到 Resize Handle
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
            if (SelectedItem == null) return;

            var p = ClientToPage(e.Location);

            if (_dragMode == DragMode.Move)
            {
                var dx = p.X - _dragStart.X;
                var dy = p.Y - _dragStart.Y;

                var newBounds = new RectangleF(
                    _originalBounds.X + dx,
                    _originalBounds.Y + dy,
                    _originalBounds.Width,
                    _originalBounds.Height);

                SelectedItem.Bounds = newBounds;

                // 🔹 計算對齊輔助線
                _snapLines = FindSnapLines(SelectedItem);

                Invalidate();
            }
            else if (_dragMode == DragMode.Resize)
            {
                var dx = p.X - _dragStart.X;
                var dy = p.Y - _dragStart.Y;
                var b = _originalBounds;

                switch (_resizeHandleIndex)
                {
                    case 0: SelectedItem.Bounds = new RectangleF(b.X + dx, b.Y + dy, b.Width - dx, b.Height - dy); break;
                    case 1: SelectedItem.Bounds = new RectangleF(b.X, b.Y + dy, b.Width, b.Height - dy); break;
                    case 2: SelectedItem.Bounds = new RectangleF(b.X, b.Y + dy, b.Width + dx, b.Height - dy); break;
                    case 3: SelectedItem.Bounds = new RectangleF(b.X, b.Y, b.Width + dx, b.Height); break;
                    case 4: SelectedItem.Bounds = new RectangleF(b.X, b.Y, b.Width + dx, b.Height + dy); break;
                    case 5: SelectedItem.Bounds = new RectangleF(b.X, b.Y, b.Width, b.Height + dy); break;
                    case 6: SelectedItem.Bounds = new RectangleF(b.X + dx, b.Y, b.Width - dx, b.Height + dy); break;
                    case 7: SelectedItem.Bounds = new RectangleF(b.X + dx, b.Y, b.Width - dx, b.Height); break;
                }

                _snapLines = FindSnapLines(SelectedItem);  // 🔹 resize 時也支援
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _dragMode = DragMode.None;
            _resizeHandleIndex = -1;

            // 🔹 放開時清除輔助線
            _snapLines.Clear();
            Invalidate();
        }

        protected override bool IsInputKey(Keys keyData) => true;

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

        // 🔹 計算對齊線
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
    }
}
