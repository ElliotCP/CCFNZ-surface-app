using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using FluidKit.Controls;
using MessagingToolkit.QRCode.Codec;
using Microsoft.Surface;
using Image = System.Drawing.Image;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Collections.Generic;

using TweetSharp;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;

namespace CCF_app
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        //used in screen saver
        private readonly DispatcherTimer _timer;

        private RadioButton _ck; //used to check which radio button has been checked
        private Pages _currentPage = Pages.Home; // Keeps track of the current page that is on focus.

        //used for delay in progressbar
        private readonly DispatcherTimer _progressBarTimer;
        private int _currentDonationAmount;

        //donation bar variables
        private string _donationMethod = "";

        private Point _initialTouchPoint; // Keeps track of the first finger touch.
        private FrameworkElement _currentImage; // Stores the image element of the image currently being displayed in the carousel.
        private FrameworkElement _nextImage; // Stores the image element of the image about to be displayed in the carousel.

        // Used to prevent a long swipe from being processed as multiple smaller swipes.
        private bool _alreadySwiped = false;
        private bool _alreadySwipedCarousel = false;

        // Used to prevent multiple image movements with one mouse click
        bool _isMouseDownOnCarousel = false;

        // Initial mouse click on carousel image
        Point _initialMouseClick;

        //Used to keep track of scrollviewer position, and to reset it when page switches
        private double _scrollViewerOffset = 0;

        private double _angleBuffer = 0d;

        // Globe texture image size
        private static int _imageWidth = 1920;
        private static int _imageHeight = 1200;

        // Store data that will be displayed on the globe.
        private List<DonatingPlace> _DonatingCities;

        // Use these variables if the mouse is used to interact with the globe.
        private bool _isMouseDown;
        private Point _startPoint;

        // Keeps track of the state of the globe.
        private bool _isGlobeOpen = false;
        
		// Initialises a twitter refresh timer and the refresh rate 
		System.Windows.Threading.DispatcherTimer twitterTimer;
        int TwitterRefreshRate = 180;

        // creating animation instances for screen transtion. DoubleAnimation(final opacity, duration)
        DoubleAnimation animation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.3));
        DoubleAnimation unanimation = new DoubleAnimation(0.5, TimeSpan.FromSeconds(0.3));

        public MainWindow()
        {
            InitializeComponent();

            Home_BtnRec.Visibility = Visibility.Collapsed;
            
            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
            
			//load event to MainWindow_Loaded
			Loaded += new RoutedEventHandler(MainWindow_Loaded);
            
			//creating timer and binding event handler to keep track of timer
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, Constants.ScreenSaverWaitTime);
            _timer.Start();

            //creating timer and binding event handler to keep track of progress bar timer
            _progressBarTimer = new DispatcherTimer();
            _progressBarTimer.Tick += ProgressBarTimer_Tick;
            _progressBarTimer.Interval = new TimeSpan(0, 0, Constants.ProgressBarWaitTime);
            _progressBarTimer.Start();

            //when user touches screen wen screen saver is up
            ScreenSaver.MouseDown += ScreenSaver_MouseDown;

            //http://stackoverflow.com/questions/2105607/wpf-catch-last-window-click-anywhere - jano
            InputManager.Current.PreProcessInput += (sender, e) =>
            {
                var args = e.StagingItem.Input as MouseButtonEventArgs;
                if (args != null)
                    GlobalClickEventHandler();
            };
            Touch.FrameReported += Touch_FrameReported;

            int offset = 30;
            _DonatingCities = new List<DonatingPlace>(10);
            _DonatingCities.Add(new DonatingPlace() { Name = "Auckland", AmountOfDonaters = 200, Bound = new Rect(1893 - offset, 844.5 - offset, 50, 50) });
            _DonatingCities.Add(new DonatingPlace() { Name = "Queenstown", AmountOfDonaters = 200, Bound = new Rect(1867.5 - offset, 892.5 - offset, 50, 50) });
            _DonatingCities.Add(new DonatingPlace() { Name = "Malaysia", AmountOfDonaters = 200, Bound = new Rect(1506 - offset, 607.5 - offset, 50, 50) });
            _DonatingCities.Add(new DonatingPlace() { Name = "Russia", AmountOfDonaters = 200, Bound = new Rect(1632 - offset, 192 - offset, 50, 50) });
            _DonatingCities.Add(new DonatingPlace() { Name = "France", AmountOfDonaters = 200, Bound = new Rect(984 - offset, 294 - offset, 50, 50) });
            _DonatingCities.Add(new DonatingPlace() { Name = "Germany", AmountOfDonaters = 200, Bound = new Rect(1020 - offset, 261 - offset, 50, 50) });
            _DonatingCities.Add(new DonatingPlace() { Name = "United Kingdom", AmountOfDonaters = 200, Bound = new Rect(960 - offset, 250.5 - offset, 50, 50) });
            _DonatingCities.Add(new DonatingPlace() { Name = "Indiana", AmountOfDonaters = 200, Bound = new Rect(541.5 - offset, 270 - offset, 50, 50) });
            _DonatingCities.Add(new DonatingPlace() { Name = "Missouri", AmountOfDonaters = 200, Bound = new Rect(466.5 - offset, 321 - offset, 50, 50) });
            _DonatingCities.Add(new DonatingPlace() { Name = "Sydney", AmountOfDonaters = 200, Bound = new Rect(1758 - offset, 828 - offset, 50, 50) });

            // Closed at startup.
            GlobeCloseButton.Visibility = Visibility.Collapsed;
            GlobeCloseButton_Gesture.Visibility = Visibility.Collapsed;
        }

        // Twitter refresh timer activates this 
        private void twitterTimer_Tick(object sender, EventArgs e)
        {
            // Reload Twitter
            GetTweets_Click();
        }

		//Initialises the twitter feed and pulls the latest tweets
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TweetList.ItemsSource = _tweets;
            GetTweets_Click();
            // DispatcherTimer setup for the twitter timer 
            twitterTimer = new System.Windows.Threading.DispatcherTimer();
            twitterTimer.Tick += new EventHandler(twitterTimer_Tick);
            twitterTimer.Interval = new TimeSpan(0, 0, this.TwitterRefreshRate);
            twitterTimer.Start();
        }
		
        /// <summary>
        ///     Showing home page and hiding the screen saver when screen is touched
        /// </summary>
        private void ScreenSaver_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ScreenSaver.Visibility = Visibility.Collapsed;
			
			//Displays twitter feed when screensaver is disrupted
			TwitterButtonClick(sender, e);
            Storyboard storyboard = Resources["BoxSlideOut"] as Storyboard;
            storyboard.Begin(TweetList);
			
            OnHomePageClick(sender, e);
        }

        /// <summary>
        ///     reseting screen saver timer each time screen is touched
        /// </summary>
        private void GlobalClickEventHandler()
        {
            _timer.Interval = new TimeSpan(0, 0, Constants.ScreenSaverWaitTime);
        }

        /// <summary>
        ///     shows screen saver when timer has reached a certain value
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            Unanimate(); //unanimating button transitions

            //displaying hompage to go behind screen saver
            CollapseAllPages();
            HomePage.Visibility = Visibility.Visible;
			
			//If the twitter feed is visible than it is hidden with the screensaver
            if (CloseTwitterFeed.IsVisible)
            {
                CloseTwitterFeed.Visibility = System.Windows.Visibility.Collapsed;
                OpenTwitterFeed.Visibility = System.Windows.Visibility.Visible;
                Storyboard storyboard = Resources["BoxSlideIn"] as Storyboard;
                storyboard.Begin(TweetList);
            }
			
			
            //screen saver
            ScreenSaver.Visibility = Visibility.Visible;
            HideGlobe(); // Hide the globe when screensaver is activated.
        }

        /// <summary>
        ///     Occurs when the window is about to close.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        ///     Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        ///     Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        ///     This is called when the user can interact with the application's window.
        /// </summary>
        private void OnWindowInteractive(object sender, EventArgs e) {   }

        /// <summary>
        ///     This is called when the user can see but not interact with the application's window.
        /// </summary>
        private void OnWindowNoninteractive(object sender, EventArgs e) {   }

        /// <summary>
        ///     This is called when the application's window is not visible or interactive.
        /// </summary>
        private void OnWindowUnavailable(object sender, EventArgs e) {   }

        /// <summary>
        ///     This is called when the homepage button is clicked
        /// </summary>
        private void OnHomePageClick(object sender, EventArgs e)
        {
            Unanimate();
            CollapseAllPages();

            ResetScrollViewerPosition(); //reset scrollviewer back to original position

            HomePage.Visibility = Visibility.Visible;
            Home_BtnRec.BeginAnimation(HeightProperty, Constants.Da);

            _currentPage = Pages.Home;
            HideGlobe(); // Hide globe if HomePage button is clicked
            RunPageAnimation(); //Start screen fade animation
        }

        /// <summary>
        ///     displays the "how you I can help" page
        /// </summary>
        private void OnHelpPageClick(object sender, EventArgs e)
        {            
            //transition effects
            Unanimate();
            CollapseAllPages();

            ResetScrollViewerPosition(); //reset scrollviewer back to original position

            //loading and playing video            
            MyVideo1.NavigateToString(Constants.YoutubeVideo_Help);
            MyVideo1.Visibility = Visibility.Visible;

            InformationPage1.Visibility = Visibility.Visible;
            Help_BtnRec.BeginAnimation(HeightProperty, Constants.Da);

            InformationPage1Title.Text = "How Can I Help?";

            InformationPage1Pointer.Source = new BitmapImage(new Uri("Assets/Icons/pointer_green.png", UriKind.RelativeOrAbsolute));

            //setting the text on the page
            InformationPage1Text1.Text = Constants.HelpText1;
            InformationPage1Text2.Text = Constants.HelpText2;
            _currentPage = Pages.Help;
            HideGlobe(); // Hide globe if HelpPage button is clicked
            RunPageAnimation(); //Start screen fade animation
        }

        /// <summary>
        ///     displays the "how can i get support" page
        /// </summary>
        private void OnSupportPageClick(object sender, EventArgs e)
        {
            Unanimate();
            CollapseAllPages();

            ResetScrollViewerPosition(); //reset scrollviewer back to original position

            MyVideo2.NavigateToString(Constants.YoutubeVideo_Support);
            MyVideo2.Visibility = Visibility.Visible;

            InformationPage2.Visibility = Visibility.Visible;
            Support_BtnRec.BeginAnimation(HeightProperty, Constants.Da);

            InformationPage2Title.Text = "How Can I Get Support?";

            InformationPage2Text1.Text = Constants.SupportText1;
            InformationPage2Text2.Text = Constants.SupportText2;
            InformationPage2Pointer.Source = new BitmapImage(new Uri("Assets/Icons/pointer_orange.png", UriKind.RelativeOrAbsolute));
            _currentPage = Pages.Support;
            HideGlobe(); // Hide globe if SupportPage button is clicked
            RunPageAnimation(); //Start screen fade animation
        }

        /// <summary>
        ///     displays the "what we do" page
        /// </summary>
        private void OnAboutUsPageClick(object sender, EventArgs e)
        {
            Unanimate();
            CollapseAllPages();

            ResetScrollViewerPosition(); //reset scrollviewer back to original position

            MyVideo3.NavigateToString(Constants.YoutubeVideo_About);
            MyVideo3.Visibility = Visibility.Visible;

            InformationPage3.Visibility = Visibility.Visible;
            AboutUs_BtnRec.BeginAnimation(HeightProperty, Constants.Da);

            InformationPage3Title.Text = "What Is CCFNZ?";

            InformationPage3Text1.Text = Constants.AboutUsText1;
            InformationPage3Text2.Text = Constants.AboutUsText2;
            InformationPage3Pointer.Source = new BitmapImage(new Uri("Assets/Icons/pointer_purple.png", UriKind.RelativeOrAbsolute));
            _currentPage = Pages.AboutUs;
            HideGlobe(); // Hide globe if AboutUsPage button is clicked
            RunPageAnimation(); //Start screen fade animation
        }

        /// <summary>
        ///     displays the "donate" page
        /// </summary>
        private void OnDonatePageClick(object sender, EventArgs e)
        {
            Unanimate();
            CollapseAllPages();

            ResetScrollViewerPosition(); //reset scrollviewer back to original position

            DonatePage_Pointer.Source =
                new BitmapImage(new Uri("Assets/Icons/pointer_red.png", UriKind.RelativeOrAbsolute));

            DonatePage.Visibility = Visibility.Visible;
            Donate_BtnRec.BeginAnimation(HeightProperty, Constants.Da);

            UpdateProgressBarAndText(0);

            _donationMethod = "";
            //used to figure out whether or not to display the donation help text (i.e "choose amount you would like to donate ....")

            //only displaying the 2 buttons to choose donation method
            CustomAmount.Visibility = Visibility.Collapsed;
            Donate_Grid.Visibility = Visibility.Collapsed;
            QrCodeDonation.Visibility = Visibility.Collapsed;
            Txt_Donation.Visibility = Visibility.Collapsed;
            DonationMethodSwitch_Button.Visibility = Visibility.Collapsed;
            Donations_Instructions.Visibility = Visibility.Collapsed;
            QrDonate_Button.Visibility = Visibility.Visible;
            TxtDonate_Button.Visibility = Visibility.Visible;
            QrDonate_Button_Gesture.Visibility = Visibility.Visible;
            TxtDonate_Button_Gesture.Visibility = Visibility.Visible;

            //removing the donation help text if it is unnneeded
            if (Donation_Help.Visibility == Visibility.Visible && _donationMethod != null)
            {
                Donation_Help.Visibility = Visibility.Collapsed;
            }

            UncheckRadioButtons();
            _currentPage = Pages.Donate;
            HideGlobe(); // Hide globe if DonatePage button is clicked
            RunPageAnimation(); //Start screen fade animation
        }

        /// <summary>
        ///     Control Multi-Touch inputs using this method.
        ///     Currently provides page switch on swipe with a single finger.
        /// </summary>
        private void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {
            if (_isGlobeOpen)
            {
                var touchPoints = e.GetTouchPoints(viewport_globe);
                if (touchPoints.Count >= 2 && touchPoints[0].Action == TouchAction.Up)
                {
                    this.TouchLine.X1 = touchPoints[0].Position.X;
                    this.TouchLine.X2 = touchPoints[1].Position.X;
                    this.TouchLine.Y1 = touchPoints[0].Position.Y;
                    this.TouchLine.Y2 = touchPoints[1].Position.Y;
                }
            }
            else
            {
                if (Viewbox != null)
                {
                    // Reset screensaver timer on touch interation as it previously only resetted on mouse interaction.
                    _timer.Interval = new TimeSpan(0, 0, Constants.ScreenSaverWaitTime);

                    // Interact with each of the finger touches.
                    foreach (TouchPoint touchPoint in e.GetTouchPoints(Viewbox))
                    {
                        TouchPoint primaryTouchPoint = e.GetPrimaryTouchPoint(Viewbox); // First touch point on the ViewBox
                        if (touchPoint.Action == TouchAction.Down)
                        {

                            Debug.WriteLine("Home Button Touched. " + Home_BtnRec.AreAnyTouchesCaptured);

                            // Make sure the touches are captured from the viewbox.
                            // Might need to adjust depending on components that might affected by swipe gestures. - ASA
                            touchPoint.TouchDevice.Capture(Viewbox);
                            _initialTouchPoint.X = touchPoint.Position.X;
                        }
                        // Compare id of this touch with the original. If the id's are different then this touch belongs to another finger.
                        else if (touchPoint.Action == TouchAction.Move && e.GetPrimaryTouchPoint(Viewbox) != null)
                        {
                            // First finger touch.
                            TouchPoint point = e.GetPrimaryTouchPoint(Viewbox);
                            if (primaryTouchPoint != null && touchPoint.TouchDevice.Id == primaryTouchPoint.TouchDevice.Id)
                            {
                                if (_currentPage == Pages.Home)
                                {
                                    ReleaseAllTouchCaptures();
                                    touchPoint.TouchDevice.Capture(ImageGrid);
                                    if (!_alreadySwipedCarousel)
                                    {
                                        // Swipe Left with 50px threshold.
                                        if (touchPoint.Position.X < (_initialTouchPoint.X - Constants.CarouselImageSwipeThreshold))
                                        {
                                            NextImage_MouseDown(null, null); // Transition to the next carousel image.
                                            _alreadySwipedCarousel = true;
                                        }
                                        // Swipe Right with 50px threshold.
                                        if (touchPoint.Position.X > (_initialTouchPoint.X + Constants.CarouselImageSwipeThreshold))
                                        {
                                            PreviousImage_MouseDown(null, null); // Transition to the previous carousel image.
                                            _alreadySwipedCarousel = true;
                                        }
                                    }
                                }
                                else if (_currentPage == Pages.AboutUs || _currentPage == Pages.Help || _currentPage == Pages.Support)
                                {
                                    ReleaseAllTouchCaptures();

                                    PageDataScrollViewer.ScrollToHorizontalOffset(_scrollViewerOffset + _initialTouchPoint.X - touchPoint.Position.X);
                                    InformationPage2PageDataScrollViewer.ScrollToHorizontalOffset(_scrollViewerOffset + _initialTouchPoint.X - touchPoint.Position.X);
                                    InformationPage1PageDataScrollViewer.ScrollToHorizontalOffset(_scrollViewerOffset + _initialTouchPoint.X - touchPoint.Position.X);
                                }
                            }
                            // Perform second finger touch point.
                            else
                            {
                                if (point != null && touchPoint.TouchDevice.Id != point.TouchDevice.Id)
                                {
                                    // _touchPoint is now the object of the second finger.
                                    if (!_alreadySwiped)
                                    {
                                        // Swipe Left with 50px threshold.
                                        if (touchPoint.Position.X > (_initialTouchPoint.X + 50))
                                        {
                                            SwitchPage(true); // Switch Pages
                                            _alreadySwiped = true;
                                        }

                                        // Swipe Right with 50px threshold.
                                        if (touchPoint.Position.X < (_initialTouchPoint.X - 50))
                                        {
                                            SwitchPage(false); // Switch pages.
                                            _alreadySwiped = true;
                                        }
                                    }
                                }
                            }
                        }
                        else if (touchPoint.Action == TouchAction.Up)
                        {
                            _alreadySwiped = false;
                            _alreadySwipedCarousel = false;
                            // Release viewbox touch capture.
                            if (Equals(touchPoint.TouchDevice.Captured, Viewbox))
                            {
                                Viewbox.ReleaseTouchCapture(touchPoint.TouchDevice);
                            }
                            // Release ImageGrid (Carousel) touch capture.
                            else if ((Equals(touchPoint.TouchDevice.Captured, ImageGrid)))
                            {
                                ImageGrid.ReleaseTouchCapture(touchPoint.TouchDevice);
                            }
                            else if ((Equals(touchPoint.TouchDevice.Captured, PageDataScrollViewer)))
                            {
                                PageDataScrollViewer.ReleaseTouchCapture(touchPoint.TouchDevice);
                            }
                            else if ((Equals(touchPoint.TouchDevice.Captured, InformationPage1PageDataScrollViewer)))
                            {
                                InformationPage1PageDataScrollViewer.ReleaseTouchCapture(touchPoint.TouchDevice);
                            }
                            else if ((Equals(touchPoint.TouchDevice.Captured, InformationPage2PageDataScrollViewer)))
                            {
                                InformationPage2PageDataScrollViewer.ReleaseTouchCapture(touchPoint.TouchDevice);
                            }
                            // Update current position of scrollviewer
                            PageDataScrollViewer.UpdateLayout(); //in case we didn't get the current position
                            _scrollViewerOffset = PageDataScrollViewer.HorizontalOffset;

                            InformationPage1PageDataScrollViewer.UpdateLayout(); //in case we didn't get the current position
                            _scrollViewerOffset = InformationPage1PageDataScrollViewer.HorizontalOffset;

                            InformationPage2PageDataScrollViewer.UpdateLayout(); //in case we didn't get the current position
                            _scrollViewerOffset = InformationPage2PageDataScrollViewer.HorizontalOffset;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        ///     Transition page to the next depending on the direction of swipe.
        /// </summary>
        /// <param name="isSwipeLeft">When "true" swipe is left. When "false" swipe is right.</param>
        private void SwitchPage(bool isSwipeLeft)
        {
            // Switch pages to the right.
            if (!isSwipeLeft)
            {
                switch (_currentPage)
                {
                    case Pages.Home: // Current Page is Home
                        OnAboutUsPageClick(null, null); // Switch to AboutUs
                        break;
                    case Pages.AboutUs:
                        OnHelpPageClick(null, null);
                        break;
                    case Pages.Help:
                        OnSupportPageClick(null, null);
                        break;
                    case Pages.Support:
                        OnDonatePageClick(null, null);
                        break;
                    case Pages.Donate:
                        // Comment this for non-cyclic page transition (i.e. back-and-forth) - ASA
                        OnHomePageClick(null, null);
                        HideGlobe();
                        break;
                }
            }
                // Switch pages to the left.
            else
            {
                switch (_currentPage)
                {
                    case Pages.Home:
                        // Comment this for non-cyclic page transition (i.e. back-and-forth) - ASA
                        OnDonatePageClick(null, null);
                        break;
                    case Pages.AboutUs: // When on AboutUs page
                        OnHomePageClick(null, null); // Go back left to HomePage.
                        break;
                    case Pages.Help:
                        OnAboutUsPageClick(null, null);
                        break;
                    case Pages.Support:
                        OnHelpPageClick(null, null);
                        break;
                    case Pages.Donate:
                        HideGlobe();
                        OnSupportPageClick(null, null);
                        break;
                }
            }
        }

        /// <summary>
        ///     sets the visiblity of all pages to collapsed and stops the videos playing
        /// </summary>
        private void CollapseAllPages()
        {
            HomePage.Visibility = Visibility.Collapsed;

            InformationPage1.Visibility = Visibility.Collapsed;
            InformationPage2.Visibility = Visibility.Collapsed;
            InformationPage3.Visibility = Visibility.Collapsed;
            DonatePage.Visibility = Visibility.Collapsed;

            AboutUs_BtnRec.Visibility = Visibility.Visible;
            Home_BtnRec.Visibility = Visibility.Visible;
            Help_BtnRec.Visibility = Visibility.Visible;
            Support_BtnRec.Visibility = Visibility.Visible;
            Donate_BtnRec.Visibility = Visibility.Visible;

            MyVideo1.Visibility = Visibility.Collapsed;
            MyVideo1.NavigateToString("about:blank");
         
            MyVideo2.Visibility = Visibility.Collapsed;
            MyVideo2.NavigateToString("about:blank");

            MyVideo3.Visibility = Visibility.Collapsed;
            MyVideo3.NavigateToString("about:blank");
        }


        /// <summary>
        ///     animation to reset the button style to how it was when it was unclicked
        /// </summary>
        private void Unanimate()
        {
            AboutUs_BtnRec.BeginAnimation(HeightProperty, Constants.Da2);
            Home_BtnRec.BeginAnimation(HeightProperty, Constants.Da2);
            Help_BtnRec.BeginAnimation(HeightProperty, Constants.Da2);
            Support_BtnRec.BeginAnimation(HeightProperty, Constants.Da2);
            Donate_BtnRec.BeginAnimation(HeightProperty, Constants.Da2);
        }

        /// <summary>
        ///     Adding transition to the TransitionPresenter which will perform the slide animation/transition.
        /// </summary>
        public void AddTransition(FrameworkElement imageOneElement, FrameworkElement imageTwoElement)
        {
            if (imageOneElement == null) throw new ArgumentNullException("imageOneElement");
            TransitionPresenter.ApplyTransition(imageOneElement, imageTwoElement);
        }

        /// <summary>
        ///     When the 'next' arrow on the homepage is pressed, make the carousel transition to the right.
        /// </summary>
        private void NextImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Debug.WriteLine("Changing image forwards");
            var transition = Resources["SlideTransitioner"] as SlideTransition;
            if (transition != null)
            {
                transition.Direction = Direction.RightToLeft;
                TransitionPresenter.Transition = transition;
            }

            // Move image transition to the right depending on the
            // current image on the carousel.
            // If Img1 show Img2 etc. since 'next' arrow is pressed.
            switch (Constants.CurrentHomeImage)
            {
                case HomePageImages.Img1:
                    // Retrieve image elements
                    _currentImage = (FrameworkElement) FindName("Img1");
                    _nextImage = (FrameworkElement) FindName("Img2");
                    AddTransition(_currentImage, _nextImage); // Begin transition
                    Constants.CurrentHomeImage = HomePageImages.Img2; // Set next image as current
                    break;
                case HomePageImages.Img2:
                    // Retrieve image elements
                    _currentImage = (FrameworkElement) FindName("Img2");
                    _nextImage = (FrameworkElement) FindName("Img3");
                    AddTransition(_currentImage, _nextImage); // Begin transition
                    Constants.CurrentHomeImage = HomePageImages.Img3; // Set next image as current
                    break;
                case HomePageImages.Img3:
                    // Retrieve image elements
                    _currentImage = (FrameworkElement) FindName("Img3");
                    _nextImage = (FrameworkElement) FindName("Img1");
                    AddTransition(_currentImage, _nextImage); // Begin transition
                    Constants.CurrentHomeImage = HomePageImages.Img1; // Set next image as current
                    break;
            }
        }

        /// <summary>
        ///     When the 'previous' arrow on the homepage is pressed, make the carousel transition to the left.
        /// </summary>
        private void PreviousImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Debug.WriteLine("Changing image backwards");

            var transition = Resources["SlideTransitioner"] as SlideTransition;
            if (transition != null)
            {
                transition.Direction = Direction.LeftToRight;
                TransitionPresenter.Transition = transition;
            }

            // Move image transition to the left depending on the
            // current image on the carousel.
            // If Img3 show Img2 etc. since 'previous' arrow is pressed.
            switch (Constants.CurrentHomeImage)
            {
                case HomePageImages.Img1:
                    // Retrieve image elements
                    _currentImage = (FrameworkElement) FindName("Img1");
                    _nextImage = (FrameworkElement) FindName("Img3");

                    AddTransition(_currentImage, _nextImage); // Begin transition
                    Constants.CurrentHomeImage = HomePageImages.Img3; // Set next image as current
                    break;
                case HomePageImages.Img2:
                    // Retrieve image elements
                    _currentImage = (FrameworkElement) FindName("Img2");
                    _nextImage = (FrameworkElement) FindName("Img1");

                    AddTransition(_currentImage, _nextImage); // Begin transition
                    Constants.CurrentHomeImage = HomePageImages.Img1; // Set next image as current
                    break;
                case HomePageImages.Img3:
                    // Retrieve image elements
                    _currentImage = (FrameworkElement) FindName("Img3");
                    _nextImage = (FrameworkElement) FindName("Img2");

                    AddTransition(_currentImage, _nextImage); // Begin transition
                    Constants.CurrentHomeImage = HomePageImages.Img2; // Set next image as current
                    break;
            }
        }

        /// <summary>

        ///     added mouse click event for the page navigation buttons using an invisible rectangle.
        /// </summary>
        private void BtnRec_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var rec = (Rectangle) sender;
            string x = rec.Name;
            switch (x)
            {   
                case "AboutUs_BtnRec_Gesture":
                    OnAboutUsPageClick(sender, e);
                    break;
                case "Home_BtnRec_Gesture":
                    OnHomePageClick(sender, e);
                    break;

                case "Help_BtnRec_Gesture":
                    OnHelpPageClick(sender, e);
                    break;

                case "Support_BtnRec_Gesture":
                    OnSupportPageClick(sender, e);
                    break;

                case "Donate_BtnRec_Gesture":
                    OnDonatePageClick(sender, e);
                    break;
                    
                case "QrDonate_Button_Gesture":
                    QRDonate_Clicked(null, null);
                    break;

                case "TxtDonate_Button_Gesture":
                    TxtDonate_Clicked(sender, e);
                    break;

                case "OpenTwitterFeed_Gesture":
                    TwitterButtonClick(sender, e);
                    break;

                case "GlobeViewButton_Gesture":
                    GlobeViewButton_Click(sender, e);
                    break;

                case "GlobeCloseButton_Gesture":
                    GlobeCloseButton_Click(sender, e);
                    break;

                case "GlobeCloseButton":
                    GlobeCloseButton_Click(sender, e);
                    break;
            }
        }

        /// <summary>
        ///     functionality for when the radio buttons on donate page is checked
        /// </summary>
        private void Donations_Radio_Checked(object sender, RoutedEventArgs e)
        {
            _ck = sender as RadioButton;

            Donations_Instructions.Visibility = Visibility.Visible;

            // Toggle visibility of QRCode image and txt number depending on the payment type.
            if (_donationMethod == "QR" && _donationMethod != null)
            {
                QrCodeDonation.Visibility = Visibility.Visible;
                Txt_Donation.Visibility = Visibility.Collapsed;
            }
            else if (_donationMethod == "Txt" && _donationMethod != null)
            {
                QrCodeDonation.Visibility = Visibility.Collapsed;
                Txt_Donation.Visibility = Visibility.Visible;
            }

            if (_ck != null)
            {
                String donationAmount = _ck.Content.ToString();
                const string donationServer = "223.27.24.159:8080";
                if (donationAmount == "Custom")
                {
                    // Set initial amount to zero. 
                    String qrCodeContent = donationServer + "/?amount=" + Donation_CustomAmount.Text;
                    Display_QRCode(qrCodeContent, 5); // Generate and set QR_Code image.
                    if (Convert.ToInt32(Donation_CustomAmount.Text) < 999)
                    {
                        Txt_Donation.Text = "3032 " + Donation_CustomAmount.Text;
                    }
                    else
                    {

                        Txt_Donation.Text = "Please use a QR Code";
                    }
                }
                else
                {
                    // Remove "$" character that gets inherited from retrieving name/value of the donation option selected.
                    _currentDonationAmount = Convert.ToInt32(donationAmount.Replace("$", ""));
                    _progressBarTimer.Interval = new TimeSpan(0, 0, Constants.ProgressBarWaitTime); //just in case we stopped last time, reset the timer anyway
                    _progressBarTimer.Start(); //start timer for updating progress bar with selected amount
                    String qrCodeContent = donationServer + "/?amount=" + donationAmount;
                    Display_QRCode(qrCodeContent, 5); // Generate and set QR_Code image.
                    Txt_Donation.Text = "3032 " + donationAmount.Replace("$", "");
                }
            }
            _donationMethod = null; // Reset donation method.
            Donation_Help.Visibility = Visibility.Collapsed;
            if (_ck != null && _ck.Content.ToString() == "Custom") //showing custom amount textbox
            {
                CustomAmount.Visibility = Visibility.Visible;
                CustomAmount.Opacity = 0;
                CustomAmount.BeginAnimation(OpacityProperty, Constants.Da3);
            }
            else
            {
                CustomAmount.BeginAnimation(OpacityProperty, Constants.Da4);
                CustomAmount.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        ///     Generates a QRCode with the given string to be encoded and the level (ie size) of the qr code.
        /// </summary>
        /// <param name="encodedString">The string that should be encoded into the QRCode.</param>
        /// <param name="level">Detail level of QRCode can be changed using this. Higher = more detail but less time to process.</param>
        /// <returns>bitmap of the generated QRCode</returns>
        private Image QRGenerator(string encodedString, int level)
        {
            var qrEncoder = new QRCodeEncoder
            {
                QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE,
                QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L,
                QRCodeVersion = level
            };
            // Encode image into new bitmap instance.
            Bitmap bitmap = qrEncoder.Encode(encodedString);
            return bitmap;
        }

        // Convert Drawing Image to ImageSource
        // Source: http://social.msdn.microsoft.com/Forums/vstudio/en-US/833ca60f-6a11-4836-bb2b-ef779dfe3ff0/
        private BitmapImage convertImageToImageSource(Image img)
        {
            // Winforms Image we want to get the WPF Image from...
            Image imgWinForms = img;
            // ImageSource ...
            var bi = new BitmapImage();
            bi.BeginInit();
            var ms = new MemoryStream();
            // Save to a memory stream...
            imgWinForms.Save(ms, ImageFormat.Bmp);
            // Rewind the stream..
            ms.Seek(0, SeekOrigin.Begin);
            // Tell the WPF image to use this stream...
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
        }

        /// <summary>
        /// Increase the total Donation amount and refresh Donation progress text.
        /// We need this method seperate because we need to set initial amount at start up
        /// </summary>
        /// <param name="amount"></param>
        private void UpdateProgressBarAndText(int amount)
        {
            Constants.DonateTotalDonated += amount;
            Constants.DonatePercentFunded = Constants.DonateTotalDonated * 100 / Constants.DonateTarget;
            DonateProgressText.Text = String.Format("{0}% funded | ${1} donated | {2} days to go", Constants.DonatePercentFunded, Constants.DonateTotalDonated, Constants.DonateDaysToGo);
            DonationProgress_Bar.Value = Constants.DonatePercentFunded;
            _progressBarTimer.Stop(); //we've updated the progress bar, don't update until radio buttons are clicked again
        }

        /// <summary>
        /// Calls the UpdateProgressBarAndText method when the timer ticks.
        /// </summary>
        private void ProgressBarTimer_Tick(object sender, EventArgs e)
        {
            UpdateProgressBarAndText(_currentDonationAmount);
        }

        /// <summary>
        /// Resets the scrollviewer to original position (far left)
        /// </summary>
        private void ResetScrollViewerPosition()
        {
            _scrollViewerOffset = 0;
            PageDataScrollViewer.ScrollToHorizontalOffset(_scrollViewerOffset);
        }

        /// <summary>
        /// Display QRCode on the donate page by replacing placeholder qr_image.
        /// </summary>
        private void Display_QRCode(String text, int level)
        {
            Image img = QRGenerator(text, level);
            QrCodeImg.Source = convertImageToImageSource(img);
        }

        /// <summary>
        ///     when user chooses to donate via QR code
        /// </summary>
        private void QRDonate_Clicked(object sender, RoutedEventArgs e)
        {
            if (_donationMethod != null)
                //changing the visibiltiy of the instructions depending on if a radio button has been pressed before
            {
                Donation_Help.Visibility = Visibility.Visible;
                _donationMethod = "QR";
            }

            DonationMethod_Text.Text = "Donate Via Smart Device";
            Donate_Grid.Visibility = Visibility.Visible;

            if (_donationMethod == null)
            {
                QrCodeDonation.Visibility = Visibility.Visible;
                Txt_Donation.Visibility = Visibility.Collapsed;
            }

            QrDonate_Button.Visibility = Visibility.Collapsed;
            TxtDonate_Button.Visibility = Visibility.Collapsed;
            QrDonate_Button_Gesture.Visibility = Visibility.Collapsed;
            TxtDonate_Button_Gesture.Visibility = Visibility.Collapsed;

            DonationMethodSwitch_Button.Visibility = Visibility.Visible;


            Donations_Instructions.Margin = new Thickness(0, 0, 0, 420);
            Donations_Instructions.Text = "Scan The Following QR Code To Donate:";
        }

        /// <summary>
        ///     when user chooses to donate via text
        /// </summary>
        private void TxtDonate_Clicked(object sender, RoutedEventArgs e)
        {
            if (_donationMethod != null)
                //changing the visibiltiy of the instructions depending on if a radio button has been pressed before
            {
                Donation_Help.Visibility = Visibility.Visible;
                _donationMethod = "Txt";
            }

            DonationMethod_Text.Text = "Donate Via Txt";

            Donate_Grid.Visibility = Visibility.Visible;

            if (_donationMethod == null)
            {
                QrCodeDonation.Visibility = Visibility.Collapsed;
                Txt_Donation.Visibility = Visibility.Visible;
            }


            QrDonate_Button.Visibility = Visibility.Collapsed;
            TxtDonate_Button.Visibility = Visibility.Collapsed;
            QrDonate_Button_Gesture.Visibility = Visibility.Collapsed;
            TxtDonate_Button_Gesture.Visibility = Visibility.Collapsed;

            DonationMethodSwitch_Button.Visibility = Visibility.Visible;


            Donations_Instructions.Margin = new Thickness(0, 0, 0, 200);
            Donations_Instructions.Text = "Text The Following Number To Donate:";
        }

        /// <summary>
        ///     when user wants to change the donation method
        /// </summary>
        private void DonationMethod_Change(object sender, RoutedEventArgs e)
        {
            Donate_Grid.Visibility = Visibility.Collapsed;
            QrDonate_Button.Visibility = Visibility.Visible;
            QrDonate_Button_Gesture.Visibility = Visibility.Visible;
            
            QrCodeDonation.Visibility = Visibility.Collapsed;
            TxtDonate_Button.Visibility = Visibility.Visible;
            TxtDonate_Button_Gesture.Visibility = Visibility.Visible;

            DonationMethodSwitch_Button.Visibility = Visibility.Collapsed;

            if (Donation_Help.Visibility == Visibility.Visible && _donationMethod != null)
            {
                Donation_Help.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        ///     unchecks all radio button selections
        /// </summary>
        private void UncheckRadioButtons()
        {
            try
            {
                _ck.IsChecked = false;
            }
            catch (NullReferenceException) //if no buttons are checked anyway
            {
            }
        }

        string prev_donation_text = "";
        double donation_amount = 0;
        /// <summary>
        ///     When text is entered into the custom donation textbox, update the donation 
        ///     QRCode or the call number depending on current payment selected.
        /// </summary>
        private void Donation_KeyDown(object sender, KeyEventArgs e)
        {
            Donations_Instructions.Visibility = Visibility.Visible;

            // Toggle visibility of QRCode image and txt number depending on the payment type.
            if (_donationMethod == "QR" && _donationMethod != null)
            {
                QrCodeDonation.Visibility = Visibility.Visible;
                Txt_Donation.Visibility = Visibility.Collapsed;
            }
            else if (_donationMethod == "Txt" && _donationMethod != null)
            {
                QrCodeDonation.Visibility = Visibility.Collapsed;
                Txt_Donation.Visibility = Visibility.Visible;
            }

            try // Try and convert custom donation amount to integer
            {
                prev_donation_text = Donation_CustomAmount.Text;
                donation_amount = Convert.ToInt32(Donation_CustomAmount.Text);
            }
            catch (FormatException) // Donation amount entered is not a number so resolve the issue.
            {
                // Remove invalid characters by setting textbox to an empty string.
                Donation_CustomAmount.Text = "";
                // Prevent further (potentially incorrect) processing by breaking out of the method.
                return;
            }

            // String together information that will be stored into the QRCode.
            const string donationServer = "223.27.24.159:8080";
            String qrCodeContent = donationServer + "/?amount=" + Donation_CustomAmount.Text;
            Display_QRCode(qrCodeContent, 5); // Generate and set QR_Code image with the given information.

            if (Convert.ToInt32(Donation_CustomAmount.Text) < 999)
            {
                Txt_Donation.Text = "3032 " + Donation_CustomAmount.Text;
            }
            else if (Convert.ToInt32(Donation_CustomAmount.Text) >= 999)
            {
                // Redirect user to use the QRCode for their own privacy.
                Txt_Donation.Text = "Please use a QR Code";
            }
            else
            {
                Txt_Donation.Text = "3032 02";
            }
            _donationMethod = null;
            Donation_Help.Visibility = Visibility.Collapsed;
            CustomAmount.Visibility = Visibility.Visible;
            CustomAmount.Opacity = 0;
            CustomAmount.BeginAnimation(OpacityProperty, Constants.Da3);
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            scaleTransform.ScaleX *= e.DeltaManipulation.Scale.X;
            scaleTransform.ScaleY *= e.DeltaManipulation.Scale.Y;
            scaleTransform.ScaleZ *= e.DeltaManipulation.Scale.X;

            this._angleBuffer++;
            // To avoid screen slash and to save a few CPU resource, do not rotate the scene whenever a maniputation event occurs.
            // Only rotate the scene if the angle cumulated enough.
            if (_angleBuffer >= 0)
            {
                Vector delta = e.DeltaManipulation.Translation;
                this.RotateGlobe(delta);
            }
            e.Handled = true;
        }

        /// <summary>
        /// Rotate the global. Given 'delta' the distance the finger/mouse has moved. 
        /// </summary>
        private void RotateGlobe(Vector delta)
        {
            if (delta.X != 0 || delta.Y != 0)
            {
                // Convert delta to a 3D vector.
                Vector3D vOriginal = new Vector3D(-delta.X, delta.Y, 0d);
                Vector3D vZ = new Vector3D(0, 0, 1);
                // Find a vector that is perpendicular with the delta vector on the XY surface. This will be the rotation axis.
                Vector3D perpendicular = Vector3D.CrossProduct(vOriginal, vZ);
                RotateTransform3D rotate = new RotateTransform3D();
                // The QuaternionRotation3D allows you to easily specify a rotation axis.
                QuaternionRotation3D quatenion = new QuaternionRotation3D();
                quatenion.Quaternion = new Quaternion(perpendicular, 3);
                rotate.Rotation = quatenion;
                transformGroup.Children.Add(rotate);
                _angleBuffer = 0;
            }
        }

        /// <summary>
        ///     On touch up. Check to see if the touch point matches the point of interest
        ///     on the globe.
        /// </summary>
        private void OnTouchUp(object sender, TouchEventArgs e)
        {
            DoHitTest(e.GetTouchPoint(GlobeGrid).Position);
        }

        /// <summary>
        ///     Check if a given point on the globe matches a point of interest.
        /// </summary>
        private void DoHitTest(Point point)
        {
            VisualTreeHelper.HitTest(GlobeGrid, null, new HitTestResultCallback(target =>
            {
                RayMeshGeometry3DHitTestResult result = target as RayMeshGeometry3DHitTestResult;
                if (result != null)
                {
                    // Calculate the hit point using barycentric coordinates formula:
                    // p = p1 * w1 + p2 * w2 + p3 * w3.
                    // For more information, please refer to http://en.wikipedia.org/wiki/Barycentric_coordinates_%28mathematics%29.
                    Point p1 = result.MeshHit.TextureCoordinates[result.VertexIndex1];
                    Point p2 = result.MeshHit.TextureCoordinates[result.VertexIndex2];
                    Point p3 = result.MeshHit.TextureCoordinates[result.VertexIndex3];
                    double hitX = p1.X * result.VertexWeight1 + p2.X * result.VertexWeight2 + p3.X * result.VertexWeight3;
                    double hitY = p1.Y * result.VertexWeight1 + p2.Y * result.VertexWeight2 + p3.Y * result.VertexWeight3;
                    Point pointHit = new Point(hitX * _imageWidth, hitY * _imageHeight);
                    //// If a data center circle is hit, display the information.
                    foreach (DonatingPlace dc in this._DonatingCities)
                    {
                        if (dc.Bound.Contains(pointHit))
                        {
                            this.InfoTextBox.Text = "Number of donaters in " + dc.Name + " are "+dc.AmountOfDonaters;
                            Storyboard sb = this.Resources["sb"] as Storyboard;
                            if (sb != null)
                            {
                                sb.Begin();
                            }
                            return HitTestResultBehavior.Stop;
                        }
                    }
                }
                return HitTestResultBehavior.Continue;
            }), new PointHitTestParameters(point));
        }

        // The following are event handlers for mouse simulation.
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = true;
            _startPoint = e.GetPosition(GlobeGrid);
        }

        /// <summary>
        ///     If mouse is being used. Rotate globe on drag.
        /// </summary>
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown && MouseSimulationCheckBox.IsChecked.Value)
            {
                _angleBuffer++;
                if (_angleBuffer >= 0)
                {
                    Point currentPoint = e.GetPosition(GlobeGrid);
                    Vector delta = new Vector(currentPoint.X - _startPoint.X, currentPoint.Y - _startPoint.Y);
                    RotateGlobe(delta);
                }
            }
        }

        /// <summary>
        ///     When the mouse button is clicked. Call method that checks if a point of interest
        ///     on the globe is clicked.
        /// </summary>
        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this._isMouseDown = false;
            if (MouseSimulationCheckBox.IsChecked.Value)
            {
                DoHitTest(e.GetPosition(GlobeGrid));
            }
        }

        /// <summary>
        ///     Zoom on Scroll if mouse is being used.
        /// </summary>
        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (MouseSimulationCheckBox.IsChecked.Value)
            {
                double delta = e.Delta > 0 ? 1.2 : 0.8;
                scaleTransform.ScaleX *= delta;
                scaleTransform.ScaleY *= delta;
                scaleTransform.ScaleZ *= delta;
            }
        }

        private void GlobeViewButton_Click(object sender, RoutedEventArgs e)
        {
        	DisplayGlobe();
            // Swap view/close globe buttons.
            GlobeViewButton.Visibility = Visibility.Collapsed;
            GlobeViewButton_Gesture.Visibility = Visibility.Collapsed;
            GlobeCloseButton.Visibility = Visibility.Visible;
            GlobeCloseButton_Gesture.Visibility = Visibility.Visible;
        }
		
        /// <summary>
        ///     Call this method to display the globe and hide donate page elements.
        /// </summary>
		private void DisplayGlobe()
		{
            // Collapse Donation Page elements when the globe is displayed.
            QrDonate_Button.Visibility = Visibility.Collapsed;
			TxtDonate_Button.Visibility = Visibility.Collapsed;
                        QrDonate_Button_Gesture.Visibility = Visibility.Collapsed;
            TxtDonate_Button_Gesture.Visibility = Visibility.Collapsed;

			ProgressGrid.Visibility = Visibility.Collapsed;
            Donate_Grid.Visibility = Visibility.Collapsed;
            Donation_Help.Visibility = Visibility.Collapsed;


            GlobeGrid.Visibility = Visibility.Visible;
            _isGlobeOpen = true;
		}

        /// <summary>
        ///     Call this method to hide the globe.
        /// </summary>
        private void HideGlobe()
        {
            GlobeGrid.Visibility = Visibility.Collapsed;
            ProgressGrid.Visibility = Visibility.Visible;
            _isGlobeOpen = false;

            // Swap view/close globe buttons.
            GlobeViewButton.Visibility = Visibility.Visible;
            GlobeViewButton_Gesture.Visibility = Visibility.Visible;
            GlobeCloseButton.Visibility = Visibility.Collapsed;
            GlobeCloseButton_Gesture.Visibility = Visibility.Collapsed;
        }

		/// <summary>
		/// 	Hide the globe and go back to the start of donate page.
		/// </summary>
        private void GlobeCloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	HideGlobe();
			OnDonatePageClick(null, null);
        }

        // Provide the carousel images with an identifier for easy reference.
        public enum HomePageImages
        {
            Img1 = 1,
            Img2,
            Img3
        };

        // Provide pages with an identifier for easy reference.
        private enum Pages
        {
            Home = 1,
            AboutUs,
            Help,
            Support,
            Donate
        };

        /// <summary>
        /// 	Provides mouse support for moving carousel images left and right
        /// </summary>
        private void ImageGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (!_isMouseDownOnCarousel)
                {
                    _isMouseDownOnCarousel = true;
                    // Initial point where mouse is clicked on carousel
                    _initialMouseClick = PointToScreen(Mouse.GetPosition(Application.Current.MainWindow));
                }
                else
                {
                    Point currentPosition = PointToScreen(Mouse.GetPosition(Application.Current.MainWindow));
                    double deltaX = currentPosition.X - _initialMouseClick.X; //change in mouse from initial click position
                    if (!_alreadySwipedCarousel) //if carousel hasn't been swiped yet
                    {
                        if (deltaX > Constants.CarouselImageSwipeThreshold) //if change in X position is greater than threshold value
                        {
                            PreviousImage_MouseDown(null, null); // Transition to the next carousel image.
                            _alreadySwipedCarousel = true;
                        }
                        else if (deltaX < -Constants.CarouselImageSwipeThreshold) //if change in X position is less than threshold value
                        {
                            NextImage_MouseDown(null, null); // Transition to the next carousel image.
                            _alreadySwipedCarousel = true;
                        }
                    }
                }
            }
            else if (Mouse.LeftButton == MouseButtonState.Released)
            {
                //reset for next mouse click
                _isMouseDownOnCarousel = false;
                _alreadySwipedCarousel = false;
            }

        }
		
	    private void TwitterButtonClick(object sender, RoutedEventArgs e)
        {
            if (OpenTwitterFeed.Visibility == System.Windows.Visibility.Visible)
            {
                CloseTwitterFeed.Visibility = System.Windows.Visibility.Visible;
                OpenTwitterFeed.Visibility = System.Windows.Visibility.Collapsed;
                this.TwitterFeed.Visibility = System.Windows.Visibility.Visible;
                Storyboard storyboard = Resources["BoxSlideOut"] as Storyboard;
                storyboard.Begin(TweetList);
            }
            else
            {
                CloseTwitterFeed.Visibility = System.Windows.Visibility.Collapsed;
                OpenTwitterFeed.Visibility = System.Windows.Visibility.Visible;
                Storyboard storyboard = Resources["BoxSlideIn"] as Storyboard;
                storyboard.Begin(TweetList);
            }
        }

        private ObservableCollection<Tweet> _tweets = new ObservableCollection<Tweet>();
        private void GetTweets_Click()
        {


            _tweets.Clear();
            int twitterCount = 0;
            var service = new TwitterService("gdszkrjT9BXALsntZI3BxQ", "ltpb4xzjzxRf1w9Sq6wqhwOBfDNCKpWDcUkQyth5MeE");
            service.AuthenticateWith("1966078789-R92gYWO9THXuYJ5uE6DkifcQ9mDLEZFT6dgUcuH", "g3Jb0b8BSt4CgSDrWJMatw6DaXwtocPk4kMhX52jnq4");

            var tweets = service.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions());

            try
            {
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
                        _tweets.Add(new Tweet("@" + name, status, timeString.Replace(".0", "")));
                        twitterCount++;
                    }
                }


            }
            catch (NullReferenceException)
            {
                Console.WriteLine("NOOOOOOOOOOO");
            }

        }

        /// <summary>
        /// makes all the pages transparent
        /// </summary>
        public void MakeAllPagesOpacityZero()
        {
            HomePage.Opacity = 0;

            InformationPage1.Opacity = 0;
            InformationPage2.Opacity = 0;
            InformationPage3.Opacity = 0;
            DonatePage.Opacity = 0;
        }
        /// <summary>
        ///  collapses all the pages (doesn't make top nav and bottom nav invisible)
        /// </summary>
        public void MakeAllPagesInvisible()
        {
            HomePage.Visibility = Visibility.Hidden;

            InformationPage1.Visibility = Visibility.Hidden;
            InformationPage2.Visibility = Visibility.Hidden;
            InformationPage3.Visibility = Visibility.Hidden;
            DonatePage.Visibility = Visibility.Hidden;

        }

        /// <summary>
        ///  reverts the animation effect on the pages. needed for re-animation
        /// </summary>
        public void UnAnimatePages()
        {
            HomePage.BeginAnimation(Grid.OpacityProperty, unanimation);

            InformationPage1.BeginAnimation(Grid.OpacityProperty, unanimation);
            InformationPage2.BeginAnimation(Grid.OpacityProperty, unanimation);
            InformationPage3.BeginAnimation(Grid.OpacityProperty, unanimation);
            DonatePage.BeginAnimation(Grid.OpacityProperty, unanimation);
        }

        /// <summary>
        ///  runs page animations depending on the current page in focus
        /// </summary>
        public void RunPageAnimation()
        {
            UnAnimatePages();
            MakeAllPagesOpacityZero();

            switch (_currentPage)
            {

                case Pages.Home:
                    //begins animation if the page is home page
                    HomePage.BeginAnimation(Grid.OpacityProperty, animation);
                    break;
                case Pages.AboutUs:
                    //begins animation if the page is about us page

                    InformationPage3.BeginAnimation(Grid.OpacityProperty, animation);
                    break;
                case Pages.Help:
                    //begins animation if the page is help page

                    InformationPage1.BeginAnimation(Grid.OpacityProperty, animation);
                    break;
                case Pages.Support:
                    //begins animation if the page is support page

                    InformationPage2.BeginAnimation(Grid.OpacityProperty, animation);
                    break;
                case Pages.Donate:
                    //begins animation if the page is donate page
                    DonatePage.BeginAnimation(Grid.OpacityProperty, animation);
                    break;
            }



        }


    }
}
