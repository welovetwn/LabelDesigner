// 📁 LabelApi/Controllers/LabelsController.cs

using LabelApi.Data;
using LabelApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabelApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LabelsController : ControllerBase
    {
        private readonly LabelDbContext _db;

        public LabelsController(LabelDbContext db)
        {
            _db = db;
        }

        // GET: http://localhost:5210/api/labels
        [HttpGet]
        public async Task<Dictionary<string, string>> Get()
        {
            return await _db.Labels.ToDictionaryAsync(l => l.Name, l => l.Value);
        }

        // POST: http://localhost:5210/api/labels
        // BODY: { "Name": "Product", "Value": "TSMC123" }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Label label)
        {
            _db.Labels.Add(label);
            await _db.SaveChangesAsync();
            return Ok(label);
        }
    }
}
