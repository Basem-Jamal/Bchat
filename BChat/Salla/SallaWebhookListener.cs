using System.Net;
using System.Text;
using System.Text.Json;
using System.Diagnostics;

namespace BChat.Salla
{
    public class SallaWebhookListener
    {
        private HttpListener? _listener;
        private CancellationTokenSource? _cts;
        private const string WebhookSecret = "7c86a7eb7f47dc4db5d23f3f686b7c0cc5612ee7d3b32e8d529a80bca7aa5983";

        public event Action<string, JsonElement>? OnEventReceived;

        public void Start()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://+:5000/webhook/");
            _listener.Start();
            _cts = new CancellationTokenSource();
            Task.Run(() => ListenAsync(_cts.Token));
            Debug.WriteLine("✅ Webhook Listener started on port 5000");
        }

        private async Task ListenAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var context = await _listener!.GetContextAsync();
                    _ = Task.Run(() => HandleRequestAsync(context));
                }
                catch { break; }
            }
        }

        private async Task HandleRequestAsync(HttpListenerContext context)
        {
            try
            {
                using var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8);
                string body = await reader.ReadToEndAsync();

                // اطبع كل الهيدرز
                foreach (string key in context.Request.Headers.AllKeys)
                {
                    Debug.WriteLine($"🔑 Header: {key} = {context.Request.Headers[key]}");
                }

                Debug.WriteLine($"📥 Raw Body: {body}");

                // تجاهل التحقق مؤقتاً
                var json = JsonSerializer.Deserialize<JsonElement>(body);
                string eventName = json.GetProperty("event").GetString() ?? "";
                Debug.WriteLine($"📥 Event: {eventName}");

                if (eventName == "app.store.authorize")
                {
                    var data = json.GetProperty("data");

                    var token = new BChat.Models.StoreToken
                    {
                        StoreId = json.GetProperty("merchant").GetInt64().ToString(),
                        AccessToken = data.GetProperty("access_token").GetString() ?? "",
                        RefreshToken = data.TryGetProperty("refresh_token", out var rt) ? rt.GetString() : null,
                        ExpiresAt = data.TryGetProperty("expires", out var exp)
                                       ? DateTimeOffset.FromUnixTimeSeconds(exp.GetInt64()).UtcDateTime
                                       : null
                    };

                    BChat.Data.DataStore.StoreTokenRepository.Save(token);
                    Debug.WriteLine($"✅ Token saved for store: {token.StoreId}");
                }
           

                OnEventReceived?.Invoke(eventName, json);
                context.Response.StatusCode = 200;
                context.Response.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error: {ex.Message}");
                context.Response.StatusCode = 500;
                context.Response.Close();
            }
        }
        public void Stop()
        {
            _cts?.Cancel();
            _listener?.Stop();
            Debug.WriteLine("🛑 Webhook Listener stopped");
        }
    }
}