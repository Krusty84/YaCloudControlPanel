using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace YaCloudControlPanel
{
    /// <summary>
    /// Interaction logic for CreateNewFunction.xaml
    /// </summary>
    public partial class CreateNewOrRenameFunction : Window
    {
        private string folderIDRoot4Function;
        private string functionID4Rename;
        private string selectedFunction;
        private string selectedFunctionDesc;
        private bool isNewFunction;
        private Regex regex = new Regex("^[a-z][-a-z0-9]{1,61}[a-z0-9]*$");

        public CreateNewOrRenameFunction(YaCloudControlPanel.YaModel.Folder folderID)
        {
            this.folderIDRoot4Function = folderID.id;
            this.InitializeComponent();
            this.buttonCreateOrRenameFunction.IsEnabled = false;
            this.textBoxNewFunctionDesc.IsEnabled = false;
            this.buttonCreateOrRenameFunction.Content = "Create";
            this.Title = "Create a new Function";
            this.isNewFunction = true;
        }

        public CreateNewOrRenameFunction(YaCloudControlPanel.YaModel.Function functionID)
        {
            // folderIDRoot4Function = folderID.id;
            this.folderIDRoot4Function = functionID.folderId;
            this.functionID4Rename = functionID.id;
            this.selectedFunction = functionID.name;
            if (functionID.description != null)
            {
                this.selectedFunctionDesc = functionID.description;
            }

            this.InitializeComponent();
            this.buttonCreateOrRenameFunction.IsEnabled = false;
            this.textBoxNewFunctionDesc.IsEnabled = false;
            this.buttonCreateOrRenameFunction.Content = "Rename";
            this.Title = "Rename Function";
            this.isNewFunction = false;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Helpers.UsefulStuff.BusyIndicator(this.busyPanel, this.busyProgress, true);
#if DEBUG
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(YaCloudControlPanel.YaFunctions.YaClouds.YaCloudOAuthCode);
#else
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(Helpers.UsefulStuff.SettingsHelper.ReadSetting("General", "OAuthKey"));
#endif
            var task2GetFunctions = YaCloudControlPanel.YaFunctions.YaClouds.YaCloudFunctions.GetExistsFunctionsAsync(this.folderIDRoot4Function, yaAuthToken);
            YaCloudControlPanel.YaModel.FunctionsRoot existFunctions = await task2GetFunctions;

            if (existFunctions.functions != null && existFunctions.functions.Count != 0)
            {
                for (int i = 0; i < existFunctions.functions.Count; i++)
                {
                    this.comboBoxNewFunctionName.Items.Add(existFunctions.functions[i].name);
                }
            }

            this.comboBoxNewFunctionName.Text = this.selectedFunction;
            this.textBoxNewFunctionDesc.Text = this.selectedFunctionDesc;
            Helpers.UsefulStuff.BusyIndicator(this.busyPanel, this.busyProgress, false);
        }

        private async void ButtonCreateOrRenameFunction_Click(object sender, RoutedEventArgs e)
        {
            Helpers.UsefulStuff.BusyIndicator(this.busyPanel, this.busyProgress, true);

#if DEBUG
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(YaCloudControlPanel.YaFunctions.YaClouds.YaCloudOAuthCode);
#else
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(Helpers.UsefulStuff.SettingsHelper.ReadSetting("General", "OAuthKey"));
#endif
            if (this.isNewFunction == true)
            {
                var task2CreateNewFunction = YaCloudControlPanel.YaFunctions.YaClouds.YaCloudFunctions.CreateNewFunctionAsync(this.folderIDRoot4Function, this.comboBoxNewFunctionName.Text, this.textBoxNewFunctionDesc.Text, yaAuthToken);
                await task2CreateNewFunction;
            }
            else
            {
                var task2EditNameExistFunction = YaCloudControlPanel.YaFunctions.YaClouds.YaCloudFunctions.RenameExistFunctionAsync(this.functionID4Rename, this.comboBoxNewFunctionName.Text, this.textBoxNewFunctionDesc.Text, yaAuthToken);
                await task2EditNameExistFunction;
            }

            // Update after creaded/edited function
            this.comboBoxNewFunctionName.Items.Clear();

            // textBoxNewFunctionDesc.Clear();
            var task2GetFunctions = YaCloudControlPanel.YaFunctions.YaClouds.YaCloudFunctions.GetExistsFunctionsAsync(this.folderIDRoot4Function, yaAuthToken);
            YaCloudControlPanel.YaModel.FunctionsRoot existFunctions = await task2GetFunctions;

            if (existFunctions.functions != null && existFunctions.functions.Count != 0)
            {
                for (int i = 0; i < existFunctions.functions.Count; i++)
                {
                    this.comboBoxNewFunctionName.Items.Add(existFunctions.functions[i].name);
                }
            }

            // comboBoxNewFunctionName.Text = selectedFunction;
            // textBoxNewFunctionDesc.Text = selectedFunctionDesc;
            Helpers.UsefulStuff.BusyIndicator(this.busyPanel, this.busyProgress, false);
        }

        private void ComboBoxNewFunctionName_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.isNewFunction == true)
            {
                this.buttonCreateOrRenameFunction.IsEnabled = false;
                this.textBoxNewFunctionDesc.IsEnabled = false;
                if (this.comboBoxNewFunctionName.Items.Count > 0)
                {
                    if ((this.comboBoxNewFunctionName.SelectedItem == this.comboBoxNewFunctionName.Text) && this.comboBoxNewFunctionName.Text.Length <= 0)
                    {
                        this.buttonCreateOrRenameFunction.IsEnabled = false;
                        this.textBoxNewFunctionDesc.IsEnabled = false;
                    }
                    else if ((this.comboBoxNewFunctionName.SelectedItem != this.comboBoxNewFunctionName.Text) && this.regex.IsMatch(this.comboBoxNewFunctionName.Text) && !this.comboBoxNewFunctionName.Text.EndsWith("-"))
                    {
                        this.buttonCreateOrRenameFunction.IsEnabled = true;
                        this.textBoxNewFunctionDesc.IsEnabled = true;
                    }
                }
                else
                {
                    if (this.regex.IsMatch(this.comboBoxNewFunctionName.Text))
                    {
                        this.buttonCreateOrRenameFunction.IsEnabled = true;
                        //this.textBoxNewFunctionDesc.IsEnabled = true;
                    }
                }
            }
            else
            {
                this.buttonCreateOrRenameFunction.IsEnabled = false;
                this.textBoxNewFunctionDesc.IsEnabled = false;
                if (this.comboBoxNewFunctionName.Items.Count > 0)
                {
                    if ((this.comboBoxNewFunctionName.SelectedItem == this.comboBoxNewFunctionName.Text) && this.comboBoxNewFunctionName.Text.Length <= 0)
                    {
                        this.buttonCreateOrRenameFunction.IsEnabled = false;
                        this.textBoxNewFunctionDesc.IsEnabled = false;
                    }
                    else if ((this.comboBoxNewFunctionName.SelectedItem != this.comboBoxNewFunctionName.Text) && this.regex.IsMatch(this.comboBoxNewFunctionName.Text) && !this.comboBoxNewFunctionName.Text.EndsWith("-"))
                    {
                        this.buttonCreateOrRenameFunction.IsEnabled = true;
                        this.textBoxNewFunctionDesc.IsEnabled = true;
                    }
                }
                else
                {
                    if (this.regex.IsMatch(this.comboBoxNewFunctionName.Text))
                    {
                        this.buttonCreateOrRenameFunction.IsEnabled = true;
                        //this.textBoxNewFunctionDesc.IsEnabled = true;
                    }
                }
            }
        }

        private void TextBoxNewFunctionDesc_KeyUp(object sender, KeyEventArgs e)
        {
            this.buttonCreateOrRenameFunction.IsEnabled = true;
            this.textBoxNewFunctionDesc.IsEnabled = true;
        }
    }
}