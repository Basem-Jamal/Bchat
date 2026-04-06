using BChat.Salla;
using System.Diagnostics;
using System.Text.Json;


namespace BChat.Salla
{
    public partial class MainForm : Form
    {
        private SallaWebhookListener? _webhook;

        public MainForm()
        {
            InitializeComponent();

            _webhook = new SallaWebhookListener();
            _webhook.OnEventReceived += HandleSallaEvent;
            _webhook.Start();
        }

        private void HandleSallaEvent(string eventName, JsonElement data)
        {
            this.Invoke(() =>
            {
                switch (eventName)
                {
                    case "app.store.authorize":
                        string? accessToken = data
                            .GetProperty("data")
                            .GetProperty("access_token")
                            .GetString();
                        Debug.WriteLine($"🔑 Token: {accessToken}");
                        break;

                    case "order.created":
                        Console.WriteLine("🛒 طلب جديد!");
                        break;
                }
            });
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _webhook?.Stop();
            base.OnFormClosing(e);
        }
    }
}
