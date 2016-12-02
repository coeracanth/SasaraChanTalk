namespace AddStacker
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.Remoting;
	using System.Text;
	using System.Threading.Tasks;

	internal class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length < 1)
			{
				return;
			}

			var text = args[0];

			IpcSample.IpcClient client = new IpcSample.IpcClient();

			try
			{
			// null と "" は弾きたい
			if (text?.Length > 0)
			{
				// stackListに直接addできないっぽい
				client.RemoteObject.OnMessageReceived(text);
			}
			}
			catch (RemotingException ex)
			{
				Console.WriteLine(ex.Message);
				return;
			}
		}
	}
}
