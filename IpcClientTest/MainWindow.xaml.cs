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

namespace IpcClientTest
{
	public class IpcViewModel
	{
		public IpcSample.IpcClient Client { get; set; }

		public IpcViewModel()
		{
			Client = new IpcSample.IpcClient();
		}
	}

	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		private IpcViewModel _viewModel;

		public MainWindow()
		{
			InitializeComponent();

			this._viewModel = (IpcViewModel)this.DataContext;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this._viewModel.Client.RemoteObject.Counter++;
		}
	}
}
