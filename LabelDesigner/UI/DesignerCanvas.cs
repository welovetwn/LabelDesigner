using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using LabelDesigner.Items;
using LabelDesigner.Model;
using LabelDesigner.Services;

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

        private bool _dragging = false;
        private PointF _dragStart;
        private RectangleF _originalBounds;

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
                if (item == SelectedItem) item.DrawSelection(e.Graphics);
                e.Graphics.Restore(stateItem);
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

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.Focus();

            var page = GetCenteredPageRect(_document.PagePixelSize);
            var p = new PointF(e.X - page.X, e.Y - page.Y);

            SelectedItem = _document.Items.LastOrDefault(it => it.HitTest(p));
            SelectionChanged?.Invoke(this, EventArgs.Empty);
            if (SelectedItem != null)
            {
                _dragging = true;
                _dragStart = p;
                _originalBounds = SelectedItem.Bounds;
            }
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_dragging && SelectedItem != null)
            {
                var page = GetCenteredPageRect(_document.PagePixelSize);
                var p = new PointF(e.X - page.X, e.Y - page.Y);
                var dx = p.X - _dragStart.X;
                var dy = p.Y - _dragStart.Y;
                SelectedItem.Bounds = new RectangleF(_originalBounds.X + dx, _originalBounds.Y + dy, _originalBounds.Width, _originalBounds.Height);
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _dragging = false;
        }

        protected override bool IsInputKey(Keys keyData) => true;

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            if (SelectedItem is TextItem txt)
            {
                var input = Microsoft.VisualBasic.Interaction.InputBox("編輯文字 (支援 {{FIELD:Name}}、{{DATE}})", "文字編輯", txt.Text);
                if (!string.IsNullOrEmpty(input)) { txt.Text = input; Invalidate(); }
            }
        }

        private void SelectItem(CanvasItem? item)
        {
            SelectedItem = item;
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
