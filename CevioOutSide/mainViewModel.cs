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
using System.Collections.ObjectModel;
using System.Windows;

namespace CevioOutSide
{
	class mainViewModel:ViewModelBase, IMainViewModel
	{
		private string _nowTalkText;
		private SpeakingState _speakingState;

		#region prop
		public Talker Talker
		{
			get;
			set;
		} = new Talker();

		/// <summary>
		/// 有効なキャスト
		/// </summary>
		public IList<string> AvailabeCast
		{
			get
			{
				return Talker.AvailableCasts;
			}
		}

		/// <summary>
		/// トーク入力用
		/// </summary>
		public string TalkText
		{
			get;
			set;
		} = "テストですよー？";

		public IpcSample.IpcServer IpcServer { get; set; }

		/// <summary>
		/// 最終発言、表示用
		/// </summary>
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

		public IList<string> TalkStack { get; set; } = new ObservableCollection<string>();
		#endregion

		public mainViewModel()
		{
			InitCeVIO();
			InitIpcServer();
		}

		/// <summary>
		/// メッセージ受信サーバを設定
		/// </summary>
		private void InitIpcServer()
		{
			IpcServer = new IpcSample.IpcServer();
			IpcServer.RemoteObject.MessageReceived += RemoteObject_MessageReceived;
		}

		/// <summary>
		/// CeVIOと接続、パラの初期化
		/// </summary>
		private void InitCeVIO()
		{
			ServiceControl.StartHost(false);
			Talker.Cast = AvailabeCast?[0] ?? null;

			Talker.Volume = 100;
			Talker.Speed = 50;
			Talker.Tone = 50;
			Talker.Alpha = 50;
			Talker.ToneScale = 100;
		}

		/// <summary>
		/// トーク受信処理
		/// </summary>
		/// <param name="talkText"></param>
		private void RemoteObject_MessageReceived(string talkText)
		{
			try
			{
				if(talkText?.Length > 0)
				{
					TalkStack.Add(talkText);
				}

				if (!isTalking)
				{
					isTalking = true;
					Speak();
					isTalking = false;
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
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
				//再帰処理の終了条件
				if(TalkStack.Count == 0)
				{
					return;
				}

				NowTalkText = GetNextTalkText();

				_speakingState = Talker.Speak(this.NowTalkText);

				await Task.Run(() =>
				{
					_speakingState.Wait();
				});

				//再帰
				Speak();
			}
		}

		/// <summary>
		/// 次のトークの取得、Getと言いつつコレクション操作してる注意
		/// 前提 : TalkStackにnullでないItemが存在する
		/// </summary>
		/// <returns></returns>
		private string GetNextTalkText()
		{
			var text = TalkStack.First();

			//取得できたら削除
			TalkStack.RemoveAt(0);

			//整形
			text = Regex.Replace(text, @"https?://[\w/:%#\$&\?\(\)~\.=\+\-]+", "URL省略。").ToUpper();

			//100文字制限への対応
			//超過分は分割してスタックの先頭に返す
			if (text.Length > 100)
			{
				var over = text.Substring(100);
				TalkStack.Insert(0, over);

				text = text.Substring(0, 100);
			}

			return text;
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

		void DelTalkStack();
		void AddTalkStack();

	}
}
