using System.Windows;

namespace YaCloudControlPanel
{
    /// <summary>
    /// Interaction logic for VMInfromationWindow.xaml
    /// </summary>
    public partial class VMInfromationWindow : Window
    {
        public VMInfromationWindow(YaCloudControlPanel.YaModel.VMInstance.VMInstancesRoot existVMs)
        {
            this.InitializeComponent();
            for (int i = 0; i < existVMs.instances.Count; i++)
            {
                this.dataGridVMInfo.Items.Add(existVMs.instances[i]);
            }
        }
    }
}