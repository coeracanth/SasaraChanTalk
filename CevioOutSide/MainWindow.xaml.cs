namespace SasaraChanTalk
{
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
	using System.Windows.Threading;

	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();

			this.SliderPanel.Children.Add(this.LoadComponentsSlider());
		}

		private IMainViewModel ViewModel
		{
			get { return (IMainViewModel)this.DataContext; }
		}

		/// <summary>
		/// 感情パラのスライダを生成
		/// </summary>
		/// <returns>slider</returns>
		private StackPanel LoadComponentsSlider()
		{
			var panel = new StackPanel();

			panel.SetValue(Grid.ColumnProperty, 1);

			// foreachは使えないみたい
			var count = this.ViewModel.Talker.Components.Count;
			for (int counta = 0; counta < count; counta++)
			{
				Binding bind = new Binding();
				var proppath = new PropertyPath($"Talker.Components[{counta}].Value");
				bind.Path = proppath;

				TextBox tbox = new TextBox();
				tbox.SetValue(DockPanel.DockProperty, Dock.Right);
				tbox.SetBinding(TextBox.TextProperty, bind);

				Slider sld = new Slider();
				sld.SetBinding(Slider.ValueProperty, bind);

				DockPanel dockp = new DockPanel();
				dockp.Children.Add(tbox);
				dockp.Children.Add(sld);

				GroupBox gbox = new GroupBox();
				gbox.Header = this.ViewModel.Talker.Components[counta].Name;
				gbox.Content = dockp;

				panel.Children.Add(gbox);
			}

			return panel;
		}

		/// <summary>
		/// talktextの内容をスタックへ
		/// </summary>
		/// <param name="sender">親</param>
		/// <param name="e">イベント</param>
		private void Button_addStack_Click(object sender, RoutedEventArgs e)
		{
			this.ViewModel.AddTalkStack();
		}

		/// <summary>
		/// キャスト変更で感情パラ用スライダを変更
		/// propChange検知とselectionchangeどちらがいいか
		/// </summary>
		/// <param name="sender">親</param>
		/// <param name="e">イベント</param>
		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.SliderPanel.Children.Count >= 2)
			{
				this.SliderPanel.Children.RemoveAt(1);
				this.SliderPanel.Children.Add(this.LoadComponentsSlider());
			}
		}

		/// <summary>
		/// スタックをクリア
		/// </summary>
		/// <param name="sender">親</param>
		/// <param name="e">送信元</param>
		private void Button_clear_Click(object sender, RoutedEventArgs e)
		{
			this.ViewModel.DelTalkStack();
		}
	}
}
