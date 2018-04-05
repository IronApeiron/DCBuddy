using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
using System.Windows.Shapes;

namespace DCPal
{
    /// <summary>
    /// Interaction logic for AddCCEntry.xaml
    /// </summary>
    public partial class AddCCEntry : MetroWindow
    {
        /// <summary>
        /// The main window instance.
        /// </summary>
        MainWindow mainWindow;

        public AddCCEntry(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
           
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Marketplace.MarketFeed.CCAuctionHouse.AddEntry(Double.Parse(txtRate.Text), txtDesc.Text, Int32.Parse(txtQuantity.Text)))
                {
                    MessageBox.Show("Cannot add entry. Please check your input fields and try again.", "DCPal");
                }
                else
                {
                    MessageBox.Show("Successfully added your entry.", "DCPal");
                    mainWindow.ccDataGrid.ItemsSource = Marketplace.MarketFeed.CCAuctionHouse.GetTableView();
                }
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Cannot add entry. Please check your input fields and try again.", "DCPal");
            }
        }
    }
}
