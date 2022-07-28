using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using log4net.Config;

namespace YaCloudControlPanel.WIndow
{
    /// <summary>
    /// Interaction logic for MainWindowControl.
    /// </summary>
    public partial class MainWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowControl"/> class.
        /// </summary>
        // for sexy fonts
        private readonly FontStyleConverter fontStyleConverter = new FontStyleConverter();

        private string billingAccountId;

        public MainWindowControl()
        {
            this.InitializeComponent();
            this.errorPanel.Visibility = Visibility.Hidden;
        }

        private void MyToolWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.GetYaExistClouds();
        }

        private async void GetYaExistClouds()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo(YaCloudControlPanel.YaFunctions.YaClouds.Log4NETCongif));
            try
            {
                Helpers.UsefulStuff.BusyIndicator(this.busyPanel, this.busyProgress, true);
                Helpers.UsefulStuff.ErrorConnectIndicator(this.errorPanel, false);
                this.treeView.Items.Clear();

#if DEBUG
                string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(YaCloudControlPanel.YaFunctions.YaClouds.YaCloudOAuthCode);
                var task2GetClouds = YaCloudControlPanel.YaFunctions.YaClouds.YaCloudFunctions.GetExistsCloudAsync(yaAuthToken);
                var task2GetBillingData = YaCloudControlPanel.YaFunctions.YaClouds.YaBillingData.GetSpentCloudMoneyAsync(yaAuthToken);
#else
                string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(Helpers.UsefulStuff.SettingsHelper.ReadSetting("General", "OAuthKey"));
                var task2GetClouds = YaCloudControlPanel.YaFunctions.YaClouds.YaCloudFunctions.GetExistsCloudAsync(yaAuthToken);
                var task2GetBillingData = YaCloudControlPanel.YaFunctions.YaClouds.YaBillingData.GetSpentCloudMoneyAsync(yaAuthToken);
#endif

                YaCloudControlPanel.YaModel.CloudsRoot cloudsData = await task2GetClouds;
                YaCloudControlPanel.YaModel.BillingDataRoot spentMoney = await task2GetBillingData;

                for (int i = 0; i < cloudsData.clouds.Count; i++)
                {
                    // Pay attention, billing accounts can be more than one!
                    TreeViewItem cloudRootNode = new TreeViewItem() { Header = cloudsData.clouds[i].name + " (Balance: " + /*spentMoney.billingAccounts[0].balance*/Math.Round(double.Parse(spentMoney.billingAccounts[0].balance, CultureInfo.InvariantCulture), 2).ToString() + " " + spentMoney.billingAccounts[0].currency + " )", Tag = cloudsData.clouds[i] };

                    // Dispaying negative balance
                    if (Math.Abs(Math.Round(double.Parse(spentMoney.billingAccounts[0].balance, CultureInfo.InvariantCulture), 2)) > 500)
                    {
                        cloudRootNode.Background = Brushes.IndianRed;
                    }
                    else if (Math.Round(double.Parse(spentMoney.billingAccounts[0].balance, CultureInfo.InvariantCulture), 2) < 0)
                    {
                        cloudRootNode.Background = Brushes.Yellow;
                    }

                    cloudRootNode.Expanded += this.Item_CloudLevelExpand;
                    Helpers.UsefulStuff.TreeHelpers.AddDummy(cloudRootNode);
                    this.treeView.Items.Add(cloudRootNode);
                }

                // Pay attention, billing accounts can be more than one!
                this.billingAccountId = spentMoney.billingAccounts[0].id;
                Helpers.UsefulStuff.BusyIndicator(this.busyPanel, this.busyProgress, false);
            }
            catch (Exception ex)
            {
                Helpers.UsefulStuff.BusyIndicator(this.busyPanel, this.busyProgress, false);
                Helpers.UsefulStuff.ErrorConnectIndicator(this.errorPanel, true);
                XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
                YaCloudControlPanel.YaFunctions.YaClouds.Log.Error("Something was wrong during Authorization in Yandex Cloud. " + ex.Message);
                EnvDTE.DTE dte = (EnvDTE.DTE)Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE));
                dte.ExecuteCommand("Tools.Options", "FD5ECC49-C4F4-4994-8A7E-BEB619E956F0");
            }
        }

        private async void Item_CloudLevelExpand(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;

            // Prevent recursive calling handlers
            if (item.Tag.ToString() == "YaCloudControlPanel.YaModel.Cloud")
            {
                Helpers.UsefulStuff.TreeHelpers.RemoveDummy(item);
                item.Items.Add("Loading cloud folders...");

#if DEBUG
                string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(YaCloudControlPanel.YaFunctions.YaClouds.YaCloudOAuthCode);
                var task2GetCloudFolders = YaCloudControlPanel.YaFunctions.YaClouds.YaFolders.GetExistsFoldersAsync(((YaCloudControlPanel.YaModel.Cloud)item.Tag).id.ToString(), yaAuthToken);
#else
                string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(Helpers.UsefulStuff.SettingsHelper.ReadSetting("General", "OAuthKey"));
                var task2GetCloudFolders = YaCloudControlPanel.YaFunctions.YaClouds.YaFolders.GetExistsFoldersAsync(((YaCloudControlPanel.YaModel.Cloud)item.Tag).id.ToString(), yaAuthToken);
#endif

                YaCloudControlPanel.YaModel.FoldersRoot foldersData = await task2GetCloudFolders;
                if (foldersData.folders != null && foldersData.folders.Count != 0)
                {
                    item.Items.Clear();
                    for (int i = 0; i < foldersData.folders.Count; i++)
                    {
                        TreeViewItem folderRootNode = new TreeViewItem() { Header = foldersData.folders[i].name, Tag = foldersData.folders[i] };
                        Helpers.UsefulStuff.TreeHelpers.AddDummy(folderRootNode);
                        folderRootNode.Expanded += new RoutedEventHandler(this.Item_FolderLevelExpand);
                        item.Items.Add(folderRootNode);
                    }
                }
                else
                {
                    return;
                }
            }

            Helpers.UsefulStuff.BusyIndicator(this.busyPanel, this.busyProgress, false);
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        // [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        // [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void Item_FolderLevelExpand(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;
            TreeViewItem itemFunctions = new TreeViewItem() { Header = "Functions", Tag = "Functions" };
            TreeViewItem itemStorage = new TreeViewItem() { Header = "Storages", Tag = "Storages" };
            TreeViewItem itemVM = new TreeViewItem() { Header = "Virtual Machines", Tag = "VirtualMachines" };
            itemFunctions.Expanded += new RoutedEventHandler(this.Item_FunctionsLevelExpand);
            itemStorage.Expanded += new RoutedEventHandler(this.Item_StorageLevelExpand);
            itemVM.Expanded += new RoutedEventHandler(this.Item_VMLevelExpand);
            Helpers.UsefulStuff.TreeHelpers.AddDummy(itemFunctions);
            Helpers.UsefulStuff.TreeHelpers.AddDummy(itemStorage);
            Helpers.UsefulStuff.TreeHelpers.AddDummy(itemVM);
            if (Helpers.UsefulStuff.TreeHelpers.HasDummy(item))
            {
                this.Cursor = Cursors.Wait;
                Helpers.UsefulStuff.TreeHelpers.RemoveDummy(item);

                item.Items.Add(itemFunctions);
                item.Items.Add(itemStorage);
                item.Items.Add(itemVM);

                this.Cursor = Cursors.Arrow;
            }
        }

        private async void Item_VMLevelExpand(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;
            TreeViewItem itemRootFolder = new TreeViewItem();
            itemRootFolder = item.Parent as TreeViewItem;
            item.Items.Clear();
            Helpers.UsefulStuff.TreeHelpers.RemoveDummy(item);
            item.Items.Add("Loading virtual machines...");
#if DEBUG
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(YaCloudControlPanel.YaFunctions.YaClouds.YaCloudOAuthCode);
#else
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(Helpers.UsefulStuff.SettingsHelper.ReadSetting("General", "OAuthKey"));
#endif
            var task2GetVMs = YaCloudControlPanel.YaFunctions.YaClouds.YaVirtualMachines.GetExistsVMAsync(((YaCloudControlPanel.YaModel.Folder)itemRootFolder.Tag).id, yaAuthToken);
            YaCloudControlPanel.YaModel.VMInstance.VMInstancesRoot existVMs = await task2GetVMs;

            if (existVMs.instances != null && existVMs.instances.Count != 0)
            {
                item.Items.Clear();
                for (int i = 0; i < existVMs.instances.Count; i++)
                {
                    TreeViewItem vmsUnderFolder = new TreeViewItem() { Header = existVMs.instances[i].name, Tag = existVMs.instances[i] };

                    if (existVMs.instances[i].status == "RUNNING")
                    {
                        vmsUnderFolder.Foreground = Brushes.Green;
                    }

                    item.Items.Add(vmsUnderFolder);
                }
            }
            else
            {
                item.Items.Clear();
                TreeViewItem vmsUnderFolder = new TreeViewItem
                {
                    Header = "VMs are missing",
                    Foreground = Brushes.Gray,
                    FontStyle = (FontStyle)this.fontStyleConverter.ConvertFrom("Italic"),
                };
                item.FontStyle = new FontStyle();
                item.Items.Add(vmsUnderFolder);
            }
        }

        private async void Item_FunctionsLevelExpand(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;
            TreeViewItem itemRootFolder = new TreeViewItem();
            itemRootFolder = item.Parent as TreeViewItem;
            item.Items.Clear();
            Helpers.UsefulStuff.TreeHelpers.RemoveDummy(item);
            item.Items.Add("Loading functions...");
#if DEBUG
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(YaCloudControlPanel.YaFunctions.YaClouds.YaCloudOAuthCode);
#else
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(Helpers.UsefulStuff.SettingsHelper.ReadSetting("General", "OAuthKey"));
#endif
            var task2GetFunctions = YaCloudControlPanel.YaFunctions.YaClouds.YaCloudFunctions.GetExistsFunctionsAsync(((YaCloudControlPanel.YaModel.Folder)itemRootFolder.Tag).id, yaAuthToken);
            YaCloudControlPanel.YaModel.FunctionsRoot existFunctions = await task2GetFunctions;

            if (existFunctions.functions != null && existFunctions.functions.Count != 0)
            {
                item.Items.Clear();
                for (int i = 0; i < existFunctions.functions.Count; i++)
                {
                    TreeViewItem functionsUnderFolder = new TreeViewItem() { Header = existFunctions.functions[i].name, Tag = existFunctions.functions[i] };
                    item.Items.Add(functionsUnderFolder);
                }
            }
            else
            {
                item.Items.Clear();
                TreeViewItem functionsUnderFolder = new TreeViewItem
                {
                    Header = "Functions are missing",
                    Foreground = Brushes.Gray,
                    FontStyle = (FontStyle)this.fontStyleConverter.ConvertFrom("Italic"),
                };
                item.FontStyle = new FontStyle();
                item.Items.Add(functionsUnderFolder);
            }
        }

        private async void Item_StorageLevelExpand(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;
            TreeViewItem itemRootFolder = new TreeViewItem();
            itemRootFolder = item.Parent as TreeViewItem;
            item.Items.Clear();
            Helpers.UsefulStuff.TreeHelpers.RemoveDummy(item);
            item.Items.Add("Loading buckets...");
#if DEBUG
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(YaCloudControlPanel.YaFunctions.YaClouds.YaCloudOAuthCode);
#else
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(Helpers.UsefulStuff.SettingsHelper.ReadSetting("General", "OAuthKey"));
#endif
            var task2GetStorages = YaCloudControlPanel.YaFunctions.YaClouds.YaStorage.GetExistsStorageAsync(((YaCloudControlPanel.YaModel.Folder)itemRootFolder.Tag).id, yaAuthToken);
            YaCloudControlPanel.YaModel.StoragesRoot existStorages = await task2GetStorages;

            if (existStorages.buckets != null && existStorages.buckets.Count != 0)
            {
                item.Items.Clear();
                for (int i = 0; i < existStorages.buckets.Count; i++)
                {
                    TreeViewItem storagesUnderFodler = new TreeViewItem() { Header = existStorages.buckets[i].name, Tag = existStorages.buckets[i] };
                    item.Items.Add(storagesUnderFodler);
                }
            }
            else
            {
                item.Items.Clear();
                TreeViewItem functionsUnderFolder = new TreeViewItem
                {
                    Header = "Buckets are missing",
                    Foreground = Brushes.Gray,
                    FontStyle = (FontStyle)this.fontStyleConverter.ConvertFrom("Italic"),
                };
                item.FontStyle = new FontStyle();
                item.Items.Add(functionsUnderFolder);
            }
        }

        #region RightMouseButton Menu

        private void TreeView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;
            if (item != null)
            {
                Point mousePointer = e.GetPosition(item);
                var controlType = item.InputHitTest(mousePointer);

                if (!(controlType is null))
                {
                    if (!(controlType is Border))
                    {
                        item.IsSelected = true;
                        if (item != null && item.Tag.ToString() == "YaCloudControlPanel.YaModel.Cloud")
                        {
                            MenuItem mnuItemRefreshSelectedCloud = new MenuItem
                            {
                                Header = "Refresh",
                            };
                            mnuItemRefreshSelectedCloud.Click += this.MnuItemRefreshSelectedCloud_Click;

                            MenuItem mnuItemWebBrowserCloud = new MenuItem
                            {
                                Header = "Open Cloud in Browser",
                            };
                            mnuItemWebBrowserCloud.Click += this.MnuItemWebBrowserCloud_Click;

                            MenuItem mnuItemWebBrowserCloudBilling = new MenuItem
                            {
                                Header = "Open Billing in Browser",
                            };
                            mnuItemWebBrowserCloudBilling.Click += this.MnuItemWebBrowserCloudBilling_Click;

                            ContextMenu menu = new ContextMenu() { };
                            menu.Items.Add(mnuItemRefreshSelectedCloud);
                            menu.Items.Add(mnuItemWebBrowserCloud);
                            menu.Items.Add(mnuItemWebBrowserCloudBilling);
                            item.ContextMenu = menu;
                        }
                        else if (item != null && item.Tag.ToString() == "Functions")
                        {
                            MenuItem mnuItemCreateNewFunction = new MenuItem
                            {
                                Header = "Create a new function",
                            };
                            mnuItemCreateNewFunction.Click += this.MnuItemCreateNewFunction_Click;

                            MenuItem mnuItemRefreshSelectedFunction = new MenuItem
                            {
                                Header = "Refresh",
                            };
                            mnuItemRefreshSelectedFunction.Click += this.MnuItemRefreshSelectedFunction_Click;

                            MenuItem mnuItemWebBrowserFunction = new MenuItem
                            {
                                Header = "Open Functions in Browser",
                            };
                            mnuItemWebBrowserFunction.Click += this.MnuItemWebBrowserFunction_Click;

                            MenuItem mnuItemHealthStatusFunction = new MenuItem
                            {
                                Header = "Health Status",
                            };
                            mnuItemHealthStatusFunction.Click += this.MnuItemHealthStatusFunction_Click;

                            ContextMenu menu = new ContextMenu() { };
                            menu.Items.Add(mnuItemCreateNewFunction);
                            menu.Items.Add(mnuItemRefreshSelectedFunction);
                            menu.Items.Add(new Separator());
                            menu.Items.Add(mnuItemWebBrowserFunction);
                            menu.Items.Add(mnuItemHealthStatusFunction);
                            item.ContextMenu = menu;
                        }
                        else if (item != null && item.Tag.ToString() == "YaCloudControlPanel.YaModel.Function")
                        {
                            MenuItem mnuItemDeploySelectedFunction = new MenuItem
                            {
                                Header = "Deploy",
                            };
                            mnuItemDeploySelectedFunction.Click += this.MnuItemDeploySelectedFunction_Click;

                            MenuItem mnuItemRunSelectedFunction = new MenuItem
                            {
                                Header = "Run",
                            };
                            mnuItemRunSelectedFunction.Click += this.MnuItemRunSelectedFunction_Click;

                            MenuItem mnuItemEditNameSelectedFunction = new MenuItem
                            {
                                Header = "Edit",
                            };
                            mnuItemEditNameSelectedFunction.Click += this.MnuItemEditNameSelectedFunction_Click;

                            MenuItem mnuItemLoadLogsSelectedFunction = new MenuItem
                            {
                                Header = "Load Logs",
                            };
                            mnuItemLoadLogsSelectedFunction.Click += this.MnuItemLoadLogsSelectedFunction_Click;

                            MenuItem mnuItemLoadVersionsSelectedFunction = new MenuItem
                            {
                                Header = "Load Versions",
                            };
                            mnuItemLoadVersionsSelectedFunction.Click += this.MnuItemLoadVersionsSelectedFunction_Click;

                            MenuItem mnuItemDeleteSelectedFunction = new MenuItem
                            {
                                Header = "Delete",
                            };
                            mnuItemDeleteSelectedFunction.Click += this.MnuItemDeleteSelectedFunction_Click;

                            MenuItem mnuItemCopyIDSelectedFunction = new MenuItem
                            {
                                Header = "Copy ID & Name",
                            };
                            mnuItemCopyIDSelectedFunction.Click += this.MnuItemCopyIDSelectedFunction_Click;

                            MenuItem mnuItemCopyHttpInvokeSelectedFunction = new MenuItem
                            {
                                Header = "Copy Invoke URL",
                            };
                            mnuItemCopyHttpInvokeSelectedFunction.Click += this.MnuItemCopyHttpInvokeSelectedFunction_Click;

                            MenuItem mnuItemOpenInWebBrowserSelectedFunction = new MenuItem
                            {
                                Header = "Open Function in Browser",
                            };
                            mnuItemOpenInWebBrowserSelectedFunction.Click += this.MnuItemOpenInWebBrowserSelectedFunction_Click;

                            ContextMenu menu = new ContextMenu() { };
                            menu.Items.Add(mnuItemDeploySelectedFunction);
                            menu.Items.Add(mnuItemRunSelectedFunction);
                            menu.Items.Add(mnuItemEditNameSelectedFunction);
                            menu.Items.Add(new Separator());

                            // it will be in the future.... maybe
                            // menu.Items.Add(mnuItemLoadLogsSelectedFunction);
                            menu.Items.Add(mnuItemLoadVersionsSelectedFunction);
                            menu.Items.Add(mnuItemDeleteSelectedFunction);
                            menu.Items.Add(new Separator());
                            menu.Items.Add(mnuItemCopyIDSelectedFunction);
                            menu.Items.Add(mnuItemCopyHttpInvokeSelectedFunction);
                            menu.Items.Add(mnuItemOpenInWebBrowserSelectedFunction);
                            item.ContextMenu = menu;
                        }
                        else if (item != null && item.Tag.ToString() == "VirtualMachines")
                        {
                            MenuItem mnuItemRefreshVMs = new MenuItem
                            {
                                Header = "Refresh",
                            };
                            mnuItemRefreshVMs.Click += this.MnuItemRefreshVMs_Click;

                            MenuItem mnuItemGetInfoAboutVMs = new MenuItem
                            {
                                Header = "Information about VMs",
                            };
                            mnuItemGetInfoAboutVMs.Click += this.MnuItemGetInfoAboutVMs_Click;

                            MenuItem mnuItemWebBrowserVMs = new MenuItem
                            {
                                Header = "Open VMs in Browser",
                            };
                            mnuItemWebBrowserVMs.Click += this.MnuItemWebBrowserVMs_Click;

                            MenuItem mnuItemHealthStatusVMs = new MenuItem
                            {
                                Header = "Health Status",
                            };
                            mnuItemHealthStatusVMs.Click += this.MnuItemHealthStatusVMs_Click;

                            ContextMenu menu = new ContextMenu() { };
                            menu.Items.Add(mnuItemRefreshVMs);
                            menu.Items.Add(mnuItemGetInfoAboutVMs);
                            menu.Items.Add(new Separator());
                            menu.Items.Add(mnuItemWebBrowserVMs);
                            menu.Items.Add(mnuItemHealthStatusVMs);
                            item.ContextMenu = menu;
                        }
                        else if (item != null && item.Tag.ToString() == "YaCloudControlPanel.YaModel.VMInstance.Instance")
                        {
                            ContextMenu menu = new ContextMenu() { };
                            if (((YaCloudControlPanel.YaModel.VMInstance.Instance)item.Tag).status == "RUNNING")
                            {
                                MenuItem mnuItemStopSelectedVM = new MenuItem
                                {
                                    Header = "Stop",
                                };
                                mnuItemStopSelectedVM.Click += this.MnuItemStopSelectedVM_Click;

                                MenuItem mnuItemCopyIPandNameSelectedVM = new MenuItem
                                {
                                    Header = "Copy IP & Name",
                                };
                                mnuItemCopyIPandNameSelectedVM.Click += this.MnuItemCopyIPandNameSelectedVM_Click;

                                menu.Items.Add(mnuItemStopSelectedVM);
                                menu.Items.Add(mnuItemCopyIPandNameSelectedVM);
                            }
                            else if (((YaCloudControlPanel.YaModel.VMInstance.Instance)item.Tag).status == "STOPPED")
                            {
                                MenuItem mnuItemStartSelectedVM = new MenuItem
                                {
                                    Header = "Start",
                                };
                                mnuItemStartSelectedVM.Click += this.MnuItemStartSelectedVM_Click;
                                menu.Items.Add(mnuItemStartSelectedVM);
                            }

                            MenuItem mnuItemWebBrowserSelectedVM = new MenuItem
                            {
                                Header = "Open VM in Browser",
                            };
                            mnuItemWebBrowserSelectedVM.Click += this.MnuItemOpenInWebBrowserSelectedVM_Click;
                            menu.Items.Add(new Separator());
                            menu.Items.Add(mnuItemWebBrowserSelectedVM);
                            item.ContextMenu = menu;
                        }
                        else if (item != null && item.Tag.ToString() == "YaCloudControlPanel.YaModel.Folder")
                        {
                            MenuItem mnuItemOpenInWebBrowserSelectedFolder = new MenuItem
                            {
                                Header = "Open Folder in Browser",
                            };
                            mnuItemOpenInWebBrowserSelectedFolder.Click += this.MnuItemOpenInWebBrowserSelectedFolder_Click;

                            ContextMenu menu = new ContextMenu() { };
                            menu.Items.Add(mnuItemOpenInWebBrowserSelectedFolder);
                            item.ContextMenu = menu;
                        }
                        else if (item != null && item.Tag.ToString() == "Storages")
                        {
                            MenuItem mnuItemRefreshStorages = new MenuItem
                            {
                                Header = "Refresh",
                            };
                            mnuItemRefreshStorages.Click += this.MnuItemRefreshStorages_Click;

                            MenuItem mnuItemWebBrowserStorages = new MenuItem
                            {
                                Header = "Open Storages in Browser",
                            };
                            mnuItemWebBrowserStorages.Click += this.MnuItemWebBrowserStorages_Click;

                            MenuItem mnuItemHealthStatusStorages = new MenuItem
                            {
                                Header = "Health Status",
                            };
                            mnuItemHealthStatusStorages.Click += this.MnuItemHealthStatusStorages_Click;

                            ContextMenu menu = new ContextMenu() { };
                            menu.Items.Add(mnuItemRefreshStorages);
                            menu.Items.Add(mnuItemWebBrowserStorages);
                            menu.Items.Add(mnuItemHealthStatusStorages);
                            item.ContextMenu = menu;
                        }
                        else if (item != null && item.Tag.ToString() == "YaCloudControlPanel.YaModel.Bucket")
                        {
                            MenuItem mnuItemCopyNameSelectedBucket = new MenuItem
                            {
                                Header = "Copy Name",
                            };
                            mnuItemCopyNameSelectedBucket.Click += this.MnuItemCopyNameSelectedBucket_Click;

                            MenuItem mnuItemWebBrowserBucket = new MenuItem
                            {
                                Header = "Open Bucket in Browser",
                            };
                            mnuItemWebBrowserBucket.Click += this.MnuItemWebBrowserBucket_Click;

                            ContextMenu menu = new ContextMenu() { };
                            menu.Items.Add(mnuItemCopyNameSelectedBucket);
                            menu.Items.Add(mnuItemWebBrowserBucket);
                            item.ContextMenu = menu;
                        }
                    }
                }
            }
        }

        #endregion RightMouseButton Menu

        #region RightMouseButton Menu Event

        private async void MnuItemRefreshSelectedCloud_Click(object sender, EventArgs e)
        {
            this.GetYaExistClouds();
        }

        private void MnuItemWebBrowserCloudBilling_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start($"https://console.cloud.yandex.ru/billing/accounts/{this.billingAccountId}?section=overview");
        }

        private async void MnuItemWebBrowserCloud_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start($"https://console.cloud.yandex.ru/cloud");
        }

        private void MnuItemHealthStatusFunction_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start($"https://status.cloud.yandex.com/dashboard?service=Cloud%20Functions");
        }

        private void MnuItemHealthStatusVMs_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start($"https://status.cloud.yandex.com/dashboard?service=Compute%20Cloud");
        }

        private void MnuItemHealthStatusStorages_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start($"https://status.cloud.yandex.com/dashboard?service=Object%20Storage");
        }

        private async void MnuItemDeploySelectedFunction_Click(object sender, EventArgs e)
        {
#if DEBUG
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(YaCloudControlPanel.YaFunctions.YaClouds.YaCloudOAuthCode);
#else
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(Helpers.UsefulStuff.SettingsHelper.ReadSetting("General", "OAuthKey"));
#endif
            var task2GetFunctiionRuntime = YaCloudControlPanel.YaFunctions.YaClouds.YaCloudFunctions.GetExistsRuntimeAsync(yaAuthToken);
            YaCloudControlPanel.YaModel.RuntimeListRoot existsRuntime = await task2GetFunctiionRuntime;

            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            DeployNewFunctionVersionWindow deployNewFunctionVersionWindow = new DeployNewFunctionVersionWindow((YaCloudControlPanel.YaModel.Function)item.Tag, existsRuntime);
            deployNewFunctionVersionWindow.Show();
        }

        private void MnuItemCopyNameSelectedBucket_Click(object sender, EventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            Clipboard.SetText("Bucket Name: " + ((YaCloudControlPanel.YaModel.Bucket)item.Tag).name);
        }

        private void MnuItemWebBrowserBucket_Click(object sender, EventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            TreeViewItem itemParentMenuNodesLevel = (TreeViewItem)item.Parent;
            TreeViewItem itemParentFolder = (TreeViewItem)itemParentMenuNodesLevel.Parent;
            System.Diagnostics.Process.Start($"https://console.cloud.yandex.ru/folders/{((YaCloudControlPanel.YaModel.Folder)itemParentFolder.Tag).id}/storage/buckets/{((YaCloudControlPanel.YaModel.Bucket)item.Tag).name}");
        }

        private void MnuItemRefreshStorages_Click(object sender, EventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            item.IsExpanded = false;
            item.IsExpanded = true;
        }

        private void MnuItemWebBrowserStorages_Click(object sender, EventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            TreeViewItem itemParentFolder = (TreeViewItem)item.Parent;

            System.Diagnostics.Process.Start($"https://console.cloud.yandex.ru/folders/{((YaCloudControlPanel.YaModel.Folder)itemParentFolder.Tag).id}/storage/buckets");
        }

        private void MnuItemOpenInWebBrowserSelectedFolder_Click(object sender, EventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            System.Diagnostics.Process.Start($"https://console.cloud.yandex.ru/folders/{((YaCloudControlPanel.YaModel.Folder)item.Tag).id}");
        }

        private void MnuItemOpenInWebBrowserSelectedVM_Click(object sender, EventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            TreeViewItem itemParentMenuNodesLevel = (TreeViewItem)item.Parent;
            TreeViewItem itemParentFolder = (TreeViewItem)itemParentMenuNodesLevel.Parent;
            System.Diagnostics.Process.Start($"https://console.cloud.yandex.ru/folders/{((YaCloudControlPanel.YaModel.Folder)itemParentFolder.Tag).id}/compute/instance/{((YaCloudControlPanel.YaModel.VMInstance.Instance)item.Tag).id}/overview");
        }

        private void MnuItemOpenInWebBrowserSelectedFunction_Click(object sender, EventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            TreeViewItem itemParentMenuNodesLevel = (TreeViewItem)item.Parent;
            TreeViewItem itemParentFolder = (TreeViewItem)itemParentMenuNodesLevel.Parent;
            System.Diagnostics.Process.Start($"https://console.cloud.yandex.ru/folders/{((YaCloudControlPanel.YaModel.Folder)itemParentFolder.Tag).id}/functions/functions/{((YaCloudControlPanel.YaModel.Function)item.Tag).id}/overview");
        }

        private void MnuItemWebBrowserVMs_Click(object sender, EventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            TreeViewItem itemParentFolder = (TreeViewItem)item.Parent;
            System.Diagnostics.Process.Start($"https://console.cloud.yandex.ru/folders/{((YaCloudControlPanel.YaModel.Folder)itemParentFolder.Tag).id}/compute/instances");
        }

        private void MnuItemWebBrowserFunction_Click(object sender, EventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            TreeViewItem itemParentFolder = (TreeViewItem)item.Parent;
            System.Diagnostics.Process.Start($"https://console.cloud.yandex.ru/folders/{((YaCloudControlPanel.YaModel.Folder)itemParentFolder.Tag).id}/functions/functions");
        }

        private void MnuItemCopyIPandNameSelectedVM_Click(object sender, EventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            Clipboard.SetText("VM Name: " + ((YaCloudControlPanel.YaModel.VMInstance.Instance)item.Tag).name + " /IP Adress: " + ((YaCloudControlPanel.YaModel.VMInstance.Instance)item.Tag).networkInterfaces[0].primaryV4Address.oneToOneNat.address);
        }

        private async void MnuItemGetInfoAboutVMs_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            TreeViewItem itemParentFolder = (TreeViewItem)item.Parent;
