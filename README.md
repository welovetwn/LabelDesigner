# LabelDesigner 功能說明

LabelDesigner 是一個簡單的 C# WinForms 專案，提供使用者設計標籤(Label)
的功能。

------------------------------------------------------------------------

## 📦 目前支援的功能

### 🎨 畫布 (DesignerCanvas)

-   **滑鼠操作**
    -   點選物件 → 選取
    -   拖曳物件 → 移動
    -   使用縮放控制點 (8 個小方塊) → 改變大小
-   **對齊輔助線**
    -   拖曳時，物件會自動顯示紅色對齊輔助線 (上下、左右、中心)
-   **滑鼠游標提示**
    -   在控制點上會顯示不同的滑鼠游標 (水平 / 垂直 / 對角線縮放)

### ⌨️ 鍵盤快捷鍵

-   **Delete** → 刪除選取物件
-   **Ctrl+C** → 複製物件 (儲存在內部剪貼簿)
-   **Ctrl+V** → 貼上物件 (產生新的複本)

### 📑 物件 (CanvasItem)

目前支援三種類型： 1. **文字 (TextItem)** -
可設定字型、大小、顏色、對齊方式 - 支援「欄位替換」 (例如 `{{DATE}}`,
`{{FIELD:Name}}`) 2. **圖片 (ImageItem)** - 支援維持比例縮放 3. **條碼
(BarcodeItem)** - 支援 `Code128`, `QRCode`, `EAN13` -
可選擇是否顯示文字標籤

### 🔧 自訂欄位解析器 (FieldResolver)

在 LabelDesigner 中，`FieldResolver` 用來將 **文字標記** 轉換成
**實際值**。\
這讓標籤內容可以根據動態資料產生，而不只是固定文字。

#### 📌 內建支援

-   `{{DATE}}` → 顯示今天的日期\
-   `{{FIELD:Name}}` → 從資料來源取出 `Name` 欄位的值

#### 📌 可擴充的範例

開發者可以在 `FieldResolver.Resolve()` 中加入更多規則，例如：

  ----------------------------------------------------------------------------------------
  標記                         說明                       範例輸出
  ---------------------------- -------------------------- --------------------------------
  `{{NOW:yyyy/MM/dd HH:mm}}`   依指定格式顯示目前時間     `2025/08/22 15:30`

  `{{USERNAME}}`               顯示目前登入的使用者名稱   `john.doe`

  `{{PRODUCT:Price}}`          從資料來源取出商品價格     `299.00`
  ----------------------------------------------------------------------------------------

#### 📌 範例程式碼

``` csharp
public class FieldResolver
{
    private readonly Dictionary<string, string> _fields;

    public FieldResolver(Dictionary<string, string> fields)
    {
        _fields = fields;
    }

    public string Resolve(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";

        if (input.StartsWith("{{FIELD:"))
        {
            var key = input.Replace("{{FIELD:", "").Replace("}}", "");
            return _fields.TryGetValue(key, out var value) ? value : $"[未知欄位:{key}]";
        }

        if (input == "{{DATE}}")
            return DateTime.Now.ToShortDateString();

        if (input.StartsWith("{{NOW:"))
        {
            var format = input.Replace("{{NOW:", "").Replace("}}", "");
            return DateTime.Now.ToString(format);
        }

        if (input == "{{USERNAME}}")
            return Environment.UserName;

        return input;
    }
}
```

------------------------------------------------------------------------

## 🚀 未來可以改進的功能

-   支援群組 (Group/Ungroup)
-   支援旋轉 (Rotation Handle)
-   支援多選物件 (Shift + Click)
-   Undo / Redo 系統
-   將設計匯出成 PNG / PDF
