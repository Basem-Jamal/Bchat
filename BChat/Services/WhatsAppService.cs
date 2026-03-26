using System.Text;
using System.Text.Json;

namespace BChat.Services
{
    public class WhatsAppService
    {
        private readonly string _accessToken;
        private readonly string _phoneNumberId;
        private readonly HttpClient _httpClient;

        public WhatsAppService()
        {
            _accessToken = "EAAUaVvZBRoRwBRLpfluJ827dKKYbLqycH3QuJt0kSB4KCZCWQ4P4yjzt4hxgZBFGRoZBHbyhK3GocizVR6a9QPHtpwwVISKqRtQ3MKg0Snn6H" +
                "oPdV9fZBG6ptdoZAZA5STR6vzZBAguKhEK81V2IerKI3OGOLKuQcmRZA" +
                "E25SLPIRDvSxWuXnkfgNEYtC8rAAxGwA1HojGjInrIAZCbMRWgSpNUI4paXSInjQLQiv" +
                "tfc0b5pCgvYAV22stSahcLI0rwJ6aEZCnQRp3PenZACl6hP2oZCK";
            _phoneNumberId = "969555369583968";
            _httpClient = new HttpClient();
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

                // ← أضف هذا السطر مؤقتاً
                MessageBox.Show(respBody, "API Response");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                return false;
            }
        }
    }
}