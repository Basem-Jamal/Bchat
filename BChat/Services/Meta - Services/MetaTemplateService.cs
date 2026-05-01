using BChat.Data.DataStore;
using BChat.Data.DataStore.Apis;
using BChat.Models.Meta_Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BChat.Services.Meta___Services
{
    public class MetaTemplateService
    {
        private static HttpClient _client = new HttpClient();

        public static async Task<List<WhatsAppTemplate>> GetTemplatesAsync()
        {
            var businessAccountId = ApiSettingsRepository.GetValue("WhatsApp", "BusinessAccountId");
            var accessToken       = ApiSettingsRepository.GetValue("WhatsApp", "AccessToken");

            var url = $"https://graph.facebook.com/v19.0/{businessAccountId}/message_templates?limit=200";

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var respones = await _client.GetAsync(url);
            var json     = await  respones.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"📋 Templates: {json}");

            return ParseTemplates (json);
        }

        private static List<WhatsAppTemplate> ParseTemplates(string json)
        {
            var list = new List<WhatsAppTemplate>();

            var doc   = JsonDocument.Parse(json);
            var data = doc.RootElement.GetProperty("data");

            foreach (var item in data.EnumerateArray())
            {
                var template = new WhatsAppTemplate
                {
                    MetaTemplateId  = item.GetProperty("id").GetString(),
                    Name            = item.GetProperty("name").GetString(),
                    Status          = item.GetProperty("status").GetString(),
                    Language        = item.GetProperty("language").GetString(),
                    Category = item.GetProperty("category").GetString(),


                };

                if (item.TryGetProperty("components", out var components))
                {
                    foreach (var comp in components.EnumerateArray())
                    {
                        string type = comp.GetProperty("type").GetString() ?? "";

                        if (type == "BODY" && comp.TryGetProperty("text", out var bodyText))
                            template.BodyText = bodyText.ToString();

                        if (type == "HEADER")
                        {
                            template.HeaderType = comp.TryGetProperty("format", out var fmt)
                                ? fmt.GetString() : "NONE";

                            if (comp.TryGetProperty("text", out var headerText))
                                template.HeaderText = headerText.ToString();
                        }


                    }
                }
                list.Add(template);

            }
            return list;
        }


        public static async Task SyncTemplatesToDbAsync()
        {
            var templates = await GetTemplatesAsync();

            foreach (var template in templates)
            {

                if (template.Status != "APPROVED") continue;


                TemplateRepository.Upsert(template);

            }

            System.Diagnostics.Debug.WriteLine($"✅ تمت المزامنة: {templates.Count} قالب");
        }
    }
}
