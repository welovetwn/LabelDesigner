using LabelDesigner.Services;
using System;
using System.Drawing;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;

namespace LabelDesigner.Items
{
    public class BarcodeItem : CanvasItem
    {
        public string Symbology { get; set; } = "Code128";
        public string Value { get; set; } = "12345678";
        public bool ShowText { get; set; } = true;

        public override void Draw(Graphics g, FieldResolver resolver)
        {
            string data = resolver.Resolve(Value);

            BarcodeFormat format = BarcodeFormat.CODE_128; // 預設 Code128

            if (Symbology.Equals("QRCode", StringComparison.OrdinalIgnoreCase))
                format = BarcodeFormat.QR_CODE;
            else if (Symbology.Equals("EAN13", StringComparison.OrdinalIgnoreCase))
                format = BarcodeFormat.EAN_13;
            // 其他格式可再擴充

            var writer = new BarcodeWriter
            {
                Format = format,
                Options = new EncodingOptions
                {
                    Width = (int)Bounds.Width,
                    Height = (int)Bounds.Height,
                    Margin = 0,
                    PureBarcode = true
                }
            };

            using var bitmap = writer.Write(data);
            g.DrawImage(bitmap, Bounds);

            if (ShowText && format == BarcodeFormat.CODE_128) // QRCode/EAN13 不一定要加字
            {
                using var font = new Font("Segoe UI", 9);
                using var brush = new SolidBrush(Color.Black);
                var rect = new RectangleF(Bounds.X, Bounds.Bottom + 2, Bounds.Width, 16);
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                g.DrawString(data, font, brush, rect, sf);
            }
        }

        /// <summary>
        /// 建立這個物件的深複製
        /// </summary>
        public override CanvasItem Clone()
        {
            return new BarcodeItem
            {
                Id = Guid.NewGuid(),   // ⚡ 新的 Guid，避免跟原本重複
                Name = this.Name,
                Bounds = this.Bounds,
                Rotation = this.Rotation,
                Symbology = this.Symbology,
                Value = this.Value,
                ShowText = this.ShowText
            };
        }
    }
}
