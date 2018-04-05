using Authenticator;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
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

namespace DCPal
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : MetroWindow
    {
        public About()
        {
            InitializeComponent();

            try
            {
                lblVersion.Content = "Version: " + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            catch (Exception e)
            {
                lblVersion.Content = "Version: 0.1.1.0";
            }

            lblLoggedInAs.Content = "Logged in as " + LoginManager.LocalUser.LoginID;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
