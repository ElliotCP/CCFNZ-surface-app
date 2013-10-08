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

// Alias for Point because of the namespace clash between
// System.Window.Point and System.Drawing.Point
using WinPoint = System.Windows.Point;
using WinColor = System.Windows.Media.Color;


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

            //var bc = new BrushConverter();
            
            //CollapseAllPages();

            //this.HelpPage.Visibility = System.Windows.Visibility.Visible;

            //Brush background = (Brush)bc.ConvertFrom("#FFFFFFFF");
            //Brush foreground = (Brush)bc.ConvertFrom("#FF9CB208");

            //this.Home_Btn.Background = background;
            //this.Home_Btn.Foreground = foreground;
            //this.Support_Btn.Background = background;
            //this.Support_Btn.Foreground = foreground;





            //this.Help_Btn.Background = SelectedButtonGradientSet();
            //this.Help_Btn.Foreground = (Brush)bc.ConvertFrom("#FF000000");
        
        }


        private void OnHomePageClick(object sender, EventArgs e)
        {
            //var bc = new BrushConverter();
            

            //CollapseAllPages();

            //this.HomePage.Visibility = System.Windows.Visibility.Visible;


            //Brush background = (Brush)bc.ConvertFrom("#FFFFFFFF");
            //Brush foreground = (Brush)bc.ConvertFrom("#FF9CB208");

            //this.Help_Btn.Background = background;
            //this.Help_Btn.Foreground = foreground;
            //this.Support_Btn.Background = background;
            //this.Support_Btn.Foreground = foreground;


            //this.Home_Btn.Background = SelectedButtonGradientSet();
            //this.Home_Btn.Foreground = (Brush)bc.ConvertFrom("#FF000000");

        }

        private void OnSupportPageClick(object sender, EventArgs e)
        {
            //var bc = new BrushConverter();


            //CollapseAllPages();

            //this.SupportPage.Visibility = System.Windows.Visibility.Visible;


            //Brush background = (Brush)bc.ConvertFrom("#FFFFFFFF");
            //Brush foreground = (Brush)bc.ConvertFrom("#FF9CB208");

            //this.Home_Btn.Background = background;
            //this.Home_Btn.Foreground = foreground;
            //this.Help_Btn.Background = background;
            //this.Help_Btn.Foreground = foreground;


            //this.Support_Btn.Background = SelectedButtonGradientSet();
            //this.Support_Btn.Foreground = (Brush)bc.ConvertFrom("#FF000000");

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
            lgb.StartPoint = new WinPoint(0.5, 0);
            lgb.EndPoint = new WinPoint(0.5, 1);
            lgb.GradientStops.Add(new GradientStop(WinColor.FromArgb(225, 224, 230, 172), 0.0));
            lgb.GradientStops.Add(new GradientStop(WinColor.FromArgb(225, 192, 215, 45), 0.4));

            return lgb;
        }

        private void SurfaceWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        // Probably not logical place to put this function.
        // But for testing purposes its situated here. - Amruth
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
        private BitmapImage convertImageToImageSource(System.Drawing.Image img) { 
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

        private void qrgen_btn_onclick(object sender, RoutedEventArgs e)
        {
            // TODO Replace the harded string with <ip-address>:8080/amount=xx
            System.Drawing.Image img = QRGenerator("This is a test", 7);
            this.QrCode_img.Source = convertImageToImageSource(img);
        }

    }
}