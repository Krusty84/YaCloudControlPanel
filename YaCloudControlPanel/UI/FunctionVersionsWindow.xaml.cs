using System.Windows;

namespace YaCloudControlPanel
{
    /// <summary>
    /// Interaction logic for FunctionVersionsWindow.xaml
    /// </summary>
    public partial class FunctionVersionsWindow : Window
    {
        private YaCloudControlPanel.YaModel.FunctionVersionRoot currentFunctionVersion;
        private string currentFunctionName;

        public FunctionVersionsWindow(YaCloudControlPanel.YaModel.FunctionVersionRoot functionVersions, string functionName)
        {
            this.InitializeComponent();
            this.currentFunctionVersion = functionVersions;
            this.currentFunctionName = functionName;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.currentFunctionVersion.versions != null)
            {
                this.Title = "Function: " + this.currentFunctionName + " has: " + this.currentFunctionVersion.versions.Count + " versions";
                for (int i = 0; i < this.currentFunctionVersion.versions.Count; i++)
                {
                    this.dataGridFunctionVersions.Items.Add(this.currentFunctionVersion.versions[i]);
                }
            }
            else
            {
                this.Title = "Function: " + this.currentFunctionName + " has no versions";
                this.dataGridFunctionVersions.IsEnabled = false;
            }
        }
    }
}