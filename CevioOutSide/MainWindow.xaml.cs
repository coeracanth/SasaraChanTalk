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

namespace CevioOutSide
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		private IMainViewModel _viewMdel;
		private DispatcherTimer _timer;

		public MainWindow()
		{
			InitializeComponent();
			this._viewMdel = (mainViewModel)DataContext;

			_timer = new DispatcherTimer();
			_timer.Interval = new TimeSpan(0, 0, 1);
			_timer.Tick += _timer_Tick;
			_timer.Start();

			SliderPanel.Children.Add(loadComponentsSlider());

		}

		/// <summary>
		/// speakの呼び出し
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _timer_Tick(object sender, EventArgs e)
		{
			_viewMdel.Speak();
		}


		/// <summary>
		/// 感情パラのスライダを動的生成
		/// </summary>
		/// <returns></returns>
		private StackPanel loadComponentsSlider()
		{
			var panel = new StackPanel();

			panel.SetValue(Grid.ColumnProperty, 1);

			//foreachは使えないみたい
			var count = _viewMdel.Talker.Components.Count;
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
				gbox.Header = _viewMdel.Talker.Components[counta].Name;
				gbox.Content = dockp;

				panel.Children.Add(gbox);
			}

			return panel;
		}

		/// <summary>
		/// talktextの内容をスタックへ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this._viewMdel.AddTalkStack();
		}

		/// <summary>
		/// キャスト変更で感情パラ用スライダを変更
		/// propChange検知してできないかなあ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(SliderPanel.Children.Count >= 2)
			{
				SliderPanel.Children.RemoveAt(1);
				SliderPanel.Children.Add(loadComponentsSlider());
			}
		}
	}
}
