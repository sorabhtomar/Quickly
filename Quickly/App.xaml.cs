using Quickly.Domain.SchemaModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Media.SpeechRecognition;
using Windows.System;
using Quickly.Models;
using Quickly.Views;

namespace Quickly
{

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static Automation automationInfo;
        public static List<Automation> allAutomationInfo;
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            ///n1yyas
            ///Code for installing VCD file
            await RegisterVCDAsync();

            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame = new Frame();
            rootFrame.NavigationFailed += OnNavigationFailed;

            rootFrame.Navigate(typeof(MainPage));
            Window.Current.Content = rootFrame;
            Window.Current.Activate();

            //n1yyas -- add code for helppage navigation
        }

        private async Task RegisterVCDAsync()
        {
            try {
                // Install the main VCD. 
                StorageFile vcdStorageFile = await Package.Current.InstalledLocation
                    .GetFileAsync(@"QuicklyCommands_1.xml");

                await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.
                    InstallCommandDefinitionsFromStorageFileAsync(vcdStorageFile);
                System.Diagnostics.Debug.WriteLine("Installed Voice Commands");
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("Installing Voice Commands Failed: " + ex.ToString());
            }
        }

        protected async override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            if (args.Kind == ActivationKind.VoiceCommand) {

                var commandArgs = args as VoiceCommandActivatedEventArgs;
                SpeechRecognitionResult speechRecognitionResult = commandArgs.Result;
                string voiceCommandName = speechRecognitionResult.RulePath[0];
                string textSpoken = speechRecognitionResult.Text;
                string commandMode = this.SemanticInterpretation("commandMode", speechRecognitionResult);
                string label;
                //string label1 = this.SemanticInterpretation("url", speechRecognitionResult);
                //label = textSpoken.Split("wake up and create");
                string xmlfilename = null;
                //automationInfo = await ParseAutomationFileAsync(@"TIA.xml");

                Collection collection = await Collection.GetCollectionAsync();

                switch (voiceCommandName) {
                    case "Automation": {
                            label = this.SemanticInterpretation("command", speechRecognitionResult);
                            foreach (command c in collection.commands) {
                                if (label == c.key) {
                                    xmlfilename = c.value;
                                    break;
                                }
                            }

                            automationInfo = await ParseAutomationFileAsync(xmlfilename);

                            Frame rootFrame = Window.Current.Content as Frame;
                            rootFrame = new Frame();
                            rootFrame.NavigationFailed += OnNavigationFailed;

                            rootFrame.Navigate(typeof(ContentPage), textSpoken);
                            Window.Current.Content = rootFrame;
                            Window.Current.Activate();
                            break;
                        }
                    case "URLaunch": {
                            label = this.SemanticInterpretation("url", speechRecognitionResult);
                            Uri website;
                            switch (label) {
                                case "time card": {
                                        website = new Uri(@"https://tiweb.industrysoftware.automation.siemens.com/tc3/tc3_start.cgi");
                                        break;
                                    }
                                case "help desk": {
                                        website = new Uri(@"https://helpdesk.industrysoftware.automation.siemens.com/CAisd/pdmweb.exe");
                                        break;
                                    }
                                case "STT": {
                                        website = new Uri(@"https://tiweb.industrysoftware.automation.siemens.com/stt/stt.cgi");
                                        break;
                                    }
                                default: {

                                        website = new Uri(@"http:\\www.google.com");
                                        break;
                                    }
                            }

                            var success = await Launcher.LaunchUriAsync(website);
                            break;
                        }

                }

            }
            else if (args.Kind == ActivationKind.Protocol) {
                var commandArgs = args as ProtocolActivatedEventArgs;
                Windows.Foundation.WwwFormUrlDecoder decoder = new Windows.Foundation.WwwFormUrlDecoder(commandArgs.Uri.Query);
                var destination = decoder.GetFirstValueByName("LaunchContext");
            }
            //Frame rootFrame = Window.Current.Content as Frame;
            //rootFrame = new Frame();
            //rootFrame.NavigationFailed += OnNavigationFailed;
            //automationInfo = await ParseAutomationFileAsync();
            //rootFrame.Navigate(typeof(MainPage));
            //Window.Current.Content = rootFrame;
        }

        public static async Task<Automation> ParseAutomationFileAsync(string filename)
        {
            StorageFolder storageFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Automations");
            StorageFile sampleFile = await storageFolder.GetFileAsync(filename);
            string text = await FileIO.ReadTextAsync(sampleFile);
            var serializer = new XmlSerializer(typeof(Automation));

            TextReader reader = new StringReader(text);

            var result = (Automation)serializer.Deserialize(reader);

            reader.Dispose();

            return result;
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
        private string SemanticInterpretation(string interpretationKey, SpeechRecognitionResult speechRecognitionResult)
        {
            return speechRecognitionResult.SemanticInterpretation.Properties[interpretationKey].FirstOrDefault();
        }
    }
}
