# LabelDesigner

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-blue.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)](https://www.microsoft.com/windows)

LabelDesigner 是一個功能完整的 **C# WinForms** 標籤設計工具，專為 .NET 8 開發。提供類似 Visual Studio 設計器的直覺操作介面，支援拖放、對齊輔助、復原/重做等專業功能。

![LabelDesigner Screenshot](docs/images/screenshot.png)

## 📋 目錄

- [主要特色](#主要特色)
- [系統需求](#系統需求)
- [快速開始](#快速開始)
- [功能說明](#功能說明)
  - [工具箱系統](#工具箱系統)
  - [物件類型](#物件類型)
  - [欄位替換系統](#欄位替換系統)
  - [操作快捷鍵](#操作快捷鍵)
- [架構說明](#架構說明)
- [API 整合](#api-整合)
- [開發指南](#開發指南)
- [授權條款](#授權條款)

---

## 🎯 主要特色

### 專業設計介面
- 🎨 **VS2022 風格工具箱** - 左側浮動工具箱,支援拖放與點擊繪製
- 📐 **智慧對齊輔助線** - 自動顯示物件對齊輔助線(頁面中心、邊界、其他物件)
- 🎯 **精確控制點** - 8 個控制點支援任意方向縮放
- 🖱️ **智慧游標提示** - 根據位置自動切換適當游標樣式

### 物件操作
- ✅ **選取與移動** - 點選物件即可選取,拖曳移動
- ↔️ **自由縮放** - 透過 8 個控制點任意縮放物件
- 📋 **複製貼上** - Ctrl+C / Ctrl+V 快速複製
- 🗑️ **刪除物件** - Delete 鍵快速刪除
- ↩️ **Undo/Redo** - Ctrl+Z / Ctrl+Y 完整歷史記錄

### 多樣化物件類型
1. **文字物件 (TextItem)**
   - 字型、大小、顏色、對齊方式
   - 支援動態欄位替換 `{{FIELD:Name}}`
   - 雙擊編輯內容

2. **圖片物件 (ImageItem)**
   - 支援 PNG, JPG, BMP 格式
   - Base64 編碼儲存,無需外部檔案
   - 維持比例縮放選項

3. **條碼物件 (BarcodeItem)**
   - Code128, QRCode, EAN13
   - 可選擇是否顯示文字標籤
   - 支援動態條碼內容

4. **矩形物件 (RectangleItem)** ✨ 新增
   - 可設定外框顏色、填滿顏色
   - 線寬粗細、透明度控制
   - 支援填滿/空心切換

5. **直線物件 (LineItem)** ✨ 新增
   - 可設定線條顏色、線寬
   - 支援實線、虛線等樣式
   - 透明度控制

6. **圓形/橢圓物件 (CircleItem)** ✨ 新增
   - 可設定外框顏色、填滿顏色
   - 線寬粗細、透明度控制
   - 自動抗鋸齒繪製

### 動態資料整合
- 🔗 **API 資料來源** - 支援從 REST API 動態取得資料
- 📊 **欄位替換系統** - `{{DATE}}`, `{{FIELD:Name}}`, `{{NOW:format}}`
- 🧪 **資料測試介面** - 內建測試介面驗證欄位替換

---

## 💻 系統需求

- **作業系統**: Windows 10/11 (64-bit)
- **Framework**: .NET 8.0 Runtime 或更高版本
- **記憶體**: 建議 4GB 以上
- **解析度**: 1280x720 以上

### 開發環境需求
- Visual Studio 2022 (17.8+) 或 Visual Studio Code
- .NET 8.0 SDK
- C# 12.0

---

## 🚀 快速開始

### 1. 下載執行檔
```bash
# Clone repository
git clone https://github.com/welovetwn/LabelDesigner.git
cd LabelDesigner

# 建置專案
dotnet build --configuration Release

# 執行
dotnet run --project LabelDesigner
```

### 2. 使用預編譯版本
前往 [Releases](https://github.com/welovetwn/LabelDesigner/releases) 下載最新版本的執行檔。

### 3. 開始設計標籤
1. 從左側工具箱選擇物件類型
2. 在畫布上拖曳繪製或點擊放置
3. 使用右側屬性面板調整參數
4. 儲存為 `.label` 檔案

---

## 📚 功能說明

### 工具箱系統

左側工具箱提供 7 種工具,支援兩種操作方式:

#### 方式一:拖放操作
1. 從工具箱拖曳工具到畫布
2. 物件會在放下的位置以預設大小建立
3. 自動選取新建立的物件

#### 方式二:繪製操作
1. 點選工具箱中的工具(游標變為十字)
2. 在畫布上按住滑鼠左鍵拖曳
3. 放開滑鼠完成物件建立
4. 自動回到指標模式

```
工具箱配置:
┌─────────────┐
│  指標       │ ← 選取/移動模式
├─────────────┤
│  文字       │ ← 文字標籤
├─────────────┤
│  圖片       │ ← 圖片匯入
├─────────────┤
│  條碼       │ ← 條碼/QRCode
├─────────────┤
│  矩形       │ ← 矩形
├─────────────┤
│  直線       │ ← 直線
├─────────────┤
│  圓形/      │ ← 圓形
└─────────────┘
```

### 物件類型

#### 1. 文字物件
```csharp
// 屬性設定
Name: "產品名稱"
Text: "{{FIELD:ProductName}}"
FontFamily: "微軟正黑體"
FontSize: 14
Color: Black
Alignment: MiddleLeft
```

**支援的欄位標記:**
- `{{DATE}}` - 今天日期
- `{{NOW:yyyy/MM/dd}}` - 自訂格式時間
- `{{USERNAME}}` - 目前使用者名稱
- `{{FIELD:欄位名}}` - 動態欄位值

#### 2. 圖片物件
- **支援格式**: PNG, JPG, JPEG, BMP
- **儲存方式**: Base64 編碼(無需外部檔案)
- **特殊選項**: MaintainAspect (維持比例)

#### 3. 條碼物件
```csharp
Symbology: "Code128" | "QRCode" | "EAN13"
Value: "{{FIELD:Barcode}}"
ShowText: true
```

#### 4. 矩形物件 (新增)
```csharp
BorderColor: Color.Black       // 外框顏色
FillColor: Color.LightBlue     // 填滿顏色
LineWidth: 2f                  // 線寬 (0 = 無外框)
BorderAlpha: 255               // 外框透明度 (0-255)
FillAlpha: 128                 // 填滿透明度 (0-255)
IsFilled: true                 // 是否填滿
```

#### 5. 直線物件 (新增)
```csharp
LineColor: Color.Black         // 線條顏色
LineWidth: 3f                  // 線寬
LineAlpha: 255                 // 透明度 (0-255)
DashStyle: Solid               // 線條樣式: Solid, Dash, Dot, DashDot
```

#### 6. 圓形物件 (新增)
```csharp
BorderColor: Color.DarkRed     // 外框顏色
FillColor: Color.Yellow        // 填滿顏色
LineWidth: 2f                  // 線寬
BorderAlpha: 255               // 外框透明度
FillAlpha: 128                 // 填滿透明度
IsFilled: true                 // 是否填滿
```

### 欄位替換系統

`FieldResolver` 負責將標記轉換為實際值:

```csharp
// 基本使用
var resolver = new FieldResolver();
string result = resolver.Resolve("Hello, {{USERNAME}}!");
// 輸出: "Hello, John!"

// 使用字典資料
var fields = new Dictionary<string, string>
{
    { "ProductName", "iPhone 15" },
    { "Price", "NT$ 32,900" }
};
var resolver = new FieldResolver(fields);
string result = resolver.Resolve("{{FIELD:ProductName}} - {{FIELD:Price}}");
// 輸出: "iPhone 15 - NT$ 32,900"
```

**擴充範例:**
```csharp
// 在 FieldResolver.Resolve() 中加入自訂規則
if (input == "{{COMPANY}}")
    return "我的公司名稱";

if (input.StartsWith("{{CALC:"))
{
    // 實作簡單計算功能
    var expr = input.Replace("{{CALC:", "").Replace("}}", "");
    return EvaluateExpression(expr);
}
```

### 操作快捷鍵

| 功能 | 快捷鍵 | 說明 |
|------|--------|------|
| 選取物件 | 點擊 | 點選物件選取 |
| 移動物件 | 拖曳 | 拖曳已選取物件 |
| 縮放物件 | 拖曳控制點 | 8 個控制點任意縮放 |
| 複製 | Ctrl+C | 複製選取物件 |
| 貼上 | Ctrl+V | 貼上物件(偏移 10px) |
| 刪除 | Delete | 刪除選取物件 |
| 復原 | Ctrl+Z | 回到上一步 |
| 重做 | Ctrl+Y | 取消復原 |
| 編輯文字 | 雙擊 | 雙擊文字物件編輯 |

---

## 🏗️ 架構說明

### 專案結構
```
LabelDesigner/
├── Items/                      # 物件類別
│   ├── CanvasItem.cs          # 抽象基礎類別
│   ├── TextItem.cs            # 文字物件
│   ├── ImageItem.cs           # 圖片物件
│   ├── BarcodeItem.cs         # 條碼物件
│   ├── RectangleItem.cs       # 矩形物件 (新增)
│   ├── LineItem.cs            # 直線物件 (新增)
│   └── CircleItem.cs          # 圓形物件 (新增)
│
├── Model/                      # 資料模型
│   └── LabelDocument.cs       # 標籤文件模型
│
├── Services/                   # 服務層
│   ├── FieldResolver.cs       # 欄位解析器
│   ├── JsonStorage.cs         # JSON 儲存服務
│   ├── PrintService.cs        # 列印服務
│   ├── ColorJsonConverter.cs  # Color 序列化轉換器
│   ├── ApiDataProvider.cs     # API 資料提供者
│   └── UndoRedoManager.cs     # 復原/重做管理器
│
├── UI/                         # 使用者介面
│   ├── DesignerCanvas.cs      # 設計畫布
│   ├── ToolboxControl.cs      # 工具箱控制項 (新增)
│   ├── ToolboxItem.cs         # 工具項目定義 (新增)
│   └── ApiTestForm.cs         # API 測試介面
│
├── MainForm.cs                 # 主視窗
└── Program.cs                  # 程式進入點
```

### 核心類別說明

#### CanvasItem (抽象基礎類別)
```csharp
public abstract class CanvasItem
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public RectangleF Bounds { get; set; }
    public float Rotation { get; set; }
    
    public abstract void Draw(Graphics g, FieldResolver resolver);
    public abstract CanvasItem Clone();
    public virtual void DrawSelection(Graphics g);
    public virtual List<RectangleF> GetResizeHandles();
}
```

#### LabelDocument (文件模型)
```csharp
public class LabelDocument
{
    public float Dpi { get; set; } = 300f;
    public float PageWidthMm { get; set; } = 100f;
    public float PageHeightMm { get; set; } = 50f;
    public List<CanvasItem> Items { get; set; } = new();
    
    public SizeF PagePixelSize { get; }  // 計算像素大小
}
```

#### DesignerCanvas (設計畫布)
核心功能:
- 物件選取、移動、縮放
- 拖放支援 (從工具箱)
- 繪製模式 (點擊繪製)
- 對齊輔助線
- Undo/Redo 管理
- 座標轉換 (Client ↔ Page)

### JSON 儲存格式
```json
{
  "Dpi": 300,
  "PageWidthMm": 100,
  "PageHeightMm": 50,
  "Items": [
    {
      "$type": "text",
      "Id": "guid",
      "Name": "產品名稱",
      "Bounds": { "X": 50, "Y": 50, "Width": 200, "Height": 40 },
      "Text": "{{FIELD:ProductName}}",
      "FontFamily": "微軟正黑體",
      "FontSize": 14,
      "Color": "#FF000000"
    },
    {
      "$type": "rectangle",
      "BorderColor": "#FF000000",
      "FillColor": "#80ADD8E6",
      "LineWidth": 2.0,
      "IsFilled": true
    }
  ]
}
```

---

## 🔌 API 整合

### 從 API 取得資料

LabelDesigner 支援從 REST API 動態取得資料並套用到標籤:

```csharp
// 1. 實作 API 端點
[HttpGet("api/labels")]
public IActionResult GetLabelData()
{
    return Ok(new Dictionary<string, string>
    {
        { "ProductName", "iPhone 15 Pro" },
        { "Price", "NT$ 36,900" },
        { "Barcode", "1234567890123" }
    });
}

// 2. 在 LabelDesigner 中使用
var provider = new ApiDataProvider();
var fields = await provider.FetchAsync(
    "https://api.example.com/labels", 
    "GET", 
    null
);

var resolver = new FieldResolver(fields);
canvas.SetResolver(resolver);
```

### API 測試介面

內建的 `ApiTestForm` 提供:
- URL 設定
- HTTP Method 選擇 (GET/POST)
- Request Body 編輯
- 即時測試與預覽

---

## 👨💻 開發指南

### 新增自訂物件類型

```csharp
// 1. 建立新的物件類別
public class MyCustomItem : CanvasItem
{
    public string CustomProperty { get; set; }
    
    public override void Draw(Graphics g, FieldResolver resolver)
    {
        // 實作繪製邏輯
    }
    
    public override CanvasItem Clone()
    {
        return new MyCustomItem
        {
            Id = Guid.NewGuid(),
            Name = this.Name,
            Bounds = this.Bounds,
            CustomProperty = this.CustomProperty
        };
    }
}

// 2. 註冊到 CanvasItem
[JsonDerivedType(typeof(MyCustomItem), typeDiscriminator: "mycustom")]

// 3. 在 ToolboxControl 中加入工具
AddItem(new ToolboxItem
{
    Name = "MyCustom",
    DisplayName = "自訂物件",
    Icon = CreateMyCustomIcon(),
    ItemType = typeof(MyCustomItem)
});

// 4. 在 DesignerCanvas.CreateItemAt() 中處理建立
else if (itemType == typeof(MyCustomItem))
{
    item = new MyCustomItem
    {
        Name = "MyCustom",
        Bounds = bounds,
        CustomProperty = "預設值"
    };
}
```

### 擴充欄位解析器

```csharp
// 在 FieldResolver.Resolve() 中加入新規則
public string Resolve(string input)
{
    // ... 現有規則 ...
    
    // 新增: 隨機數字
    if (input.StartsWith("{{RANDOM:"))
    {
        var parts = input.Replace("{{RANDOM:", "")
                        .Replace("}}", "")
                        .Split('-');
        int min = int.Parse(parts[0]);
        int max = int.Parse(parts[1]);
        return new Random().Next(min, max).ToString();
    }
    
    // 新增: 資料庫查詢
    if (input.StartsWith("{{DB:"))
    {
        var query = input.Replace("{{DB:", "").Replace("}}", "");
        return ExecuteDatabaseQuery(query);
    }
    
    return input;
}
```

### 建置與發佈

```bash
# Debug 建置
dotnet build

# Release 建置
dotnet build --configuration Release

# 發佈單一執行檔 (Windows x64)
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true

# 發佈資料夾 (含 .NET Runtime)
dotnet publish -c Release -r win-x64 --self-contained true
```

---

## 🧪 測試

```bash
# 執行所有測試
dotnet test

# 執行特定測試
dotnet test --filter "FullyQualifiedName~FieldResolverTests"

# 產生覆蓋率報告
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

---

## 📝 授權條款

本專案採用 MIT 授權條款。詳見 [LICENSE](LICENSE) 檔案。

```
MIT License

Copyright (c) 2025 welovetwn

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction...
```

---

## 🤝 貢獻指南

歡迎提交 Issue 或 Pull Request!

### 貢獻流程
1. Fork 本專案
2. 建立功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交變更 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 開啟 Pull Request

### 程式碼規範
- 遵循 C# Coding Conventions
- 每個 public 方法都要有 XML 註解
- 單元測試覆蓋率 > 80%
- 所有 PR 都要通過 CI 檢查

---

## 📞 聯絡資訊

- **專案首頁**: https://github.com/welovetwn/LabelDesigner
- **Issue 追蹤**: https://github.com/welovetwn/LabelDesigner/issues
- **Email**: support@welovetwn.com

---

## 🎉 致謝

感謝以下開源專案:
- [ZXing.Net](https://github.com/micjahn/ZXing.Net) - 條碼產生
- [Newtonsoft.Json](https://www.newtonsoft.com/json) - JSON 處理 (備用)

---

## 📜 更新日誌

### v2.0.0 (2025-10-30)
- ✨ 新增工具箱系統 (VS2022 風格)
- ✨ 新增矩形、直線、圓形物件
- ✨ 支援拖放與繪製兩種建立方式
- ✨ 新增透明度控制
- ✨ 新增線條樣式選擇
- 🐛 修正 resize 控制點判斷問題
- 🎨 優化使用者介面配置

### v1.0.0 (2025-01-01)
- 🎉 初始版本發佈
- ✅ 文字、圖片、條碼物件
- ✅ 基本編輯功能
- ✅ JSON 儲存/載入
- ✅ 列印功能

---

## 📸 螢幕截圖

### 主介面
![主介面](docs/images/main-interface.png)

### 工具箱
![工具箱](docs/images/toolbox.png)

### 屬性面板
![屬性面板](docs/images/properties.png)

### 對齊輔助線
![對齊輔助線](docs/images/snap-lines.png)

---

**Made with ❤️ by welovetwn team**
