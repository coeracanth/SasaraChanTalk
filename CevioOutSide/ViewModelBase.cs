namespace ViewModels
{
	using System;
	using System.ComponentModel;
	using System.Linq.Expressions;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// ViewModel 基底クラス を表現します。
	/// </summary>
	public class ViewModelBase : INotifyPropertyChanged
	{
		/// <summary>
		/// プロパティ値が変更されたことをクライアントに通知します。
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// PropertyChanged イベント を発生させます。
		/// </summary>
		/// <param name="propertyName">変更されたプロパティの名前</param>
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}