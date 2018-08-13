using Quickly.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using Windows.Storage;
using System.Linq;
using Quickly.Views;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Quickly.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private List<Automation> Automations;

        //public static Automation automation;
        public HomePage()
        {
            this.InitializeComponent();
            Automations = Task.Run(async () => await AutomationManager.GetAutomationsAsync()).Result;
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            App.automationInfo = (Automation)e.ClickedItem;
            ContentFrame.Navigate(typeof(ContentPage));
        }
    }
}
