using MLS.ViewModel;
using System;
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
            MessageBox.Show("i'm ysj");
        }

        private void cmbTop_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var top = cmbTop.SelectedValue.ToString();
            var count = 0;
            if (top == "全部")
            {
                count = Int32.MaxValue;
            }
            else
            {
                count = Convert.ToInt32(top.Substring(1));
            }
            _viewModel.Top(count);
        }

        private void btnQueryScore_Click(object sender, RoutedEventArgs e)
        {
            var key = tbKey.Text.Trim();
            if( string.IsNullOrEmpty(key))
                return;
            _viewModel.Search(key);
        }
    }
}
