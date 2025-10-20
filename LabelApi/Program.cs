// 📁 LabelApi/Program.cs

using LabelApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 綁定 port 5210
builder.WebHost.UseUrls("http://localhost:5210");

// 註冊 DbContext，使用 SQLite
builder.Services.AddDbContext<LabelDbContext>(options =>
    options.UseSqlite("Data Source=labels.db"));

builder.Services.AddControllers();

// ✅ 加入 Swagger/OpenAPI 服務
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Label API",
        Version = "v1",
        Description = "這是用於管理標籤的 RESTful API",
    });
});

var app = builder.Build();

app.UseRouting();

// ✅ 啟用 Swagger 中介軟體，只在開發環境開啟
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Label API v1");
        c.RoutePrefix = ""; // 👉 讓你打開 http://localhost:5210 就能看到 Swagger UI
    });
}

app.MapControllers();

// 初始化資料庫
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LabelDbContext>();
    db.Database.EnsureCreated();
}

app.Run();