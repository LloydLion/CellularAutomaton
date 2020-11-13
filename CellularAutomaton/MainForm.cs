using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Video.FFMPEG;
using StandardLibrary.Other.Stade;

namespace CellularAutomaton
{
	public partial class MainForm : Form
	{
		private const int scale = 8;
		private const int resolution = 100;

		private readonly GameLogic logic;
		private readonly StadeMachine<GlobalStade, MainForm, StandardStade<GlobalStade, MainForm>> stadeMachine;
		private readonly Dictionary<Toggle, bool> toggleValues = new Dictionary<Toggle, bool>(Enum.GetValues(typeof(Toggle)).Length);
		private readonly VideoFileWriter videoWriter = new VideoFileWriter();


		public MainForm()
		{
			InitializeComponent();

			Height = Width = resolution * scale;

			logic = new GameLogic(resolution, resolution);
			logic.OnFieldChanged += (s) => { UpdateGraphicsFromField(); };

			foreach (var item in Enum.GetValues(typeof(Toggle)).OfType<Toggle>())
				toggleValues.Add(item, false);
			

			stadeMachine = new StadeMachine<GlobalStade, MainForm, StandardStade<GlobalStade, MainForm>>(control: this, stades: new StandardStade<GlobalStade, MainForm>[]
			{
				new StandardStade<GlobalStade, MainForm>(GlobalStade.Calculating, StadeSelected_Calculating),
				new StandardStade<GlobalStade, MainForm>(GlobalStade.Running, StadeSelected_Running),
				new StandardStade<GlobalStade, MainForm>(GlobalStade.Stopped, StadeSelected_Stopped, StadeDeselected_Stopped),
			});
		}

		private void GlobalTicker_Tick(object sender, EventArgs e)
		{
			var lastStade = stadeMachine.CurrentStade;

			if(lastStade.ShortcutValue == GlobalStade.Running)
			{
				stadeMachine.SetStade(GlobalStade.Calculating);

				if(toggleValues[Toggle.IsRecordingVideo] == true)
				{
					videoWriter.WriteVideoFrame(pictureBox.Image as Bitmap);
				}

				UpdateField();

				stadeMachine.SetStade(lastStade.ShortcutValue);
			}
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			logic.GenerateRandomField();
			UpdateGraphicsFromField();
		}

		private void UpdateField()
		{
			logic.Update();
		}

		private void UpdateGraphicsFromField()
		{
			pictureBox.Image = new Bitmap(resolution * scale, resolution * scale);
			var graphics = Graphics.FromImage(pictureBox.Image);

			graphics.FillRectangle(Brushes.Red, new Rectangle(new Point(0, 0), new Size(pictureBox.Image.Width, pictureBox.Image.Height)));

			var mainBuffer = logic.GetField();

			for (int x = 0; x <= mainBuffer.GetUpperBound(0); x++)
			{
				for (int y = 0; y <= mainBuffer.GetUpperBound(1); y++)
				{
					graphics.FillRectangle(new SolidBrush(CellStade.KeyConverter[mainBuffer[x, y]].DrawColor), new Rectangle(new Point(x * scale, y * scale), new Size(scale, scale)));
				}
			}

			graphics.Save();
			pictureBox.Update();

			graphics.Dispose();
		}

		private void PictureBox_MouseMove(object sender, MouseEventArgs e)
		{
			if(stadeMachine.CurrentStade.ShortcutValue == GlobalStade.Stopped)
			{
				if(e.Button.HasFlag(MouseButtons.Left))
				{
					try
					{ logic.SetCell(CellStadeKey.Filled, e.Location.X / scale, e.Location.Y / scale); }
					catch (Exception) { }

					UpdateGraphicsFromField();
				}
				else if(e.Button.HasFlag(MouseButtons.Right))
				{
					try
					{ logic.SetCell(CellStadeKey.Empty, e.Location.X / scale, e.Location.Y / scale); }
					catch (Exception) { }

					UpdateGraphicsFromField();
				}
			}
		}

