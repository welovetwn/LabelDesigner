using System;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LabelDesigner.Services
{
    public class ColorJsonConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var colorString = reader.GetString();

            if (string.IsNullOrWhiteSpace(colorString))
                return Color.Black;

            if (colorString.StartsWith("#"))
            {
                colorString = colorString.Substring(1); // 去掉 #
                if (colorString.Length == 6) // RRGGBB
                {
                    return Color.FromArgb(
                        255, // 不透明
                        Convert.ToByte(colorString.Substring(0, 2), 16),
                        Convert.ToByte(colorString.Substring(2, 2), 16),
                        Convert.ToByte(colorString.Substring(4, 2), 16)
                    );
                }
                else if (colorString.Length == 8) // AARRGGBB
                {
                    return Color.FromArgb(
                        Convert.ToByte(colorString.Substring(0, 2), 16),
                        Convert.ToByte(colorString.Substring(2, 2), 16),
                        Convert.ToByte(colorString.Substring(4, 2), 16),
                        Convert.ToByte(colorString.Substring(6, 2), 16)
                    );
                }
            }

            // 如果格式錯誤，回傳黑色
            return Color.Black;
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            // 輸出 #AARRGGBB
            writer.WriteStringValue($"#{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}");
        }
    }
}
