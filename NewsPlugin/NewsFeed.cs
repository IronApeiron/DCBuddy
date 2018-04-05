using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Xceed.Wpf.Toolkit;
using Npgsql;
using Authenticator;

namespace NewsPlugin
{

    /// <summary>
    /// Represents a set of mixed news to be displayed in the News plugin in DCPal.
    /// </summary>
    public class NewsFeed : ObservableCollection<NewsEntry>
    {

        public NewsFeed()
        {

            foreach (DCNewsEntry entry in GetAllDCEntries())
            {
                Add(entry);
            }

        }


        const String DRIFT_CITY_BASE_URL = "http://driftcity.gamescampus.com";

        const String GAMESCAMPUS_INITIALS = "GC";

        static void stuff()
        {

        }


        /// <summary>
        /// Obtains all DCNewsEntries from the GamesCampus Website.
        /// </summary>
        /// <returns>A List of those entries.</returns>
        public static List<DCNewsEntry> GetAllDCEntries()
        {

            String url = "http://webcache.googleusercontent.com/search?q=cache:tAQCQtkcRMQJ:gamescampus.com/launcher/driftcity/+&cd=1&hl=en&ct=clnk&gl=ca";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            // Get the beginning of the list chain
            HtmlNodeCollection parentUlNode = doc.DocumentNode.SelectNodes("//ul/li");

            List<DCNewsEntry> entries = new List<DCNewsEntry>();

            foreach (HtmlNode entryNode in parentUlNode)
            {
                HtmlNode titleNode = entryNode.SelectSingleNode(".//dl/dt/a");
                HtmlNode dateNode = entryNode.SelectSingleNode(".//dl/dt/span");
                HtmlNode descNode = entryNode.SelectSingleNode(".//dl/dd");

                if (titleNode != null && dateNode != null && descNode != null)
                {
                    entries.Add(new DCNewsEntry("GamesCampus", titleNode.InnerText, descNode.InnerText, dateNode.InnerText,
                        GAMESCAMPUS_INITIALS, DRIFT_CITY_BASE_URL + titleNode.Attributes["href"].Value));
                }
            }

            entries = entries.OrderBy(d => d.EntryDate).ToList();
            entries.Reverse();

            return entries;
        }
    }

    /// <summary>
    /// Represents a Drift City website news entry.
    /// </summary>
    public class DCNewsEntry : NewsEntry
    {
        /// <summary>
        /// The entry text color.
        /// </summary>
        const String ENTRY_FONT_COLOR = "Gray";

        /// <summary>
        /// The entry text color.
        /// </summary>
        const String ENTRY_COLOR = "#FF2A2238";

        /// <summary>
        /// Initializes a DriftCity news entry.
        /// </summary>
        /// <param name="posterName"></param>
        /// <param name="entryName"></param>
        /// <param name="desc"></param>
        /// <param name="entryDate">The entry date. Must be in the format of MMM dd, YYYY</param>
        public DCNewsEntry(String posterName, String entryName, String desc, String entryDate, String profileInitials, String entryURL) : base(posterName, entryName, desc, entryDate, profileInitials, entryURL, ENTRY_COLOR, ENTRY_FONT_COLOR)
        {
            
        }
       
    }

    /// <summary>
    /// Represents general attributes of a single, generic news entry.
    /// </summary>
    public class NewsEntry
    {
        /// <summary>
        /// The entry text color.
        /// </summary>
        const String DEFAULT_ENTRY_FONT_COLOR = "#FF2C243E";

        /// <summary>
        /// The entry text color.
        /// </summary>
        const String DEFAULT_ENTRY_COLOR = "Purple";

        /// <summary>
        /// The profile initials to be displayed.
        /// </summary>
        public String ProfileInitials { get; set; }

        /// <summary>
        /// The URL for the entry as a String.
        /// </summary>
        public String EntryURL { get; set; }

        /// <summary>
        /// The name of the entry.
        /// </summary>
        public String EntryName { get; set; }

        /// <summary>
        /// The description of the entry
        /// </summary>
        public String Desc { get; set; }

        /// <summary>
        /// The entry date of some entry.
        /// </summary>
        public DateTime EntryDate { get; set; }

        /// <summary>
        /// The name of the poster of this news entry.
        /// </summary>
        public String PosterName { get; set; }

        /// <summary>
        /// The font color that this entry will be displayed in as a color String.
        /// </summary>
        public String FontSolidColorCode { get; set; }

        /// <summary>
        /// The color that this entry will be displayed in as a color String.
        /// </summary>
        public String EntryColorCode { get; set; }

        public NewsEntry(String posterName, String entryName, String desc, String entryDate, 
            String profileInitials, String entryURL, String entryColorCode = DEFAULT_ENTRY_COLOR, String fontSolidColorCode = DEFAULT_ENTRY_FONT_COLOR) 
        {
            // Initialize values.
            this.PosterName = posterName;
            this.EntryName = entryName;
            this.Desc = desc;
            this.ProfileInitials = profileInitials;
            this.FontSolidColorCode = fontSolidColorCode;
            this.EntryColorCode = entryColorCode;
            this.EntryURL = entryURL;
            this.EntryDate = DateTime.ParseExact(entryDate, "MMM dd, yyyy", CultureInfo.InvariantCulture);
        }

        public String GetFormattedTimeElapsed
        {
            get
            {
                if((DateTime.Today - EntryDate).TotalDays <= 0)
                {
                    if ((DateTime.Today - EntryDate).TotalHours <= 0)
                    {
                        return "Today";
                    }
                    else
                    {
                        return (DateTime.Today - EntryDate).TotalHours.ToString() + "h ago";
                    }
                }

                return (DateTime.Today - EntryDate).TotalDays.ToString() + "d ago";
            }
        }

    }
    }

