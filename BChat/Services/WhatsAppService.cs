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
            _accessToken = "EAAUaVvZBRoRwBRLLbshethbD56eT1CIMew7ZCYV4sZB8nIf5a5olHlYkLFpHeIkHXvM432IJBcPicwcrPfIxEXopjHieOnzcZAEmiO47kgtsaAkuKniVEZC" +
                "pAjgsM1qIOLIRVc7fhHr9BHwkZAGXIbvsvN4NY5oZBq0Sk8g1WkktIeOvMli7ycgEGydYHuChb8byp4jSeZB0tO5ZBCgoteiLPeSNSPZB1LikSTGY6OvKnR60xM6tmrNHK21" +
                "ix5JLQRPf1ICE25o7QVCIh9gunuszZAZB";
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
                        name = "eid_distributions",
                        language = new { code = "ar" }
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