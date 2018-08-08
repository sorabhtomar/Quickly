using Quickly.Domain.SchemaModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Quickly.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContentPage : Page
    {
        
        private List<TextBlock> labels;
        private List<TextBox> boxes;
        private List<RadioButton> buttons;
        private Queue<string> userValues;
        private List<string> command;

        public ContentPage()
        {
            this.InitializeComponent();
            ShowContent();
        }

        private void ShowContent()
        {
            contentGrid.Children.Clear();
            DescriptionBlock.Text = App.automationInfo.Description;
            labels = new List<TextBlock>();
            boxes = new List<TextBox>();
            buttons = new List<RadioButton>();

            int i = 0;
            foreach (Argument arg in App.automationInfo.Arguments) {
                if (arg.AskUser == true) {

                    RowDefinition row = new RowDefinition() {
                        Height = GridLength.Auto
                    };

                    TextBlock label = new TextBlock() {
                        Text = arg.AskPhrase,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(10, 0, 0, 0)
                    };
                    Grid.SetRow(label, i);
                    Grid.SetColumn(label, 0);
                    labels.Add(label);
                    contentGrid.Children.Add(label);

                    if (arg.Options.Count == 0) {
                        TextBox box = new TextBox() {
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(10, 0, 0, 0)
                        };
                        boxes.Add(box);
                        Grid.SetRow(box, i);
                        Grid.SetColumn(box, 1);
                        contentGrid.Children.Add(box);
                    }
                    else {
                        int j = 0;
                        foreach (Option o in arg.Options)
                        //for(int k=0;k<10;k++)
                        {
                            RadioButton rb = new RadioButton() {
                                Content = o.Value,
                                GroupName = ("Radio" + i.ToString()),
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center,
                                Margin = new Thickness((j * 100) + 10, 0, 0, 0)
                            };
                            buttons.Add(rb);
                            Grid.SetRow(rb, i);
                            Grid.SetColumn(rb, 1);
                            contentGrid.Children.Add(rb);
                            j++;
                        }
                    }
                    contentGrid.RowDefinitions.Add(row);
                    i++;
                }
            }
            RunButton.Visibility = Visibility.Visible;
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            int missFlag = 0;
            userValues = new Queue<string>();
            command = new List<string>();
            foreach (UIElement v in contentGrid.Children.Where(v => v.GetType().ToString() != "Windows.UI.Xaml.Controls.TextBlock")) {
                if (missFlag > 0) {
                    missFlag--;
                    continue;
                }
                string group = null;
                if (v.GetType().ToString() == "Windows.UI.Xaml.Controls.TextBox") {
                    userValues.Enqueue(((TextBox)v).Text.ToString());
                }
                else if (v.GetType().ToString() == "Windows.UI.Xaml.Controls.RadioButton") {
                    RadioButton rb = (RadioButton)v;
                    group = rb.GroupName;

                    int buttoncount = 0;
                    RadioButton checkedOne = null; // = new RadioButton();
                    foreach (UIElement v1 in contentGrid.Children.Where
                        (v1 => (v1.GetType().ToString() == "Windows.UI.Xaml.Controls.RadioButton") && (
                        (RadioButton)v1).GroupName == group)) {
                        if (((RadioButton)v1).IsChecked == true) {
                            checkedOne = new RadioButton();
                            checkedOne = (RadioButton)v1;
                        }
                        buttoncount++;
                    }
                    missFlag = buttoncount - 1;
                    if (checkedOne == null)  //(Object.ReferenceEquals(checkedOne,null) || object.Equals(null, checkedOne))
                    {
                        userValues.Enqueue("");
                    }
                    else
                        userValues.Enqueue(checkedOne.Content.ToString());
                }
            }

            foreach (Argument arg in App.automationInfo.Arguments) {
                if (string.IsNullOrWhiteSpace(arg.Value) == false) {
                    command.Add(arg.Value);
                }
                else {
                    if (arg.Options.Count == 0)//textbox
                    {
                        string qval = userValues.Dequeue();
                        if (string.IsNullOrWhiteSpace(qval) == false)
                            command.Add(qval);
                    }
                    else//radiobutton
                    {
                        string qval = userValues.Dequeue();
                        foreach (Option o in arg.Options) {
                            if (qval == o.Value) {
                                if (string.IsNullOrWhiteSpace(o.ArgString) == false)
                                    command.Add(o.ArgString);
                            }
                        }
                    }
                }
            }
            var task = Task.Run(async () => await CreateFileAsync());
            task.Wait();
            //RunButton.Visibility = Visibility.Collapsed;
            ContentDialog SuccessDialog = new ContentDialog {
                Title = "Success",
                Content = "Command triggered successfully!",
                PrimaryButtonText = "OK"
            };
            var result = SuccessDialog.ShowAsync();
        }

        private async Task CreateFileAsync()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await storageFolder.CreateFileAsync(
                @"command.txt", CreationCollisionOption.ReplaceExisting);
            await storageFolder.GetFileAsync("command.txt");
            //await FileIO.AppendTextAsync(sampleFile, automation.Path);
            await FileIO.AppendTextAsync(sampleFile, App.automationInfo.Path + "\\" + App.automationInfo.Target + " ");
            foreach (string s in command)
                await FileIO.AppendTextAsync(sampleFile, s + " ");
            await sampleFile.RenameAsync("command.bat", NameCollisionOption.ReplaceExisting);
        }
    }
}
