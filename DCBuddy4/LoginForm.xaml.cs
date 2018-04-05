using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using Authenticator;
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;

namespace DCPal
{
    /// <summary>
    /// Interaction logic for LoginForm.xaml
    /// </summary>
    public partial class LoginForm : MetroWindow
    {
        public LoginForm()
        {

            InitializeComponent();
            txtUserName.Focus();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }


        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private async void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            // Activate the loading bar
            logInBusyIndicator.IsBusy = true;

            try {
                // Try to verify the user

                // UPDATE: Commented out as this is an overkill for an app like this :)
                // User u = LoginManager.Authenticate(txtUserName.Text, txtPassword.Password);

                // But... let's create a User anyways
                User u = new User("User", "someEpicPw", 0, "Online");

                // Authenticated, so monitor this user
                LoginManager.Monitor(u);

                // Move on to the MainWindow
                MainWindow m = new MainWindow();
                m.Show();

                Close();
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("Log In", ex.Message);
            }
            finally
            {
                // Close the indicator
                logInBusyIndicator.IsBusy = false;
            }

        }
    }
}