#if DEBUG
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(YaCloudControlPanel.YaFunctions.YaClouds.YaCloudOAuthCode);
#else
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(Helpers.UsefulStuff.SettingsHelper.ReadSetting("General", "OAuthKey"));
#endif
            var task2GetVMs = YaCloudControlPanel.YaFunctions.YaClouds.YaVirtualMachines.GetExistsVMAsync(((YaCloudControlPanel.YaModel.Folder)itemParentFolder.Tag).id, yaAuthToken);
            YaCloudControlPanel.YaModel.VMInstance.VMInstancesRoot existVMs = await task2GetVMs;
            VMInfromationWindow vmInformationWindow = new VMInfromationWindow(existVMs);
            vmInformationWindow.Show();
        }

        private async void MnuItemStartSelectedVM_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            TreeViewItem itemParentMenuNodesLevel = (TreeViewItem)item.Parent;
#if DEBUG
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(YaCloudControlPanel.YaFunctions.YaClouds.YaCloudOAuthCode);
#else
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(Helpers.UsefulStuff.SettingsHelper.ReadSetting("General", "OAuthKey"));
#endif
            await YaCloudControlPanel.YaFunctions.YaClouds.YaVirtualMachines.StartVMAsync(((YaCloudControlPanel.YaModel.VMInstance.Instance)item.Tag).id, yaAuthToken);

            itemParentMenuNodesLevel.IsExpanded = false;
            itemParentMenuNodesLevel.IsExpanded = true;
        }

        private async void MnuItemStopSelectedVM_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            TreeViewItem itemParentMenuNodesLevel = (TreeViewItem)item.Parent;
