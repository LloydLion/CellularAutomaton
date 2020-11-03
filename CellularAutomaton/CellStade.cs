using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace CellularAutomaton
{
	class CellStade
	{
		public static readonly CellStade Empty = new CellStade(Color.White);
		public static readonly CellStade Filled = new CellStade(Color.Black);

		public static readonly IReadOnlyDictionary<CellStadeKey, CellStade> KeyConverter = new Dictionary<CellStadeKey, CellStade>()
		{
			{ CellStadeKey.Empty, Empty }, 
			{ CellStadeKey.Filled, Filled } 
		};


		public Color DrawColor { get; }

		public CellStade(Color drawColor)
		{
			DrawColor = drawColor;
		}
	}


	enum CellStadeKey
	{
		Empty,
		Filled
	}
}
