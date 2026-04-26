using BChat.Auth;
using BChat.Data.DataStore;
using BChat.Data.DataStore.Users_DB;
using BChat.Global;
using System.Diagnostics;

namespace BChat
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            ApplicationConfiguration.Initialize();

            //---------
            //Users
            AppCache.Users = UsersRepository.GetAll();
            //---------
            AppCache.Customers = CustomerRepository.GetAll();
            AppCache.Groups = GroupRepository.GetAll();
            AppCache.GroupMembers = GroupMemberRepository.GetAll();
            AppCache.ChatMessages = ChatMessageRepository.GetAll(); // ← هذا فقط


            CalculateMembersCount();

            AppCache.WhatsAppListener = new BChat.WhatsApp.WhatsAppWebhookListener();
            AppCache.WhatsAppListener.Start();

            //try
            //{
            //    AppCache.WhatsAppListener = new BChat.WhatsApp.WhatsAppWebhookListener();
            //    AppCache.WhatsAppListener.Start();
            //}
            //catch { /* تجاهل لو Port مشغول */ }
            //try
            //{
            //    AppCache.WhatsAppListener = new BChat.WhatsApp.WhatsAppWebhookListener();
            //    AppCache.WhatsAppListener.Start();
            //}
            //catch { /* تجاهل لو Port مشغول */ }

            while (true)
            {
                using (var login = new Login())
                {
                    if (login.ShowDialog() != DialogResult.OK)
                        break;
                }
                var home = new Home();
                Application.Run(home);

            }
        }

        private static void CalculateMembersCount()
        {
            foreach (var group in AppCache.Groups)
            {
                int count = AppCache.GroupMembers.Count(m => m.GroupId == group.Id);
                group.StatOneValue = count.ToString();
                group.StatOneLabel = "عضو";
            }
        }
        
        private static void UpdateAzureIP()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-ExecutionPolicy Bypass -WindowStyle Hidden -File \"C:\\Scripts\\update-azure-firewall.ps1\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using (var process = Process.Start(psi))
                {
                    process.WaitForExit(30000);
                }

            }
            catch { /* تجاهل لو فشل */ }

        }


    }
}