#if DEBUG
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(YaCloudControlPanel.YaFunctions.YaClouds.YaCloudOAuthCode);
#else
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(Helpers.UsefulStuff.SettingsHelper.ReadSetting("General", "OAuthKey"));
#endif
            await YaCloudControlPanel.YaFunctions.YaClouds.YaVirtualMachines.StopVMAsync(((YaCloudControlPanel.YaModel.VMInstance.Instance)item.Tag).id, yaAuthToken);

            itemParentMenuNodesLevel.IsExpanded = false;
            itemParentMenuNodesLevel.IsExpanded = true;
        }

        private void MnuItemRefreshVMs_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            item.IsExpanded = false;
            item.IsExpanded = true;
        }

        private void MnuItemRefreshSelectedFunction_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            item.IsExpanded = false;
            item.IsExpanded = true;
        }

        private void MnuItemCreateNewFunction_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            TreeViewItem itemParentMenuNodesLevel = (TreeViewItem)item.Parent;
            CreateNewOrRenameFunction renameExistFunction = new CreateNewOrRenameFunction((YaCloudControlPanel.YaModel.Folder)itemParentMenuNodesLevel.Tag);
            renameExistFunction.Closed += this.ClosedCreateNewFuncWindow;
            renameExistFunction.ShowDialog();
        }

        private void ClosedCreateNewFuncWindow(object sender, System.EventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            item.IsExpanded = false;
            item.IsExpanded = true;
        }

        private void MnuItemRunSelectedFunction_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            RunFunctionVersionWindow runFuncVersionWindpw = new RunFunctionVersionWindow((YaCloudControlPanel.YaModel.Function)item.Tag);
            runFuncVersionWindpw.ShowDialog();
        }

        private void MnuItemEditNameSelectedFunction_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            TreeViewItem itemParentMenuNodesLevel = (TreeViewItem)item.Parent;
            TreeViewItem itemParentFolder = (TreeViewItem)itemParentMenuNodesLevel.Parent;

            CreateNewOrRenameFunction renameExistFunction = new CreateNewOrRenameFunction((YaCloudControlPanel.YaModel.Function)item.Tag);
            renameExistFunction.Closed += this.ClosedRenameFuncWindow;
            renameExistFunction.ShowDialog();
        }

        private void ClosedRenameFuncWindow(object sender, System.EventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            TreeViewItem itemParentMenuNodesLevel = (TreeViewItem)item.Parent;
            itemParentMenuNodesLevel.IsExpanded = false;
            itemParentMenuNodesLevel.IsExpanded = true;
        }

        private void MnuItemLoadLogsSelectedFunction_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            MessageBox.Show(((YaCloudControlPanel.YaModel.Function)item.Tag).createdAt.ToString());
        }

        private async void MnuItemLoadVersionsSelectedFunction_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
