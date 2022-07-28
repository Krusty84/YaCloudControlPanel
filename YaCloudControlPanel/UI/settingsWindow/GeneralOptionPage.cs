using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;

namespace YaCloudControlPanel.UI.Settings
{
    [ComVisible(true)]
    [Guid("FD5ECC49-C4F4-4994-8A7E-BEB619E956F0")]
    public class GeneralOptionPage : UIElementDialogPage
    {
        protected override UIElement Child
        {
            get
            {
                GeneralOptionsPageWindow page = new GeneralOptionsPageWindow
                {
                    generalOptionsPage = this,
                };
                page.Initialize();
                return page;
            }
        }
    }
}