using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using static Helpers.UsefulStuff;

namespace YaCloudControlPanel
{
    /// <summary>
    /// Interaction logic for AddOrEditEnvironmentVariablesWindow.xaml
    /// </summary>
    public partial class AddOrEditEnvironmentVariablesWindow : Window
    {
        private DataGrid dataGridEnvVar;
        private bool isEditExist;
        private Regex regex = new Regex("^[a-zA-Z][a-zA-Z0-9_]*$");

        public AddOrEditEnvironmentVariablesWindow(DataGrid dataGrid)
        {
            this.InitializeComponent();

            this.dataGridEnvVar = dataGrid;
            this.buttonOk.IsEnabled = false;
            this.textBoxEnvValue.IsEnabled = false;
        }

        public AddOrEditEnvironmentVariablesWindow(DataGrid dataGrid, Helpers.UsefulStuff.EnvVariable currentVariable)
        {
            this.InitializeComponent();
            this.dataGridEnvVar = dataGrid;
            this.textBoxEnvName.Text = currentVariable.EnvName;
            this.textBoxEnvValue.Text = currentVariable.EnvValue;
            this.isEditExist = true;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (this.isEditExist == false)
            {
                if (this.textBoxEnvName.Text != "" && this.textBoxEnvValue.Text != "")
                {
                    this.dataGridEnvVar.Items.Add(new EnvVariable() { EnvName = this.textBoxEnvName.Text, EnvValue = this.textBoxEnvValue.Text });
                    this.Close();
                }
                else
                {
                    this.textBoxEnvName.Focus();
                }
            }
            else
            {
                ((EnvVariable)this.dataGridEnvVar.SelectedItem).EnvName = this.textBoxEnvName.Text;
                ((EnvVariable)this.dataGridEnvVar.SelectedItem).EnvValue = this.textBoxEnvValue.Text;
                this.dataGridEnvVar.Items.Refresh();
                this.Close();
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBoxEnvName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (this.regex.IsMatch(this.textBoxEnvName.Text))
            {
                this.buttonOk.IsEnabled = true;
                this.textBoxEnvValue.IsEnabled = true;
            }
            else
            {
                this.buttonOk.IsEnabled = false;
                this.textBoxEnvValue.IsEnabled = false;
            }
        }

        private void TextBoxEnvName_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (this.regex.IsMatch(this.textBoxEnvName.Text))
            {
                this.buttonOk.IsEnabled = true;
                this.textBoxEnvValue.IsEnabled = true;
            }
            else
            {
                this.buttonOk.IsEnabled = false;
                this.textBoxEnvValue.IsEnabled = false;
            }
        }
    }
}