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
using System.Data.OleDb;
using MLS.ViewModel;

namespace MLS
{
    /// <summary>
    /// 马拉松
    /// </summary>
    public partial class MainWindow : Window
    {
        private MLSViewModel _viewModel = null;
        public MainWindow()
        {
            InitializeComponent();

            this._viewModel = new MLSViewModel();
            _viewModel.Init();
            this.DataContext = _viewModel;
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
