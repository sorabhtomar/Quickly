﻿using Quickly.Domain.SchemaModels;
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
        private Automation automation;
        private List<TextBlock> labels;
        private List<TextBox> boxes;
        private List<RadioButton> buttons;
        private Queue<string> userValues;
        private List<string> command;

        public MainPage()
        {
            this.InitializeComponent();
            this.MyLayout();
        }

        private void MyLayout()
        {
            if (App.automationInfo == null)
            {
                showhelp();
                return;
            }
            automation = App.automationInfo;
            pageTitle.Text = (automation.Name + " Quickly"); //Update Title in the first Row.
            labels = new List<TextBlock>();
            boxes = new List<TextBox>();
            buttons = new List<RadioButton>();


            int i = 0;
            foreach (Argument arg in automation.Arguments)
            {
                if (arg.AskUser == true)
                {

                    RowDefinition row = new RowDefinition()
                    {
                        Height = GridLength.Auto
                    };

                    TextBlock label = new TextBlock()
                    {
                        Text = arg.AskPhrase,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(10, 0, 0, 0)
                    };
                    Grid.SetRow(label, i);
                    Grid.SetColumn(label, 0);
                    labels.Add(label);
                    contentGrid.Children.Add(label);

                    if (arg.Options.Count == 0)
                    {
                        TextBox box = new TextBox()
                        {
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(10, 0, 0, 0)
                        };
                        boxes.Add(box);
                        Grid.SetRow(box, i);
                        Grid.SetColumn(box, 1);
                        contentGrid.Children.Add(box);
                    }
                    else
                    {
                        int j = 0;
                        foreach (Option o in arg.Options)
                        //for(int k=0;k<10;k++)
                        {

                            RadioButton rb = new RadioButton()
                            {
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




            //mainGrid.HorizontalAlignment = HorizontalAlignment.Left;
            //mainGrid.VerticalAlignment = VerticalAlignment.Top;

            //RowDefinition row1 = new RowDefinition();
            //row1.Height = new GridLength(60, GridUnitType.Pixel);
            //RowDefinition row2 = new RowDefinition();
            //row2.Height = GridLength.Auto;
            //RowDefinition row3 = new RowDefinition();
            //row3.Height = GridLength.Auto;

            //RowDefinition row4 = new RowDefinition();
            //row4.Height = new GridLength(1, GridUnitType.Star);
            //RowDefinition row5 = new RowDefinition();
            //row5.Height = GridLength.Auto;

            //mainGrid.RowDefinitions.Add(row1);
            //mainGrid.RowDefinitions.Add(row2);
            //mainGrid.RowDefinitions.Add(row3);
            //mainGrid.RowDefinitions.Add(row4);
            //mainGrid.RowDefinitions.Add(row5);

            //this.Content = mainGrid;




        }

        private void showhelp()
        {
            
        }

        private void pageTitle_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (App.automationInfo == null)
                return;
            int missFlag = 0;
            userValues = new Queue<string>();
            command = new List<string>();
            foreach (UIElement v in contentGrid.Children.Where(v => v.GetType().ToString() != "Windows.UI.Xaml.Controls.TextBlock"))
            {
                if (missFlag > 0)
                {
                    missFlag--;
                    continue;
                }
                string group = null;
                if (v.GetType().ToString() == "Windows.UI.Xaml.Controls.TextBox")
                {
                    userValues.Enqueue(((TextBox)v).Text.ToString());
                }
                else if (v.GetType().ToString() == "Windows.UI.Xaml.Controls.RadioButton")
                {
                    RadioButton rb = (RadioButton)v;
                    group = rb.GroupName;

                    int buttoncount = 0;
                    RadioButton checkedOne = null; // = new RadioButton();
                    foreach (UIElement v1 in contentGrid.Children.Where
                        (v1 => (v1.GetType().ToString() == "Windows.UI.Xaml.Controls.RadioButton") && (
                        (RadioButton)v1).GroupName == group))
                    {
                        if (((RadioButton)v1).IsChecked == true)
                        {
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

            foreach (Argument arg in automation.Arguments)
            {
                if (string.IsNullOrWhiteSpace(arg.Value) == false)
                {
                    command.Add(arg.Value);
                }
                else
                {
                    if (arg.Options.Count == 0)//textbox
                    {
                        string qval = userValues.Dequeue();
                        if (string.IsNullOrWhiteSpace(qval) == false)
                            command.Add(qval);
                    }
                    else//radiobutton
                    {
                        string qval = userValues.Dequeue();
                        foreach (Option o in arg.Options)
                        {
                            if (qval == o.Value)
                            {
                                if (string.IsNullOrWhiteSpace(o.ArgString) == false)
                                    command.Add(o.ArgString);
                            }
                        }
                    }
                }
            }
            var task = Task.Run(async () => await CreateFileAsync());
            task.Wait();
            App.Current.Exit();
        }

        private async System.Threading.Tasks.Task CreateFileAsync()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await storageFolder.CreateFileAsync(
                @"command.txt", CreationCollisionOption.ReplaceExisting);
            await storageFolder.GetFileAsync("command.txt");
            //await FileIO.AppendTextAsync(sampleFile, automation.Path);
            await FileIO.AppendTextAsync(sampleFile,"F:\\Setup\\"+automation.Name+"\\"+automation.Target+" ");
            foreach (string s in command)
            await FileIO.AppendTextAsync(sampleFile,s+" ");
            await sampleFile.RenameAsync("command.bat",NameCollisionOption.ReplaceExisting);
        }
    }
}










            //else
            //{
            //    if(contentGrid.Children[i].GetType().ToString() == "Windows.UI.Xaml.Controls.TextBox")
            //    {
            //        TextBox tb = (TextBox)contentGrid.Children[i];
            //        command.Add(tb.Text);
            //    }
            //    if (contentGrid.Children[i].GetType().ToString() == "Windows.UI.Xaml.Controls.RadioButton")
            //    {
            //        if(((RadioButton)contentGrid.Children[i]).IsChecked == true)
            //        {
            //            command.Add(((RadioButton)contentGrid.Children[i]).Content.ToString());

            //        }
            //    }
            //    i++;
        

        //foreach (UIElement v in contentGrid.Children.Where(v => v.GetType().ToString() != "Windows.UI.Xaml.Controls.TextBlock"))
        //{

        //}
    







//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Runtime.InteropServices.WindowsRuntime;
//using Windows.Foundation;
//using Windows.Foundation.Collections;
//using Windows.UI.Xaml;
//using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Controls.Primitives;
//using Windows.UI.Xaml.Data;
//using Windows.UI.Xaml.Input;
//using Windows.UI.Xaml.Media;
//using Windows.UI.Xaml.Navigation;

//// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

//namespace Quickly
//{
//    /// <summary>
//    /// An empty page that can be used on its own or navigated to within a Frame.
//    /// </summary>
//    public sealed partial class MainPage : Page
//    {
//        public MainPage()
//        {
//            this.InitializeComponent();
//            this.DynamicForm(3);
//        }
//        private void pageTitle_SelectionChanged(object sender, RoutedEventArgs e)
//        {

//        }

//        public void DynamicForm(int v)
//        {//< TextBox HorizontalAlignment = "Left" Margin = "1427,1346,0,-298" TextWrapping = "Wrap" Text = "TextBox" VerticalAlignment = "Top" />
//            {
//                Grid grid = new Grid();
//                grid.Width = 200;
//                grid.Height = 50;
//                grid.Margin = new Thickness(50, 50, 0, 0);
//                grid.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
//                RowDefinition titleRow = new RowDefinition();
//                TextBlock titleBlock = new TextBlock();
//                titleBlock.Text = "AutomationHost";
//                grid.RowDefinitions.Add(titleRow);

//                ColumnDefinition col1 = new ColumnDefinition();
//                col1.Width = new GridLength(0, GridUnitType.Auto);
//                ColumnDefinition col2 = new ColumnDefinition();
//                col1.Width = new GridLength(1, GridUnitType.Auto);
//                grid.ColumnDefinitions.Add(col1);
//                grid.ColumnDefinitions.Add(col2);

//                //for ( int i = 0; i < 5; i++)
//                //{
//                //    CheckBox cbox = new CheckBox();
//                //    cbox.MinWidth = 32;
//                //    cbox.HorizontalAlignment = HorizontalAlignment.Left;
//                //    cbox.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
//                //    grid.Children.Add(cbox);
//                //    Grid.SetColumn(cbox, i);
//                //}
//                //ColumnDefinition col1 = new ColumnDefinition();
//                //ColumnDefinition col2 = new ColumnDefinition();
//                //ColumnDefinition col3 = new ColumnDefinition();
//                //col1.Width = new GridLength(0, GridUnitType.Auto);
//                //col2.Width = new GridLength(0, GridUnitType.Auto);
//                //col3.Width = new GridLength(1, GridUnitType.Star);
//                //grid.ColumnDefinitions.Add(col1);
//                //grid.ColumnDefinitions.Add(col2);
//                //grid.ColumnDefinitions.Add(col3);
//                //CheckBox cbox = new CheckBox();
//                //cbox.MinWidth = 32;
//                //cbox.HorizontalAlignment = HorizontalAlignment.Left;
//                //cbox.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
//                //TextBlock tblock = new TextBlock();
//                //tblock.FontSize = 16;
//                //tblock.HorizontalAlignment = HorizontalAlignment.Left;
//                //tblock.VerticalAlignment = VerticalAlignment.Center;
//                //tblock.Text = "text";
//                TextBox tbox = new TextBox();
//                tbox.FontSize = 16;
//                tbox.HorizontalAlignment = HorizontalAlignment.Left;
//                tbox.VerticalAlignment = VerticalAlignment.Center;
//                //grid.Children.Add(cbox);
//                //grid.Children.Add(tblock);
//                grid.Children.Add(tbox);
//                //Grid.SetColumn(cbox, 0);
//                //Grid.SetColumn(tblock, 1);
//                Grid.SetColumn(titleBlock, 2);
//                Grid.SetColumn(tbox, 2);
//                Window.Current.Content = grid;
//                Window.Current.Activate();
//            }
//        }
//    }
//}
