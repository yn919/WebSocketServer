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
using WebSocketServer.ViewModels;

namespace WebSocketServer.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        private MainViewModel mainViewModel { get; }
        public MainWindow()
        {
            InitializeComponent();

            this.Closing += MainWindow_Closing;

            mainViewModel = new MainViewModel();
            mainViewModel.Start();
            DataContext = mainViewModel;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainViewModel.End();
            mainViewModel.Dispose();
        }
    }
}
