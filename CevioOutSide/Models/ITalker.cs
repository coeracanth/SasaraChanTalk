namespace SasaraChanTalk.Models
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using CeVIO.Talk.RemoteService;
	using Microsoft.Practices.Prism.Mvvm;

	public interface ITalker
	{
		uint Volume { get; set; }

		uint Speed { get; set; }

		uint Tone { get; set; }

		uint Alpha { get; set; }

		uint ToneScale { get; set; }

		TalkerComponentCollection Components { get; }

		string Cast { get; set; }

		bool IsSpeaking { get; }

		Task SpeakAsync(string talk);
	}

	internal class Talker : BindableBase, ITalker
	{
		private SpeakingState state;

		public Talker()
		{
			this.Cast = TalkerAgent.AvailableCasts.FirstOrDefault();

			this.Volume = Properties.Settings.Default.Volume;
			this.Speed = Properties.Settings.Default.Speed;
			this.Tone = Properties.Settings.Default.Tone;
			this.Alpha = Properties.Settings.Default.Alpha;
			this.ToneScale = Properties.Settings.Default.ToneScale;
		}

		public uint Alpha
		{
			get
			{
				return this.CevioTalker.Alpha;
			}

			set
			{
				this.CevioTalker.Alpha = value;
				this.OnPropertyChanged(nameof(this.Alpha));
			}
		}

		public bool IsSpeaking
		{
			get
			{
				return !this.state?.IsCompleted ?? false;
			}
		}

		public uint Speed
		{
			get
			{
				return this.CevioTalker.Speed;
			}

			set
			{
				this.CevioTalker.Speed = value;
				this.OnPropertyChanged(nameof(this.Speed));
			}
		}

		public uint Tone
		{
			get
			{
				return this.CevioTalker.Tone;
			}

			set
			{
				this.CevioTalker.Tone = value;
				this.OnPropertyChanged(nameof(this.Tone));
			}
		}

		public uint ToneScale
		{
			get
			{
				return this.CevioTalker.ToneScale;
			}

			set
			{
				this.CevioTalker.ToneScale = value;
				this.OnPropertyChanged(nameof(this.ToneScale));
			}
		}

		public uint Volume
		{
			get
			{
				return this.CevioTalker.Volume;
			}

			set
			{
				this.CevioTalker.Volume = value;
				this.OnPropertyChanged(nameof(this.Volume));
			}
		}

		public TalkerComponentCollection Components
		{
			get
			{
				return this.CevioTalker.Components;
			}
		}

		public string Cast
		{
			get
			{
				return this.CevioTalker.Cast;
			}

			set
			{
				this.CevioTalker.Cast = value;
				this.OnPropertyChanged(nameof(this.Cast));
			}
		}

		private CeVIO.Talk.RemoteService.Talker CevioTalker { get; set; } = new CeVIO.Talk.RemoteService.Talker();

		public async Task SpeakAsync(string talk)
		{
			this.state = this.CevioTalker.Speak(talk);

			await Task.Run(() =>
			{
				this.state.Wait();
			});

			this.SaveProp();
		}

		private void SaveProp()
		{
			Properties.Settings.Default.Volume = this.Volume;
			Properties.Settings.Default.Speed = this.Speed;
			Properties.Settings.Default.Tone = this.Tone;
			Properties.Settings.Default.Alpha = this.Alpha;
			Properties.Settings.Default.ToneScale = this.ToneScale;
			Properties.Settings.Default.Save();
		}
	}
}
