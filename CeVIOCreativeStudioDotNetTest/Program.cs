using CeVIO.Talk.RemoteService;

namespace CeVIOCreativeStudioDotNetTest
{
	class PluginCevio
	{
		static void Main(string[] args)
		{
			if(args.Length < 1)
			{
				return;
			}

			string talkText = args[0];

			ServiceControl.StartHost(true);

			Talker talker = new Talker();

			// キャスト設定
			talker.Cast = "さとうささら";

			// （例）音量設定
			talker.Volume = 100;
			talker.Speed = 56;
			talker.ToneScale = 100;

			// （例）再生
			SpeakingState state = talker.Speak(talkText);

			state.Wait();
		}
	}
}
