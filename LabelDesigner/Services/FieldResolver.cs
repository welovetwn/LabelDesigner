using System;
using System.Collections.Generic;

namespace LabelDesigner.Services
{
    public class FieldResolver
    {
        private Dictionary<string, string> _fields = new();

        public FieldResolver() { }

        public FieldResolver(Dictionary<string, string> fields)
        {
            _fields = new Dictionary<string, string>(fields);
        }

        /// <summary>
        /// �]�w���r��
        /// </summary>
        public void SetFields(Dictionary<string, string> fields)
        {
            _fields.Clear();
            foreach (var kv in fields)
                _fields[kv.Key] = kv.Value;
        }

        /// <summary>
        /// �ѪR�r�ꤤ�� {{���W}} �аO
        /// </summary>
        public string Resolve(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            string output = input;
            foreach (var kv in _fields)
            {
                output = output.Replace("{{" + kv.Key + "}}", kv.Value);
            }
            return output;
        }
    }
}
