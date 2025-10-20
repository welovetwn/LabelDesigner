// 📁 LabelApi/Data/LabelDbContext.cs

using LabelApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LabelApi.Data
{
    public class LabelDbContext : DbContext
    {
        public LabelDbContext(DbContextOptions<LabelDbContext> options) : base(options) { }

        public DbSet<Label> Labels { get; set; }
    }
}
