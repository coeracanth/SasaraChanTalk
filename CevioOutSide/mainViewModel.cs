using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;
using CeVIO.Talk.RemoteService;

namespace CevioOutSide
{
	class mainViewModel:ViewModelBase, IMainViewModel
	{
		public Talker Talker
		{
			get
;
			set
;
		} = new Talker();

		public IList<string> AvailabeCast
		{
			get
			{
				return Talker.AvailableCasts;
			}
		}

		public string TalkText
		{
			get
;
			set
;
		} = "テストですにー。テストと言ったらテストなんですにー。おちんぽしゃぶしゃぶ！";

		private SpeakingState _speakingState;

		public IpcSample.IpcServer server { get; set; }

		public mainViewModel()
		{
			ServiceControl.StartHost(false);
			Talker.Cast = AvailabeCast?[0] ?? null;

			Talker.Volume = 100;
			Talker.Speed = 50;
			Talker.Tone = 50;
			Talker.Alpha = 50;
			Talker.ToneScale = 100;

			server = new IpcSample.IpcServer();
		}

		public void Speak()
		{
			if (_speakingState?.IsCompleted ?? true)
			{
			_speakingState = Talker.Speak(TalkText);

			}
		}
	}

	interface IMainViewModel
	{
		IList<string> AvailabeCast { get; }
		Talker Talker { get; set; }

		string TalkText { get; set; }

		void Speak();
	}
}
