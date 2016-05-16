using MLS.ViewModel;
using System.Windows;

namespace MLS
{
    /// <summary>
    /// mls
    /// </summary>
    public partial class MainWindow 
    {
        private MLSViewModel _viewModel = null;
        public MainWindow()
        {
            InitializeComponent();

            this._viewModel = new MLSViewModel();
            this.DataContext = _viewModel;
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
