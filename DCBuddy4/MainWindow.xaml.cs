using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using NewsPlugin;
using System.Diagnostics;
using Authenticator;
using Npgsql;
using System.Configuration;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using MahApps.Metro;
using System.Data;
using System.ComponentModel;
using Microsoft.Win32;
using System.IO;
using System.Deployment.Application;
using System.Drawing;
using System.Reflection;

namespace DCPal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// This is where the main controls are hosted after the LoginForm.
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        /** Dictonaries mapping docking part letter grades to quantitative points **/
        Dictionary<string, int> superDockBases = new Dictionary<string, int>();
        Dictionary<string, int> hyperDockBases = new Dictionary<string, int>();
        Dictionary<string, int> ultraDockBases = new Dictionary<string, int>();


        public MainWindow()
        {
            if (LoginManager.LocalUser == null)
            {
                return;
            }
            else
            {
                PleaseWaitForm pl = new PleaseWaitForm();
                pl.Show();
                ForceUIToUpdate();

                InitializeComponent();
    
                try {
                    lblVersion.Content = "Version: " + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                }catch(Exception e)
                {
                    lblVersion.Content = "Version: 0.1.1.0";
                }

                WindowStartupLocation = WindowStartupLocation.CenterScreen;

                // Set timer
                DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Tick += dispatcherTimer_Tick;
                dispatcherTimer.Interval = new TimeSpan(0, 30, 0);
                dispatcherTimer.Start();

                // Set another timer
                DispatcherTimer dispatcherTimer2 = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer2.Tick += dispatcherTimer2_Tick;
                dispatcherTimer2.Interval = new TimeSpan(0, 5, 0);
                dispatcherTimer2.Start();

                // Set the user ID in the app
                //txtbUserName.Text = LoginManager.LocalUser.LoginID;
                txtbUserName.Text = "OfflineUser";

                // Drop shadow effect on text
                DropShadowEffect effect = new DropShadowEffect();

                // Set the color of the user ID if applicable
                if (LoginManager.LocalUser.IsAdmin())
                {
                    txtbUserName.Foreground = new SolidColorBrush(Colors.LimeGreen);
                    effect.Color = Colors.White;

                }
                else if (LoginManager.LocalUser.IsDirector())
                {
                    txtbUserName.Foreground = new SolidColorBrush(Colors.Red);
                    effect.Color = Colors.White;
                }
                else if (LoginManager.LocalUser.IsVIP())
                {
                    txtbUserName.Foreground = new SolidColorBrush(Colors.SkyBlue);
                    effect.Color = Colors.White;
                }

                // Set the direction of where the shadow is cast to 320 degrees.
                effect.Direction = 320;

                // Set the depth of the shadow being cast.
                effect.ShadowDepth = 1;

                // Set the shadow opacity to half opaque or in other words - half transparent.
                // The range is 0-1.
                effect.Opacity = 0.4;

                txtbUserName.Effect = effect;

                // Add listeners
                newsFeedList.MouseDoubleClick += new MouseButtonEventHandler(newsFeedList_MouseDoubleClick);

                // Docking stuff: To be moved to docking solution later
                superDockBases.Add("A", 5);
                superDockBases.Add("B", 10);
                superDockBases.Add("C", 15);
                superDockBases.Add("D", 20);
                superDockBases.Add("E", 25);
                superDockBases.Add("F", 30);
                superDockBases.Add("G", 34);
                superDockBases.Add("H", 39);
                superDockBases.Add("I", 44);
                superDockBases.Add("J", 49);

                hyperDockBases.Add("A", 3);
                hyperDockBases.Add("B", 6);
                hyperDockBases.Add("C", 9);
                hyperDockBases.Add("D", 13);
                hyperDockBases.Add("E", 16);
                hyperDockBases.Add("F", 19);
                hyperDockBases.Add("G", 22);
                hyperDockBases.Add("H", 25);
                hyperDockBases.Add("I", 28);
                hyperDockBases.Add("J", 31);
                hyperDockBases.Add("K", 35);
                hyperDockBases.Add("L", 38);
                hyperDockBases.Add("M", 40);
                hyperDockBases.Add("N", 41);
                hyperDockBases.Add("O", 44);

          
                ultraDockBases.Add("A", 5);
                ultraDockBases.Add("B", 11);
                ultraDockBases.Add("C", 17);
                ultraDockBases.Add("D", 23);
                ultraDockBases.Add("E", 29);
                ultraDockBases.Add("F", 35);
                ultraDockBases.Add("G", 41);
                ultraDockBases.Add("H", 47);
                ultraDockBases.Add("I", 53);
                ultraDockBases.Add("J", 59);
                ultraDockBases.Add("K", 65);
                ultraDockBases.Add("L", 69);
                ultraDockBases.Add("M", 74);
                ultraDockBases.Add("N", 76);
                ultraDockBases.Add("O", 82);

                chnlDatePicker.SelectedDate = FullChannelBattleFeed.PreviousSaturday(DateTime.Now);

                string appFolderPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                pl.Close();
            }
            
        }

        public static void ForceUIToUpdate()
        {
            DispatcherFrame frame = new DispatcherFrame();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);

            Dispatcher.PushFrame(frame);
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => { })).Wait();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            try {
                //NpgsqlConnection conn = LoginManager.LocalUser.Verify();
                //conn.Close();
            }catch(Exception ex)
            {
                this.IsEnabled = false;
                MessageBox.Show(ex.Message, "DCPal: Error");
                Application.Current.Shutdown();
            }

            // Check for updates
            UpdateCheckInfo info = null;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

                try
                {
                    info = ad.CheckForDetailedUpdate();

                }
                catch (DeploymentDownloadException dde)
                {
                    MessageBox.Show("The new version of DCPal cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " + dde.Message);
                    return;
                }
                catch (InvalidDeploymentException ide)
                {
                    MessageBox.Show("Cannot check for a new version of DCPal. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " + ide.Message);
                    return;
                }
                catch (InvalidOperationException ioe)
                {
                    MessageBox.Show("DCPal cannot be updated. It is likely not a ClickOnce application. Error: " + ioe.Message);
                    return;
                }

                if (info.UpdateAvailable)
                {
                    Boolean doUpdate = true;

                    // Display a message that the app MUST reboot. Display the minimum required version.
                    MessageBox.Show("DCPal has detected a mandatory update from your current " +
                        "version to version " + info.MinimumRequiredVersion.ToString() +
                        ". The application will now install the update and restart.",
                        "DCPal - Update Available");

                    if (doUpdate)
                    {
                        try
                        {
                            ad.Update();

                            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                            Application.Current.Shutdown();
                        }
                        catch (DeploymentDownloadException dde)
                        {
                            MessageBox.Show("Cannot install the latest version DCPal. \n\nPlease check your network connection, or try again later. Error: " + dde);
                            return;
                        }
                    }
                }
            }
        }

        private void dispatcherTimer2_Tick(object sender, EventArgs e)
        {
            PleaseWaitForm pl = new PleaseWaitForm();
            pl.Show();
            ForceUIToUpdate();

            // Update CC Auction House
            ccDataGrid.ItemsSource = Marketplace.MarketFeed.CCAuctionHouse.GetTableView();

            // Update Mito Auction House
            itemDataGrid.ItemsSource = Marketplace.MarketFeed.ItemAuctionHouse.GetTableView();
            pl.Close();
           
        }

        private void ColorRows()
        {
            

        }


        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listBox_Loaded(object sender, RoutedEventArgs e)
        {
          
        }


        private void newsFeedList_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            // Attempt to open up a webpage with some link bounded in the affected listbox
            if(sender is ListBox)
            {
                var lBox = sender as ListBox;

                if(lBox.SelectedItem != null)
                {
                    // Get the associated news entry.
                    NewsEntry newsEntry = ((NewsEntry) lBox.SelectedValue);
                    Debug.WriteLine(newsEntry.EntryURL);
                    
                    // Open up the link in the default webbrowser.
                    Process.Start(newsEntry.EntryURL);
                }
            }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ccDataGrid_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {


        }

        private void ccDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            PleaseWaitForm pl = new PleaseWaitForm();
            pl.Show();
            ForceUIToUpdate();
            ccDataGrid.ItemsSource = Marketplace.MarketFeed.CCAuctionHouse.GetTableView();
            pl.Close();
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            // Sign out
            NpgsqlConnection conn = LoginManager.LocalUser.Verify();

            // Delete the entry from the DCPal_OnlineUsers table since you're no longer online
            NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM DCPal_OnlineUsers WHERE uid = '" + LoginManager.LocalUser.LoginID + "';", conn);
            cmd.ExecuteNonQuery();

            // Close the connection
            conn.Close();

            Application.Current.Shutdown();
        }

        private void btnAddCCEntry_Click(object sender, RoutedEventArgs e)
        {
            // Initialize add cc entry window
            AddCCEntry addCCEntryWindow = new AddCCEntry(this);
            addCCEntryWindow.Show();

        }

        private void btnDeleteCCEntry_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected row
            try
            {
                DataRowView row = (DataRowView)ccDataGrid.SelectedItems[0];

                // Ask the user to confirm deletion
                var confirmResult = MessageBox.Show("Are you sure you want to delete this entry?",
                                         "Confirm Delete",
                                         MessageBoxButton.YesNo);

                if (confirmResult == MessageBoxResult.Yes)
                {
                    // Delete the row
                    if (!Marketplace.MarketFeed.CCAuctionHouse.DeleteRow(row["Username"].ToString()))
                    {
                        // Error!
                        MessageBox.Show("Could not delete entry. Please try again.", "DCPal");
                    }

                    // Refresh the table
                    ccDataGrid.ItemsSource = Marketplace.MarketFeed.CCAuctionHouse.GetTableView();
                }

            }
            catch (Exception exx)
            {
                MessageBox.Show("Please select a valid entry.", "DCPal");
                return;
            }


        }


        private void marketplace_Clicked(object sender, MouseButtonEventArgs e)
        {

        }

        private void itemDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
        
            itemDataGrid.ItemsSource = Marketplace.MarketFeed.ItemAuctionHouse.GetTableView();
     
        }

        private void btnAddItemEntry_Click(object sender, RoutedEventArgs e)
        {
            // Initialize add cc entry window
            AddItemEntry addItemEntry = new AddItemEntry(this);
            addItemEntry.Show();
        }

        private void btnDeleteItemEntry_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected row
            try {
                DataRowView row = (DataRowView)itemDataGrid.SelectedItems[0];

                // Ask the user to confirm deletion
                var confirmResult = MessageBox.Show("Are you sure you want to delete this entry?",
                                         "Confirm Delete",
                                         MessageBoxButton.YesNo);

                if (confirmResult == MessageBoxResult.Yes)
                {
                    // Delete the row
                    if (!Marketplace.MarketFeed.ItemAuctionHouse.DeleteRow(row["Time Stamp"].ToString(), row["Username"].ToString()))
                    {
                        // Error!
                        MessageBox.Show("Could not delete entry. Check your permissions and try again.", "DCPal");
                        // Refresh the table
                        itemDataGrid.ItemsSource = Marketplace.MarketFeed.ItemAuctionHouse.GetTableView();
                        return;
                    }

                    // Error!
                    MessageBox.Show("Delete was successful.", "DCPal");

                    // Refresh the table
                    itemDataGrid.ItemsSource = Marketplace.MarketFeed.ItemAuctionHouse.GetTableView();
                }

            }
            catch(Exception exx)
            {
                MessageBox.Show("Please select a valid entry.", "DCPal");
                return;
            }

        }

        private void btnSaveNotes_Click(object sender, RoutedEventArgs e)
        {
            string fileText = txtNote.Text;

            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Text Files(*.txt)|*.txt|All(*.*)|*"
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, fileText);
            }
        }

        private void btnOpenNotes_Click(object sender, RoutedEventArgs e)
        {
            string fileText = txtNote.Text;

            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Text Files(*.txt)|*.txt|All(*.*)|*"
            };

            if (dialog.ShowDialog() == true)
            {
                txtNote.Text = File.ReadAllText(dialog.FileName);
            }
        }

        private void btnNewNotes_Click(object sender, RoutedEventArgs e)
        {
            if(txtNote.Text == "")
            {
                // Don't need to do anything
            }
            else
            {
                // Ask the user to save before making a new note
                var confirmResult = MessageBox.Show("Do you want to save the current note before making another one?",
                                         "DCPal",
                                         MessageBoxButton.YesNo);

                if (confirmResult == MessageBoxResult.Yes)
                {
                    // Save the note
                    string fileText = txtNote.Text;

                    OpenFileDialog dialog = new OpenFileDialog()
                    {
                        Filter = "Text Files(*.txt)|*.txt|All(*.*)|*"
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        txtNote.Text = File.ReadAllText(dialog.FileName);
                    }
                    else
                    {
                        // Return. User isn't sure if they should proceed
                        return;
                    }
                }
                else
                {
                    // Clear the note
                    txtNote.Text = "";
                }
            }
        }

        private void chnlDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FullChannelBattleFeed.date = chnlDatePicker.SelectedDate.Value.ToString("MM/dd/yyyy");
            SemiChannelBattleFeed.date = chnlDatePicker.SelectedDate.Value.ToString("MM/dd/yyyy");

            fullChannelBattleList.ItemsSource = new FullChannelBattleFeed();
            semiChannelList.ItemsSource = new SemiChannelBattleFeed();

