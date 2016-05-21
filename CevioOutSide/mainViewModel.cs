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
		/// <summary>
		/// 声の大きさ
		/// </summary>
		public uint Volume { get; set; } = 100;
		/// <summary>
		/// 話す速さ
		/// </summary>
		public uint Speed { get; set; } = 50;
		/// <summary>
		/// 声の高さ
		/// </summary>
		public uint Tone { get; set; } = 50;
		/// <summary>
		/// 声質
		/// </summary>
		public uint Alpha { get; set; } = 50;
		/// <summary>
		/// 抑揚
		/// </summary>
		public uint ToneScale { get; set; } = 100;
		/// <summary>
		/// 利用可能なキャスト名
		/// </summary>
		public IList<string> AvailabeCast { get { return Talker.AvailableCasts; } }
		/// <summary>
		/// 現在選択中のキャスト
		/// </summary>
		public string CurrentCast { get; set; }

		public string TalkText
		{
			get
;
			set
;
		} = "テストですにーよ。テストと言ったらテストなんですにー";

		public TalkerComponentCollection TalkerComponentCollection
		{
			get
;
			set
;		}

		public Talker Talker
		{
			get
;
			set
;
		} = new Talker();

		public mainViewModel()
		{
			ServiceControl.StartHost(true);
			CurrentCast = AvailabeCast?[0] ?? null;
		}

		public void Speak()
		{
			Talker talker = new Talker(CurrentCast);

			talker.Volume = this.Volume;
			talker.Speed = this.Speed;
			talker.Tone = this.Tone;
			talker.Alpha = this.Alpha;
			talker.ToneScale = this.ToneScale;

			SpeakingState state = talker.Speak(TalkText);

			state.Wait();
		}
	}

	interface IMainViewModel
	{
		uint Volume { get; set; }
		uint Speed { get; set; }
		uint Tone { get; set; }
		uint Alpha { get; set; }
		uint ToneScale { get; set; }
		IList<string> AvailabeCast { get; }
		string CurrentCast { get; set; }
		string TalkText { get; set; }
		TalkerComponentCollection TalkerComponentCollection { get; set; }

		Talker Talker { get; set; }

		void Speak();
	}
}
