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

        private void OnHelpPageClick(object sender, EventArgs e)
        {

            
            CollapseAllPages();

            this.InformationPage.Visibility = System.Windows.Visibility.Visible;

            this.InformationPageTitle.Text = "How Can I Help?";

            this.Pointer.Source = new BitmapImage(new Uri("Assets/Icons/pointer_orange.png", UriKind.RelativeOrAbsolute));

            this.More1.Foreground = Help_Btn_Color;
            this.More2.Foreground = Help_Btn_Color;
            this.QRCode_Text.Foreground = Help_Btn_Color;
        
        }


        private void OnHomePageClick(object sender, EventArgs e)
        {

            CollapseAllPages();

            this.HomePage.Visibility = System.Windows.Visibility.Visible;

        }

        private void OnSupportPageClick(object sender, EventArgs e)
        {
            
            CollapseAllPages();

            this.InformationPage.Visibility = System.Windows.Visibility.Visible;
            this.InformationPageTitle.Text = "How Can I Get Support?";

            this.Pointer.Source = new BitmapImage(new Uri("Assets/Icons/pointer_red.png", UriKind.RelativeOrAbsolute));

            this.More1.Foreground = Support_Btn_Color;
            this.More2.Foreground = Support_Btn_Color;
            this.QRCode_Text.Foreground = Support_Btn_Color;

        }

        private void OnAboutUsPageClick(object sender, EventArgs e)
        {
            CollapseAllPages();

            this.InformationPage.Visibility = System.Windows.Visibility.Visible;
            this.InformationPageTitle.Text = "What Is CCFNZ?";

            this.Pointer.Source = new BitmapImage(new Uri("Assets/Icons/pointer_blue.png", UriKind.RelativeOrAbsolute));

            this.More1.Foreground = AboutUs_Btn_Color;
            this.More2.Foreground = AboutUs_Btn_Color;
            this.QRCode_Text.Foreground = AboutUs_Btn_Color;
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