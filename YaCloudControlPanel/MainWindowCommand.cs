using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace YaCloudControlPanel.WIndow
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class MainWindowCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("dff265a0-366c-488c-8fea-edf02f92e850");

        private MainWindow window;

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private MainWindowCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static MainWindowCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        /*private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }
        */

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in MainWindowCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new MainWindowCommand(package, commandService);
        }

        /// <summary>
        /// Shows the tool window when the menu item is clicked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            this.package.JoinableTaskFactory.RunAsync(async delegate
            {
                this.window = (MainWindow)this.package.FindToolWindow(typeof(MainWindow), 0, true);
                if ((this.window == null) || (this.window.Frame == null))
                {
                    throw new NotSupportedException("Cannot create tool window");
                }

                IVsWindowFrame windowFrame = (IVsWindowFrame)this.window.Frame;
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

                ////I will not use the ToolWindow Toolbar
                /*OleMenuCommandService mcs = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;

                var toolbarbtnRefreshCmd = new CommandID(new Guid(YaCloudControlPanelPackage.guidYaCloudControlPanelPackageCmdSet),
                    YaCloudControlPanelPackage.TWRefreshButtonCommand);
                var menuItem = new MenuCommand(new EventHandler(
                     ButtonRefreshHandler), toolbarbtnRefreshCmd);
                mcs.AddCommand(menuItem);*/
            });
        }

        ////I will not use the ToolWindow Toolbar
        /*
         private void ButtonRefreshHandler(object sender, EventArgs arguments)
         {
             TreeViewItem item = (TreeViewItem)window.control.treeView.SelectedItem;
             if (item != null && item.Tag.ToString() == "Functions")
             {
                 item.IsExpanded = false;
                 item.IsExpanded = true;
             }
             else if (item != null && item.Tag.ToString() == "YaCloudControlPanel.YaModel.Function")
             {
                 TreeViewItem itemParentMenuNodesLevel = (TreeViewItem)item.Parent;
                 itemParentMenuNodesLevel.IsExpanded = false;
                 itemParentMenuNodesLevel.IsExpanded = true;
             }
             else if (item != null && item.Tag.ToString() == "VirtualMachines")
             {
                 item.IsExpanded = false;
                 item.IsExpanded = true;
             }
             else if (item != null && item.Tag.ToString() == "YaCloudControlPanel.YaModel.VMInstance.Instance")
             {
                 TreeViewItem itemParentMenuNodesLevel = (TreeViewItem)item.Parent;
                 itemParentMenuNodesLevel.IsExpanded = false;
                 itemParentMenuNodesLevel.IsExpanded = true;
             }
             else if (item != null && item.Tag.ToString() == "YaCloudControlPanel.YaModel.Folder")
             {
                 item.IsExpanded = false;
                 item.IsExpanded = true;
             }
             else if (item != null && item.Tag.ToString() == "Storages")
             {
                 item.IsExpanded = false;
                 item.IsExpanded = true;
             }
             else if (item != null && item.Tag.ToString() == "YaCloudControlPanel.YaModel.Bucket")
             {
                 TreeViewItem itemParentMenuNodesLevel = (TreeViewItem)item.Parent;
                 itemParentMenuNodesLevel.IsExpanded = false;
                 itemParentMenuNodesLevel.IsExpanded = true;
             }
         }
         */
    }
}