using LabelDesigner.Services;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabelDesigner.UI
{
    public partial class ApiTestForm : Form
    {
        private readonly HttpClient _httpClient = new();
        private FieldResolver _resolver = new();        
        public FieldResolver Resolver => _resolver;// 🔑 加這個公開屬性
        public ApiTestForm()
        {
            InitializeComponent();
            txtUrl.Text = "https://jsonplaceholder.typicode.com/users/1";
        }

        private async void btnCallApi_Click(object sender, EventArgs e)
        {
            try
            {
                string url = txtUrl.Text.Trim();
                if (string.IsNullOrEmpty(url))
                {
                    MessageBox.Show("請輸入 API URL");
                    return;
                }

                string json = await _httpClient.GetStringAsync(url);

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // 提取想要的欄位
                var dict = new Dictionary<string, string>
                {
                    ["Id"] = root.GetProperty("id").ToString(),
                    ["Name"] = root.GetProperty("name").GetString() ?? "",
                    ["UserName"] = root.GetProperty("username").GetString() ?? "",
                    ["Email"] = root.GetProperty("email").GetString() ?? "",
                    ["City"] = root.GetProperty("address").GetProperty("city").GetString() ?? ""
                };

                _resolver = new FieldResolver(dict);

                txtResult.Text = $"✅ 已載入 {dict.Count} 個欄位\n" +
                                 string.Join(Environment.NewLine, dict);
            }
            catch (Exception ex)
            {
                txtResult.Text = "❌ 發生錯誤: " + ex.Message;
            }
        }

        private void btnTestResolve_Click(object sender, EventArgs e)
        {
            string input = txtInput.Text;
            string output = _resolver.Resolve(input);
            txtOutput.Text = output;
        }
    }
}
