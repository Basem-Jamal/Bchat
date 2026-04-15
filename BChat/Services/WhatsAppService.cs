using BChat.Data.DataStore;
using BChat.Models.Meta_Business;
using System.Text;
using System.Text.Json;

namespace BChat.Services
{
    public class WhatsAppService
    {
        private readonly string _accessToken;
        private readonly string _phoneNumberId;
        private readonly string _wabaId;
        private readonly HttpClient _httpClient;

        public WhatsAppService()
        {
            _accessToken = "EAAUFWHIkrd8BRJV8CQ3JooKMNHWlg5lOvjqMVYKxbLjrtdqI43qOkFqBuQsCGTKSZAV9UA0hpDxZCKytZB4nGTFU0pdeKCZBIZCAvnUy06MZAxfGPz8DNwzx68bDqt5bhUuSykRM8ZA2qQA5ZBneHAPSdRoYOhFieJCWiBm6zySXJ6Xjk9gXWPm4ULv7lLZCKwdIO7I0Bmbb7iMWlkBpHXTX1K8UFh6aP8hrwocZCa";
            _phoneNumberId = "277769305429634";
            _wabaId = "332467619947421";
            _httpClient = new HttpClient();
        }


        public async Task SyncTemplatesToDatabase()
        {
            string url = $"https://graph.facebook.com/v19.0/{_wabaId}/message_templates?limit=100";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");

            var response = await _httpClient.GetAsync(url);
            string body = await response.Content.ReadAsStringAsync();

            var json = JsonDocument.Parse(body);
            var data = json.RootElement.GetProperty("data");

            foreach (var item in data.EnumerateArray())
            {
                if (!item.TryGetProperty("status", out var statusProp)) continue;
                if (statusProp.GetString() != "APPROVED") continue;

                string name = item.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
                string language = item.TryGetProperty("language", out var l) ? l.GetString() ?? "" : "";
                string category = item.TryGetProperty("category", out var c) ? c.GetString() ?? "" : "";
                string componentsJson = item.TryGetProperty("components", out var comp)
                    ? comp.GetRawText()
                    : "[]";

                // استخرج نص الـ Body
                string bodyText = "";
                if (item.TryGetProperty("components", out var components))
                {
                    foreach (var co in components.EnumerateArray())
                    {
                        if (co.TryGetProperty("type", out var t) && t.GetString() == "BODY")
                        {
                            if (co.TryGetProperty("text", out var tx))
                                bodyText = tx.GetString() ?? "";
                            break;
                        }
                    }
                }

                // ✅ هنا تضيف استخراج Header
                string headerType = "NONE";
                string headerText = "";
                if (item.TryGetProperty("components", out var comps2))
                {
                    foreach (var co in comps2.EnumerateArray())
                    {
                        if (!co.TryGetProperty("type", out var t)) continue;
                        if (t.GetString() != "HEADER") continue;
                        if (co.TryGetProperty("format", out var fmt))
                            headerType = fmt.GetString() ?? "NONE";
                        if (co.TryGetProperty("text", out var htxt))
                            headerText = htxt.GetString() ?? "";
                        break;
                    }
                }

                TemplateRepository.Upsert(new WhatsAppTemplate
                {
                    Name = name,
                    Language = language,
                    Category = category,
                    BodyText = bodyText,
                    ComponentsJson = componentsJson,
                    HeaderType = headerType,  // ✅
                    HeaderText = headerText   // ✅
                });
            }     
          
        }
        // ── إرسال رسالة نصية ──────────────────────────────
        public async Task<bool> SendTextMessage(string toPhone, string message)
        {
            try
            {
                string url = $"https://graph.facebook.com/v18.0/{_phoneNumberId}/messages";

                var cleanPhone = toPhone.Replace(" ", "").Replace("-", "").Replace("+", "");
                cleanPhone = "+" + cleanPhone;

                var payload = new
                {
                    messaging_product = "whatsapp",
                    to = cleanPhone,
                    type = "template",
                    template = new
                    {
                        name = "test1",
                        language = new { code = "en" }
                    }
                };

                string json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");

                var response = await _httpClient.PostAsync(url, content);
                string respBody = await response.Content.ReadAsStringAsync();

                MessageBox.Show(respBody, "API Response");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                return false;
            }
        }

        public async Task<List<WhatsAppTemplate>> GetTemplatesAsync()
        {
            try
            {
                string url = $"https://graph.facebook.com/v19.0/{_wabaId}/message_templates?limit=100";

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");

                var response = await _httpClient.GetAsync(url);
                string body = await response.Content.ReadAsStringAsync();

                MessageBox.Show(body, "API Response"); // ← أضف هذا

                var json = JsonDocument.Parse(body);
                var data = json.RootElement.GetProperty("data");
                var templates = new List<WhatsAppTemplate>();

                foreach (var item in data.EnumerateArray())
                {
                    if (!item.TryGetProperty("status", out var statusProp)) continue;
                    if (statusProp.GetString() != "APPROVED") continue;

                    string bodyText = "";

                    if (item.TryGetProperty("components", out var components))
                    {
                        foreach (var comp in components.EnumerateArray())
                        {
                            if (!comp.TryGetProperty("type", out var typeProp)) continue;
                            if (typeProp.GetString() == "BODY")
                            {
                                if (comp.TryGetProperty("text", out var textProp))
                                    bodyText = textProp.GetString() ?? "";
                                break;
                            }
                        }
                    }

                    templates.Add(new WhatsAppTemplate
                    {
                        Name = item.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "",
                        Language = item.TryGetProperty("language", out var l) ? l.GetString() ?? "" : "",
                        Category = item.TryGetProperty("category", out var c) ? c.GetString() ?? "" : "",
                        BodyText = bodyText
                    });
                }

                return templates;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                return new List<WhatsAppTemplate>();
            }
        }


        public async Task<bool> SendTemplateMessage(
    string toPhone,
    string templateName,
    string language,
    string headerType,
    string imageUrl = "")
        {
            string url = $"https://graph.facebook.com/v19.0/{_phoneNumberId}/messages";

            var cleanPhone = toPhone.Replace(" ", "").Replace("-", "").Replace("+", "");

            // ── بناء الـ Components ────────────────────────────
            var components = new List<object>();

            if (headerType == "IMAGE" && !string.IsNullOrWhiteSpace(imageUrl))
            {
                components.Add(new
                {
                    type = "header",
                    parameters = new[]
                    {
                new
                {
                    type = "image",
                    image = new { link = imageUrl }
                }
            }
                });
            }

            // ── بناء الـ Payload الكامل ────────────────────────
            object payload;

            if (components.Count > 0)
            {
                payload = new
                {
                    messaging_product = "whatsapp",
                    to = cleanPhone,
                    type = "template",
                    template = new
                    {
                        name = templateName,
                        language = new { code = language },
                        components = components
                    }
                };
            }
            else
            {
                payload = new
                {
                    messaging_product = "whatsapp",
                    to = cleanPhone,
                    type = "template",
                    template = new
                    {
                        name = templateName,
                        language = new { code = language }
                    }
                };
            }

            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");

            var response = await _httpClient.PostAsync(url, content);

            // ── تسجيل نتيجة الحملة في الـ Console للتشخيص ────
            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[Campaign Failed] {toPhone} → {error}");
            }

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> CreateTemplateAsync(object payload)
        {
            string url = $"https://graph.facebook.com/v19.0/{_wabaId}/message_templates";

            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[CreateTemplate Failed] {error}");
            }

            return response.IsSuccessStatusCode;
        }
    }
}