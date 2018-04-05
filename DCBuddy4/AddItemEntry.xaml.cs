using MahApps.Metro.Controls;
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
using Marketplace;

namespace DCPal
{
    /// <summary>
    /// Interaction logic for AddItemEntry.xaml
    /// </summary>
    public partial class AddItemEntry : MetroWindow
    {
        /// <summary>
        /// The main window instance.
        /// </summary>
        MainWindow mainWindow;

        public AddItemEntry(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void btnAddEntry_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if(!MarketFeed.ItemAuctionHouse.AddEntry(txtItem.Text, txtDesc.Text, cmbCurrency.Text, Double.Parse(txtPrice.Text), Int32.Parse(txtQuantity.Text)))
                {
                    // Entry failed
                    MessageBox.Show("Cannot add entry. You have reached your posting limit.", "DCPal");
                }
                else
                {
                    MessageBox.Show("Successfully added your entry.", "DCPal");
                    mainWindow.itemDataGrid.ItemsSource = MarketFeed.ItemAuctionHouse.GetTableView();
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot add entry. Please check your input fields and try again.", "DCPal");
            }
        }
    }
}
