using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LabelDesigner.Services
{
    /// <summary>
    /// 從外部 WebAPI 取得資料，並轉成欄位字典
    /// </summary>
    public class ApiDataProvider
    {
        private readonly HttpClient _http;

        public ApiDataProvider(HttpClient? httpClient = null)
        {
            _http = httpClient ?? new HttpClient();
        }

        /// <summary>
        /// 呼叫 API 取得資料
        /// </summary>
        /// <param name="url">API URL</param>
        /// <param name="method">GET / POST</param>
        /// <param name="payload">如果是 POST，這裡放 JSON 內容</param>
        /// <returns>Dictionary 形式的資料，可直接丟給 FieldResolver</returns>
        public async Task<Dictionary<string, string>> FetchAsync(
            string url, string method = "GET", string? payload = null)
        {
            HttpResponseMessage response;

            if (method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                response = await _http.PostAsync(url,
                    new StringContent(payload ?? "{}", System.Text.Encoding.UTF8, "application/json"));
            }
            else
            {
                response = await _http.GetAsync(url);
            }

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in doc.RootElement.EnumerateObject())
                    {
                        dict[prop.Name] = prop.Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                dict["error"] = $"JSON parse error: {ex.Message}";
            }

            return dict;
        }
    }
}