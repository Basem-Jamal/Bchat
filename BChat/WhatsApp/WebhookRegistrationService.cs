using System;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Threading.Timer;

namespace BChat.WhatsApp
{
    /// <summary>
    /// يسجل جهاز BChat في webhook server عند الفتح
    /// ويلغي التسجيل عند الإغلاق
    /// </summary>
    public class WebhookRegistrationService : IDisposable
    {
        int test = 0;
        private readonly string _webhookServerUrl;
        private readonly HttpClient _httpClient;
        private readonly string _localIp;
        private Timer _heartbeatTimer;

        public WebhookRegistrationService(string webhookServerUrl)
        {
            _webhookServerUrl = webhookServerUrl.TrimEnd('/');
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            _localIp = GetLocalIpAddress();
        }

        /// <summary>
        /// يسجل الجهاز عند فتح BChat
        /// </summary>
        public async Task RegisterAsync()
        {
            try
            {
                var body = JsonSerializer.Serialize(new { ip = _localIp });
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                await _httpClient.PostAsync($"{_webhookServerUrl}/api/register", content);

                System.Diagnostics.Debug.WriteLine($"✅ Registered: {_localIp}");

                // Heartbeat كل دقيقتين عشان يبقى مسجل
                _heartbeatTimer = new Timer(
                    async _ => await RegisterAsync(),
                    null,
                    TimeSpan.FromMinutes(2),
                    TimeSpan.FromMinutes(2)
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Registration failed: {ex.Message}");
            }
        }

        /// <summary>
        /// يلغي تسجيل الجهاز عند إغلاق BChat
        /// </summary>
        public async Task UnregisterAsync()
        {
            try
            {
                _heartbeatTimer?.Dispose();
                var body = JsonSerializer.Serialize(new { ip = _localIp });
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                await _httpClient.PostAsync($"{_webhookServerUrl}/api/unregister", content);

                System.Diagnostics.Debug.WriteLine($"🔴 Unregistered: {_localIp}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Unregister failed: {ex.Message}");
            }
        }

        /// <summary>
        /// يجيب IP الجهاز على الشبكة المحلية
        /// </summary>
        private static string GetLocalIpAddress()
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != OperationalStatus.Up) continue;
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;

                foreach (var addr in ni.GetIPProperties().UnicastAddresses)
                {
                    if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        var ip = addr.Address.ToString();
                        if (ip.StartsWith("192.168.") || ip.StartsWith("10."))
                            return ip;
                    }
                }
            }
            return "127.0.0.1";
        }

        public void Dispose()
        {
            _heartbeatTimer?.Dispose();
            _httpClient?.Dispose();
        }
    }
}