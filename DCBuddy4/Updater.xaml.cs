using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DCPal
{
    /// <summary>
    /// Interaction logic for Updater.xaml
    /// No longer used, as it now uses ClickOnce deployment.
    /// </summary>
    public partial class Updater : MetroWindow
    {
        public Updater()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Focus();
            Show();
            CheckForUpdates();
        }

        private void CheckForUpdates()
        {
            WebClient client = new WebClient();

            Stream stream = client.OpenRead("txt_containing_url_to_client_here.com");
            StreamReader reader = new StreamReader(stream);

            String infoLine = reader.ReadLine();
            String[] splitedLine = infoLine.Split('|');

            String version = splitedLine[0];
            String dUrl = splitedLine[1];

            Version dVersion = Version.Parse(version);

            if (Assembly.GetExecutingAssembly().GetName().Version < dVersion) {
                client.DownloadFile(dUrl, "DCBuddy.tmp.zip");
                ZipFile.ExtractToDirectory("DCBuddy.tmp.zip", Environment.CurrentDirectory);
                File.SetAttributes(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Replace(".exe", "") + "-u.exe", FileAttributes.Normal);
                System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Replace(".exe", "-u.exe")); // to start new instance of application
                this.Close();
            }
            else
            {
                File.SetAttributes(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Replace("-u.exe", ".exe").ToString(), FileAttributes.Normal);
                try {
                    File.Delete(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Replace("-u.exe", ".exe").ToString());
                }catch(Exception e)
                {
                    // fix later
                }
                new LoginForm().Show();
                this.Close();
            }

        }

    }
}
