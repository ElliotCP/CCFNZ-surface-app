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

        Brush Home_Btn_Color = (Brush)new BrushConverter().ConvertFrom("#FFC7D960");
        Brush AboutUs_Btn_Color = (Brush)new BrushConverter().ConvertFrom("#FF68CEEC");
        Brush Help_Btn_Color = (Brush)new BrushConverter().ConvertFrom("#FFFAAB5E");
        Brush Support_Btn_Color = (Brush)new BrushConverter().ConvertFrom("#FFE05D5D");
        Brush Donate_Btn_Color = (Brush)new BrushConverter().ConvertFrom("#FFB57BEA");

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

        public MainWindow()
        {
            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
            this.More1.MouseDown += new MouseButtonEventHandler(More1_MouseDown);
            this.More2.MouseDown += new MouseButtonEventHandler(More2_MouseDown);
            this.QRCode.MouseDown += new MouseButtonEventHandler(QRCode_MouseDown);
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

            CollapseAllPages();

            this.HomePage.Visibility = System.Windows.Visibility.Visible;

        }

        private void OnHelpPageClick(object sender, EventArgs e)
        {

            
            CollapseAllPages();

            this.InformationPage.Visibility = System.Windows.Visibility.Visible;

            this.InformationPageTitle.Text = "How Can I Help?";

            this.Pointer.Source = new BitmapImage(new Uri("Assets/Icons/pointer_orange.png", UriKind.RelativeOrAbsolute));

            this.Text1.Text = helpText1;
            this.Text2.Text = helpText2;

            this.More1.Foreground = Help_Btn_Color;
            this.More2.Foreground = Help_Btn_Color;
            this.QRCode_Text.Foreground = Help_Btn_Color;
            this.Image1.Source = new BitmapImage(new Uri("Assets/Images/help1.jpg", UriKind.RelativeOrAbsolute));
            this.Image2.Source = new BitmapImage(new Uri("Assets/Images/help2.png", UriKind.RelativeOrAbsolute));
            
        
        }

        private void OnSupportPageClick(object sender, EventArgs e)
        {
            
            CollapseAllPages();

            this.InformationPage.Visibility = System.Windows.Visibility.Visible;
            this.InformationPageTitle.Text = "How Can I Get Support?";

            this.Text1.Text = supportText1;
            this.Text2.Text = supportText2;

            this.Pointer.Source = new BitmapImage(new Uri("Assets/Icons/pointer_red.png", UriKind.RelativeOrAbsolute));

            this.More1.Foreground = Support_Btn_Color;
            this.More2.Foreground = Support_Btn_Color;
            this.QRCode_Text.Foreground = Support_Btn_Color;

            this.Image1.Source = new BitmapImage(new Uri("Assets/Images/support1.png", UriKind.RelativeOrAbsolute));
            this.Image2.Source = new BitmapImage(new Uri("Assets/Images/support2.png", UriKind.RelativeOrAbsolute));
            
        }

        private void OnAboutUsPageClick(object sender, EventArgs e)
        {
            CollapseAllPages();

            this.InformationPage.Visibility = System.Windows.Visibility.Visible;
            this.InformationPageTitle.Text = "What Is CCFNZ?";

            this.Text1.Text = aboutUsText1;
            this.Text2.Text = aboutUsText2;

            this.Pointer.Source = new BitmapImage(new Uri("Assets/Icons/pointer_blue.png", UriKind.RelativeOrAbsolute));

            this.More1.Foreground = AboutUs_Btn_Color;
            this.More2.Foreground = AboutUs_Btn_Color;
            this.QRCode_Text.Foreground = AboutUs_Btn_Color;

            this.Image1.Source = new BitmapImage(new Uri("Assets/Images/aboutUs1.png", UriKind.RelativeOrAbsolute));
            this.Image2.Source = new BitmapImage(new Uri("Assets/Images/aboutUs2.jpg", UriKind.RelativeOrAbsolute));
            
        }

        private void OnDonatePageClick(object sender, EventArgs e)
        {
            CollapseAllPages();

            this.DonatePage.Visibility = System.Windows.Visibility.Visible;

        }


        private void CollapseAllPages()
        {
            this.HomePage.Visibility = System.Windows.Visibility.Collapsed;
            this.InformationPage.Visibility = System.Windows.Visibility.Collapsed;
            this.DonatePage.Visibility = System.Windows.Visibility.Collapsed;

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
    }
}