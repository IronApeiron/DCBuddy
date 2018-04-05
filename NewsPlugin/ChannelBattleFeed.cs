using Authenticator;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NewsPlugin
{

    public class FullChannelBattleFeed : ObservableCollection<ChannelBattleEntry>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static String date = PreviousSaturday(DateTime.Now).ToString("MM/dd/yyyy");

        public static string ViewingDate{

            get
            {
                return date;
            }
            set
            {
                date = value;
                
            }

        }


     //   const String GAMESCAMPUS_PROFILE_URI = "/NewsPlugin;component/Assets/Images/gc_icon.PNG";

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public FullChannelBattleFeed()
            {

                foreach (ChannelBattleEntry entry in GetMainChannelBattleEntries())
                {
                    Add(entry);
                }


            }

            /// <summary>
            /// Obtains all main current Channel Battle entries.
            /// </summary>
            /// <returns>A List of those entries.</returns>
            public List<ChannelBattleEntry> GetMainChannelBattleEntries()
            {

            try
            {
                List<ChannelBattleEntry> entries = new List<ChannelBattleEntry>();

                // Refresh the connection
                NpgsqlConnection conn = LoginManager.LocalUser.Verify();

                NpgsqlCommand cmd = new NpgsqlCommand("select * from DCPal_ChannelBattleStandings WHERE date = '" + ViewingDate + "' ORDER BY channelNum ASC", conn);

                NpgsqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    // Create a new channel battle entry
                    // dr[0] = date, dr[1] = channel num, dr[2] = main channels, dr[3] = crew name
                    string crewNonSpecialChars = RemoveSpecialCharacters(dr[3].ToString());
                    ChannelBattleEntry cbe = new ChannelBattleEntry(Int32.Parse(dr[1].ToString()), dr[3].ToString(), crewNonSpecialChars.First().ToString().ToUpper() + crewNonSpecialChars.Last().ToString().ToUpper());

                    // Check if the channel is a part of the full or semi set
                    cbe.IsSemi = Boolean.Parse(dr[2].ToString());

                    // Check if this channel is not a semi
                    if (!cbe.IsSemi)
                    {
                        if (cbe.ChannelNum == 1 || cbe.ChannelNum == 2)
                        {
                            // Custom highlighting for majors
                            cbe.EntryColorCode = "#FF3E1B18";
                        }

                        // Add the main entries to the list
                        entries.Add(cbe);
                    }
                }

                conn.Close();
                return entries;
            }
            catch (Exception)
            {
                // No internet connection
                List<ChannelBattleEntry> entries = new List<ChannelBattleEntry>();
                ChannelBattleEntry c = new ChannelBattleEntry(1, "PlaceHolder Crew", "PH", false);
                entries.Add(c);
                return entries;
            }
            }

        // Source: http://stackoverflow.com/questions/1120198/most-efficient-way-to-remove-special-characters-from-string
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static DateTime PreviousSaturday(DateTime date)
           {
                do
                {
                    date = date.AddDays(-1);
                }
                while (!(date.DayOfWeek == DayOfWeek.Saturday));
     
                return date;
           }

                private bool IsWeekend(DateTime date)
                {
                    return date.DayOfWeek == DayOfWeek.Saturday ||
                           date.DayOfWeek == DayOfWeek.Sunday;
                }


        }

    public class SemiChannelBattleFeed : ObservableCollection<ChannelBattleEntry>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static String date = PreviousSaturday(DateTime.Now).ToString("MM/dd/yyyy");

        public static string ViewingDate
        {

            get
            {
                return date;
            }
            set
            {
                date = value;

             //   NotifyPropertyChanged("ViewingDate");
            }

        }

  //      const String GAMESCAMPUS_PROFILE_URI = "/NewsPlugin;component/Assets/Images/gc_icon.PNG";


        public SemiChannelBattleFeed()
        {

            foreach (ChannelBattleEntry entry in GetSemiChannelBattleEntries())
            {
                Add(entry);
            }


        }

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Obtains all main current Channel Battle entries.
        /// </summary>
        /// <returns>A List of those entries.</returns>
        public List<ChannelBattleEntry> GetSemiChannelBattleEntries()
        {
            try
            {
                List<ChannelBattleEntry> entries = new List<ChannelBattleEntry>();

                // Refresh the connection
                NpgsqlConnection conn = LoginManager.LocalUser.Verify();

                NpgsqlCommand cmd = new NpgsqlCommand("select * from DCPal_ChannelBattleStandings WHERE date = '" + ViewingDate + "' ORDER BY channelNum ASC", conn);

                NpgsqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    // Create a new channel battle entry
                    // dr[0] = date, dr[1] = channel num, dr[2] = main channels, dr[3] = crew name
                    string crewNonSpecialChars = RemoveSpecialCharacters(dr[3].ToString());
                    ChannelBattleEntry cbe = new ChannelBattleEntry(Int32.Parse(dr[1].ToString()), dr[3].ToString(), crewNonSpecialChars.First().ToString().ToUpper() + crewNonSpecialChars.Last().ToString().ToUpper());

                    // Check if the channel is a part of the full or semi set
                    cbe.IsSemi = Boolean.Parse(dr[2].ToString());

                    // Check if this channel is a semi
                    if (cbe.IsSemi)
                    {
                        // Add the semi entries to the list
                        entries.Add(cbe);
                    }
                }

                conn.Close();
                return entries;
            }
            catch (Exception)
            {
                // No internet connection
                List<ChannelBattleEntry> entries = new List<ChannelBattleEntry>();
                ChannelBattleEntry c = new ChannelBattleEntry(1, "MinorPlaceHolder Crew", "MH", true);
                entries.Add(c);
                return entries;
            }

        }

        // Source: http://stackoverflow.com/questions/1120198/most-efficient-way-to-remove-special-characters-from-string
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static DateTime PreviousSaturday(DateTime date)
        {
            do
            {
                date = date.AddDays(-1);
            }
            while (!(date.DayOfWeek == DayOfWeek.Saturday));

            return date;
        }

        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday ||
                   date.DayOfWeek == DayOfWeek.Sunday;
        }

    }



    public class ChannelBattleEntry
        {
            /// <summary>
            /// The entry text color.
            /// </summary>
            const String DEFAULT_ENTRY_FONT_COLOR = "Gray";

            /// <summary>
            /// The entry text color.
            /// </summary>
            const String DEFAULT_ENTRY_COLOR = "#FF2A2238";

            /// <summary>
            /// The profile initials to be displayed
            /// </summary>
            public String ProfileInitials { get; set; }

            /// <summary>
            /// The crew name of the channel.
            /// </summary>
            public String CrewName { get; set; }

            /// <summary>
            /// The font color that this entry will be displayed in as a color String.
            /// </summary>
            public String FontSolidColorCode { get; set; }

            /// <summary>
            /// The color that this entry will be displayed in as a color String.
            /// </summary>
            public String EntryColorCode { get; set; }

            /// <summary>
            /// Holds the channel number of the entry.
            /// </summary>
            public int ChannelNum { get; set; }

            /// <summary>
            /// Indicates if the channel is a part of the semi set. 
            /// </summary>
            public bool IsSemi { get; set; }

            public ChannelBattleEntry(int channelNum, String crewName, String profileInitials, bool isSemi = true,
                String entryColorCode = DEFAULT_ENTRY_COLOR, String fontSolidColorCode = DEFAULT_ENTRY_FONT_COLOR)
            {
                // Initialize values.
                this.CrewName = crewName;
                this.ProfileInitials = profileInitials;
                this.FontSolidColorCode = fontSolidColorCode;
                this.EntryColorCode = entryColorCode;
                this.ChannelNum = channelNum;
                this.IsSemi = isSemi;
            }
        }
    
}
