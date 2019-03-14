using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HomeBudget
{
    /// <summary>
    /// Interaction logic for LoginDialogWindow.xaml
    /// </summary>
    public partial class LoginDialogWindow : Window
    {
        public LoginDialogWindow()
        {
            InitializeComponent();
        }

        private void Accept(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn.Content.ToString() == "OK")
                DialogResult = auth.AuthorizeUser(loginTextBox.Text, passTextBox.Password);

            else if (btn.Content.ToString() == "Anuluj")
                DialogResult = false;
        }

        private Authorization auth = new Authorization();
    }
}
