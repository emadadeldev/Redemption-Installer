// =======================================================
// Developer: Emad Adel
// Source Code https://github.com/emadadeldev/Redemption
// =======================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Net.Http;
using System.IO.Compression;
using System.Windows.Forms;

namespace Redemption_Team
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Options_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Visibility = Visibility.Collapsed;
            options.Visibility = Visibility.Visible;
        }

        private void CloseBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show(
                "هل أنت متأكد من الخروج؟",
                "تأكيد",                 
                MessageBoxButton.YesNo,    
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
        }
    }
}
