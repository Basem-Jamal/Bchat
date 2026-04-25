using BChat.Data.DataStore.Apis;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BChat.WhatsApp
{
    public static class MetaSender
    {
        private static readonly HttpClient _client = new HttpClient();

        public static async Task<bool> SendTextAsync(string toPhone, string message)
        {
            try
            {
                var phoneNumberId = ApiSettingsRepository.GetValue("WhatsApp", "PhoneNumberId");
                var accessToken = ApiSettingsRepository.GetValue("WhatsApp", "AccessToken");

                var url = $"https://graph.facebook.com/v19.0/{phoneNumberId}/messages";

                var payload = new
                {
                    messaging_product = "whatsapp",
                    to = toPhone,
                    type = "text",
                    text = new { body = message }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var response = await _client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"📤 Meta Send: {response.StatusCode} - {result}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ MetaSender Error: {ex.Message}");
                return false;
            }
        }
    }
}