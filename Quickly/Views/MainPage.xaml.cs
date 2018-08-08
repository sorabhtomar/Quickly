using Quickly.Domain.SchemaModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Quickly
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            PageFrame.Navigate(typeof(HomePage));
        }
        
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            PageFrame.Navigate(typeof(HomePage));
            //Navigate to Home Page
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            //Navigate to Add Automation page
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            PageFrame.Navigate(typeof(HelpPage));
            //Navigate to HelpPage
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}