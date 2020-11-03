﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CellularAutomaton
{
	public partial class MainForm : Form
	{
		private const int resolution = 16;
		private readonly GameLogic logic;
		private Task logicUpdateTask;
		private bool stopped;


		public MainForm()
		{
			InitializeComponent();

			if(Screen.PrimaryScreen.Bounds.Height >= Screen.PrimaryScreen.Bounds.Width)
			{
				this.Height = (int)(Math.Round(Screen.PrimaryScreen.Bounds.Height * 0.75f / (float)resolution) * resolution);
				this.Width = (int)(Math.Round(this.Height * (Screen.PrimaryScreen.Bounds.Width / (float)Screen.PrimaryScreen.Bounds.Height) / (float)resolution) * resolution);
			}
			else
			{
				this.Width = (int)(Math.Round(Screen.PrimaryScreen.Bounds.Width * 0.75f / (float)resolution) * resolution);
				this.Height = (int)(Math.Round(this.Width * (Screen.PrimaryScreen.Bounds.Height / (float)Screen.PrimaryScreen.Bounds.Width) / (float)resolution) * resolution);
			}

			logic = new GameLogic(pictureBox.Width / resolution, pictureBox.Height / resolution);
		}

		private void GlobalTicker_Tick(object sender, EventArgs e)
		{
			if(logicUpdateTask != null) return;
			tasksUpdateTimer.Enabled = true;
			logicUpdateTask = Task.Run(logic.Update);
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			logic.GenerateRandomField();
			UpdateGraphicsFromField();
		}

		private void UpdateGraphicsFromField()
		{
			pictureBox.Image = new Bitmap(pictureBox.Width, pictureBox.Height);
			var graphics = Graphics.FromImage(pictureBox.Image);
			graphics.FillRectangle(Brushes.Red, new Rectangle(new Point(0, 0), new Size(pictureBox.Width, pictureBox.Height)));
			var mainBuffer = logic.GetField();

			for (int x = 0; x < mainBuffer.GetUpperBound(0); x++)
			{
				for (int y = 0; y < mainBuffer.GetUpperBound(1); y++)
				{
					if(mainBuffer[x, y] == null) graphics.FillRectangle(Brushes.Blue, new Rectangle(new Point(x * resolution, y * resolution), new Size(resolution, resolution)));
					else graphics.FillRectangle(new SolidBrush(mainBuffer[x, y].DrawColor), new Rectangle(new Point(x * resolution, y * resolution), new Size(resolution, resolution)));
				}
			}

			graphics.Save();
			pictureBox.Update();

			graphics.Dispose();
		}

		private void TasksUpdateTimer_Tick(object sender, EventArgs e)
		{
			if(logicUpdateTask.IsCompleted)
			{
				UpdateGraphicsFromField();
				tasksUpdateTimer.Enabled = false;
				logicUpdateTask = null;
			}
		}

		private void PictureBox_MouseMove(object sender, MouseEventArgs e)
		{
			if(stopped)
			{
				if(e.Button.HasFlag(MouseButtons.Left))
				{
					try
					{ logic.SetCell(CellStade.Filled, e.Location.X / resolution, e.Location.Y / resolution); }
					catch (Exception) { }

					UpdateGraphicsFromField();
				}
				else if(e.Button.HasFlag(MouseButtons.Right))
				{
					try
					{ logic.SetCell(CellStade.Empty, e.Location.X / resolution, e.Location.Y / resolution); }
					catch (Exception) { }

					UpdateGraphicsFromField();
				}
			}
		}

		private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
		{
			if(e.KeyChar == 'f' && logicUpdateTask == null)
			{
				if(stopped == false)
				{
					stopped = true;
					globalTicker.Enabled = false;
					Text = "Клеточный автомат (STOPPED)";
				}
				else
				{
					stopped = false;
					globalTicker.Enabled = true;
					Text = "Клеточный автомат";
				}
			}

			if(e.KeyChar == 'r' && stopped == true)
			{
				var res = MessageBox.Show("Вы уверены что хотите случайно заполнить поле?", "вы нажали клавишу [r]", MessageBoxButtons.YesNo);
				if(res == DialogResult.Yes)
				{
					logic.GenerateRandomField();
				}
			}

			if (e.KeyChar == 'c' && stopped == true)
			{
				var res = MessageBox.Show("Вы уверены что хотите очистить поле?", "вы нажали клавишу [c]", MessageBoxButtons.YesNo);
				if (res == DialogResult.Yes)
				{
					logic.ClearField(CellStade.Empty);
				}
			}


			UpdateGraphicsFromField();
		}
	}
}