		private void MainForm_KeyPress(object sender, KeyEventArgs e)
		{
			bool handled = false;

			if(e.KeyCode == Keys.F)
			{
				handled = true;
				if(stadeMachine.CurrentStade.ShortcutValue == GlobalStade.Stopped) ResumeGame(); else StopGame();
			}

			if(e.KeyCode == Keys.R && stadeMachine.CurrentStade.ShortcutValue == GlobalStade.Stopped)
			{
				handled = true;
				var res = MessageBox.Show("Вы уверены что хотите случайно заполнить поле?", "вы нажали клавишу [r]", MessageBoxButtons.YesNo);
				if(res == DialogResult.Yes)
				{
					logic.GenerateRandomField();
				}
			}

			if (e.KeyCode == Keys.C && stadeMachine.CurrentStade.ShortcutValue == GlobalStade.Stopped)
			{
				handled = true;
				var res = MessageBox.Show("Вы уверены что хотите очистить поле?", "вы нажали клавишу [c]", MessageBoxButtons.YesNo);
				if (res == DialogResult.Yes)
				{
					logic.ClearField(CellStadeKey.Empty);
				}
			}

			if(e.KeyCode == Keys.S && stadeMachine.CurrentStade.ShortcutValue == GlobalStade.Stopped)
			{
				handled = true;
				var res = saveGameImageDialog.ShowDialog();
				if (res == DialogResult.OK)
				{
					Image img = pictureBox.Image;
					img.Save(saveGameImageDialog.FileName);
				}
			}

			if (e.KeyCode == Keys.V)
			{
				handled = true;

				if(toggleValues[Toggle.IsRecordingVideo] == false && stadeMachine.CurrentStade.ShortcutValue == GlobalStade.Stopped)
				{
					if(saveVideoDialog.ShowDialog() == DialogResult.OK)
					{
						toggleValues[Toggle.IsRecordingVideo] = true;
						videoWriter.Open(saveVideoDialog.FileName, pictureBox.Image.Width, pictureBox.Image.Height, new Accord.Math.Rational(1 / (globalTicker.Interval / 1000f)), VideoCodec.Default);
						
						ResumeGame();
					}
				}
				else if(toggleValues[Toggle.IsRecordingVideo] == true)
				{
					videoWriter.Flush();
					videoWriter.Close();

					toggleValues[Toggle.IsRecordingVideo] = false;
					MessageBox.Show("Video frames saved");
					if(stadeMachine.CurrentStade.ShortcutValue != GlobalStade.Stopped) StopGame();
				}
			}

			e.Handled = handled;
		}

		private void ResumeGame()
		{
			if(stadeMachine.CurrentStade.ShortcutValue == GlobalStade.Stopped)
			{
				MessageBox.Show("Игра востоновлена", "вы нажали клавишу [f]");
				stadeMachine.SetStade(GlobalStade.Running);
			}
			else throw new InvalidOperationException("Can't Resume non stopped game");
		}

		private void StopGame()
		{
			if (stadeMachine.CurrentStade.ShortcutValue != GlobalStade.Stopped)
			{
				MessageBox.Show("Игра остановлена", "вы нажали клавишу [f]");
				stadeMachine.SetStade(GlobalStade.Stopped);
			}
			else throw new InvalidOperationException("Can't Resume non stopped game");
		}

		private void StadeSelected_Stopped(MainForm _, StadeSelectedEventArgs<GlobalStade, MainForm> args)
		{
			Text = "Клеточный автомат (STOPPED)";
		}
		
		private void StadeDeselected_Stopped(MainForm _, StadeDeselectedEventArgs<GlobalStade, MainForm> args)
		{
			Text = "Клеточный автомат";
		}
		
		private void StadeSelected_Running(MainForm _, StadeSelectedEventArgs<GlobalStade, MainForm> args)
		{

		}

		private void StadeSelected_Calculating(MainForm _, StadeSelectedEventArgs<GlobalStade, MainForm> args)
		{

		}


		enum GlobalStade
		{
			Running,
			Calculating,
			Stopped,
		}

		enum Toggle
		{
			IsRecordingVideo
		}
	}
}
