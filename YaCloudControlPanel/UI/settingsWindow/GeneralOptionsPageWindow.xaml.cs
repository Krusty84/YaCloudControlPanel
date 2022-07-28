using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace YaCloudControlPanel.UI.Settings
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class GeneralOptionsPageWindow : UserControl
    {
        internal GeneralOptionPage generalOptionsPage;
        private string yaAuthToken;
        private string fileNameLog;

        public GeneralOptionsPageWindow()
        {
            this.InitializeComponent();
            this.comboBoxExistsCloud.IsEnabled = false;
            this.comboBoxExistsCloud.Items.Clear();
            Helpers.UsefulStuff.SettingsHelper.CreateCatalog("General");

            if (Helpers.UsefulStuff.SettingsHelper.ReadSetting("General", "OAuthKey") == "Setting is missing")
            {
                this.textBoxOAuthKey.Text = "Enter OAuthKey";
            }
            else
            {
                this.textBoxOAuthKey.Text = Helpers.UsefulStuff.SettingsHelper.ReadSetting("General", "OAuthKey");
            }

            if (Helpers.UsefulStuff.SettingsHelper.ReadSetting("General", "YaCloudName") == "Setting is missing")
            {
            }
            else
            {
                this.comboBoxExistsCloud.Items.Add(new YaCloudControlPanel.YaModel.CloudDataCombobox() { YaCloudName = Helpers.UsefulStuff.SettingsHelper.ReadSetting("General", "YaCloudName"), YaCloudID = Helpers.UsefulStuff.SettingsHelper.ReadSetting("General", "YaCloudID") });

                // this.comboBoxExistsCloud.SelectedIndex = 0;
            }
        }

        public void Initialize()
        {
        }

        private async void ButtonCheckOAuthKey_Click(object sender, RoutedEventArgs e)
        {
            var task1GetAuthToken = YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(this.textBoxOAuthKey.Text);
            this.yaAuthToken = await task1GetAuthToken;
            if (this.yaAuthToken == "OAuth token is invalid or expired")
            {
                this.textBoxOAuthKey.Background = Brushes.MediumVioletRed;
            }
            else
            {
                this.textBoxOAuthKey.Background = Brushes.LightGreen;
                /*this.comboBoxExistsCloud.Items.Clear();

                var task2GetExistsCloud = YaCloudControlPanel.YaFunctions.YaClouds.YaCloudFunctions.GetExistsCloudAsync(this.yaAuthToken);
                YaCloudControlPanel.YaModel.CloudsRoot yaExistsCloud = await task2GetExistsCloud;
                for (int i = 0; i < yaExistsCloud.clouds.Count; i++)
                {
                    this.comboBoxExistsCloud.Items.Add(new YaCloudControlPanel.YaModel.CloudDataCombobox() { YaCloudName = yaExistsCloud.clouds[i].name, YaCloudID = yaExistsCloud.clouds[i].id });
                }

                this.comboBoxExistsCloud.IsEnabled = true;
                */
                Helpers.UsefulStuff.SettingsHelper.WriteSetting("General", "OAuthKey", this.textBoxOAuthKey.Text);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.comboBoxExistsCloud.SelectedItem != null)
            {
                Helpers.UsefulStuff.SettingsHelper.WriteSetting("General", "YaCloudName", ((YaCloudControlPanel.YaModel.CloudDataCombobox)this.comboBoxExistsCloud.SelectedItem).YaCloudName);
                Helpers.UsefulStuff.SettingsHelper.WriteSetting("General", "YaCloudID", ((YaCloudControlPanel.YaModel.CloudDataCombobox)this.comboBoxExistsCloud.SelectedItem).YaCloudID);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
        }

        private void YaCloudConsoleURL_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void GithubProjectURL_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void LogFile_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.fileNameLog);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.fileNameLog = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\YaCloudControlPanel.log";
            if (File.Exists(this.fileNameLog))
            {
                this.logFile_URL.IsEnabled = true;
            }
            else
            {
                this.logFile_URL.IsEnabled = false;
            }
        }
    }
}