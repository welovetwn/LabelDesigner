using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using LabelDesigner.Model;
using LabelDesigner.Services;

namespace LabelDesigner.Services
{
    public class PrintService
    {
        public void PrintDocument(LabelDocument doc, IWin32Window owner)
        {
            using var pd = new PrintDocument();
            pd.DefaultPageSettings.Margins = new Margins(0,0,0,0);
            pd.PrintPage += (s, e) =>
            {
                // fit label page to printable area
                var pagePx = doc.PagePixelSize;
                float scaleX = e.MarginBounds.Width / pagePx.Width;
                float scaleY = e.MarginBounds.Height / pagePx.Height;
                float scale = Math.Min(scaleX, scaleY);

                var g = e.Graphics;
                g.TranslateTransform(e.MarginBounds.Left, e.MarginBounds.Top);
                g.ScaleTransform(scale, scale);

                var resolver = new FieldResolver(); // default empty
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
