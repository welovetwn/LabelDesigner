using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LabelDesigner.Services
{
    public class FieldResolver
    {
        private readonly Dictionary<string, string> _fields;

        public FieldResolver(Dictionary<string, string>? fields = null)
        {
            _fields = fields ?? new();
        }

        public void Set(string key, string value) => _fields[key] = value;

        public string Resolve(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            string s = input;

            // {{DATE}} -> yyyy-MM-dd
            s = s.Replace("{{DATE}}", DateTime.Now.ToString("yyyy-MM-dd"));

            // {{FIELD:Name}}
            s = Regex.Replace(s, @"\{\{FIELD:(.*?)\}\}", m =>
            {
                var key = m.Groups[1].Value;
                return _fields.TryGetValue(key, out var val) ? val : m.Value;
            });

            return s;
        }
    }
}
