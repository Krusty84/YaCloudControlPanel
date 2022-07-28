using System.Windows;
using System.Windows.Input;

namespace YaCloudControlPanel
{
    /// <summary>
    /// Interaction logic for FunctionDeleteConfirmationWindow.xaml
    /// </summary>
    public partial class FunctionDeleteConfirmationWindow : Window
    {
        private string functionID4Delete;

        public FunctionDeleteConfirmationWindow(YaCloudControlPanel.YaModel.Function functionID)
        {
            this.functionID4Delete = functionID.id;
            this.InitializeComponent();
            this.buttonConfirmDelete.IsEnabled = false;
            Helpers.UsefulStuff.BusyIndicator(this.busyPanel, this.busyProgress, false);
        }

        private void TextBoxConfirmationWord_KeyUp(object sender, KeyEventArgs e)
        {
            this.buttonConfirmDelete.IsEnabled = false;
            if (this.textBoxConfirmationWord.Text == "delete")
            {
                this.buttonConfirmDelete.IsEnabled = true;
            }
        }

        private async void ButtonConfirmDelete_Click(object sender, RoutedEventArgs e)
        {
            Helpers.UsefulStuff.BusyIndicator(this.busyPanel, this.busyProgress, true);
#if DEBUG
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(YaCloudControlPanel.YaFunctions.YaClouds.YaCloudOAuthCode);
#else
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(Helpers.UsefulStuff.SettingsHelper.ReadSetting("General", "OAuthKey"));
#endif
            var task2DeleteFunction = YaCloudControlPanel.YaFunctions.YaClouds.YaCloudFunctions.DeleteExistFunctionAsync(this.functionID4Delete, yaAuthToken);
            await task2DeleteFunction;
            Helpers.UsefulStuff.BusyIndicator(this.busyPanel, this.busyProgress, false);
            this.Close();
        }
    }
}