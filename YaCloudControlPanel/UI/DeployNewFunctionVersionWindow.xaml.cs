using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json.Linq;

namespace YaCloudControlPanel
{
    /// <summary>
    /// Interaction logic for DeployNewFunctionVersionWindow.xaml
    /// </summary>
    public partial class DeployNewFunctionVersionWindow : System.Windows.Window
    {
        private string fileNameConfigDeploy;

        public DeployNewFunctionVersionWindow(YaCloudControlPanel.YaModel.Function functionID, YaCloudControlPanel.YaModel.RuntimeListRoot runtimeList)
        {
            this.InitializeComponent();

            this.listViewSrcFiles.Items.Clear();
            this.comboBoxRuntimeList.SelectedIndex = 0;
            this.dataGridEnvVar.Items.Clear();
            this.dataGridTag.Items.Clear();

            Helpers.UsefulStuff.BusyIndicator(this.busyPanel, this.busyProgress, false);
            this.textBoxFunctionID.Text = functionID.id;
            this.textBoxFunctionName.Text = functionID.name;

            #region checking conf Data and retrieving data

            this.fileNameConfigDeploy = $"{System.IO.Path.GetDirectoryName((ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE).Solution.FullName)}\\{this.textBoxFunctionID.Text}.json";
            if (File.Exists(this.fileNameConfigDeploy))
            {
                Helpers.UsefulStuff.FilesHelper.ReadExistConfiguration(this.fileNameConfigDeploy, this.textBoxFunctionDescription, this.textBoxEntryPoint, this.textBoxConsumedRAM, this.sliderConsumedRAM, this.textBoxTimeOut, this.comboBoxRuntimeList, this.dataGridTag, this.dataGridEnvVar, this.listViewSrcFiles);
                this.buttonLoadConfiguration.IsEnabled = true;
            }

            #endregion checking conf Data and retrieving data

            else
            {
                IEnumerable<string> filteredRuntimeList_dotnet = runtimeList.runtimes.Where(x => x.StartsWith("dotnet"));

                foreach (string runtimeDotNet in filteredRuntimeList_dotnet)
                {
                    this.comboBoxRuntimeList.Items.Add(runtimeDotNet);
                }

                this.buttonLoadConfiguration.IsEnabled = false;
            }
        }

        private void ButtonAddFile_Click(object sender, RoutedEventArgs e)
        {
            var dlgSrcFiles = new Microsoft.Win32.OpenFileDialog();
            dlgSrcFiles.InitialDirectory = System.IO.Path.GetDirectoryName((ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE).Solution.FullName);
            dlgSrcFiles.Filter = "Visual C# Files| *.cs;*.csproj";
            dlgSrcFiles.Multiselect = true;
            dlgSrcFiles.DefaultExt = ".cs,";
            bool? result = dlgSrcFiles.ShowDialog();
            if (result == true)
            {
                foreach (String file in dlgSrcFiles.FileNames)
                {
                    var sysicon = System.Drawing.Icon.ExtractAssociatedIcon(file);
                    var bmpSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                sysicon.Handle,
                                System.Windows.Int32Rect.Empty,
                                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    sysicon.Dispose();
                    this.listViewSrcFiles.Items.Add(new Helpers.UsefulStuff.SrcFiles { IconFile = bmpSrc, File = file });
                }
            }
        }

        private void ButtonDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.listViewSrcFiles.Items.Count > 0)
            {
                this.listViewSrcFiles.Items.RemoveAt(this.listViewSrcFiles.SelectedIndex);
            }
        }

        private async void ButtonDeploy_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(YaCloudControlPanel.YaFunctions.YaClouds.YaCloudOAuthCode);

#else
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(Helpers.UsefulStuff.SettingsHelper.ReadSetting("General", "OAuthKey"));

