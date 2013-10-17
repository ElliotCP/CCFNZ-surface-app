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
using System.Drawing;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;

using System.IO;
using System.Drawing.Imaging;

// QrCode Ref
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Ecc;
using MessagingToolkit.QRCode.Codec.Data;
using MessagingToolkit.QRCode.Codec.Util;

using System.Windows.Media.Animation;
using System.Diagnostics;

/* Alias for conflicting namespaces */
// e.g. System.Drawing.Brush conflicts with System.Windows.Media.Brush.
// There might be a better way of doing this. -AA
using Brush = System.Windows.Media.Brush;
using Rectangle = System.Windows.Shapes.Rectangle;
using Point = System.Windows.Point;
using Color = System.Windows.Media.Color;

namespace CCF_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : SurfaceWindow
    {
        /// <summary>
        /// Default constructor.
        /// </summary>

        Brush Home_Btn_Color = (Brush)new BrushConverter().ConvertFrom("#FF33B5E5");
        Brush AboutUs_Btn_Color = (Brush)new BrushConverter().ConvertFrom("#FFAA66CC");
        Brush Help_Btn_Color = (Brush)new BrushConverter().ConvertFrom("#FF99CC00");
        Brush Support_Btn_Color = (Brush)new BrushConverter().ConvertFrom("#FFFFBB33");
        Brush Donate_Btn_Color = (Brush)new BrushConverter().ConvertFrom("#FFFF4444");

        string helpText1 = "We rely on the generosity of big-hearted New Zealander's to help us continue what we do. There are a variety of ways you can support children with cancer and their families."
        + "\nOur Beads of Courage ?presents children with a bead representing an Act of Courage. Ideally (and sadly) this year, we expect that we will need approximately 5000 handmaid beads donated. We are currently reaching only 1800 and we need all the help we can get to help our kids.";
        string helpText2 = "We rely on donations to continue our services. You can make a one-off donation through your credit card; it is simple, secure and super rewarding."
        + "\nYou can become a regular supporter of Child Cancer Foundation by setting up a regular donation from your credit card or bank account. More information on donations can be found on our website."
        + "\nEvery donation, no matter how big or small, helps us continue to support our children and families affected by this traumatic disease.";

        string supportText1 = "Our Family Support team work in conjunction with the foundation’s branch members (parents, caregivers, and volunteers) to deliver a range of support services to ensure every child and their family walking the child cancer journey will never feel alone."
        + " They offer individual and group support, information, financial assistance, and advocacy. Our Coordinators also offer support for bereaved families. They connect similar families and provide a link to other agencies and community support groups.";
        string supportText2 = "There are a variety of local and regional child, parent, grandparent, sibling and bereaved support programmes and events that aim to inform, reduce isolation and support your family through the experiences and challenges of child cancer."
        + " Parent events, children’s holiday programmes and sibling days are among many that are well attended. ";

        string aboutUsText1 = "Child Cancer Foundation New Zealand's mission is that every child and their family walking the child cancer journey will never feel alone."
        +"\nEvery week in New Zealand three families are told their child has cancer. We support these families from the very beginning. By doing this we reduce isolation and the impact of cancer. We aim to reduce the impact of cancer by offering services to ensure children and their families are supported, informed and well cared for on their journey with cancer.";
        string aboutUsText2 = "This assistance is delivered throughout New Zealand by our Family Support team working in conjunction with the foundation's branch members (parents and volunteers) in the local community."
        + "\nEach year we need at least $6 million to continue our services. This is raised through the generosity of individuals, grants, donations and sponsorships."
        + "\nThe Foundation's work with children with cancer and their families is unique and receives no direct government funding or support from other cancer agencies.";

        string donateText1 = "It's simple. It's fast. Make your donation now and help us make the world a better place for children with cancer.";

        string donateText2 = "";

        string donateText3 = "";

        int donatePercentFunded = 60;
        int donateTotalDonated = 60;
        int donateTarget = 200;
        int donateDaysToGo = 15;

        private string donationMethod = "";


        ImageBrush[] backgrounds = new ImageBrush[3];
        DoubleAnimation fadeIn;
        DoubleAnimation fadeOut;
        string[] backgroundImages = new string[3] { "pack://application:,,,/CCF_app;component/Assets/Images/HomePage_Pic1.jpg", "pack://application:,,,/CCF_app;component/Assets/Images/HomePage_Pic2.jpg", "pack://application:,,,/CCF_app;component/Assets/Images/HomePage_Pic3.jpg" };

        int currentImage = 0;
        DoubleAnimation da = new DoubleAnimation(0, TimeSpan.FromMilliseconds(70));
        DoubleAnimation da2 = new DoubleAnimation(40, TimeSpan.FromMilliseconds(70));
        DoubleAnimation da3 = new DoubleAnimation(1, TimeSpan.FromMilliseconds(400));
        DoubleAnimation da4 = new DoubleAnimation(0, TimeSpan.FromMilliseconds(400));

        System.Windows.Threading.DispatcherTimer dt;

        private int ScreenSaverWaitTime = 10;

        public MainWindow()
        {
            InitializeComponent();
            int i = 0;
            foreach (string s in backgroundImages)
            {
                ImageBrush ib = new ImageBrush();
                ib.Stretch = Stretch.UniformToFill;
                ib.ImageSource = new BitmapImage(new Uri(@s, UriKind.RelativeOrAbsolute));
                backgrounds[i] = ib;
                i++;
            }


            this.Home_BtnRec.Visibility = System.Windows.Visibility.Collapsed;

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
            this.More1.MouseDown += new MouseButtonEventHandler(More1_MouseDown);
            this.More2.MouseDown += new MouseButtonEventHandler(More2_MouseDown);
            this.QRCode.MouseDown += new MouseButtonEventHandler(QRCode_MouseDown);

            dt = new System.Windows.Threading.DispatcherTimer();
            dt.Tick +=new EventHandler(dt_Tick);
            dt.Interval = new TimeSpan(0, 0, this.ScreenSaverWaitTime);
            dt.Start();

            this.ScreenSaver.MouseDown += new MouseButtonEventHandler(ScreenSaver_MouseDown);

            //http://stackoverflow.com/questions/2105607/wpf-catch-last-window-click-anywhere - jano
            InputManager.Current.PreProcessInput += (sender, e) =>
              {
                  if (e.StagingItem.Input is MouseButtonEventArgs)
                      GlobalClickEventHandler(sender,
                        (MouseButtonEventArgs)e.StagingItem.Input);
              };

            
        }

        void ScreenSaver_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.ScreenSaver.Visibility = System.Windows.Visibility.Collapsed;
            this.OnHomePageClick(sender, e);
        }

        void GlobalClickEventHandler(object sender, MouseButtonEventArgs e)
        {
            dt.Interval = new TimeSpan(0, 0, this.ScreenSaverWaitTime);

        }

        void dt_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("zzzzzzzz");
            this.ScreenSaver.Visibility = System.Windows.Visibility.Visible;
            this.CollapseAllPages();
        }

        void QRCode_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.QRCode.Visibility = System.Windows.Visibility.Collapsed;
        }

        void More2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.QRCode.Visibility = System.Windows.Visibility.Visible;
        }

        void More1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.QRCode.Visibility = System.Windows.Visibility.Visible;
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

        private void OnHomePageClick(object sender, EventArgs e)
        {
            Unanimate();
            CollapseAllPages();

            this.HomePage.Visibility = System.Windows.Visibility.Visible;
            //this.Home_BtnRec.Visibility = System.Windows.Visibility.Collapsed;

            this.Home_BtnRec.BeginAnimation(Rectangle.HeightProperty, da);

        }

        private void OnHelpPageClick(object sender, EventArgs e)
        {
            Unanimate();
            CollapseAllPages();

            this.MyVideo1.Visibility = System.Windows.Visibility.Visible;
            this.MyVideo1.Play();

            this.InformationPage.Visibility = System.Windows.Visibility.Visible;
            //this.Help_BtnRec.Visibility = System.Windows.Visibility.Collapsed;

            this.Help_BtnRec.BeginAnimation(Rectangle.HeightProperty, da);

            this.InformationPageTitle.Text = "How Can I Help?";

            this.Pointer.Source = new BitmapImage(new Uri("Assets/Icons/pointer_green.png", UriKind.RelativeOrAbsolute));

            this.Text1.Text = helpText1;
            this.Text2.Text = helpText2;

            this.More1.Foreground = Help_Btn_Color;
            this.More2.Foreground = Help_Btn_Color;
            this.QRCode_Text.Foreground = Help_Btn_Color;
            //this.Image1.Source = new BitmapImage(new Uri("Assets/Images/help1.jpg", UriKind.RelativeOrAbsolute));
            //this.Image2.Source = new BitmapImage(new Uri("Assets/Images/help2.png", UriKind.RelativeOrAbsolute));
            
            
        }

        private void OnSupportPageClick(object sender, EventArgs e)
        {
            Unanimate();   
            CollapseAllPages();

            this.MyVideo2.Visibility = System.Windows.Visibility.Visible;
            this.MyVideo2.Play();

            this.InformationPage.Visibility = System.Windows.Visibility.Visible;
            //this.Support_BtnRec.Visibility = System.Windows.Visibility.Collapsed;
            this.Support_BtnRec.BeginAnimation(Rectangle.HeightProperty, da);
            this.InformationPageTitle.Text = "How Can I Get Support?";

            this.Text1.Text = supportText1;
            this.Text2.Text = supportText2;

            this.Pointer.Source = new BitmapImage(new Uri("Assets/Icons/pointer_orange.png", UriKind.RelativeOrAbsolute));

            this.More1.Foreground = Support_Btn_Color;
            this.More2.Foreground = Support_Btn_Color;
            this.QRCode_Text.Foreground = Support_Btn_Color;

            //this.Image1.Source = new BitmapImage(new Uri("Assets/Images/support1.png", UriKind.RelativeOrAbsolute));
            //this.Image2.Source = new BitmapImage(new Uri("Assets/Images/support2.png", UriKind.RelativeOrAbsolute));
            
        }

        private void OnAboutUsPageClick(object sender, EventArgs e)
        {
            Unanimate();
            CollapseAllPages();

            this.MyVideo3.Visibility = System.Windows.Visibility.Visible;
            this.MyVideo3.Play();

            this.InformationPage.Visibility = System.Windows.Visibility.Visible;
            //this.AboutUs_BtnRec.Visibility = System.Windows.Visibility.Collapsed;
            this.AboutUs_BtnRec.BeginAnimation(Rectangle.HeightProperty, da);

            this.InformationPageTitle.Text = "What Is CCFNZ?";

            this.Text1.Text = aboutUsText1;
            this.Text2.Text = aboutUsText2;

            this.Pointer.Source = new BitmapImage(new Uri("Assets/Icons/pointer_purple.png", UriKind.RelativeOrAbsolute));

            this.More1.Foreground = AboutUs_Btn_Color;
            this.More2.Foreground = AboutUs_Btn_Color;
            this.QRCode_Text.Foreground = AboutUs_Btn_Color;

            //this.Image1.Source = new BitmapImage(new Uri("Assets/Images/aboutUs1.png", UriKind.RelativeOrAbsolute));
            //this.Image2.Source = new BitmapImage(new Uri("Assets/Images/aboutUs2.jpg", UriKind.RelativeOrAbsolute));
            
        }

        private void OnDonatePageClick(object sender, EventArgs e)
        {
            Unanimate();
            CollapseAllPages();

            this.DonatePage_Pointer.Source = new BitmapImage(new Uri("Assets/Icons/pointer_red.png", UriKind.RelativeOrAbsolute));

            this.DonatePage.Visibility = System.Windows.Visibility.Visible;
            //this.Donate_BtnRec.Visibility = System.Windows.Visibility.Collapsed;
            this.Donate_BtnRec.BeginAnimation(Rectangle.HeightProperty, da);
            //this.DonationProgress_Bar.Visibility = System.Windows.Visibility.Visible;


            UpdateProgressBarAndText(0);

            this.donationMethod = "";
            this.CustomAmount.Visibility = System.Windows.Visibility.Collapsed;
            this.Donate_Grid.Visibility = System.Windows.Visibility.Collapsed;
            this.QRDonate_Button.Visibility = System.Windows.Visibility.Visible;

            this.QRCode_Donation.Visibility = System.Windows.Visibility.Collapsed;
            this.Txt_Donation.Visibility = System.Windows.Visibility.Collapsed;

            this.TxtDoante_Button.Visibility = System.Windows.Visibility.Visible;

            this.DonationMethodSwitch_Button.Visibility = System.Windows.Visibility.Collapsed;
            this.Donations_Instructions.Visibility = System.Windows.Visibility.Collapsed;

            if (this.Donation_Help.Visibility == System.Windows.Visibility.Visible && donationMethod != null)
            {
                this.Donation_Help.Visibility = System.Windows.Visibility.Collapsed;
            }

            this.UncheckRadioButtons();


        }

        private void UpdateProgressBarAndText(int amount)
        {
            donateTotalDonated += amount;
            donatePercentFunded = donateTotalDonated * 100 / donateTarget;
            this.DonateProgressText.Text = String.Format("{0}% funded | ${1} donated | {2} days to go", donatePercentFunded, donateTotalDonated, donateDaysToGo);
            this.DonationProgress_Bar.Value = donatePercentFunded;
        }

        private void CollapseAllPages()
        {
            this.HomePage.Visibility = System.Windows.Visibility.Collapsed;
            this.InformationPage.Visibility = System.Windows.Visibility.Collapsed;
            this.DonatePage.Visibility = System.Windows.Visibility.Collapsed;

            this.AboutUs_BtnRec.Visibility = System.Windows.Visibility.Visible;
            this.Home_BtnRec.Visibility = System.Windows.Visibility.Visible;
            this.Help_BtnRec.Visibility = System.Windows.Visibility.Visible;            
            this.Support_BtnRec.Visibility = System.Windows.Visibility.Visible;            
            this.Donate_BtnRec.Visibility = System.Windows.Visibility.Visible;


            this.MyVideo1.Stop();
            this.MyVideo1.Visibility = System.Windows.Visibility.Collapsed;
            this.MyVideo2.Stop();
            this.MyVideo2.Visibility = System.Windows.Visibility.Collapsed;
            this.MyVideo3.Stop();
            this.MyVideo3.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Unanimate()
        {
            this.AboutUs_BtnRec.BeginAnimation(Rectangle.HeightProperty, da2);
            this.Home_BtnRec.BeginAnimation(Rectangle.HeightProperty, da2);
            this.Help_BtnRec.BeginAnimation(Rectangle.HeightProperty, da2);
            this.Support_BtnRec.BeginAnimation(Rectangle.HeightProperty, da2);
            this.Donate_BtnRec.BeginAnimation(Rectangle.HeightProperty, da2);
        }

        private LinearGradientBrush SelectedButtonGradientSet()
        {
            LinearGradientBrush lgb = new LinearGradientBrush();
            lgb.StartPoint = new Point(0.5, 0);
            lgb.EndPoint = new Point(0.5, 1);
            lgb.GradientStops.Add(new GradientStop(Color.FromArgb(225, 224, 230, 172), 0.0));
            lgb.GradientStops.Add(new GradientStop(Color.FromArgb(225, 192, 215, 45), 0.4));

            return lgb;
        }

        private void NextImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.WriteLine("Changing image forwards");
            //backgrounds[currentImage].BeginAnimation(Brush.OpacityProperty, fadeOut);
            if (currentImage == 2)
            {
                currentImage = 0;
            }
            else
            {
                currentImage++;
            }

            //fadeOut.Completed += delegate(object sender1, EventArgs e1) {
            //    //once the fadeout is complete set the new back ground and fade back in. 
            //    //Create a new background brush. 
            //    ImageBrush bgBrush = backgrounds[currentImage];

            //    //Set the grid background to the new brush. 
            //    ImageGrid.Background = bgBrush;

            //    //Set the brush...(not the background property) with the animation.
            //    bgBrush.BeginAnimation(Brush.OpacityProperty, fadeInAnimation);
            //};

            fadeIn = new DoubleAnimation(0.8, TimeSpan.FromMilliseconds(800));
            backgrounds[currentImage].Opacity = 0;
            ImageGrid.Background = backgrounds[currentImage];
            Debug.WriteLine(backgrounds[currentImage].Opacity);
            backgrounds[currentImage].BeginAnimation(Brush.OpacityProperty, fadeIn);
        }

        private void PreviousImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.WriteLine("Changing image backwards");
            if (currentImage == 0)
            {
                currentImage = 2;
            }
            else
            {
                currentImage--;
            }
            fadeIn = new DoubleAnimation(0.8, TimeSpan.FromMilliseconds(800));
            backgrounds[currentImage].Opacity = 0;
            ImageGrid.Background = backgrounds[currentImage];
            Debug.WriteLine(backgrounds[currentImage].Opacity);
            backgrounds[currentImage].BeginAnimation(Brush.OpacityProperty, fadeIn);
        }

        private void BtnRec_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle rec = (Rectangle)sender;
            var x =rec.Name.ToString();
            Debug.WriteLine(x);
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
                default:
                break;
            }
        }

        private RadioButton ck;
        private void Donations_Radio_Checked(object sender, RoutedEventArgs e)
        {
            ck = sender as RadioButton;

            this.Donations_Instructions.Visibility = System.Windows.Visibility.Visible;

            // Toggle visibility of QRCode image and txt number depending on the payment type.
            if (donationMethod == "QR" && donationMethod != null)
            {
                this.QRCode_Donation.Visibility = System.Windows.Visibility.Visible;
                this.Txt_Donation.Visibility = System.Windows.Visibility.Collapsed;

            }
            else if (donationMethod == "Txt" && donationMethod != null)
            {
                this.QRCode_Donation.Visibility = System.Windows.Visibility.Collapsed;
                this.Txt_Donation.Visibility = System.Windows.Visibility.Visible;

            }

            String donationAmount = ck.Content.ToString();
            String donation_server = "49.50.241.171:8080";
            if (donationAmount == "Custom")
            {
                // Set initial amount to zero. 
                // TODO Change QRCode when user inputs a number into the custom donation text box.
                String qr_code_content = donation_server+"/?amount=0";
                Display_QRCode(qr_code_content, 5); // Generate and set QR_Code image.
            }
            else
            {
                int amountDonated = Convert.ToInt32(donationAmount.Replace("$", ""));
                UpdateProgressBarAndText(amountDonated);
                String qr_code_content = donation_server+"/?amount=" + donationAmount;
                Display_QRCode(qr_code_content, 5); // Generate and set QR_Code image.
            }
            donationMethod = null;


            this.Donation_Help.Visibility = System.Windows.Visibility.Collapsed;
            
            if (ck.Content.ToString() == "Custom")//showing custom amount textbox
            {
                this.CustomAmount.Visibility = System.Windows.Visibility.Visible;
                this.CustomAmount.Opacity = 0;
                this.CustomAmount.BeginAnimation(Grid.OpacityProperty, da3);
            }
            else
            {
                this.CustomAmount.BeginAnimation(Grid.OpacityProperty, da4);
                this.CustomAmount.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Generates a QRCode with the given string to be encoded and the level (ie size) of the qr code.
        /// </summary>
        /// <param name="encodedString">The string that should be encoded into the QRCode.</param>
        /// <param name="level">Detail level of QRCode can be changed using this. Higher = more detail.</param>
        /// <returns></returns>
        private System.Drawing.Image QRGenerator(string encodedString, int level)
        {
            MessagingToolkit.QRCode.Codec.QRCodeEncoder qrEncoder = new MessagingToolkit.QRCode.Codec.QRCodeEncoder();
            qrEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
            qrEncoder.QRCodeVersion = level;
            // Encode image into new bitmap instance.
            System.Drawing.Bitmap bitmap = qrEncoder.Encode(encodedString);
            return bitmap;
        }

        // Convert Drawing Image to ImageSource
        // Source: http://social.msdn.microsoft.com/Forums/vstudio/en-US/833ca60f-6a11-4836-bb2b-ef779dfe3ff0/
        private BitmapImage convertImageToImageSource(System.Drawing.Image img)
        {
            // Winforms Image we want to get the WPF Image from...
            System.Drawing.Image imgWinForms = img;
            // ImageSource ...
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            MemoryStream ms = new MemoryStream();
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
            System.Drawing.Image img = QRGenerator(text, level);
            this.QRCode_img.Source = convertImageToImageSource(img);
        }

        private void QRDonate_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (donationMethod != null)//changing the visibiltiy of the instructions depending on if a radio button has been pressed before
            {
                this.Donation_Help.Visibility = System.Windows.Visibility.Visible;
                this.donationMethod = "QR";
            }

            this.DonationMethod_Text.Text = "Donate Via Smart Device";
            this.Donate_Grid.Visibility = System.Windows.Visibility.Visible;

            if (donationMethod == null)
            {
                this.QRCode_Donation.Visibility = System.Windows.Visibility.Visible;
                this.Txt_Donation.Visibility = System.Windows.Visibility.Collapsed;
            }
            
            this.QRDonate_Button.Visibility = System.Windows.Visibility.Collapsed;
            this.TxtDoante_Button.Visibility = System.Windows.Visibility.Collapsed;
            
            this.DonationMethodSwitch_Button.Visibility = System.Windows.Visibility.Visible;

            this.Donations_Instructions.Margin = new Thickness(0, 0, 0, 350);
            this.Donations_Instructions.Text = "Scan The Following QR Code To Donate:";
            
        }

        private void TxtDonate_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (donationMethod != null)//changing the visibiltiy of the instructions depending on if a radio button has been pressed before
            {
                this.Donation_Help.Visibility = System.Windows.Visibility.Visible;
                this.donationMethod = "Txt";
            }

            this.DonationMethod_Text.Text = "Donate Via Txt";

            this.Donate_Grid.Visibility = System.Windows.Visibility.Visible;

            if (donationMethod == null)
            {
                this.QRCode_Donation.Visibility = System.Windows.Visibility.Collapsed;
                this.Txt_Donation.Visibility = System.Windows.Visibility.Visible;
            }


            this.QRDonate_Button.Visibility = System.Windows.Visibility.Collapsed;
            this.TxtDoante_Button.Visibility = System.Windows.Visibility.Collapsed;
            
            this.DonationMethodSwitch_Button.Visibility = System.Windows.Visibility.Visible;

            this.Donations_Instructions.Margin = new Thickness(0, 0, 0, 100);
            this.Donations_Instructions.Text = "Text The Following Number To Donate:";
        }

        private void DonationMethod_Change(object sender, System.Windows.RoutedEventArgs e)
        {
        	this.Donate_Grid.Visibility = System.Windows.Visibility.Collapsed;
            this.QRDonate_Button.Visibility = System.Windows.Visibility.Visible;

            this.QRCode_Donation.Visibility = System.Windows.Visibility.Collapsed;
            this.TxtDoante_Button.Visibility = System.Windows.Visibility.Visible;

            this.DonationMethodSwitch_Button.Visibility = System.Windows.Visibility.Collapsed;

            if (this.Donation_Help.Visibility == System.Windows.Visibility.Visible && donationMethod != null)
            {
                this.Donation_Help.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void UncheckRadioButtons()
        {
            try
            {
                ck.IsChecked = false;
            }
            catch (NullReferenceException e)
            {
                
            }
        }

        private Boolean playing = true;
        private void VideoClicked_Event(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            MediaElement m = sender as MediaElement;
            if (playing)
            {
                m.Pause();
                this.playing = false;
            }
            else
            {
                m.Play();
                this.playing = true;
            }
        }
    }
}