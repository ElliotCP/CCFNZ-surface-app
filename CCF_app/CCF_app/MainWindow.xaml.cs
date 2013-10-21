using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using FluidKit.Controls;
using MessagingToolkit.QRCode.Codec;
using Microsoft.Surface;
using Image = System.Drawing.Image;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace CCF_app
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const int ScreenSaverWaitTime = 5;

        private const string AboutUsText1 =
            "Child Cancer Foundation New Zealand's mission is that every child and their family walking the child cancer journey will never feel alone." +
            "\nEvery week in New Zealand three families are told their child has cancer. We support these families from the very beginning. By doing this we reduce isolation and the impact of cancer. We aim to reduce the impact of cancer by offering services to ensure children and their families are supported, informed and well cared for on their journey with cancer.";

        private const string AboutUsText2 =
            "This assistance is delivered throughout New Zealand by our Family Support team working in conjunction with the foundation's branch members (parents and volunteers) in the local community." +
            "\nEach year we need at least $6 million to continue our services. This is raised through the generosity of individuals, grants, donations and sponsorships." +
            "\nThe Foundation's work with children with cancer and their families is unique and receives no direct government funding or support from other cancer agencies.";

        private const int DonateTarget = 10000;

        private const string HelpText1 =
            "We rely on the generosity of big-hearted New Zealander's to help us continue what we do. There are a variety of ways you can support children with cancer and their families." +
            "\nOur Beads of Courage ?presents children with a bead representing an Act of Courage. Ideally (and sadly) this year, we expect that we will need approximately 5000 handmaid beads donated. We are currently reaching only 1800 and we need all the help we can get to help our kids.";

        private const string HelpText2 =
            "We rely on donations to continue our services. You can make a one-off donation through your credit card; it is simple, secure and super rewarding." +
            "\nYou can become a regular supporter of Child Cancer Foundation by setting up a regular donation from your credit card or bank account. More information on donations can be found on our website." +
            "\nEvery donation, no matter how big or small, helps us continue to support our children and families affected by this traumatic disease.";

        private const string SupportText1 =
            "Our Family Support team work in conjunction with the foundation’s branch members (parents, caregivers, and volunteers) to deliver a range of support services to ensure every child and their family walking the child cancer journey will never feel alone." +
            " They offer individual and group support, information, financial assistance, and advocacy. Our Coordinators also offer support for bereaved families. They connect similar families and provide a link to other agencies and community support groups.";

        private const string SupportText2 =
            "There are a variety of local and regional child, parent, grandparent, sibling and bereaved support programmes and events that aim to inform, reduce isolation and support your family through the experiences and challenges of child cancer." +
            " Parent events, children’s holiday programmes and sibling days are among many that are well attended. ";

        private const int DonateDaysToGo = 15;

        private readonly string[] _backgroundImages =
        {
            "pack://application:,,,/CCF_app;component/Assets/Images/HomePage_Pic1.jpg",
            "pack://application:,,,/CCF_app;component/Assets/Images/HomePage_Pic2.jpg",
            "pack://application:,,,/CCF_app;component/Assets/Images/HomePage_Pic3.jpg"
        };

        private readonly ImageBrush[] _backgrounds = new ImageBrush[3];
        private readonly DoubleAnimation _da = new DoubleAnimation(0, TimeSpan.FromMilliseconds(70));
        private readonly DoubleAnimation _da2 = new DoubleAnimation(40, TimeSpan.FromMilliseconds(70));
        private readonly DoubleAnimation _da3 = new DoubleAnimation(1, TimeSpan.FromMilliseconds(400));
        private readonly DoubleAnimation _da4 = new DoubleAnimation(0, TimeSpan.FromMilliseconds(400));

        //used in screen saver
        private readonly DispatcherTimer _timer;

        private bool _alreadySwiped;

        private RadioButton _ck; //used to check which radio button has been checked
        private HomePageImages _currentHomeImage = HomePageImages.Img3;
        private FrameworkElement _currentImage;
        private Pages _currentPage = Pages.Home;

        //donation bar variables
        private int _donatePercentFunded = 60;
        private int _donateTotalDonated = 6000;
        private string _donationMethod = "";

        // Provide pages with an identifier for easy reference.

        private Point _initialTouchPoint;
        private FrameworkElement _nextImage;
        private Boolean _playing = true;

        public MainWindow()
        {
            InitializeComponent();

            int i = 0;
            foreach (string s in _backgroundImages)
            {
                var ib = new ImageBrush
                {
                    Stretch = Stretch.UniformToFill,
                    ImageSource = new BitmapImage(new Uri(@s, UriKind.RelativeOrAbsolute))
                };
                _backgrounds[i] = ib;
                i++;
            }

            Home_BtnRec.Visibility = Visibility.Collapsed;

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

            //creating timer and binding event handler to keep track of timer
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, ScreenSaverWaitTime);
            _timer.Start();

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
        }

        /// <summary>
        ///     Showing home page and hiding the screen saver when screen is touched
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScreenSaver_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ScreenSaver.Visibility = Visibility.Collapsed;
            OnHomePageClick(sender, e);
        }

        /// <summary>
        ///     reseting screen saver timer each time screen is touched
        /// </summary>
        private void GlobalClickEventHandler()
        {
            _timer.Interval = new TimeSpan(0, 0, ScreenSaverWaitTime);
        }

        /// <summary>
        ///     shows screen saver when timer has reached a certain value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            Unanimate(); //unanimating button transitions

            //displaying hompage to go behind screen saver
            CollapseAllPages();
            HomePage.Visibility = Visibility.Visible;

            //screen saver
            ScreenSaver.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     Occurs when the window is about to close.
        /// </summary>
        /// <param name="e"></param>
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
        }

        private void OnHomePageClick(object sender, EventArgs e)
        {
            Unanimate();
            CollapseAllPages();

            HomePage.Visibility = Visibility.Visible;

            Home_BtnRec.BeginAnimation(HeightProperty, _da);
            _currentPage = Pages.Home;
        }

        /// <summary>
        ///     displays the "how you I can help" page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHelpPageClick(object sender, EventArgs e)
        {
            //transition effects
            Unanimate();
            CollapseAllPages();

            //loading and playing video
            MyVideo1.Visibility = Visibility.Visible;
            MyVideo1.Play();

            InformationPage.Visibility = Visibility.Visible;

            Help_BtnRec.BeginAnimation(HeightProperty, _da);

            InformationPageTitle.Text = "How Can I Help?";

            Pointer.Source = new BitmapImage(new Uri("Assets/Icons/pointer_green.png", UriKind.RelativeOrAbsolute));

            //setting the text on the page
            Text1.Text = HelpText1;
            Text2.Text = HelpText2;
            _currentPage = Pages.Help;
            //this.Image1.Source = new BitmapImage(new Uri("Assets/Images/help1.jpg", UriKind.RelativeOrAbsolute));
            //this.Image2.Source = new BitmapImage(new Uri("Assets/Images/help2.png", UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        ///     displays the "how can i get support" page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSupportPageClick(object sender, EventArgs e)
        {
            Unanimate();
            CollapseAllPages();

            MyVideo2.Visibility = Visibility.Visible;
            MyVideo2.Play();

            InformationPage.Visibility = Visibility.Visible;
            Support_BtnRec.BeginAnimation(HeightProperty, _da);

            InformationPageTitle.Text = "How Can I Get Support?";

            Text1.Text = SupportText1;
            Text2.Text = SupportText2;
            Pointer.Source = new BitmapImage(new Uri("Assets/Icons/pointer_orange.png", UriKind.RelativeOrAbsolute));
            _currentPage = Pages.Support;
        }

        /// <summary>
        ///     displays the "what we do" page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAboutUsPageClick(object sender, EventArgs e)
        {
            Unanimate();
            CollapseAllPages();

            MyVideo3.Visibility = Visibility.Visible;
            MyVideo3.Play();

            InformationPage.Visibility = Visibility.Visible;
            AboutUs_BtnRec.BeginAnimation(HeightProperty, _da);

            InformationPageTitle.Text = "What Is CCFNZ?";

            Text1.Text = AboutUsText1;
            Text2.Text = AboutUsText2;

            Pointer.Source = new BitmapImage(new Uri("Assets/Icons/pointer_purple.png", UriKind.RelativeOrAbsolute));
            _currentPage = Pages.AboutUs;
        }

        /// <summary>
        ///     displays the "donate" page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDonatePageClick(object sender, EventArgs e)
        {
            Unanimate();
            CollapseAllPages();

            DonatePage_Pointer.Source =
                new BitmapImage(new Uri("Assets/Icons/pointer_red.png", UriKind.RelativeOrAbsolute));

            DonatePage.Visibility = Visibility.Visible;
            Donate_BtnRec.BeginAnimation(HeightProperty, _da);

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

            //removing the donation help text if it is unnneeded
            if (Donation_Help.Visibility == Visibility.Visible && _donationMethod != null)
            {
                Donation_Help.Visibility = Visibility.Collapsed;
            }

            UncheckRadioButtons();
            _currentPage = Pages.Donate;
        }

        /// <summary>
        ///     Control Multi-Touch inputs using this method.
        ///     Currently provides page switch on swipe with a single finger.
        /// </summary>
        private void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {
            if (Viewbox != null)
            {
                // Reset screensaver timer on touch interation as it previously only resetted on mouse interaction.
                _timer.Interval = new TimeSpan(0, 0, ScreenSaverWaitTime);

                // Interact with each of the finger touches.
                foreach (TouchPoint touchPoint in e.GetTouchPoints(Viewbox))
                {
                    TouchPoint primaryTouchPoint = e.GetPrimaryTouchPoint(Viewbox);
                    if (touchPoint.Action == TouchAction.Down)
                    {
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
                                // Swipe Left with 50px threshold.
                                if (touchPoint.Position.X > (_initialTouchPoint.X + 50))
                                {
                                    NextImage_MouseDown(null, null); // Transition to the next carousel image.
                                    _alreadySwiped = true;
                                }

                                // Swipe Right with 50px threshold.
                                if (touchPoint.Position.X < (_initialTouchPoint.X - 50))
                                {
                                    PreviousImage_MouseDown(null, null); // Transition to the previous carousel image.
                                    _alreadySwiped = true;
                                }
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
                        // Release viewbox touch capture.
                        if (Equals(touchPoint.TouchDevice.Captured, Viewbox))
                        {
                            Viewbox.ReleaseTouchCapture(touchPoint.TouchDevice);
                        }
                        else if ((Equals(touchPoint.TouchDevice.Captured, ImageGrid)))
                        {
                            ImageGrid.ReleaseTouchCapture(touchPoint.TouchDevice);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Transition page to the next depending on the direction of swipe.
        /// </summary>
        private void SwitchPage(bool isSwipeLeft)
        {
            Debug.WriteLine(_currentPage);
            // Switch pages to the right.
            if (!isSwipeLeft)
            {
                switch (_currentPage)
                {
                    case Pages.Home:
                        OnAboutUsPageClick(null, null);
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
                        OnHomePageClick(null, null);
                        // Comment this for non-cyclic page transition (i.e. back-and-forth) - ASA
                        break;
                }
            }
                // Switch pages to the left.
            else
            {
                switch (_currentPage)
                {
                    case Pages.Home:
                        OnDonatePageClick(null, null);
                        // Comment this for non-cyclic page transition (i.e. back-and-forth) - ASA
                        break;
                    case Pages.AboutUs:
                        OnHomePageClick(null, null);
                        break;
                    case Pages.Help:
                        OnAboutUsPageClick(null, null);
                        break;
                    case Pages.Support:
                        OnHelpPageClick(null, null);
                        break;
                    case Pages.Donate:
                        OnSupportPageClick(null, null);
                        break;
                }
            }
        }

        private void UpdateProgressBarAndText(int amount)
        {
            _donateTotalDonated += amount;
            _donatePercentFunded = _donateTotalDonated*100/DonateTarget;
            DonateProgressText.Text = String.Format("{0}% funded | ${1} donated | {2} days to go", _donatePercentFunded,
                _donateTotalDonated, DonateDaysToGo);
            DonationProgress_Bar.Value = _donatePercentFunded;
        }

        /// <summary>
        ///     sets the visiblity of all pages to collapsed and stops the videos playing
        /// </summary>
        private void CollapseAllPages()
        {
            HomePage.Visibility = Visibility.Collapsed;
            InformationPage.Visibility = Visibility.Collapsed;
            DonatePage.Visibility = Visibility.Collapsed;

            AboutUs_BtnRec.Visibility = Visibility.Visible;
            Home_BtnRec.Visibility = Visibility.Visible;
            Help_BtnRec.Visibility = Visibility.Visible;
            Support_BtnRec.Visibility = Visibility.Visible;
            Donate_BtnRec.Visibility = Visibility.Visible;


            MyVideo1.Stop();
            MyVideo1.Visibility = Visibility.Collapsed;
            MyVideo2.Stop();
            MyVideo2.Visibility = Visibility.Collapsed;
            MyVideo3.Stop();
            MyVideo3.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        ///     animation to reset the button style to how it was when it was unclicked
        /// </summary>
        private void Unanimate()
        {
            AboutUs_BtnRec.BeginAnimation(HeightProperty, _da2);
            Home_BtnRec.BeginAnimation(HeightProperty, _da2);
            Help_BtnRec.BeginAnimation(HeightProperty, _da2);
            Support_BtnRec.BeginAnimation(HeightProperty, _da2);
            Donate_BtnRec.BeginAnimation(HeightProperty, _da2);
        }

        /// <summary>
        ///     Adding transition to the TransitionPresenter which will perform the slide animation/transition.
        /// </summary>
        public void AddTransition(FrameworkElement imageOneElement, FrameworkElement imageTwoElement)
        {
            if (imageOneElement == null) throw new ArgumentNullException("imageOneElement");
            TransitionPresenter.ApplyTransition(imageOneElement, imageTwoElement);
        }

        private void NextImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Changing image forwards");
            var transition = Resources["SlideTransitioner"] as SlideTransition;
            if (transition != null)
            {
                transition.Direction = Direction.RightToLeft;
                TransitionPresenter.Transition = transition;
            }

            // Move image transition to the right.
            switch (_currentHomeImage)
            {
                case HomePageImages.Img1:
                    // Retrieve image elements
                    _currentImage = (FrameworkElement) FindName("Img1");
                    _nextImage = (FrameworkElement) FindName("Img2");
                    AddTransition(_currentImage, _nextImage); // Begin transition
                    _currentHomeImage = HomePageImages.Img2; // Set next image as current
                    break;
                case HomePageImages.Img2:
                    // Retrieve image elements
                    _currentImage = (FrameworkElement) FindName("Img2");
                    _nextImage = (FrameworkElement) FindName("Img3");
                    AddTransition(_currentImage, _nextImage); // Begin transition
                    _currentHomeImage = HomePageImages.Img3; // Set next image as current
                    break;
                case HomePageImages.Img3:
                    // Retrieve image elements
                    _currentImage = (FrameworkElement) FindName("Img3");
                    _nextImage = (FrameworkElement) FindName("Img1");
                    AddTransition(_currentImage, _nextImage); // Begin transition
                    _currentHomeImage = HomePageImages.Img1; // Set next image as current
                    break;
            }

            //backgrounds[currentImage].BeginAnimation(Brush.OpacityProperty, fadeOut);
            //if (currentImage == 2)
            //{
            //    currentImage = 0;
            //}
            //else
            //{
            //    currentImage++;
            //}

            //fadeOut.Completed += delegate(object sender1, EventArgs e1) {
            //    //once the fadeout is complete set the new back ground and fade back in. 
            //    //Create a new background brush. 
            //    ImageBrush bgBrush = backgrounds[currentImage];

            //    //Set the grid background to the new brush. 
            //    ImageGrid.Background = bgBrush;

            //    //Set the brush...(not the background property) with the animation.
            //    bgBrush.BeginAnimation(Brush.OpacityProperty, fadeInAnimation);
            //};

            //fadeIn = new DoubleAnimation(0.8, TimeSpan.FromMilliseconds(800));
            //backgrounds[currentImage].Opacity = 0;
            //ImageGrid.Background.Opacity = 0;
            //ImageGrid.Background = backgrounds[currentImage];
            //Debug.WriteLine(backgrounds[currentImage].Opacity);
            //backgrounds[currentImage].BeginAnimation(Brush.OpacityProperty, fadeIn);
        }

        private void PreviousImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Changing image backwards");

            var transition = Resources["SlideTransitioner"] as SlideTransition;
            if (transition != null)
            {
                transition.Direction = Direction.LeftToRight;
                TransitionPresenter.Transition = transition;
            }

            switch (_currentHomeImage)
            {
                case HomePageImages.Img1:
                    // Retrieve image elements
                    _currentImage = (FrameworkElement) FindName("Img1");
                    _nextImage = (FrameworkElement) FindName("Img3");

                    AddTransition(_currentImage, _nextImage); // Begin transition
                    _currentHomeImage = HomePageImages.Img3; // Set next image as current
                    break;
                case HomePageImages.Img2:
                    // Retrieve image elements
                    _currentImage = (FrameworkElement) FindName("Img2");
                    _nextImage = (FrameworkElement) FindName("Img1");

                    AddTransition(_currentImage, _nextImage); // Begin transition
                    _currentHomeImage = HomePageImages.Img1; // Set next image as current
                    break;
                case HomePageImages.Img3:
                    // Retrieve image elements
                    _currentImage = (FrameworkElement) FindName("Img3");
                    _nextImage = (FrameworkElement) FindName("Img2");

                    AddTransition(_currentImage, _nextImage); // Begin transition
                    _currentHomeImage = HomePageImages.Img2; // Set next image as current
                    break;
            }

            //if (currentImage == 0)
            //{
            //    currentImage = 2;
            //}
            //else
            //{
            //    currentImage--;
            //}

            //fadeIn = new DoubleAnimation(0.8, TimeSpan.FromMilliseconds(800));
            //backgrounds[currentImage].Opacity = 0;
            //ImageGrid.Background = backgrounds[currentImage];
            //Debug.WriteLine(backgrounds[currentImage].Opacity);
            //backgrounds[currentImage].BeginAnimation(Brush.OpacityProperty, fadeIn);
        }

        /// <summary>
        ///     added mouse click event for the bottom 1/2 of the buttons (darker part)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRec_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var rec = (Rectangle) sender;
            string x = rec.Name;
            switch (x)
            {
                case "AboutUs_BtnRec":
                    OnAboutUsPageClick(sender, e);
                    break;
                case "Home_BtnRec":
                    OnHomePageClick(sender, e);
                    break;
                case "Help_BtnRec":
                    OnHelpPageClick(sender, e);
                    break;
                case "Support_BtnRec":
                    OnSupportPageClick(sender, e);
                    break;
                case "Donate_BtnRec":
                    OnDonatePageClick(sender, e);
                    break;
            }
        }

        /// <summary>
        ///     functionality for when the radio buttons on donate page is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        Txt_Donation.Text = "Please use a QR Code to donate more than $1000.";
                    }
                }
                else
                {
                    int amountDonated = Convert.ToInt32(donationAmount.Replace("$", ""));
                    UpdateProgressBarAndText(amountDonated);
                    String qrCodeContent = donationServer + "/?amount=" + donationAmount;
                    Display_QRCode(qrCodeContent, 5); // Generate and set QR_Code image.
                    Txt_Donation.Text = "3032 " + donationAmount.Replace("$", "");
                }
            }
            _donationMethod = null;


            Donation_Help.Visibility = Visibility.Collapsed;

            if (_ck != null && _ck.Content.ToString() == "Custom") //showing custom amount textbox
            {
                CustomAmount.Visibility = Visibility.Visible;
                CustomAmount.Opacity = 0;
                CustomAmount.BeginAnimation(OpacityProperty, _da3);
            }
            else
            {
                CustomAmount.BeginAnimation(OpacityProperty, _da4);
                CustomAmount.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        ///     Generates a QRCode with the given string to be encoded and the level (ie size) of the qr code.
        /// </summary>
        /// <param name="encodedString">The string that should be encoded into the QRCode.</param>
        /// <param name="level">Detail level of QRCode can be changed using this. Higher = more detail.</param>
        /// <returns></returns>
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

        private void Display_QRCode(String text, int level)
        {
            Image img = QRGenerator(text, level);
            QrCodeImg.Source = convertImageToImageSource(img);
        }

        /// <summary>
        ///     when user chooses to donate via QR code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            DonationMethodSwitch_Button.Visibility = Visibility.Visible;

            Donations_Instructions.Margin = new Thickness(0, 0, 0, 350);
            Donations_Instructions.Text = "Scan The Following QR Code To Donate:";
        }

        /// <summary>
        ///     when user chooses to donate via text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            DonationMethodSwitch_Button.Visibility = Visibility.Visible;

            Donations_Instructions.Margin = new Thickness(0, 0, 0, 100);
            Donations_Instructions.Text = "Text The Following Number To Donate:";
        }

        /// <summary>
        ///     when user wants to change the donation method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DonationMethod_Change(object sender, RoutedEventArgs e)
        {
            Donate_Grid.Visibility = Visibility.Collapsed;
            QrDonate_Button.Visibility = Visibility.Visible;

            QrCodeDonation.Visibility = Visibility.Collapsed;
            TxtDonate_Button.Visibility = Visibility.Visible;

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

        /// <summary>
        ///     when user pauses/unpauses a video
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VideoClicked_Event(object sender, MouseButtonEventArgs e)
        {
            var m = sender as MediaElement;
            if (_playing)
            {
                if (m != null) m.Pause();
                _playing = false;
            }
            else
            {
                if (m != null) m.Play();
                _playing = true;
            }
        }


        private void GotFocux(object sender, KeyEventArgs e)
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

            const string donationServer = "223.27.24.159:8080";
            // Set initial amount to zero. 
            String qrCodeContent = donationServer + "/?amount=" + Donation_CustomAmount.Text;
            Display_QRCode(qrCodeContent, 5); // Generate and set QR_Code image.

            if (Donation_CustomAmount.Text != "" && Convert.ToInt32(Donation_CustomAmount.Text) < 999)
            {
                Txt_Donation.Text = "3032 " + Donation_CustomAmount.Text;
            }
            else if (Donation_CustomAmount.Text != "" && Convert.ToInt32(Donation_CustomAmount.Text) >= 999)
            {
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
            CustomAmount.BeginAnimation(OpacityProperty, _da3);
        }

        private void GotFocux(object sender, TextChangedEventArgs e)
        {
            try
            {
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

                const string donationServer = "223.27.24.159:8080";
                // Set initial amount to zero. 
                String qrCodeContent = donationServer + "/?amount=" + Donation_CustomAmount.Text;
                Display_QRCode(qrCodeContent, 5); // Generate and set QR_Code image.
                if (Convert.ToInt32(Donation_CustomAmount.Text) < 999 && Donation_CustomAmount.Text != "")
                {
                    Txt_Donation.Text = "3032 " + Donation_CustomAmount.Text;
                }
                else if (Convert.ToInt32(Donation_CustomAmount.Text) >= 999)
                {
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
                CustomAmount.BeginAnimation(OpacityProperty, _da3);


                Donations_Instructions.Visibility = Visibility.Visible;
            }
            catch (Exception)
            {
                Debug.WriteLine("Can't close");
            }
        }

        private enum HomePageImages
        {
            Img1 = 1,
            Img2,
            Img3
        };

        private enum Pages
        {
            Home = 1,
            AboutUs,
            Help,
            Support,
            Donate
        };
    }
}