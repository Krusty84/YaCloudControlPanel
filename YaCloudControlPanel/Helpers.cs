using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RbCreation.Wpf.ProgressNotifier;

namespace Helpers
{
    public static class UsefulStuff
    {
        public static class TreeHelpers
        {
            public class DummyTreeViewItem : TreeViewItem
            {
                public DummyTreeViewItem()
                    : base()
                {
                    base.Header = "Dummy";
                    base.Tag = "Dummy";
                }
            }

            public static void AddDummy(TreeViewItem item)
            {
                item.Items.Add(new DummyTreeViewItem());
            }

            public static bool HasDummy(TreeViewItem item)
            {
                return item.HasItems && (item.Items.OfType<TreeViewItem>().ToList().FindAll(tvi => tvi is DummyTreeViewItem).Count > 0);
            }

            public static void RemoveDummy(TreeViewItem item)
            {
                var dummies = item.Items.OfType<TreeViewItem>().ToList().FindAll(tvi => tvi is DummyTreeViewItem);
                foreach (var dummy in dummies)
                {
                    item.Items.Remove(dummy);
                }
            }
        }

        public static void BusyIndicator(StackPanel busyPanel, ProgressNotifier busyIndicator, bool isEnabled)
        {
            if (isEnabled == false)
            {
                busyPanel.Visibility = Visibility.Hidden;
                busyIndicator.IsBusy = false;
            }
            else
            {
                busyPanel.Visibility = Visibility.Visible;
                busyIndicator.IsBusy = true;
            }
        }

        public static void ErrorConnectIndicator(StackPanel erroPanel, bool isEnabled)
        {
            if (isEnabled == false)
            {
                erroPanel.Visibility = Visibility.Hidden;
            }
            else
            {
                erroPanel.Visibility = Visibility.Visible;
            }
        }

        public static ImageSource LoadPngImageSource(string path)
        {
            return new PngBitmapDecoder(new Uri(path, UriKind.RelativeOrAbsolute),
                BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad).Frames[0];
        }

        public static class FilesHelper
        {
            public static async Task<string> PreparingSrcForYaCloudUploadAsync(ListView srcFiles)
            {
                string pathTempSrcArchive = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".zip";
                using (var archiveStream = File.OpenWrite(pathTempSrcArchive))
                {
                    using (ZipArchive tempSrcArchive = new ZipArchive(archiveStream, System.IO.Compression.ZipArchiveMode.Create))
                    {
                        foreach (object item in srcFiles.Items)
                        {
                            tempSrcArchive.CreateEntryFromFile(((SrcFiles)item).File, System.IO.Path.GetFileName(((SrcFiles)item).File), CompressionLevel.Optimal);
                        }
                    }
                }

                Byte[] archiveBytesRepresent = File.ReadAllBytes(pathTempSrcArchive);
                String archiveBase64Represent = Convert.ToBase64String(archiveBytesRepresent);
                Console.WriteLine(archiveBase64Represent);

                return archiveBase64Represent;
            }

            public static void ReadExistConfiguration(string fileNameConfigDeploy, TextBox funcDes, TextBox entryPoint, TextBox consRAM, Slider consRAMslider, TextBox timeOut, ComboBox runtimeList, DataGrid tagsGrid, DataGrid envVarGrid, ListView srcFilesList)
            {
                // string fileNameConfigDeploy = $"{Environment.CurrentDirectory}\\{functionID}.json";
                if (File.Exists(fileNameConfigDeploy))
                {
                    using (StreamReader streamConfigFile = new StreamReader(fileNameConfigDeploy))
                    {
                        string jsonConfigFile = streamConfigFile.ReadToEnd();

                        dynamic parametersFunctionDeploy = JsonConvert.DeserializeObject(jsonConfigFile);

                        funcDes.Text = parametersFunctionDeploy.description;
                        entryPoint.Text = parametersFunctionDeploy.entrypoint;
                        consRAM.Text = parametersFunctionDeploy.resources.memory;
                        timeOut.Text = parametersFunctionDeploy.executionTimeout;
                        runtimeList.Items.Add(parametersFunctionDeploy.runtime);
                        runtimeList.SelectedIndex = 0;
                        consRAMslider.UpdateLayout();

                        JArray tagsArray = parametersFunctionDeploy.tag;
                        if (tagsArray != null)
                        {
                            foreach (dynamic tag in tagsArray)
                            {
                                tagsGrid.Items.Add(new Helpers.UsefulStuff.Tag() { tag = tag });
                            }
                        }

                        JObject envArray = parametersFunctionDeploy.environment;
                        if (envArray != null)
                        {
                            var env = JsonConvert.DeserializeObject<Dictionary<string, string>>(envArray.ToString());

                            foreach (var envKeyValue in env)
                            {
                                envVarGrid.Items.Add(new Helpers.UsefulStuff.EnvVariable() { EnvName = envKeyValue.Key, EnvValue = envKeyValue.Value });
                            }
                        }

                        JArray srcFiles = parametersFunctionDeploy.srcs;
                        if (srcFiles != null)
                        {
                            foreach (dynamic srcFile in srcFiles)
                            {
                                var sysicon = System.Drawing.Icon.ExtractAssociatedIcon((string)srcFile);
                                var bmpSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                        sysicon.Handle,
                                        System.Windows.Int32Rect.Empty,
                                        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                                sysicon.Dispose();
                                srcFilesList.Items.Add(new Helpers.UsefulStuff.SrcFiles { IconFile = bmpSrc, File = srcFile });
                            }
                        }
                    }
                }
            }
        }

        public static class SettingsHelper
        {
            public static string ReadSetting(string settingsCatalog, string settingName)
            {
                try
                {
                    ShellSettingsManager settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
                    WritableSettingsStore userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
                    return userSettingsStore.GetString(settingsCatalog, settingName);
                }
                catch (Exception)
                {
                    return "Setting is missing";
                }
            }

            public static void WriteSetting(string settingCatalog, string settingName, string settingValue)
            {
                ShellSettingsManager settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
                WritableSettingsStore userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
                userSettingsStore.SetString(settingCatalog, settingName, settingValue);
            }

            public static void CreateCatalog(string catalogName)
            {
                ShellSettingsManager settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
                WritableSettingsStore userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
                userSettingsStore.CreateCollection(catalogName);
            }
        }

        public class SrcFiles
        {
            public BitmapSource IconFile { get; set; }

            public string File { get; set; }
        }

        public class EnvVariable
        {
            public string EnvName { get; set; }

            public string EnvValue { get; set; }
        }

        public class Tag
        {
            public string tag { get; set; }
        }
    }
}