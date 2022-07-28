using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace YaCloudControlPanel
{
    /// <summary>
    /// Interaction logic for AddOrEditEnvironmentVariablesWindow.xaml
    /// </summary>
    public partial class AddTagWindow : Window
    {
        private DataGrid dataGridTags;
        private Regex regex = new Regex("^[a-z][-_0-9a-z]*$");

        public AddTagWindow(DataGrid dataGrid)
        {
            this.InitializeComponent();
            this.dataGridTags = dataGrid;
            this.buttonOk.IsEnabled = false;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (this.textBoxTagName.Text != string.Empty)
            {
                this.dataGridTags.Items.Add(new Helpers.UsefulStuff.Tag() { tag = this.textBoxTagName.Text });
                this.Close();
            }
            else
            {
                this.textBoxTagName.Focus();
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBoxTagName_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (this.regex.IsMatch(this.textBoxTagName.Text))
            {
                this.buttonOk.IsEnabled = true;
            }
            else
            {
                this.buttonOk.IsEnabled = false;
            }
        }

        private void TextBoxTagName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (this.regex.IsMatch(this.textBoxTagName.Text))
            {
                this.buttonOk.IsEnabled = true;
            }
            else
            {
                this.buttonOk.IsEnabled = false;
            }
        }
    }
}