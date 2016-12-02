namespace IpcSample
{
	using System;
	using System.Runtime.Remoting;
	using System.Runtime.Remoting.Channels;
	using System.Runtime.Remoting.Channels.Ipc;

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
			// クライアントチャンネルの生成
			IpcClientChannel channel = new IpcClientChannel();

			//不要？
			// チャンネルを登録
			ChannelServices.RegisterChannel(channel, true);

			// リモートオブジェクトを取得
			RemoteObject = Activator.GetObject(typeof(IpcRemoteObject), "ipc://CeVIOOutSideReader/TextStack") as IpcRemoteObject;
		}

	}

	public class IpcRemoteObject : MarshalByRefObject
	{
		/// <summary>
		/// タイムアウトを回避する
		/// </summary>
		public override object InitializeLifetimeService()
		{
			return null;
		}

		/// <summary> 
		/// メッセージを受信したときに発生します。 
		/// </summary> 
		public event Action<string> MessageReceived;

		/// <summary> 
		/// MessageEventHandler イベントを発生させます。 
		/// </summary> 
		public void OnMessageReceived(string message)
		{
			if (MessageReceived != null)
				MessageReceived(message);
		}
	}
}