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
        public MainWindow()
        {
            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
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

            var bc = new BrushConverter();
            
            CollapseAllPages();

            this.HelpPage.Visibility = System.Windows.Visibility.Visible;

            Brush background = (Brush)bc.ConvertFrom("#FFFFFFFF");
            Brush foreground = (Brush)bc.ConvertFrom("#FF9CB208");

            this.Home_Btn.Background = background;
            this.Home_Btn.Foreground = foreground;
            this.Support_Btn.Background = background;
            this.Support_Btn.Foreground = foreground;





            this.Help_Btn.Background = SelectedButtonGradientSet();
            this.Help_Btn.Foreground = (Brush)bc.ConvertFrom("#FF000000");
        
        }


        private void OnHomePageClick(object sender, EventArgs e)
        {
            var bc = new BrushConverter();
            

            CollapseAllPages();

            this.HomePage.Visibility = System.Windows.Visibility.Visible;


            Brush background = (Brush)bc.ConvertFrom("#FFFFFFFF");
            Brush foreground = (Brush)bc.ConvertFrom("#FF9CB208");

            this.Help_Btn.Background = background;
            this.Help_Btn.Foreground = foreground;
            this.Support_Btn.Background = background;
            this.Support_Btn.Foreground = foreground;


            this.Home_Btn.Background = SelectedButtonGradientSet();
            this.Home_Btn.Foreground = (Brush)bc.ConvertFrom("#FF000000");

        }

        private void OnSupportPageClick(object sender, EventArgs e)
        {
            var bc = new BrushConverter();


            CollapseAllPages();

            this.SupportPage.Visibility = System.Windows.Visibility.Visible;


            Brush background = (Brush)bc.ConvertFrom("#FFFFFFFF");
            Brush foreground = (Brush)bc.ConvertFrom("#FF9CB208");

            this.Home_Btn.Background = background;
            this.Home_Btn.Foreground = foreground;
            this.Help_Btn.Background = background;
            this.Help_Btn.Foreground = foreground;


            this.Support_Btn.Background = SelectedButtonGradientSet();
            this.Support_Btn.Foreground = (Brush)bc.ConvertFrom("#FF000000");

        }


        private void CollapseAllPages()
        {
            this.HomePage.Visibility = System.Windows.Visibility.Collapsed;
            this.SupportPage.Visibility = System.Windows.Visibility.Collapsed;
            this.HelpPage.Visibility = System.Windows.Visibility.Collapsed;

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