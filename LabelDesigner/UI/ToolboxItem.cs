// 檔案路徑：UI/ToolboxItem.cs

using System;
using System.Drawing;

namespace LabelDesigner.UI
{
    /// <summary>
    /// 工具箱項目定義
    /// </summary>
    public class ToolboxItem
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public Image? Icon { get; set; }
        public Type? ItemType { get; set; } // null = 指標模式
    }
}
