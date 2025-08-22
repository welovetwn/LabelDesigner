using LabelDesigner.Items;
using LabelDesigner.Services;
using System.Text.Json;

namespace LabelDesigner.Model
{
    public class LabelDocument
    {
        public float Dpi { get; set; } = 300f;
        public float PageWidthMm { get; set; } = 100f;
        public float PageHeightMm { get; set; } = 50f;
        public SizeF PageSize { get; set; } = new SizeF(400, 300);
        public List<CanvasItem> Items { get; set; } = new();

        public static LabelDocument CreateDefault(float dpi = 300)
        {
            return new LabelDocument { Dpi = dpi, PageWidthMm = 100, PageHeightMm = 50 };
        }

        public SizeF PagePixelSize
        {
            get
            {
                float inchW = PageWidthMm / 25.4f;
                float inchH = PageHeightMm / 25.4f;
                return new SizeF(inchW * Dpi, inchH * Dpi);
            }
        }

        public static LabelDocument FromJson(string json)
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new ColorJsonConverter() }
            };

            return JsonSerializer.Deserialize<LabelDocument>(json, options);
        }
        /// <summary>
        /// 建立整份標籤文件的深複製
        /// </summary>
        public LabelDocument Clone()
        {
            var clone = new LabelDocument
            {
                PageSize = this.PageSize
            };

            foreach (var item in Items)
            {
                clone.Items.Add(item.Clone());  // 呼叫各 Item 自己的 Clone()
            }

            return clone;
        }
    }
}
