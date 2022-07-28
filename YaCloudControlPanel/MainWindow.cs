using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace YaCloudControlPanel.WIndow
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("14826de9-6f69-472a-a5af-d18778cf5665")]
    public class MainWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        ///
        public MainWindowControl control;

        public MainWindow() : base(null)
        {
            this.Caption = "YaCloud Control Panel";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.

            // this.Content = new MainWindowControl();
            this.control = new MainWindowControl();
            base.Content = this.control;

            // I will not use the ToolWindow Toolbar
            // this.ToolBar = new CommandID(new Guid(YaCloudControlPanelPackage.guidYaCloudControlPanelPackageCmdSet), YaCloudControlPanelPackage.TWToolbar);
            // this.ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;
        }
    }
}