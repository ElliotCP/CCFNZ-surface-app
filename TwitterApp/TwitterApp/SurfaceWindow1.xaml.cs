using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.Collections.ObjectModel;
using System.Net;
using System.Xml.Linq;

namespace TwitterApp
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    /// 
    
    

    public partial class SurfaceWindow1 : SurfaceWindow
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// 



        private ObservableCollection<Tweet> _tweets = new ObservableCollection<Tweet>();

        public SurfaceWindow1()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TweetList.ItemsSource = _tweets;
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

        private void GetTweets_Click(object sender, RoutedEventArgs e)
        {
            WebClient client = new WebClient();

            client.DownloadStringCompleted += (s, ea) =>
            {
                XDocument doc = XDocument.Parse(ea.Result);

                XNamespace ns = "http://www.w3.org/2005/Atom";

                var items = from item in doc.Descendants(ns + "entry")
                            select new Tweet()
                            {
                                Title = item.Element(ns + "title").Value,

                                Image = new Uri((from XElement xe in item.Descendants(ns + "link")
                                                 where xe.Attribute("type").Value == "image/png"
                                                 select xe.Attribute("href").Value).First<string>()),

                                Link = new Uri((from XElement xe in item.Descendants(ns + "link")
                                                where xe.Attribute("type").Value == "text/html"
                                                select xe.Attribute("href").Value).First<string>()),
                            };

                foreach (Tweet t in items)
                {
                    _tweets.Add(t);
                }
            };

            client.DownloadStringAsync(new Uri("http://search.twitter.com/search.atom?q=wpf"));
        }
    }
}