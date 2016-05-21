using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;

namespace IpcSample
{
	class IpcServer
	{
		public IpcRemoteObject RemoteObject { get; set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public IpcServer()
		{
			// サーバーチャンネルの生成
			IpcServerChannel channel = new IpcServerChannel("ipcSample");

			// チャンネルを登録
			ChannelServices.RegisterChannel(channel, true);

			// リモートオブジェクトを生成して公開
			RemoteObject = new IpcRemoteObject();
			RemotingServices.Marshal(RemoteObject, "test", typeof(IpcRemoteObject));
		}
	}
	class IpcClient
	{
		public IpcRemoteObject RemoteObject { get; set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public IpcClient()
		{
			// クライアントチャンネルの生成
			IpcClientChannel channel = new IpcClientChannel();

			// チャンネルを登録
			ChannelServices.RegisterChannel(channel, true);

			// リモートオブジェクトを取得
			RemoteObject = Activator.GetObject(typeof(IpcRemoteObject), "ipc://ipcSample/test") as IpcRemoteObject;
		}
	}

	public class IpcRemoteObject : MarshalByRefObject
	{
		public int Counter { get; set; }
	}
}