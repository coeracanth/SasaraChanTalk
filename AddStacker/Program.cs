using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddStacker
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 1)
			{
				return;
			}

			var text = args[0];

			IpcSample.IpcClient client = new IpcSample.IpcClient();

			//null と "" は弾きたい
			if (text?.Length > 0)
			{
				//stackListに直接addできないっぽい
				var stack = client.RemoteObject.TalkTextStack;
				stack.Add(text);
				client.RemoteObject.TalkTextStack = stack;
			}

		}
	}
}
