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
    /// Interaction logic for EditCCEntry.xaml
    /// </summary>
    public partial class EditCCEntry : MetroWindow
    {
        string oldTimeStamp { get; set; }

        double oldRate { get; set; }

        string oldDesc { get; set; }

        int oldQuantity { get; set; }

        MainWindow mainWindow { get; set; }

        public EditCCEntry(MainWindow mainWindow, string oldTimeStamp, double oldRate, string oldDesc, int oldQuantity)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;

            this.oldTimeStamp = oldTimeStamp;

            this.oldRate = oldRate;
            this.txtRate.Text = oldRate.ToString();

            this.oldDesc = oldDesc;
            this.txtDesc.Text = oldDesc;

            this.oldQuantity = oldQuantity;
            this.txtQuantity.Text = oldQuantity.ToString();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try {
                if (!MarketFeed.CCAuctionHouse.UpdateEntry(oldTimeStamp, oldRate, oldDesc, oldQuantity, Double.Parse(txtRate.Text), txtDesc.Text, Int32.Parse(txtQuantity.Text)))
                {
                    MessageBox.Show("There was a problem updating this entry.", "DCPal");
                }
                else {
                    MessageBox.Show("Successfully updated the entry.", "DCPal");
                }
                mainWindow.ccDataGrid.ItemsSource = MarketFeed.CCAuctionHouse.GetTableView();
                Close();
            } catch(Exception ex)
            {
                MessageBox.Show("Cannot update entry. Please check your input fields and try again.", "DCPal");
            }
        }
    }
}
