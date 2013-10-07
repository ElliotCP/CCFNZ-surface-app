﻿using System;
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
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : SurfaceWindow
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void NavHelp(object sender, RoutedEventArgs e)
        {
            this.frame1.Navigate(new Uri("HelpPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
