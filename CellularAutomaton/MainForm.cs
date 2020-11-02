using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CellularAutomaton
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

			if(Screen.PrimaryScreen.Bounds.Height >= Screen.PrimaryScreen.Bounds.Width)
			{
				this.Height = (int)(Screen.PrimaryScreen.Bounds.Height * 0.75f);
				this.Width = (int)(this.Height * (Screen.PrimaryScreen.Bounds.Width / (float)Screen.PrimaryScreen.Bounds.Height));
			}
			else
			{
				this.Width = (int)(Screen.PrimaryScreen.Bounds.Width * 0.75f);
				this.Height = (int)(this.Width * (Screen.PrimaryScreen.Bounds.Height / (float)Screen.PrimaryScreen.Bounds.Width));
			}
		}
	}
}
