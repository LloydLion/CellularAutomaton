using System;
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
		private readonly CellStade[][,] buffers = new CellStade[2][,];
		private CellStade[,] mainBuffer;
		readonly Random random = new Random();
		int density = 2;

		public MainForm()
		{
			InitializeComponent();

			if(Screen.PrimaryScreen.Bounds.Height >= Screen.PrimaryScreen.Bounds.Width)
			{
				this.Height = (int)(Math.Round(Screen.PrimaryScreen.Bounds.Height * 0.75f / 4f) * 4);
				this.Width = (int)(Math.Round(this.Height * (Screen.PrimaryScreen.Bounds.Width / (float)Screen.PrimaryScreen.Bounds.Height) / 4f) * 4f);
			}
			else
			{
				this.Width = (int)(Math.Round(Screen.PrimaryScreen.Bounds.Width * 0.75f / 4f) * 4f);
				this.Height = (int)(Math.Round(this.Width * (Screen.PrimaryScreen.Bounds.Height / (float)Screen.PrimaryScreen.Bounds.Width) / 4f) * 4f);
			}
		}

		private void GlobalTicker_Tick(object sender, EventArgs e)
		{
			for (int x = 0; x < mainBuffer.GetUpperBound(0); x++)
			{
				for (int y = 0; y < mainBuffer.GetUpperBound(1); y++)
				{
					mainBuffer[x, y] = random.Next(density) == 0 ? CellStade.Filled : CellStade.Empty;
				}
			}

			density++;
			UpdateGraphicsFromField();

		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			buffers[0] = mainBuffer = new CellStade[pictureBox.Width / 4, pictureBox.Height / 4];
			buffers[1] = new CellStade[pictureBox.Width / 4, pictureBox.Height / 4];


			for (int x = 0; x < mainBuffer.GetUpperBound(0); x++)
			{
				for (int y = 0; y < mainBuffer.GetUpperBound(1); y++)
				{
					mainBuffer[x, y] = random.Next(3) == 0 ? CellStade.Filled : CellStade.Empty; 
				}
			}

			UpdateGraphicsFromField();
		}

		private void UpdateGraphicsFromField()
		{
			pictureBox.Image = new Bitmap(pictureBox.Width, pictureBox.Height);
			var graphics = Graphics.FromImage(pictureBox.Image);
			graphics.FillRectangle(Brushes.Red, new Rectangle(new Point(0, 0), new Size(pictureBox.Width, pictureBox.Height)));

			for (int x = 0; x < mainBuffer.GetUpperBound(0); x++)
			{
				for (int y = 0; y < mainBuffer.GetUpperBound(1); y++)
				{
					graphics.FillRectangle(new SolidBrush(mainBuffer[x, y].DrawColor), new Rectangle(new Point(x * 4, y * 4), new Size(4, 4)));
				}
			}

			graphics.Save();
			pictureBox.Update();

			graphics.Dispose();
		}
	}
}