#if DEBUG
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(YaCloudControlPanel.YaFunctions.YaClouds.YaCloudOAuthCode);
#else
            string yaAuthToken = await YaCloudControlPanel.YaFunctions.YaClouds.YaAuthorization.GetYaCloudTokenAsync(Helpers.UsefulStuff.SettingsHelper.ReadSetting("General", "OAuthKey"));
#endif
            var task2GetFunctionVersions = YaCloudControlPanel.YaFunctions.YaClouds.YaCloudFunctions.GetFunctionVersionsAsync(((YaCloudControlPanel.YaModel.Function)item.Tag).id, yaAuthToken);
            YaCloudControlPanel.YaModel.FunctionVersionRoot functionVersions = await task2GetFunctionVersions;

            FunctionVersionsWindow funVersionWindow = new FunctionVersionsWindow(functionVersions, ((YaCloudControlPanel.YaModel.Function)item.Tag).name);
            funVersionWindow.ShowDialog();
        }

        private void MnuItemDeleteSelectedFunction_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            FunctionDeleteConfirmationWindow deleteFunction = new FunctionDeleteConfirmationWindow((YaCloudControlPanel.YaModel.Function)item.Tag);
            deleteFunction.Closed += this.ClosedFunctionDeleteWindow;
            deleteFunction.ShowDialog();
        }

        private void ClosedFunctionDeleteWindow(object sender, System.EventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            TreeViewItem itemParentMenuNodesLevel = (TreeViewItem)item.Parent;
            itemParentMenuNodesLevel.IsExpanded = false;
            itemParentMenuNodesLevel.IsExpanded = true;
        }

        private void MnuItemCopyIDSelectedFunction_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            Clipboard.SetText("Function ID: " + ((YaCloudControlPanel.YaModel.Function)item.Tag).id + " /Function Name: " + ((YaCloudControlPanel.YaModel.Function)item.Tag).name);
        }

        private void MnuItemCopyHttpInvokeSelectedFunction_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)this.treeView.SelectedItem;
            Clipboard.SetText(((YaCloudControlPanel.YaModel.Function)item.Tag).httpInvokeUrl);
        }

        #endregion RightMouseButton Menu Event
    }
}