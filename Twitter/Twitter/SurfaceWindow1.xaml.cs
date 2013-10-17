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

namespace Twitter
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
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

        private ObservableCollection<Tweet> _tweets = new ObservableCollection<Tweet>();

        private void GetTweets_Click(object sender, RoutedEventArgs e)
        {
            var service = new TwitterService("gdszkrjT9BXALsntZI3BxQ", "ltpb4xzjzxRf1w9Sq6wqhwOBfDNCKpWDcUkQyth5MeE");
            service.AuthenticateWith("1966078789-R92gYWO9THXuYJ5uE6DkifcQ9mDLEZFT6dgUcuH", "g3Jb0b8BSt4CgSDrWJMatw6DaXwtocPk4kMhX52jnq4");

            var tweets = service.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions());
            foreach (var tweet in tweets)
            {
                //Console.WriteLine("{0} says '{1}'", tweet.User.ScreenName, tweet.Text);
                //TextStatus = tweet.User.ScreenName;
                //Profileimage = tweet.User.ProfileImageUrl;

                //_tweets.Add(new Tweet(tweet.User.ScreenName, tweet.Text, tweet.User.ProfileImageUrl));
                //Uri image = GetPicture(tweet.User.ProfileImageUrl);

                String name = tweet.User.ScreenName;
                String status = tweet.Text;
                Uri image = new Uri(tweet.User.ProfileImageUrl);

                base.OnInitialized(e);
                DataContext = this;
                _tweets.Add(new Tweet(name, status, image));

                //_statusText.Text= System.String.Format("{0} says '{1}'", tweet.User.ScreenName, tweet.Text);
            }

        }
    }
}