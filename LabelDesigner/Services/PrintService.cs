// /Services/PrintService.cs
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using LabelDesigner.Model;

namespace LabelDesigner.Services
{
    public class PrintService
    {
        public void PrintDocument(LabelDocument doc, IWin32Window owner, FieldResolver resolver)
        {
            using var pd = new PrintDocument();
            pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            pd.PrintPage += (s, e) =>
            {
                var pagePx = doc.PagePixelSize;
                float scaleX = e.MarginBounds.Width / pagePx.Width;
                float scaleY = e.MarginBounds.Height / pagePx.Height;
                float scale = Math.Min(scaleX, scaleY);

                var g = e.Graphics;
                g.TranslateTransform(e.MarginBounds.Left, e.MarginBounds.Top);
                g.ScaleTransform(scale, scale);

                // ✅ 使用傳進來的 resolver，不要 new
                foreach (var item in doc.Items)
                    item.Draw(g, resolver);
            };

            using var dlg = new PrintDialog { UseEXDialog = true, Document = pd };
            if (dlg.ShowDialog(owner) == DialogResult.OK)
            {
                pd.Print();
            }
        }
    }
}