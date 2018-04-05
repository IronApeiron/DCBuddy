using MahApps.Metro.Controls;
using Marketplace;
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
    /// Interaction logic for EditItemEntry.xaml
    /// </summary>
    public partial class EditItemEntry : MetroWindow
    {
        MainWindow mainWindow;

        string oldTimeStamp;

        string oldItem;

        string oldDesc;

        string oldCurrency;

        double oldPrice;

        int oldQuantity;

        public EditItemEntry(MainWindow mainWindow, string oldTimeStamp, string oldItem, string oldDesc, string oldCurrency, double oldPrice, int oldQuantity)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;

            this.oldTimeStamp = oldTimeStamp;

            this.oldItem = oldItem;
            this.txtItem.Text = oldItem;

            this.oldDesc = oldDesc;
            this.txtDesc.Text = oldDesc;

            this.oldCurrency = oldCurrency;
            this.cmbCurrency.Text = oldCurrency;

            this.oldPrice = oldPrice;
            this.txtPrice.Text = oldPrice.ToString();

            this.oldQuantity = oldQuantity;
            this.txtQuantity.Text = oldQuantity.ToString();

        }

        private void btnUpdateEntry_Click(object sender, RoutedEventArgs e)
        {
            try {
                if (!MarketFeed.ItemAuctionHouse.UpdateEntry(oldTimeStamp, oldItem, oldDesc, oldCurrency, oldPrice, oldQuantity, txtItem.Text, txtDesc.Text, cmbCurrency.Text, Double.Parse(txtPrice.Text), Int32.Parse(txtQuantity.Text)))
                {
                    MessageBox.Show("There was a problem updating the entry.", "DCPal");
                }
                else
                {
                    MessageBox.Show("Successfully updated the entry.", "DCPal");
                }
                mainWindow.itemDataGrid.ItemsSource = MarketFeed.ItemAuctionHouse.GetTableView();
                Close();
            } catch (Exception ex)
            {
                MessageBox.Show("Cannot update entry. Please check your input fields and try again.", "DCPal");
            }
        }
    }
}
