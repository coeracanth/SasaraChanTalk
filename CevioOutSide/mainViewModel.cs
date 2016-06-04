using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;
using CeVIO.Talk.RemoteService;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Collections.Specialized;

namespace CevioOutSide
{
	class mainViewModel:ViewModelBase, IMainViewModel
	{
		private string _nowTalkText;

		#region prop
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

		public IpcSample.IpcServer IpcServer { get; set; }

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

		public IList<string> TalkStack { get; set; } = new List<string>();
		#endregion

		public mainViewModel()
		{
			ServiceControl.StartHost(false);
			Talker.Cast = AvailabeCast?[0] ?? null;

			Talker.Volume = 100;
			Talker.Speed = 50;
			Talker.Tone = 50;
			Talker.Alpha = 50;
			Talker.ToneScale = 100;

			IpcServer = new IpcSample.IpcServer();

			IpcServer.RemoteObject.MessageReceived += RemoteObject_MessageReceived;
		}

		private void RemoteObject_MessageReceived(string obj)
		{
			TalkStack.Add(obj);

			if (!isTalking)
			{
				isTalking = true;
				Speak();
				isTalking = false;
			}
		}

		private bool isTalking = false;

		/// <summary>
		/// スタックの先頭をcevioに渡す。
		/// </summary>
		public async void Speak()
		{
			if (_speakingState?.IsCompleted ?? true)
			{
				var text = TalkStack.FirstOrDefault();
				if(text == null)
				{
					return;
				}

				//取得できたら削除
				TalkStack.RemoveAt(0);

				text = Regex.Replace(text, @"https?://[\w/:%#\$&\?\(\)~\.=\+\-]+", "URL省略。");

				//100文字制限への対応
				//超過分は分割してスタックの先頭に返す
				if (text.Length > 100)
				{
					var over = text.Substring(100);
					TalkStack.Insert(0, over);

					text = text.Substring(0, 100);
				}

				NowTalkText = text.ToUpper();


				_speakingState = Talker.Speak(this.NowTalkText);

				await Task.Run(() =>
				{
					_speakingState.Wait();
				});

				//再帰
				Speak();
			}
		}

		public void AddTalkStack()
		{
			AddTalkStack(this.TalkText);
		}

		private void AddTalkStack(string talkText)
		{
			this.IpcServer.RemoteObject.OnMessageReceived(talkText);
		}

		public void DelTalkStack()
		{
			TalkStack.Clear();
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
