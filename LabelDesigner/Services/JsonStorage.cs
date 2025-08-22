using System.Text.Json;
using System.Text.Json.Serialization;
using LabelDesigner.Model;
using LabelDesigner.Items;

namespace LabelDesigner.Services
{
    public class JsonStorage
    {
        private readonly JsonSerializerOptions _opt = new()
        {
            WriteIndented = true
            // TypeInfoResolver = new DefaultJsonTypeInfoResolver() // ��������A�]���䤣�즹���O
        };

        public string ToJson(LabelDocument doc) => JsonSerializer.Serialize(doc, _opt);

        public LabelDocument FromJson(string json) => JsonSerializer.Deserialize<LabelDocument>(json, _opt)!;
    }
}