//            (fullChannelBattleList.ItemsSource as FullChannelBattleFeed).ViewingDate = chnlDatePicker.SelectedDate.Value.ToString("MM/dd/yyyy");
  //          (semiChannelList.ItemsSource as SemiChannelBattleFeed).ViewingDate = chnlDatePicker.SelectedDate.Value.ToString("MM/dd/yyyy");

            fullChannelBattleList.Items.Refresh();
            semiChannelList.Items.Refresh();
        }

        private void btnEditCCEntry_Click(object sender, RoutedEventArgs e)
        {

              if (ccDataGrid.SelectedItem != null)
              {
                // Get the associated news entry.
                //string ID = ((DataRowView)dataGrid1.SelectedItem).Row["ID"]
                DataRowView dr = ((DataRowView)ccDataGrid.SelectedItem);
                if (Marketplace.MarketFeed.CCAuctionHouse.LocalUserOwnsEntry(dr.Row["Time Stamp"].ToString(), Convert.ToDouble(dr.Row["Rate (Millions/CC)"]),
                dr.Row["Description"].ToString(), Convert.ToInt32(dr.Row["Quantity"])))
                {
                    EditCCEntry edit = new EditCCEntry(this, dr.Row["Time Stamp"].ToString(), Convert.ToDouble(dr.Row["Rate (Millions/CC)"]),
                        dr.Row["Description"].ToString(), Convert.ToInt32(dr.Row["Quantity"]));

                    edit.Show();
                }
                else
                {
                    MessageBox.Show("You cannot edit this entry. Check your permissions and try again.", "DCPal");
                }
            }

        }

        private void btnEditItemEntry_Click(object sender, RoutedEventArgs e)
        {

            if (itemDataGrid.SelectedItem != null)
            {
                // Get the associated news entry.
                //string ID = ((DataRowView)dataGrid1.SelectedItem).Row["ID"]
                DataRowView dr = ((DataRowView)itemDataGrid.SelectedItem);
                if (Marketplace.MarketFeed.ItemAuctionHouse.LocalUserOwnsEntry(dr.Row["Time Stamp"].ToString(), dr.Row["Item"].ToString(), dr.Row["Details"].ToString(),
                    dr.Row["Currency"].ToString(), Convert.ToDouble(dr.Row["Price per Item"]), Convert.ToInt32(dr.Row["Quantity"])))
                {
                    EditItemEntry edit = new EditItemEntry(this, dr.Row["Time Stamp"].ToString(), dr.Row["Item"].ToString(), dr.Row["Details"].ToString(),
                    dr.Row["Currency"].ToString(), Convert.ToDouble(dr.Row["Price per Item"]), Convert.ToInt32(dr.Row["Quantity"]));

                    edit.Show();
                }
                else
                {
                    MessageBox.Show("You cannot edit this entry. Check your permissions and try again.", "DCPal");
                }
            }
        }

        // Docking stuff
        // To be moved into its own solution

        private void btnCalculateDockPoints1_Click(object sender, RoutedEventArgs e)
        {
            int basePoints = 0;

            try { 

                switch (cmbDockGrade1.SelectedValue.ToString())
                {
                    case "Super":
                        basePoints = superDockBases[cmbDockLetter1.SelectedItem.ToString()];
                        txtblTotalPoints1.Foreground = new SolidColorBrush(Colors.SkyBlue);
                        break;

                    case "Hyper":
                        basePoints = hyperDockBases[cmbDockLetter1.SelectedItem.ToString()];
                        txtblTotalPoints1.Foreground = new SolidColorBrush(Colors.OrangeRed);
                        break;

                    case "Ultra":
                        basePoints = ultraDockBases[cmbDockLetter1.SelectedItem.ToString()];
                        txtblTotalPoints1.Foreground = new SolidColorBrush(Colors.MediumPurple);
                        break;

                    default:
                        MessageBox.Show("Please select a dock grade.", "DCPal");
                        return;
                }

                int totalPoints = basePoints + Int32.Parse(txtInputDockPoints1.Text);
                txtblTotalPoints1.Text = totalPoints.ToString();

            }catch(Exception ex)
            {
                MessageBox.Show("Error in input, please check your input fields and try again.", "DCPal");
            }
        }

        private void btnCalculateDockPoints2_Click(object sender, RoutedEventArgs e)
        {
            int basePoints = 0;

            try
            {
                switch (cmbDockGrade2.SelectedValue.ToString())
            {
                case "Super":
                    basePoints = superDockBases[cmbDockLetter2.SelectedItem.ToString()];
                    txtblTotalPoints2.Foreground = new SolidColorBrush(Colors.SkyBlue);
                    break;

                case "Hyper":
                    basePoints = hyperDockBases[cmbDockLetter2.SelectedItem.ToString()];
                        txtblTotalPoints2.Foreground = new SolidColorBrush(Colors.OrangeRed);
                        break;

                case "Ultra":
                    basePoints = ultraDockBases[cmbDockLetter2.SelectedItem.ToString()];
                        txtblTotalPoints2.Foreground = new SolidColorBrush(Colors.MediumPurple);
                        break;

                default:
                    MessageBox.Show("Please select a dock grade.", "DCPal");
                    return;
            }

                int totalPoints = basePoints + Int32.Parse(txtInputDockPoints2.Text);
                txtblTotalPoints2.Text = totalPoints.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in input, please check your input fields and try again.", "DCPal");
            }
        }

        private void btnCalculateDockPoints3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int basePoints = 0;

            switch (cmbDockGrade3.SelectedValue.ToString())
            {
                case "Super":
                    basePoints = superDockBases[cmbDockLetter3.SelectedItem.ToString()];
                    txtblTotalPoints3.Foreground = new SolidColorBrush(Colors.SkyBlue);
                    break;

                case "Hyper":
                    basePoints = hyperDockBases[cmbDockLetter3.SelectedItem.ToString()];
                    txtblTotalPoints3.Foreground = new SolidColorBrush(Colors.OrangeRed);
                        break;

                case "Ultra":
                    basePoints = ultraDockBases[cmbDockLetter3.SelectedItem.ToString()];
                        txtblTotalPoints3.Foreground = new SolidColorBrush(Colors.MediumPurple);
                        break;

                default:
                    MessageBox.Show("Please select a dock grade.", "DCPal");
                    return;
            }
                int totalPoints = basePoints + Int32.Parse(txtInputDockPoints3.Text);
                txtblTotalPoints3.Text = totalPoints.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in input, please check your input fields and try again.", "DCPal");
            }
        }

        private void btnCalculateDockPoints4_Click(object sender, RoutedEventArgs e)
        {
            int basePoints = 0;

            try
            {
                switch (cmbDockGrade4.SelectedValue.ToString())
            {
                case "Super":
                    basePoints = superDockBases[cmbDockLetter4.SelectedItem.ToString()];
                        txtblTotalPoints4.Foreground = new SolidColorBrush(Colors.SkyBlue);
                        break;

                case "Hyper":
                    basePoints = hyperDockBases[cmbDockLetter4.SelectedItem.ToString()];
                        txtblTotalPoints4.Foreground = new SolidColorBrush(Colors.OrangeRed);
                        break;

                case "Ultra":
                        txtblTotalPoints4.Foreground = new SolidColorBrush(Colors.MediumPurple);
                        basePoints = ultraDockBases[cmbDockLetter4.SelectedItem.ToString()];
                    break;

                default:
                    MessageBox.Show("Please select a dock grade.", "DCPal");
                    return;
            }

                int totalPoints = basePoints + Int32.Parse(txtInputDockPoints4.Text);
                txtblTotalPoints4.Text = totalPoints.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in input, please check your input fields and try again.", "DCPal");
            }
        }

        private void cmbDockGrade1_Selected(object sender, SelectionChangedEventArgs e)
        {
            cmbDockLetter1.Items.Clear();
            switch (cmbDockGrade1.SelectedValue.ToString())
            {
                case "Super":

                    for (int i = 'A'; i <= 'J'; ++i)
                    {
                        cmbDockLetter1.Items.Add(Convert.ToChar(i).ToString());
                    }
                    break;

                case "Hyper":
                    for (int i = 'A'; i <= 'O'; ++i)
                    {
                        cmbDockLetter1.Items.Add(Convert.ToChar(i).ToString());
                    }
                    break;

                case "Ultra":
                    for (int i = 'A'; i <= 'O'; ++i)
                    {
                        cmbDockLetter1.Items.Add(Convert.ToChar(i).ToString());
                    }
                    break;
            }

            cmbDockLetter1.Items.Refresh();
        }

        private void cmbDockGrade2_Selected(object sender, SelectionChangedEventArgs e)
        {
            cmbDockLetter2.Items.Clear();
            switch (cmbDockGrade2.SelectedValue.ToString())
            {
                case "Super":
                    for (int i = 'A'; i <= 'J'; ++i)
                    {
                        cmbDockLetter2.Items.Add(Convert.ToChar(i).ToString());
                    }
                    break;

                case "Hyper":
                    for (int i = 'A'; i <= 'O'; ++i)
                    {
                        cmbDockLetter2.Items.Add(Convert.ToChar(i).ToString());
                    }
                    break;

                case "Ultra":
                    for (int i = 'A'; i <= 'O'; ++i)
                    {
                        cmbDockLetter2.Items.Add(Convert.ToChar(i).ToString());
                    }
                    break;
            }

            cmbDockLetter2.Items.Refresh();
        }

        private void cmbDockGrade3_Selected(object sender, SelectionChangedEventArgs e)
        {
            cmbDockLetter3.Items.Clear();
            switch (cmbDockGrade3.SelectedValue.ToString())
            {
                case "Super":
                    for (int i = 'A'; i <= 'J'; ++i)
                    {
                        cmbDockLetter3.Items.Add(Convert.ToChar(i).ToString());
                    }
                    break;

                case "Hyper":
                    for (int i = 'A'; i <= 'O'; ++i)
                    {
                        cmbDockLetter3.Items.Add(Convert.ToChar(i).ToString());
                    }
                    break;

                case "Ultra":
                    for (int i = 'A'; i <= 'O'; ++i)
                    {
                        cmbDockLetter3.Items.Add(Convert.ToChar(i).ToString());
                    }
                    break;
            }

            cmbDockLetter3.Items.Refresh();
        }

        private void cmbDockGrade4_Selected(object sender, SelectionChangedEventArgs e)
        {
            cmbDockLetter4.Items.Clear();
            switch (cmbDockGrade4.SelectedValue.ToString())
            {
                case "Super":
                    for (int i = 'A'; i <= 'J'; ++i)
                    {
                        cmbDockLetter4.Items.Add(Convert.ToChar(i).ToString());
                    }
                    break;

                case "Hyper":
                    for (int i = 'A'; i <= 'O'; ++i)
                    {
                        cmbDockLetter4.Items.Add(Convert.ToChar(i).ToString());
                    }
                    break;

                case "Ultra":
                    for (int i = 'A'; i <= 'O'; ++i)
                    {
                        cmbDockLetter4.Items.Add(Convert.ToChar(i).ToString());
                    }
                    break;
            }

            cmbDockLetter4.Items.Refresh();
        }

        private void About_Menu_Clicked(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.Show();
        }

        private void Close_Menu_Clicked(object sender, RoutedEventArgs e)
        {
            // Ask the user to confirm deletion
            var confirmResult = MessageBox.Show("Are you sure you exit DCPal?",
                                     "DCPal",
                                     MessageBoxButton.YesNo);

            if (confirmResult == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();

            }
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void itemDataGrid_Loaded_1(object sender, RoutedEventArgs e)
        {
            PleaseWaitForm pl = new PleaseWaitForm();
            pl.Show();
            ForceUIToUpdate();
            itemDataGrid.ItemsSource = Marketplace.MarketFeed.ItemAuctionHouse.GetTableView();
            pl.Close();
        }
    }
}
