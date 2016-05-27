using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;
using CeVIO.Talk.RemoteService;
using System.Text.RegularExpressions;

namespace CevioOutSide
{
	class mainViewModel:ViewModelBase, IMainViewModel
	{
		private string _nowTalkText;

		public Talker Talker
		{
			get;
			set;
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
			get;
			set;
		} = "テストですよー？";

		private SpeakingState _speakingState;

		public IpcSample.IpcServer TalkStack { get; set; }

		public string NowTalkText
		{
			get
			{
				return _nowTalkText;
			}

			set
			{
				_nowTalkText = value;
				OnPropertyChanged();
			}
		}

		public mainViewModel()
		{
			ServiceControl.StartHost(false);
			Talker.Cast = AvailabeCast?[0] ?? null;

			Talker.Volume = 100;
			Talker.Speed = 50;
			Talker.Tone = 50;
			Talker.Alpha = 50;
			Talker.ToneScale = 100;

			TalkStack = new IpcSample.IpcServer();
		}

		/// <summary>
		/// スタックの先頭をcevioに渡す。
		/// timerで呼び出しかなあ、
		/// stateのcompletedを検知できればstackが尽きるまで回すとかできそうだけど
		/// state.wait()でのループはUI触れなくなるのでなし。
		/// 別スレッドでやればいいかもしれんがやり方わからん
		/// </summary>
		public void Speak()
		{
			if (_speakingState?.IsCompleted ?? true)
			{
				var text = TalkStack.RemoteObject.TalkTextStack.FirstOrDefault();
				if(text == null)
				{
					return;
				}

				//取得できたら削除
				TalkStack.RemoteObject.TalkTextStack.RemoveAt(0);

				text = Regex.Replace(text, @"https ?://[\w/:%#\$&\?\(\)~\.=\+\-]+", "URL省略。");

				//100文字制限への対応
				//超過分は分割してスタックの先頭に返す
				if (text.Length > 100)
				{
					var over = text.Substring(100);
					TalkStack.RemoteObject.TalkTextStack.Insert(0, over);

					text = text.Substring(0, 100);
				}

				NowTalkText = text.ToUpper();
				_speakingState = Talker.Speak(this.NowTalkText);
			}
		}

		public void AddTalkStack()
		{
			AddTalkStack(this.TalkText);
		}

		private void AddTalkStack(string talkText)
		{
			TalkStack.RemoteObject.TalkTextStack.Add(talkText);
		}

		public void DelTalkStack()
		{
			TalkStack.RemoteObject.TalkTextStack.Clear();
		}
	}

	interface IMainViewModel
	{
		IList<string> AvailabeCast { get; }
		Talker Talker { get; set; }

		string TalkText { get; set; }

		void Speak();
		void DelTalkStack();
		void AddTalkStack();

	}
}
