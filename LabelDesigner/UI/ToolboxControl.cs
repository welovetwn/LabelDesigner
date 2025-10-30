// 檔案路徑：UI/ToolboxControl.cs

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace LabelDesigner.UI
{
    /// <summary>
    /// 工具箱控制項 - 類似 Visual Studio 的工具箱
    /// </summary>
    public class ToolboxControl : UserControl
    {
        private readonly List<ToolboxItem> _items = new();
        private ToolboxItem? _selectedItem;
        private int _itemHeight = 34;
        private int _itemWidth = 100; // 🆕 項目寬度（可調整）
        private int _iconSize = 32;

        public event EventHandler<ToolboxItem>? ItemSelected;

        public ToolboxControl()
        {
            DoubleBuffered = true;
            BackColor = Color.FromArgb(240, 240, 240);
            AutoScroll = true;
            Padding = new Padding(8);
            Width = 20; // ✅ 預設窄一點的寬度
            InitializeDefaultItems();
        }
        #region 🆕 公開屬性：ItemWidth, ItemHeight
        /// <summary>
        /// 單一項目的寬度（不含 Padding）
        /// </summary>
        public int ItemWidth
        {
            get => _itemWidth;
            set
            {
                if (value < 34) value = 34; // 最小寬度限制
                _itemWidth = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 單一項目的高度（含 Icon）
        /// </summary>
        public int ItemHeight
        {
            get => _itemHeight;
            set
            {
                if (value < 20) value = 20;
                _itemHeight = value;
                Invalidate();
            }
        }
        #endregion
        private void InitializeDefaultItems()
        {
            // 指標工具 (選取模式)
            AddItem(new ToolboxItem
            {
                Name = "Pointer",
                DisplayName = "指標",
                Icon = CreatePointerIcon(),
                ItemType = null // null 表示選取模式
            });

            // 文字
            AddItem(new ToolboxItem
            {
                Name = "Text",
                DisplayName = "文字",
                Icon = CreateTextIcon(),
                ItemType = typeof(Items.TextItem)
            });

            // 圖片
            AddItem(new ToolboxItem
            {
                Name = "Image",
                DisplayName = "圖片",
                Icon = CreateImageIcon(),
                ItemType = typeof(Items.ImageItem)
            });

            // 條碼
            AddItem(new ToolboxItem
            {
                Name = "Barcode",
                DisplayName = "條碼",
                Icon = CreateBarcodeIcon(),
                ItemType = typeof(Items.BarcodeItem)
            });

            // 矩形
            AddItem(new ToolboxItem
            {
                Name = "Rectangle",
                DisplayName = "矩形",
                Icon = CreateRectangleIcon(),
                ItemType = typeof(Items.RectangleItem)
            });

            // 直線
            AddItem(new ToolboxItem
            {
                Name = "Line",
                DisplayName = "直線",
                Icon = CreateLineIcon(),
                ItemType = typeof(Items.LineItem)
            });

            // 圓形
            AddItem(new ToolboxItem
            {
                Name = "Circle",
                DisplayName = "圓形",
                Icon = CreateCircleIcon(),
                ItemType = typeof(Items.CircleItem)
            });
        }


        public void AddItem(ToolboxItem item)
        {
            _items.Add(item);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.Clear(BackColor);

            int y = Padding.Top;
            foreach (var item in _items)
            {
                var rect = new Rectangle(Padding.Left, y, Width - Padding.Horizontal - SystemInformation.VerticalScrollBarWidth, _itemHeight);

                // 背景
                var bgColor = item == _selectedItem ? Color.FromArgb(200, 220, 240) : BackColor;
                if (rect.Contains(PointToClient(MousePosition)))
                {
                    bgColor = item == _selectedItem ? Color.FromArgb(180, 210, 240) : Color.FromArgb(250, 250, 250);
                }

                using (var brush = new SolidBrush(bgColor))
                    g.FillRectangle(brush, rect);

                // 外框
                using (var pen = new Pen(Color.FromArgb(200, 200, 200)))
                    g.DrawRectangle(pen, rect);

                // 圖示
                if (item.Icon != null)
                {
                    var iconRect = new Rectangle(
                        rect.X + 8,
                        rect.Y + (rect.Height - _iconSize) / 2,
                        _iconSize,
                        _iconSize
                    );
                    g.DrawImage(item.Icon, iconRect);
                }

                // 文字
                using (var font = new Font("Segoe UI", 9f))
                using (var brush = new SolidBrush(Color.FromArgb(50, 50, 50)))
                {
                    var textRect = new RectangleF(
                        rect.X + _iconSize + 16,
                        rect.Y,
                        rect.Width - _iconSize - 44,
                        rect.Height
                    );
                    var sf = new StringFormat
                    {
                        LineAlignment = StringAlignment.Center,
                        Alignment = StringAlignment.Near
                    };
                    g.DrawString(item.DisplayName, font, brush, textRect, sf);
                }

                y += _itemHeight + 2;
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            int y = Padding.Top;
            foreach (var item in _items)
            {
                var rect = new Rectangle(Padding.Left, y, Width - Padding.Horizontal, _itemHeight);
                if (rect.Contains(e.Location))
                {
                    _selectedItem = item;
                    ItemSelected?.Invoke(this, item);
                    Invalidate();
                    break;
                }
                y += _itemHeight + 2;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // 啟動拖曳
            if (e.Button == MouseButtons.Left && _selectedItem != null)
            {
                DoDragDrop(_selectedItem, DragDropEffects.Copy);
                // 拖曳完成後自動回到指標模式
                _selectedItem = _items[0]; // 指標
                Invalidate();
            }
            else
            {
                Invalidate(); // 重繪以顯示 hover 效果
            }
        }

        // === 圖示生成方法 ===

        private Bitmap CreatePointerIcon()
        {
            var bmp = new Bitmap(_iconSize, _iconSize);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // 繪製指標箭頭
                var points = new PointF[]
                {
                    new PointF(8, 4),
                    new PointF(8, 24),
                    new PointF(14, 18),
                    new PointF(18, 28),
                    new PointF(22, 26),
                    new PointF(18, 16),
                    new PointF(26, 14)
                };

                using (var brush = new SolidBrush(Color.Black))
                    g.FillPolygon(brush, points);
                using (var pen = new Pen(Color.White, 2))
                    g.DrawPolygon(pen, points);
            }
            return bmp;
        }

        private Bitmap CreateTextIcon()
        {
            var bmp = new Bitmap(_iconSize, _iconSize);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (var font = new Font("Arial", 20, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.FromArgb(0, 120, 215)))
                {
                    g.DrawString("A", font, brush, new PointF(4, 2));
                }
            }
            return bmp;
        }

        private Bitmap CreateImageIcon()
        {
            var bmp = new Bitmap(_iconSize, _iconSize);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);

                // 外框
                using (var brush = new SolidBrush(Color.FromArgb(100, 150, 200)))
                    g.FillRectangle(brush, 4, 4, 24, 24);

                // 山和太陽
                using (var pen = new Pen(Color.White, 2))
                {
                    g.DrawEllipse(pen, 18, 8, 6, 6);
                    g.DrawLine(pen, 6, 22, 12, 14);
                    g.DrawLine(pen, 12, 14, 18, 20);
                    g.DrawLine(pen, 18, 20, 26, 22);
                }
            }
            return bmp;
        }

        private Bitmap CreateBarcodeIcon()
        {
            var bmp = new Bitmap(_iconSize, _iconSize);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);

                using (var brush = new SolidBrush(Color.Black))
                {
                    // 繪製條碼線條
                    g.FillRectangle(brush, 4, 6, 2, 20);
                    g.FillRectangle(brush, 8, 6, 1, 20);
                    g.FillRectangle(brush, 11, 6, 3, 20);
                    g.FillRectangle(brush, 16, 6, 1, 20);
                    g.FillRectangle(brush, 19, 6, 2, 20);
                    g.FillRectangle(brush, 23, 6, 1, 20);
                    g.FillRectangle(brush, 26, 6, 2, 20);
                }
            }
            return bmp;
        }

        private Bitmap CreateRectangleIcon()
        {
            var bmp = new Bitmap(_iconSize, _iconSize);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (var brush = new SolidBrush(Color.FromArgb(100, 200, 100, 255)))
                    g.FillRectangle(brush, 6, 8, 20, 16);
                using (var pen = new Pen(Color.FromArgb(50, 100, 200), 2))
                    g.DrawRectangle(pen, 6, 8, 20, 16);
            }
            return bmp;
        }

        private Bitmap CreateLineIcon()
        {
            var bmp = new Bitmap(_iconSize, _iconSize);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (var pen = new Pen(Color.FromArgb(200, 50, 50), 3))
                    g.DrawLine(pen, 4, 28, 28, 4);
            }
            return bmp;
        }

        private Bitmap CreateCircleIcon()
        {
            var bmp = new Bitmap(_iconSize, _iconSize);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (var brush = new SolidBrush(Color.FromArgb(100, 255, 200, 100)))
                    g.FillEllipse(brush, 6, 6, 20, 20);
                using (var pen = new Pen(Color.FromArgb(200, 150, 50), 2))
                    g.DrawEllipse(pen, 6, 6, 20, 20);
            }
            return bmp;
        }
    }
}
