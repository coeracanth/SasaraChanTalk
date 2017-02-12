namespace SasaraChanTalk
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Linq;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using System.Windows;
	using CeVIO.Talk.RemoteService;
	using ViewModels;

	internal interface IMainViewModel
	{
		IList<string> AvailabeCast { get; }

		Talker Talker { get; set; }

		string TalkText { get; set; }

		string NowTalkText { get; set; }

		void DelTalkStack();

		void AddTalkStack();
	}

	public class MainViewModel : ViewModelBase, IMainViewModel
	{
		private string nowTalkText;
		private SpeakingState speakingState;
		private bool isTalking = false;

		public MainViewModel()
		{
			this.InitCeVIO();
			this.InitIpcServer();
		}

		public Talker Talker
		{
			get;
			set;
		}

		= new Talker();

		/// <summary>
		/// 有効なキャスト
		/// </summary>
		public IList<string> AvailabeCast => TalkerAgent.AvailableCasts;

		/// <summary>
		/// Gets or sets トーク入力用
		/// </summary>
		public string TalkText
		{
			get;
			set;
		}

		= "テストですよー？";

		public IpcSample.IpcServer IpcServer { get; set; }

		/// <summary>
		/// Gets or sets 最終発言、表示用
		/// </summary>
		public string NowTalkText
		{
			get
			{
				return this.nowTalkText;
			}

			set
			{
				this.nowTalkText = value;
				this.OnPropertyChanged();
			}
		}

		public IList<string> TalkStack { get; set; } = new ObservableCollection<string>();

		/// <summary>
		/// htmlの省略、及び大文字変換
		/// </summary>
		/// <param name="text">urlString</param>
		/// <returns>trimedString</returns>
		public static string TrimText(string text)
		{
			return Regex.Replace(text, @"https?://[\w/:%#\$&\?\(\)~\.=\+\-]+", "URL省略。").ToUpper();
		}

		/// <summary>
		/// スタックの先頭をcevioに渡す。
		/// </summary>
		public async void Speak()
		{
			if (this.speakingState?.IsCompleted ?? true)
			{
				// 再帰処理の終了条件
				if (this.TalkStack.Count == 0)
				{
					return;
				}

				this.NowTalkText = this.GetNextTalkText();

				this.speakingState = this.Talker.Speak(this.NowTalkText);

				await Task.Run(() =>
				{
					this.speakingState.Wait();
				});

				// 再帰
				this.Speak();
			}
		}

		public void AddTalkStack()
		{
			this.AddTalkStack(this.TalkText);
		}

		public void DelTalkStack()
		{
			this.TalkStack.Clear();
		}

		/// <summary>
		/// メッセージ受信サーバを設定
		/// </summary>
		private void InitIpcServer()
		{
			this.IpcServer = new IpcSample.IpcServer();
			this.IpcServer.RemoteObject.MessageReceived += this.RemoteObject_MessageReceived;
		}

		/// <summary>
		/// CeVIOと接続、パラの初期化
		/// </summary>
		private void InitCeVIO()
		{
			ServiceControl.StartHost(false);
			this.Talker.Cast = this.AvailabeCast?[0] ?? null;

			this.Talker.Volume = 100;
			this.Talker.Speed = 50;
			this.Talker.Tone = 50;
			this.Talker.Alpha = 50;
			this.Talker.ToneScale = 100;
		}

		/// <summary>
		/// トーク受信処理
		/// </summary>
		/// <param name="talkText">トーク内容</param>
		private void RemoteObject_MessageReceived(string talkText)
		{
			try
			{
				if (talkText?.Length > 0)
				{
					this.TalkStack.Add(talkText);
				}

				if (!this.isTalking)
				{
					this.isTalking = true;
					this.Speak();
					this.isTalking = false;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		/// <summary>
		/// 次のトークの取得、Getと言いつつコレクション操作してる注意
		/// 前提 : TalkStackにnullでないItemが存在する
		/// </summary>
		/// <returns>トーク</returns>
		private string GetNextTalkText()
		{
			var text = this.TalkStack.First();

			// 取得できたら削除
			this.TalkStack.RemoveAt(0);
			text = TrimText(text);

			// 100文字制限への対応
			// 超過分は分割してスタックの先頭に返す
			if (text.Length > 100)
			{
				var over = text.Substring(100);
				this.TalkStack.Insert(0, over);

				text = text.Substring(0, 100);
			}

			return text;
		}

		private void AddTalkStack(string talkText)
		{
			this.IpcServer.RemoteObject.OnMessageReceived(talkText);
		}
	}
}
