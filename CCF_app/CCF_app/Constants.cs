﻿using System;
using System.Windows.Media.Animation;

namespace CCF_app
{
    public class Constants
    {
        // Strings for location of images within app package

        // Set the donation target
        public const int DonateTarget = 10000;

        // Set the number of days to go for donation
        public const int DonateDaysToGo = 15;

        // Sets the videos to begin automatically on app start/page change

        // Screen saver delay
        public const int ScreenSaverWaitTime = 50;

        // Donation progress bar delay
        public const int ProgressBarWaitTime = 20;

        // Carousel image swipe threshold
        public const int CarouselImageSwipeThreshold = 100;

        // Kept here rather than XML to keep everything all in one place //
        // About us first box
        public const string AboutUsText1 =
            "Child Cancer Foundation New Zealand's mission is that every child and their family walking the child cancer journey will never feel alone." +
            "\nEvery week in New Zealand three families are told their child has cancer. We support these families from the very beginning. By doing this" +
            " we reduce isolation and the impact of cancer. We aim to reduce the impact of cancer by offering services to ensure children and their families" +
            " are supported, informed and well cared for on their journey with cancer.";

        // About us second box
        public const string AboutUsText2 =
            "This assistance is delivered throughout New Zealand by our Family Support team working in conjunction with the foundation's branch members (parents and volunteers) in the local community." +
            "\nEach year we need at least $6 million to continue our services. This is raised through the generosity of individuals, grants, donations and sponsorships." +
            "\nThe Foundation's work with children with cancer and their families is unique and receives no direct government funding or support from other cancer agencies.";

        // How can I help first box
        public const string HelpText1 =
            "We rely on the generosity of big-hearted New Zealander's to help us continue what we do. There are a variety of ways you can support children with cancer and their families." +
            "\nOur Beads of Courage ?presents children with a bead representing an Act of Courage. Ideally (and sadly) this year, we expect that we will need approximately 5000" +
            " handmaid beads donated. We are currently reaching only 1800 and we need all the help we can get to help our kids.";

        // How can I help second box
        public const string HelpText2 =
            "We rely on donations to continue our services. You can make a one-off donation through your credit card; it is simple, secure and super rewarding." +
            "\nYou can become a regular supporter of Child Cancer Foundation by setting up a regular donation from your credit card or bank account. More information on donations can be found on our website." +
            "\nEvery donation, no matter how big or small, helps us continue to support our children and families affected by this traumatic disease.";

        // How can I get support first box
        public const string SupportText1 =
            "Our Family Support team work in conjunction with the foundation’s branch members (parents, caregivers, and volunteers) to deliver a range of support services to ensure every child" +
            " and their family walking the child cancer journey will never feel alone. They offer individual and group support, information, financial assistance, and advocacy." +
            " Our Coordinators also offer support for bereaved families. They connect similar families and provide a link to other agencies and community support groups.";

        // How can I get support second box
        public const string SupportText2 =
            "There are a variety of local and regional child, parent, grandparent, sibling and bereaved support programmes and events that aim to inform, reduce isolation and" +
            " support your family through the experiences and challenges of child cancer." +
            " Parent events, children’s holiday programmes and sibling days are among many that are well attended. ";


        // Youtube links for the media elements
        private const string Prefix = "<html>" + "<body scroll=no>" + "<iframe src=\"" + "http://www.youtube.com/embed";

        private const string Suffix =
            "?html5=1&controls=2&showinfo=0&showsearch=0&loop=1&fs=0&modestbranding=1&rel=0\"  width=\"100%\" height=\"100%\" frameborder=\"0\"  />" +
            "</iframe>" + "</body>" + "</html>";

        public const string YoutubeVideo_About = Prefix + "/AlddeaxWxcs" + Suffix;
        public const string YoutubeVideo_Help = Prefix + "/c3inKzoQ3UE" + Suffix;
        public const string YoutubeVideo_Support = Prefix + "/oBMd_CKEyIY" + Suffix;
        public const int TwitterRefreshRate = 180;
        public static readonly DoubleAnimation Da = new DoubleAnimation(0, TimeSpan.FromMilliseconds(70));
        public static readonly DoubleAnimation Da2 = new DoubleAnimation(40, TimeSpan.FromMilliseconds(70));
        public static readonly DoubleAnimation Da3 = new DoubleAnimation(1, TimeSpan.FromMilliseconds(400));
        public static readonly DoubleAnimation Da4 = new DoubleAnimation(0, TimeSpan.FromMilliseconds(400));

        // Sets which image is the image on the home page for the app when it is run
        public static MainWindow.HomePageImages CurrentHomeImage = MainWindow.HomePageImages.Img3;

        // Set the amount donated when the app is run
        public static int DonatePercentFunded = 60;

        // Set the cash amount donated at start
        public static int DonateTotalDonated = 6000;
        public static Boolean Playing = true;

        public readonly DoubleAnimation Animation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.3));

        public readonly string[] BackgroundImages =
        {
            "pack://application:,,,/CCF_app;component/Assets/Images/HomePage_Pic1.jpg",
            "pack://application:,,,/CCF_app;component/Assets/Images/HomePage_Pic2.jpg",
            "pack://application:,,,/CCF_app;component/Assets/Images/HomePage_Pic3.jpg"
        };

        public readonly DoubleAnimation Unanimation = new DoubleAnimation(0.5, TimeSpan.FromSeconds(0.3));

        public const int ImageWidth = 1920;
        public const int ImageHeight = 1200;
    }
}