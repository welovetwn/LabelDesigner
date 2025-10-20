// 📁 LabelApi/Models/Label.cs

using System.ComponentModel.DataAnnotations;

namespace LabelApi.Models
{
    public class Label
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
    }
}
