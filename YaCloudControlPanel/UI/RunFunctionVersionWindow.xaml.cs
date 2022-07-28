using System.Windows;
using NJsonSchema.CodeGeneration.CSharp;
using JsonSchema = NJsonSchema.JsonSchema;

namespace YaCloudControlPanel
{
    /// <summary>
    /// Interaction logic for RunFunctionVersionWindow.xaml
    /// </summary>
    public partial class RunFunctionVersionWindow : Window
    {
        private YaCloudControlPanel.YaModel.Function invokingFunction;
        private string jsonFunctionResponse;

        public RunFunctionVersionWindow(YaCloudControlPanel.YaModel.Function function)
        {
            this.invokingFunction = function;
            this.InitializeComponent();
            this.buttonCopyCSharpClass.IsEnabled = false;
            Helpers.UsefulStuff.BusyIndicator(this.busyPanel, this.busyProgress, false);
            this.Title = "Run cloud function: " + this.invokingFunction.name;
        }

        private async void ButtonSendRequest_Click(object sender, RoutedEventArgs e)
        {
            Helpers.UsefulStuff.BusyIndicator(this.busyPanel, this.busyProgress, true);
            var task2GetFunctionVersions = YaCloudControlPanel.YaFunctions.YaClouds.YaCloudFunctions.RunExistFunctionAsync(this.invokingFunction.httpInvokeUrl, this.textEditorRequest.Text);
            dynamic debugFunctionResponse = await task2GetFunctionVersions;
            this.jsonFunctionResponse = debugFunctionResponse.ToString();

            if (this.jsonFunctionResponse != "Something has gone wrong...")
            {
                this.buttonCopyCSharpClass.IsEnabled = true;
                this.textEditorResponse.Text = this.jsonFunctionResponse + "\n";
            }
            else
            {
                this.textEditorResponse.Text = this.jsonFunctionResponse + "\n" +
                "Maybe this function is not public\n\n" +
                 $"Check it: https://console.cloud.yandex.ru/folders/{this.invokingFunction.folderId}/functions/functions/{this.invokingFunction.id}/overview";
            }

            Helpers.UsefulStuff.BusyIndicator(this.busyPanel, this.busyProgress, false);
        }

        private void ButtonCopyCSharpClass_Click(object sender, RoutedEventArgs e)
        {
            var schemaFromFile = JsonSchema.FromSampleJson(this.jsonFunctionResponse);
            var classGenerator = new CSharpGenerator(schemaFromFile, new CSharpGeneratorSettings()
            {
                ClassStyle = CSharpClassStyle.Poco,
                GenerateDefaultValues = false,
                Namespace = "YourNamespace_CHANGE_IT",
                GenerateDataAnnotations = false,
                RequiredPropertiesMustBeDefined = false,
                InlineNamedDictionaries = false,
                GenerateNativeRecords = false,
                InlineNamedTuples = false,
                InlineNamedAny = false,
                InlineNamedArrays = false,
            });
            var generatedCSharpClasses = classGenerator.GenerateFile();
            Clipboard.SetText(generatedCSharpClasses);
            MessageBox.Show("The current JSON has been converted to C# Classes and copied to the clipboard", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}