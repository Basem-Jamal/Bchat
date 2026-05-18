using BChat.Auth;
using BChat.Data.DataStore;
using BChat.Data.DataStore.CustomerProfile_Repository;
using BChat.Data.DataStore.Customers_Repository;
using BChat.Data.DataStore.Users_DB;
using BChat.Global;
using BChat.Models.Users;
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
            //Customers
            AppCache.Customers = CustomerRepository.GetAll();
            //---------       
            ////صار ثقيل واضطريت استخدم getById
            //AppCache.CustomerProfiles = CustomerProfileRepository.GetAll();
            ////---------


            //Groups
            AppCache.Groups = GroupRepository.GetAll();
            //---------
            //GroupMembers
            AppCache.GroupMembers = GroupMemberRepository.GetAll();
            //---------
            //ChatMessages
            AppCache.ChatMessages = ChatMessageRepository.GetAll(); // ← هذا فقط
            //---------
            //Templates
            AppCache.WhatsAppTemplates = TemplateRepository.GetAll();
            //---------

           
            CalculateMembersCount();

            AppCache.WhatsAppListener = new BChat.WhatsApp.WhatsAppWebhookListener();

            while (true)
            {
                //using (var login = new Login())
                //{
                //    if (login.ShowDialog() != DialogResult.OK)
                //        break;
                //}

                string email = "BASEM";
                string password = "123";

                User? user = UsersRepository.Login(email, password);
                if (user == null)
                {
                    MessageBox.Show("البريد أو كلمة المرور غير صحيحة", "BChat",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                AppCache.CurrentUser = user;

                var home = new Home();
                home.Shown += (s, e) => AppCache.WhatsAppListener.Start(); // ← ابدأ بعد تحميل الـ UI
                Application.Run(home);
                AppCache.WhatsAppListener.Stop();

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