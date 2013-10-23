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
using TweetSharp;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Twitter
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
        System.Windows.Threading.DispatcherTimer twitterTimer;
        int TwitterRefreshRate = 10; 


        private void twitterTimer_Tick(object sender, EventArgs e)
        {
         // Reload Twitter
            GetTweets_Click();
        }

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
            // DispatcherTimer setup
            twitterTimer = new System.Windows.Threading.DispatcherTimer();
            twitterTimer.Tick += new EventHandler(twitterTimer_Tick);
            twitterTimer.Interval = new TimeSpan(0, 0, this.TwitterRefreshRate);
            twitterTimer.Start();
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

        private ObservableCollection<Tweet> _tweets = new ObservableCollection<Tweet>();
        private void Set_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Hello");
        }

        private void GetTweets_Click()
        {
            _tweets.Clear();

            int twitterCount =0; 
            var service = new TwitterService("gdszkrjT9BXALsntZI3BxQ", "ltpb4xzjzxRf1w9Sq6wqhwOBfDNCKpWDcUkQyth5MeE");
            service.AuthenticateWith("1966078789-R92gYWO9THXuYJ5uE6DkifcQ9mDLEZFT6dgUcuH", "g3Jb0b8BSt4CgSDrWJMatw6DaXwtocPk4kMhX52jnq4");

            var tweets = service.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions());


            foreach (var tweet in tweets)
            {
                
                if (twitterCount < 6)
                {
                    String name = tweet.User.ScreenName;
                    String status = tweet.Text;
                    String timeString;
                    Uri image = new Uri(tweet.User.ProfileImageUrl);
                    DateTime time = tweet.CreatedDate.ToLocalTime();
                    TimeSpan timePassed = DateTime.Now.Subtract(time);
                    if (timePassed.TotalSeconds < 60)
                    {
                        int timeInt = (int)(Math.Floor(timePassed.TotalSeconds));
                        timeString = timeInt.ToString("N0") + " seconds ago";
                    }
                    else if (timePassed.TotalMinutes < 60)
                    {
                        int timeInt = (int)(Math.Floor(timePassed.TotalMinutes));
                        timeString = timeInt.ToString("N0") + " minutes ago";
                    }
                    else if (timePassed.TotalHours < 24)
                    {
                        int timeInt = (int)Math.Floor(timePassed.TotalHours);
                        timeString = timeInt.ToString("N1") + " hours ago";
                    }
                    else
                    {
                        int timeInt = (int)(Math.Floor(timePassed.TotalDays));
                        timeString = timeInt.ToString("N1") + " days ago";
                    }

                    DataContext = this;
                    _tweets.Add(new Tweet("@" + name, status, image, timeString.Replace(".0", "")));
                    twitterCount++;
                }
            }
        }
    }
}