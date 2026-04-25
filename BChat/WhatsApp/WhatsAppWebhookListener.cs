using BChat.Data.DataStore.Apis;
using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BChat.WhatsApp
{
    public class WhatsAppWebhookListener
    {
        private readonly HttpListener _listener;
        private CancellationTokenSource _cts;

        public event Action<IncomingWhatsAppMessage>? MessageReceived;

        public WhatsAppWebhookListener()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://+:5001/");
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _listener.Start();
            Task.Run(() => ListenLoop(_cts.Token));
        }

        public void Stop()
        {
            _cts?.Cancel();
            _listener?.Stop();
        }

        private async Task ListenLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    _ = Task.Run(() => HandleRequest(context));
                }
                catch { break; }
            }
        }

        private async Task HandleRequest(HttpListenerContext context)
        {
            try
            {
                var path = context.Request.Url?.AbsolutePath ?? "";
                var method = context.Request.HttpMethod;

                // ── Meta Webhook ──────────────────────────────
                if (path.StartsWith("/webhook/meta"))
                {
                    if (method == "GET")
                    {
                        await HandleMetaVerification(context);
                        return;
                    }
                    if (method == "POST")
                    {
                        await HandleMetaMessage(context);
                        return;
                    }
                }

                // ── BChat Webhook (الحالي) ─────────────────────
                var secret = context.Request.Headers["X-BChat-Secret"];
                if (secret != "bchat-secret-key-123")
                {
                    context.Response.StatusCode = 401;
                    context.Response.Close();
                    return;
                }

                if (!path.StartsWith("/webhook/whatsapp"))
                {
                    context.Response.StatusCode = 404;
                    context.Response.Close();
                    return;
                }

                using var reader = new StreamReader(
                    context.Request.InputStream,
                    context.Request.ContentEncoding);
                var body = await reader.ReadToEndAsync();

                context.Response.StatusCode = 200;
                context.Response.Close();

                _ = Task.Run(() =>
                {
                    try
                    {
                        var json = JsonDocument.Parse(body).RootElement;
                        var msg = new IncomingWhatsAppMessage
                        {
                            Phone = json.GetProperty("phone").GetString() ?? "",
                            SenderName = json.GetProperty("senderName").GetString() ?? "",
                            Text = json.GetProperty("text").GetString() ?? "",
                            WhatsAppMessageId = json.GetProperty("whatsappMessageId").GetString() ?? "",
                            SentAt = DateTime.Parse(
                                json.GetProperty("sentAt").GetString() ?? DateTime.Now.ToString()),
                        };
                        MessageReceived?.Invoke(msg);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ WhatsApp Error: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error: {ex.Message}");
                context.Response.StatusCode = 500;
                context.Response.Close();
            }
        }

        // ── Meta Verification (GET) ────────────────────────────
        private async Task HandleMetaVerification(HttpListenerContext context)
        {
            var query = context.Request.QueryString;
            var mode = query["hub.mode"];
            var token = query["hub.verify_token"];
            var challenge = query["hub.challenge"];

            var storedToken = ApiSettingsRepository.GetValue("WhatsApp", "WebhookVerifyToken");

            System.Diagnostics.Debug.WriteLine($"🔍 Mode: {mode}, Token: {token}, Stored: {storedToken}");

            if (mode == "subscribe" && token == storedToken)
            {
                var buffer = System.Text.Encoding.UTF8.GetBytes(challenge ?? "");
                context.Response.StatusCode = 200;
                context.Response.ContentLength64 = buffer.Length;
                await context.Response.OutputStream.WriteAsync(buffer);
                context.Response.Close();
                System.Diagnostics.Debug.WriteLine("✅ Meta Webhook Verified!");
            }
            else
            {
                context.Response.StatusCode = 403;
                context.Response.Close();
                System.Diagnostics.Debug.WriteLine("❌ Meta Verification Failed!");
            }
        }

        // ── Meta Message (POST) ────────────────────────────────
        private async Task HandleMetaMessage(HttpListenerContext context)
        {
            using var reader = new StreamReader(
                context.Request.InputStream,
                context.Request.ContentEncoding);
            var body = await reader.ReadToEndAsync();

            context.Response.StatusCode = 200;
            context.Response.Close();

            System.Diagnostics.Debug.WriteLine($"📩 Meta Message: {body}");

            _ = Task.Run(() =>
            {
                try
                {
                    var json = JsonDocument.Parse(body).RootElement;
                    var entry = json.GetProperty("entry")[0];
                    var change = entry.GetProperty("changes")[0];
                    var value = change.GetProperty("value");

                    if (!value.TryGetProperty("messages", out var messages)) return;

                    var message = messages[0];
                    if (message.GetProperty("type").GetString() != "text") return;

                    var phone = message.GetProperty("from").GetString() ?? "";
                    var text = message.GetProperty("text").GetProperty("body").GetString() ?? "";
                    var msgId = message.GetProperty("id").GetString() ?? "";
                    var timestamp = long.Parse(message.GetProperty("timestamp").GetString() ?? "0");
                    var sentAt = DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;
                    var name = value.GetProperty("contacts")[0]
                                         .GetProperty("profile")
                                         .GetProperty("name").GetString() ?? phone;

                    var msg = new IncomingWhatsAppMessage
                    {
                        Phone = phone,
                        SenderName = name,
                        Text = text,
                        WhatsAppMessageId = msgId,
                        SentAt = sentAt
                    };

                    MessageReceived?.Invoke(msg);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Meta Parse Error: {ex.Message}");
                }
            });
        }
    }

    public class IncomingWhatsAppMessage
    {
        public string Phone { get; set; } = "";
        public string SenderName { get; set; } = "";
        public string Text { get; set; } = "";
        public string WhatsAppMessageId { get; set; } = "";
        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}