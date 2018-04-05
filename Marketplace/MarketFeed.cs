using Authenticator;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace
{
    public class MarketFeed
    {
        public class CCAuctionHouse
        {

            static NpgsqlConnection conn { set; get; }


            /// <summary>
            /// Adds an entry to the CC database table.
            /// </summary>
            /// <param name="rate">The rate of which the CC is being sold for.</param>
            /// <param name="desc">The description of the sale.</param>
            /// <param name="quantity">The quantity of the item</param>
            /// <returns>True, if the appending is successful, false otherwise.</returns>
            public static bool AddEntry(double rate, string desc, int quantity)
            {
              
                // Refresh the connection
                conn = LoginManager.LocalUser.Verify();

                // Get the current date
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Get the number of entries that the user has
                NpgsqlCommand cmd = new NpgsqlCommand("select count(*) from DCPal_AuctionEntries WHERE uid = " + "@LoginID" +" GROUP BY uid", conn);
                cmd.Parameters.AddWithValue("@LoginID", LoginManager.LocalUser.LoginID);
                int count = Convert.ToInt32(cmd.ExecuteScalar());


                if ((count == 0 || LoginManager.LocalUser.IsAdmin() || LoginManager.LocalUser.IsDirector()) && quantity >= 1)
                {
                    // Add the entry
                    NpgsqlCommand cmdTwo = new NpgsqlCommand("insert into DCPal_AuctionEntries VALUES('" + currentDate.ToString() + "', " +
                    "@LoginID" +", " + "@rate" + ", " + "@desc" + ", " + "@quantity" + ");", conn);

                    cmdTwo.Parameters.AddWithValue("@LoginID", LoginManager.LocalUser.LoginID);
                    cmdTwo.Parameters.AddWithValue("@rate", rate);
                    cmdTwo.Parameters.AddWithValue("@desc", desc);
                    cmdTwo.Parameters.AddWithValue("@quantity", quantity);


                    cmdTwo.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }
                else
                {
                    // Too many entries! Warn user to delete old ones first.
                    conn.Close();
                    return false;
                }
         
                return true;                
            }

            /// <summary>
            /// Updates an entry to the CC database table.
            /// </summary>
            /// <param name="rate">The rate of which the CC is being sold for.</param>
            /// <param name="desc">The description of the sale.</param>
            /// <param name="quantity">The quantity of the item</param>
            /// <returns>True, if the appending is successful, false otherwise.</returns>
            public static bool UpdateEntry(string oldTimeStamp, double oldRate, string oldDesc, int oldQuantity, double rate, string desc, int quantity)
            {
                // Refresh the connection
                conn = LoginManager.LocalUser.Verify();

                // Get the current date
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Add the entry
                Debug.WriteLine("UPDATE DCPal_AuctionEntries SET timestamp ='" + currentDate.ToString() + "', uid = " +
               "@LoginID" +", rate = " + "@rate" + ", description = " + "@desc" + ", quantity = " + "@quantity" + " WHERE timestamp = '" + oldTimeStamp + "' AND uid = " +
                "@LoginID" +" AND rate = " + "@oldRate" +" AND description = " + "@oldDesc" + " AND quantity = " + "@oldQuantity" +";");

                NpgsqlCommand cmdTwo = new NpgsqlCommand("UPDATE DCPal_AuctionEntries SET timestamp = '" + currentDate.ToString() + "', uid = " +
               "@LoginID" +", rate = " + "@rate" + ", description = " + "@desc" + ", quantity = " + "@quantity" + " WHERE timestamp = '" + oldTimeStamp + "' AND uid = " +
                "@LoginID" +" AND rate = " + "@oldRate" +" AND description = " + "@oldDesc" + " AND quantity = " + "@oldQuantity" +";", conn);

                cmdTwo.Parameters.AddWithValue("@LoginID", LoginManager.LocalUser.LoginID);
                cmdTwo.Parameters.AddWithValue("@rate", rate);
                cmdTwo.Parameters.AddWithValue("@desc", desc);
                cmdTwo.Parameters.AddWithValue("@quantity", quantity);
                cmdTwo.Parameters.AddWithValue("@oldRate", oldRate);
                cmdTwo.Parameters.AddWithValue("@oldDesc", oldDesc);
                cmdTwo.Parameters.AddWithValue("@oldQuantity", oldQuantity);

                cmdTwo.ExecuteNonQuery();
               conn.Close();
               return true;

            }

            public static bool LocalUserOwnsEntry(string timeStamp, double rate, string desc, int quantity)
            {
                conn = LoginManager.LocalUser.Verify();

                Debug.WriteLine("SELECT count(*) FROM DCPal_AuctionEntries WHERE timestamp = '" + timeStamp + "' AND uid = " +
                "@LoginID" +" AND rate = " + "@rate" + " AND description = " + "@desc" + " AND quantity = " + "@quantity" + ";");

                // Add the entry
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT count(*) FROM DCPal_AuctionEntries WHERE timestamp = '" + timeStamp + "' AND uid = " +
                "@LoginID" +" AND rate = " + "@rate" + " AND description = " + "@desc" + " AND quantity = " + "@quantity" + ";", conn);

                cmd.Parameters.AddWithValue("@LoginID", LoginManager.LocalUser.LoginID);
                cmd.Parameters.AddWithValue("@rate", rate);
                cmd.Parameters.AddWithValue("@desc", desc);
                cmd.Parameters.AddWithValue("@quantity", quantity);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if(count >= 1)
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Returns a table to be used as an ItemsSource representing a CC Marketplace Table
            /// </summary>
            /// <returns></returns>
            public static DataView GetTableView()
            {
                // Refresh the connection
                conn = LoginManager.LocalUser.Verify();

                // To be changed
                NpgsqlCommand cmd = new NpgsqlCommand("select * from DCPal_AuctionEntries", conn);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);

                DataSet dt = new DataSet();

                // DB Columns
                da.Fill(dt);

                // Custom columns
                dt.Tables[0].Columns.Add("Verified", typeof(bool));

                OrderColumns(dt, "Verified", "rate", "quantity", "timestamp", "uid", "description");

                //"Verified", "rate", "timestamp", "uid", "description"
                //"Rate (Millions/CC)", "Time Stamp", "Username", "Description"

                dt.Tables[0].Columns["rate"].ColumnName = "Rate (Millions/CC)";
                dt.Tables[0].Columns["timestamp"].ColumnName = "Time Stamp";
                dt.Tables[0].Columns["uid"].ColumnName = "Username";
                dt.Tables[0].Columns["description"].ColumnName = "Description";
                dt.Tables[0].Columns["quantity"].ColumnName = "Quantity";
                dt.Tables[0].AcceptChanges();

                // Now fill in the custom columns
                for (int i = 0; i < dt.Tables[0].Rows.Count; ++i)
                {
                    conn.Close();
                    conn = LoginManager.LocalUser.Verify();
                    cmd = new NpgsqlCommand("select uid from DCPal_AHTrusted where uid = '" + dt.Tables[0].Rows[i]["Username"] + "'", conn);
                    NpgsqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        // User is in verified list
                        dt.Tables[0].Rows[i]["Verified"] = true;
                    }
                    else
                    {
                        // Not in verified list
                        dt.Tables[0].Rows[i]["Verified"] = false;
                    }
                }

                try
                {
                    conn.Close();
                }catch(Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

                dt.Tables[0].AcceptChanges();

                return dt.Tables[0].DefaultView;
            }
            //"Verified", "rate", "timestamp", "uid", "description"
            //"Rate (Millions/CC)", "Time Stamp", "Username", "Description"
            /// <summary>
            /// Deletes a row from the CC auction house. Warning: Assumes there can only be at most one entry per user.
            /// </summary>
            /// <param name="uid">The username of the user.</param>
            /// <returns>True if the operation was successful, false if otherwise</returns>
            public static bool DeleteRow(string uid)
            {
                if (uid == LoginManager.LocalUser.LoginID || LoginManager.LocalUser.IsAdmin())
                {
                    // Delete row
                    conn = LoginManager.LocalUser.Verify();

                    NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM DCPal_AuctionEntries WHERE uid = " + "@uid" + ";", conn);
                    cmd.Parameters.AddWithValue("@uid", uid);
                    cmd.ExecuteNonQuery();

                    conn.Close();
                    return true;
                }
                else
                {
                    // Permission error
                    return false;
                }
            }


            // Source: http://stackoverflow.com/questions/3757997/how-to-change-datatable-colums-order
            private static void OrderColumns(DataSet dataSet, params String[] columnNames)
            {
                int columnIndex = 0;
                foreach (var columnName in columnNames)
                {
                    dataSet.Tables[0].Columns[columnName].SetOrdinal(columnIndex);
                    columnIndex++;
                }
            }
        }

        public class ItemAuctionHouse
        {

            static NpgsqlConnection conn { set; get; }


            /// <summary>
            /// Adds an item entry into the table.
            /// </summary>
            /// <param name="item">The item.</param>
            /// <param name="desc">The description of the transaction.</param>
            /// <param name="currency">The currency used in the transaction.</param>
            /// <param name="price">The price of the item.</param>
            /// <param name="quantity">The quantity of the item</param>
            /// <returns></returns>
            public static bool AddEntry(string item, string desc, string currency, double price, int quantity)
            {
                // Refresh the connection
                conn = LoginManager.LocalUser.Verify();

                // Get the current date
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Get the number of entries that the user has
                NpgsqlCommand cmd = new NpgsqlCommand("select count(*) from DCPal_ItemAuctionEntries WHERE uid = " + "@LoginID" +" GROUP BY uid", conn);
                cmd.Parameters.AddWithValue("@LoginID", LoginManager.LocalUser.LoginID);
                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (quantity >= 1)
                {
                    // Ordinary members only get 1 entry
                    if (LoginManager.LocalUser.IsOrdinaryMember() || LoginManager.LocalUser.IsAdmin() || LoginManager.LocalUser.IsDirector())
                    {
                        // For now, members = limited to 5 entries
                        if (count < 5)
                        {
                            // Add the entry
                            NpgsqlCommand cmdTwo = new NpgsqlCommand("insert into DCPal_ItemAuctionEntries VALUES('" + currentDate.ToString() + "', " +
                            "@LoginID" +", " + "@item" +", " + "@currency" +", " + "@desc"  + ", " + "@price" +", " + "@quantity" + ");", conn);

                            cmdTwo.Parameters.AddWithValue("@LoginID", LoginManager.LocalUser.LoginID);
                            cmdTwo.Parameters.AddWithValue("@item", item);
                            cmdTwo.Parameters.AddWithValue("@desc", desc);
                            cmdTwo.Parameters.AddWithValue("@quantity", quantity);
                            cmdTwo.Parameters.AddWithValue("@currency", currency);
                            cmdTwo.Parameters.AddWithValue("@price", price);

                            Debug.WriteLine("insert into DCPal_ItemAuctionEntries VALUES(" + "@currentDate" + ", " +
                            "@LoginID" +", " + "@item" +", " + "@currency" +", " + "@desc" + ", " + "@price" +", " + "@quantity" + ");");
                            cmdTwo.ExecuteNonQuery();
                            conn.Close();
                            return true;
                        }
                        else
                        {
                            // Too many entries! Warn user to delete old ones first.
                            conn.Close();
                            return false;
                        }
                    }
                    // Admins = unlimited
                    else if (LoginManager.LocalUser.IsAdmin() || LoginManager.LocalUser.IsDirector())
                    {
                        // Add the entry
                        NpgsqlCommand cmdTwo = new NpgsqlCommand("insert into DCPal_ItemAuctionEntries VALUES(" + "@currentDate" + ", " +
                        "@LoginID" +", " + "@item" +", " + "@currency" +", " + "@desc" + ", " + "@price" +", " + "@quantity" + ");", conn);

                        cmdTwo.Parameters.AddWithValue("@LoginID", LoginManager.LocalUser.LoginID);
                        cmdTwo.Parameters.AddWithValue("@item", item);
                        cmdTwo.Parameters.AddWithValue("@desc", desc);
                        cmdTwo.Parameters.AddWithValue("@quantity", quantity);
                        cmdTwo.Parameters.AddWithValue("@currency", currency);
                        cmdTwo.Parameters.AddWithValue("@price", price);

                        cmdTwo.ExecuteNonQuery();
                        conn.Close();
                        return true;
                    }
                    // VIPs get 5
                    else if (LoginManager.LocalUser.IsVIP())
                    {
                        if (count < 5)
                        {
                            // Add the entry
                            NpgsqlCommand cmdTwo = new NpgsqlCommand("insert into DCPal_ItemAuctionEntries VALUES(" + "@currentDate" + ", " +
                            "@LoginID" +", " + "@item" +", " + "@currency" +", " + "@desc" + ", " + "@price" +", " + "@quantity" + ");", conn);

                            cmdTwo.Parameters.AddWithValue("@LoginID", LoginManager.LocalUser.LoginID);
                            cmdTwo.Parameters.AddWithValue("@item", item);
                            cmdTwo.Parameters.AddWithValue("@desc", desc);
                            cmdTwo.Parameters.AddWithValue("@quantity", quantity);
                            cmdTwo.Parameters.AddWithValue("@currency", currency);
                            cmdTwo.Parameters.AddWithValue("@price", price);

                            cmdTwo.ExecuteNonQuery();
                            conn.Close();
                            return true;
                        }
                        else
                        {
                            // Too many entries! Warn user to delete old ones first.
                            conn.Close();
                            return false;
                        }
                    }
                }

                try
                {
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }


                return false;
            }

            /// <summary>
            /// Updates an entry to the CC database table.
            /// </summary>
            /// <param name="rate">The rate of which the CC is being sold for.</param>
            /// <param name="desc">The description of the sale.</param>
            /// <param name="quantity">The quantity of the item</param>
            /// <returns>True, if the appending is successful, false otherwise.</returns>
            public static bool UpdateEntry(string oldTimeStamp, string oldItem, string oldDesc, string oldCurrency, double oldPrice, int oldQuantity, string item, string desc, string currency, double price, int quantity)
            {
                // Refresh the connection
                conn = LoginManager.LocalUser.Verify();

                // Get the current date
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                Debug.WriteLine("UPDATE DCPal_ItemAuctionEntries SET timestamp = '" + currentDate.ToString() + "', uid = " +
                "@LoginID" +", item = " + "@item" +", description = " + "@desc" + ", currency = " + "@currency" +", price = " + "@price" +", quantity = " + "@quantity" + " WHERE timestamp = " + oldTimeStamp + " AND uid = " +
                "@LoginID" +" AND item = " + "@oldItem" +" AND description = " + "@oldDesc" + " AND currency = " + "@oldCurrency" +" AND price = " + oldPrice + " AND quantity = " + "@oldQuantity" +";");


                // Add the entry
                NpgsqlCommand cmdTwo = new NpgsqlCommand("UPDATE DCPal_ItemAuctionEntries SET timestamp = '" + currentDate.ToString() + "', uid = " +
                "@LoginID" +", item = " + "@item" +", description = " + "@desc" + ", currency = " + "@currency" +", price = " + "@price" +", quantity = " + "@quantity" + " WHERE timestamp = '" + oldTimeStamp + "' AND uid = " +
                "@LoginID" +" AND item = " + "@oldItem" +" AND description = " + "@oldDesc" + " AND currency = " + "@oldCurrency" +" AND price = " + "@oldPrice" + " AND quantity = " + "@oldQuantity" +";", conn);

                cmdTwo.Parameters.AddWithValue("@LoginID", LoginManager.LocalUser.LoginID);
                cmdTwo.Parameters.AddWithValue("@item", item);
                cmdTwo.Parameters.AddWithValue("@desc", desc);
                cmdTwo.Parameters.AddWithValue("@quantity", quantity);
                cmdTwo.Parameters.AddWithValue("@currency", currency);
                cmdTwo.Parameters.AddWithValue("@price", price);
                cmdTwo.Parameters.AddWithValue("@oldItem", oldItem);
                cmdTwo.Parameters.AddWithValue("@oldDesc", oldDesc);
                cmdTwo.Parameters.AddWithValue("@oldCurrency", oldCurrency);
                cmdTwo.Parameters.AddWithValue("@oldPrice", oldPrice);
                cmdTwo.Parameters.AddWithValue("@oldQuantity", oldQuantity);

                cmdTwo.ExecuteNonQuery();
                conn.Close();
                return true;

            }

            public static bool LocalUserOwnsEntry(string timeStamp, string item, string desc, string currency, double price, int quantity)
            {
                conn = LoginManager.LocalUser.Verify();

                // Add the entry
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT count(*) FROM DCPal_ItemAuctionEntries WHERE timestamp = '" + timeStamp + "' AND uid = " +
                "@LoginID" +" AND item = " + "@item" +" AND description = " + "@desc" + " AND currency = " + "@currency" +" AND price = " + "@price" +" AND quantity = " + "@quantity" + ";", conn);


                cmd.Parameters.AddWithValue("@LoginID", LoginManager.LocalUser.LoginID);
                cmd.Parameters.AddWithValue("@item", item);
                cmd.Parameters.AddWithValue("@desc", desc);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@currency", currency);
                cmd.Parameters.AddWithValue("@price", price);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count >= 1)
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Returns a table to be used as an ItemsSource representing a CC Marketplace Table
            /// </summary>
            /// <returns></returns>
            public static DataView GetTableView()
            {
                // Refresh the connection
                conn = LoginManager.LocalUser.Verify();

                // To be changed
                NpgsqlCommand cmd = new NpgsqlCommand("select * from DCPal_ItemAuctionEntries", conn);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);

                DataSet dt = new DataSet();

                // DB Columns
                da.Fill(dt);

                // Custom columns
                dt.Tables[0].Columns.Add("Verified", typeof(bool));

                OrderColumns(dt, "Verified", "timestamp", "item", "quantity", "price", "currency", "uid", "description");

                //"Verified", "rate", "timestamp", "uid", "description"
                //"Rate (Millions/CC)", "Time Stamp", "Username", "Description"

                dt.Tables[0].Columns["price"].ColumnName = "Price per Item";
                dt.Tables[0].Columns["timestamp"].ColumnName = "Time Stamp";
                dt.Tables[0].Columns["item"].ColumnName = "Item";
                dt.Tables[0].Columns["description"].ColumnName = "Details";
                dt.Tables[0].Columns["currency"].ColumnName = "Currency";
                dt.Tables[0].Columns["uid"].ColumnName = "Username";
                dt.Tables[0].AcceptChanges();

                try
                {
                    conn.Close();
                }catch(Exception ex)
                {

                }

                // Now fill in the custom columns
                for (int i = 0; i < dt.Tables[0].Rows.Count; ++i)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch (Exception ex)
                    {

                    }
                    conn = LoginManager.LocalUser.Verify();
                    cmd = new NpgsqlCommand("select uid from DCPal_AHTrusted where uid = '" + dt.Tables[0].Rows[i]["Username"] + "'", conn);
                    NpgsqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        // User is in verified list
                        dt.Tables[0].Rows[i]["Verified"] = true;
                    }
                    else
                    {
                        // Not in verified list
                        dt.Tables[0].Rows[i]["Verified"] = false;
                    }
                }

                dt.Tables[0].AcceptChanges();

                return dt.Tables[0].DefaultView;
            }
            //"Verified", "rate", "timestamp", "uid", "description"
            //"Rate (Millions/CC)", "Time Stamp", "Username", "Description"
            /// <summary>
            /// Deletes a row from the CC auction house. Warning: Assumes there can only be at most one entry per user.
            /// </summary>
            /// <param name="timestamp">The timestamp of the entry.</param>
            /// <param name="uid">The username of the user.</param>
            /// <returns>True if the operation was successful, false if otherwise</returns>
            public static bool DeleteRow(string timestamp, string uid)
            {
                if (uid == LoginManager.LocalUser.LoginID || LoginManager.LocalUser.IsAdmin())
                {
                    // Delete row
                    conn = LoginManager.LocalUser.Verify();


                    NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM DCPal_ItemAuctionEntries WHERE timestamp = '" + timestamp + "' AND " +
                        "uid = " + "@LoginID" +";", conn);
                    cmd.Parameters.AddWithValue("@LoginID", LoginManager.LocalUser.LoginID);

                    cmd.ExecuteNonQuery();

                    conn.Close();
                    return true;
                }
                else
                {
                    // Permission error
                    return false;
                }
            }


            // Source: http://stackoverflow.com/questions/3757997/how-to-change-datatable-colums-order
            private static void OrderColumns(DataSet dataSet, params String[] columnNames)
            {
                int columnIndex = 0;
                foreach (var columnName in columnNames)
                {
                    dataSet.Tables[0].Columns[columnName].SetOrdinal(columnIndex);
                    columnIndex++;
                }
            }
        }

    }


}
