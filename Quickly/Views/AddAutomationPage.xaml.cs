using Quickly.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Quickly.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddAutomationPage : Page
    {
        private Automation automation;
        private List<TextBlock> labels;
        private List<TextBox> boxes;
        private List<RadioButton> buttons;
        public AddAutomationPage()
        {
            this.InitializeComponent();
            GenerateAutomationForm();
        }

        private void GenerateAutomationForm()
        {
            labels = new List<TextBlock>();
            boxes = new List<TextBox>();
            buttons = new List<RadioButton>();

            AddTextField("Name", 0);
            AddTextField("Description", 1);
            AddTextField("BusinessArea", 2);
            AddTextField("FunctionalArea", 3);
            AddTextField("ExePath", 4);
            AddSectionLabel("Arguments",5);
            AddArgumentBlock(6);
        }

        private void AddArgumentBlock(int v)
        {

        }

        private void AddSectionLabel(string v1,int v2)
        {
            ///Label for Argument section
            RowDefinition row = new RowDefinition() {
                Height = GridLength.Auto
            };
            
            ///add new label
            TextBlock label = new TextBlock() {
                Text = v1,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 20, 0, 0),
                FontWeight = FontWeights.Bold
        };
            /// set place in the form for the label
            Grid.SetRow(label, v2);
            Grid.SetColumn(label, 0);
            labels.Add(label);
            FormGrid.Children.Add(label);
            FormGrid.RowDefinitions.Add(row);
        }

        private void AddTextField(string v1, int v2)
        {
            RowDefinition row = new RowDefinition() {
                Height = GridLength.Auto
            };

            ///add new label
            TextBlock label = new TextBlock() {
                Text = v1,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 10, 0, 0)
            };

            /// set place in the form for the label
            Grid.SetRow(label, v2);
            Grid.SetColumn(label, 0);
            labels.Add(label);
            FormGrid.Children.Add(label);

            TextBox box = new TextBox() {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 10, 0, 0),
                MinWidth = 100
            };
            boxes.Add(box);
            Grid.SetRow(box, v2);
            Grid.SetColumn(box, 1);
            FormGrid.Children.Add(box);

            FormGrid.RowDefinitions.Add(row);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