#endif
            this.Title = "Deploying cloud function...";
            Helpers.UsefulStuff.BusyIndicator(this.busyPanel, this.busyProgress, true);
            var task1ZippingSrcFiles = Helpers.UsefulStuff.FilesHelper.PreparingSrcForYaCloudUploadAsync(this.listViewSrcFiles);
            string srcArchiveBase64String = await task1ZippingSrcFiles;

            #region JSON Payload

            dynamic parametersFunctionDeploy = new JObject();
            parametersFunctionDeploy.functionId = this.textBoxFunctionID.Text;
            parametersFunctionDeploy.runtime = this.comboBoxRuntimeList.Text;
            parametersFunctionDeploy.description = this.textBoxFunctionDescription.Text;
            parametersFunctionDeploy.entrypoint = this.textBoxEntryPoint.Text;
            parametersFunctionDeploy.resources = new JObject();
            parametersFunctionDeploy.resources.memory = (Int64.Parse(this.textBoxConsumedRAM.Text) * 1024 * 1024).ToString();
            parametersFunctionDeploy.executionTimeout = this.textBoxTimeOut.Text + "s";
            parametersFunctionDeploy.serviceAccountId = "";

            // if environment variables are exist
            if (this.dataGridEnvVar.Items.Count > 0)
            {
                parametersFunctionDeploy.environment = new JObject();
                foreach (var item in this.dataGridEnvVar.Items)
                {
                    parametersFunctionDeploy.environment[((Helpers.UsefulStuff.EnvVariable)item).EnvName] = ((Helpers.UsefulStuff.EnvVariable)item).EnvValue;
                }
            }

            // if tags are exist
            if (this.dataGridTag.Items.Count > 0)
            {
                JArray tagsArray = new JArray();
                foreach (var item in this.dataGridTag.Items)
                {
                    tagsArray.Add(((Helpers.UsefulStuff.Tag)item).tag);
                }

                parametersFunctionDeploy.tag = tagsArray;
            }

            parametersFunctionDeploy.content = srcArchiveBase64String;

            #endregion JSON Payload

            var task2DCreatingFunctionVersion = YaCloudControlPanel.YaFunctions.YaClouds.YaCloudFunctions.CreateNewFunctionVersionAsync(Newtonsoft.Json.JsonConvert.SerializeObject(parametersFunctionDeploy), yaAuthToken);
            await task2DCreatingFunctionVersion;
            this.Title = "Cloud function was deployed";
            Helpers.UsefulStuff.BusyIndicator(this.busyPanel, this.busyProgress, false);
        }

        private void ButtonAddVariable_Click(object sender, RoutedEventArgs e)
        {
            var addOrEditEnvironmentVariablesWindow = new AddOrEditEnvironmentVariablesWindow(this.dataGridEnvVar);
            addOrEditEnvironmentVariablesWindow.Closed += this.AddOrEditEnvironmentVariablesWindow;
            addOrEditEnvironmentVariablesWindow.ShowDialog();
        }

        private void AddOrEditEnvironmentVariablesWindow(object sender, System.EventArgs e)
        {
            this.dataGridEnvVar.Items.Refresh();
        }

        private void ButtonDeleteVariable_Click(object sender, RoutedEventArgs e)
        {
            this.dataGridEnvVar.Items.Remove(this.dataGridEnvVar.SelectedItem);
            this.dataGridEnvVar.Items.Refresh();
            this.dataGridEnvVar.SelectedItem = this.dataGridEnvVar.Items.Count - 1;
        }

        private void ButtonEditVariable_Click(object sender, RoutedEventArgs e)
        {
            var addOrEditEnvironmentVariablesWindow = new AddOrEditEnvironmentVariablesWindow(this.dataGridEnvVar, (Helpers.UsefulStuff.EnvVariable)this.dataGridEnvVar.SelectedItem);
            addOrEditEnvironmentVariablesWindow.Closed += this.AddOrEditEnvironmentVariablesWindow;
            addOrEditEnvironmentVariablesWindow.ShowDialog();
        }

        private void ButtonAddTag_Click(object sender, RoutedEventArgs e)
        {
            var addTag = new AddTagWindow(this.dataGridTag);
            addTag.Closed += this.AddTagWindow;
            addTag.ShowDialog();
        }

        private void AddTagWindow(object sender, System.EventArgs e)
        {
            this.dataGridTag.Items.Refresh();
        }

        private void ButtonDeleteTag_Click(object sender, RoutedEventArgs e)
        {
            this.dataGridTag.Items.Remove(this.dataGridTag.SelectedItem);
            this.dataGridTag.Items.Refresh();
            this.dataGridTag.SelectedItem = this.dataGridTag.Items.Count - 1;
        }

        private void ButtonSaveConfiguration_Click(object sender, RoutedEventArgs e)
        {
            dynamic parametersFunctionDeploy = new JObject();
            parametersFunctionDeploy.functionId = this.textBoxFunctionID.Text;
            parametersFunctionDeploy.runtime = this.comboBoxRuntimeList.Text;
            parametersFunctionDeploy.description = this.textBoxFunctionDescription.Text;
            parametersFunctionDeploy.entrypoint = this.textBoxEntryPoint.Text;
            parametersFunctionDeploy.resources = new JObject();
            parametersFunctionDeploy.resources.memory = this.textBoxConsumedRAM.Text;
            parametersFunctionDeploy.executionTimeout = this.textBoxTimeOut.Text;
            parametersFunctionDeploy.serviceAccountId = "";

            // if environment variables are exist
            if (this.dataGridEnvVar.Items.Count > 0)
            {
                parametersFunctionDeploy.environment = new JObject();
                foreach (var item in this.dataGridEnvVar.Items)
                {
                    parametersFunctionDeploy.environment[((Helpers.UsefulStuff.EnvVariable)item).EnvName] = ((Helpers.UsefulStuff.EnvVariable)item).EnvValue;
                }
            }

            // if tags are exist
            if (this.dataGridTag.Items.Count > 0)
            {
                JArray tagsArray = new JArray();
                foreach (var item in this.dataGridTag.Items)
                {
                    tagsArray.Add(((Helpers.UsefulStuff.Tag)item).tag);
                }

                parametersFunctionDeploy.tag = tagsArray;
            }

            if (this.listViewSrcFiles.Items.Count > 0)
            {
                JArray srcFiles = new JArray();
                foreach (var item in this.listViewSrcFiles.Items)
                {
                    srcFiles.Add(((Helpers.UsefulStuff.SrcFiles)item).File);
                }

                parametersFunctionDeploy.srcs = srcFiles;
            }

            parametersFunctionDeploy.content = "";

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(parametersFunctionDeploy));

            File.WriteAllText($"{System.IO.Path.GetDirectoryName((ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE).Solution.FullName)}\\{this.textBoxFunctionID.Text}.json", JObject.FromObject(parametersFunctionDeploy).ToString());
        }

        private void ButtonLoadConfiguration_Click(object sender, RoutedEventArgs e)
        {
            this.listViewSrcFiles.Items.Clear();
            this.comboBoxRuntimeList.Items.Clear();
            this.dataGridEnvVar.Items.Clear();
            this.dataGridTag.Items.Clear();
            Helpers.UsefulStuff.FilesHelper.ReadExistConfiguration(this.fileNameConfigDeploy, this.textBoxFunctionDescription, this.textBoxEntryPoint, this.textBoxConsumedRAM, this.sliderConsumedRAM, this.textBoxTimeOut, this.comboBoxRuntimeList, this.dataGridTag, this.dataGridEnvVar, this.listViewSrcFiles);
        }
    }
}