using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Remoting.Lifetime;

namespace IpcSample
{
	public class IpcServer
	{
		public IpcRemoteObject RemoteObject { get; set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public IpcServer()
		{
			// サーバーチャンネルの生成
			IpcServerChannel channel = new IpcServerChannel("CeVIOOutSideReader");

			// チャンネルを登録
			ChannelServices.RegisterChannel(channel, true);

			// リモートオブジェクトを生成して公開
			RemoteObject = new IpcRemoteObject();
			RemotingServices.Marshal(RemoteObject, "TextStack", typeof(IpcRemoteObject));
		}
	}
	public class IpcClient
	{
		public IpcRemoteObject RemoteObject { get; set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public IpcClient()
		{
			LifetimeServices.LeaseTime = TimeSpan.Zero;
			LifetimeServices.RenewOnCallTime = TimeSpan.Zero;

			// クライアントチャンネルの生成
			IpcClientChannel channel = new IpcClientChannel();

			// チャンネルを登録
			ChannelServices.RegisterChannel(channel, true);

			// リモートオブジェクトを取得
			RemoteObject = Activator.GetObject(typeof(IpcRemoteObject), "ipc://CeVIOOutSideReader/TextStack") as IpcRemoteObject;
		}
	}

	public class IpcRemoteObject : MarshalByRefObject, INotifyPropertyChanged
	{
		public IList<string> TalkTextStack { get; set; } = new ObservableCollection<string>();

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