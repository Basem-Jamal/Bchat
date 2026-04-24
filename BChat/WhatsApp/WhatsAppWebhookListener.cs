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
                var secret = context.Request.Headers["X-BChat-Secret"];
                var path = context.Request.Url?.AbsolutePath ?? "";

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

                // قراءة الـ Body
                using var reader = new StreamReader(
                    context.Request.InputStream,
                    context.Request.ContentEncoding);
                var body = await reader.ReadToEndAsync();

                // ✅ أرسل 200 فوراً قبل المعالجة
                context.Response.StatusCode = 200;
                context.Response.Close();

                // ✅ عالج في الخلفية
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