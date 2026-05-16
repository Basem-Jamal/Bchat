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
                var url = $"https://graph.facebook.com/v21.0/{phoneNumberId}/messages";

                var payload = new
                {
                    messaging_product = "whatsapp",
                    to = toPhone,
                    type = "text",
                    text = new { body = message }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                //_client.DefaultRequestHeaders.Clear();
                //_client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
                request.Content = content;

                var response = await _client.SendAsync(request);



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

        public static async Task<bool> SendTemplateAsync(string toPhone, string templateName, string language, string headerType, string mediaId = "", string mediaUrl = "")
        {
            try
            {
                var phoneNumberId = ApiSettingsRepository.GetValue("WhatsApp", "PhoneNumberId");
                var accessToken = ApiSettingsRepository.GetValue("WhatsApp", "AccessToken");
                var url = $"https://graph.facebook.com/v21.0/{phoneNumberId}/messages";

                object templatePayload;

                if (headerType == "VIDEO" && !string.IsNullOrEmpty(mediaId))
                {
                    // ← Media ID (الأفضل)
                    templatePayload = new
                    {
                        name = templateName,
                        language = new { code = language },
                        components = new[]
                        {
                            new { type = "header", parameters = new object[]
                                {
                                    new { type = "video", video = new { id = mediaId } }
                                }
                            }
                        }
                    };
                }
                else if ((headerType == "VIDEO" || headerType == "IMAGE") && !string.IsNullOrEmpty(mediaUrl))
                {
                    // ← رابط URL (احتياطي)
                    object mediaParam = headerType == "VIDEO"
                        ? new { type = "video", video = new { link = mediaUrl } }
                        : (object)new { type = "image", image = new { link = mediaUrl } };

                    templatePayload = new
                    {
                        name = templateName,
                        language = new { code = language },
                        components = new[]
                        {
                            new { type = "header", parameters = new[] { mediaParam } }
                        }
                    };
                }
                else
                {
                    // ← بدون header
                    templatePayload = new
                    {
                        name = templateName,
                        language = new { code = language }
                    };
                }

                var payload = new
                {
                    messaging_product = "whatsapp",
                    to = toPhone,
                    type = "template",
                    template = templatePayload
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                //_client.DefaultRequestHeaders.Clear();
                //_client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
                request.Content = content;

                var response = await _client.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"📤 Template Send: {response.StatusCode} - {result}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ MetaSender Template Error: {ex.Message}");
                return false;
            }
        }
    }
}