using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellularAutomaton
{
	class GameLogic
	{
		private readonly CellStade[][,] buffers = new CellStade[2][,];
		private readonly CellStade[,] mainBuffer;
		private readonly Random random = new Random();


		public GameLogic(int wight, int height)
		{
			buffers[0] = mainBuffer = new CellStade[wight, height];
			buffers[1] = new CellStade[wight, height];
		}


		public void GenerateRandomField()
		{
			for (int x = 0; x < mainBuffer.GetUpperBound(0); x++)
			{
				for (int y = 0; y < mainBuffer.GetUpperBound(1); y++)
				{
					mainBuffer[x, y] = random.Next(3) == 0 ? CellStade.Filled : CellStade.Empty;
				}
			}
		}

		public void ClearField(CellStade clear)
		{
			for (int x = 0; x < mainBuffer.GetUpperBound(0); x++)
			{
				for (int y = 0; y < mainBuffer.GetUpperBound(1); y++)
				{
					mainBuffer[x, y] = clear;
				}
			}
		}

		public CellStade[,] GetField()
		{
			return (CellStade[,])mainBuffer.Clone();
		}

		public void Update()
		{
			for (int x = 0; x < mainBuffer.GetUpperBound(0); x++)
			{
				for (int y = 0; y < mainBuffer.GetUpperBound(1); y++)
				{
					var neightborsCount = GetNeighborsCount(x, y);
					var cell = mainBuffer[x, y];
					buffers[1][x, y] = mainBuffer[x, y];

					if(cell == CellStade.Filled)
					{
						if(!(neightborsCount == 2 || neightborsCount == 3))
						{
							buffers[1][x, y] = CellStade.Empty;
						}
					}
					else if(cell == CellStade.Empty)
					{
						if(neightborsCount == 3)
						{
							buffers[1][x, y] = CellStade.Filled;
						}
					}
					else
					{
						continue;
					}
				}
			}

			for (int x = 0; x < mainBuffer.GetUpperBound(0); x++)
			{
				for (int y = 0; y < mainBuffer.GetUpperBound(1); y++)
				{
					mainBuffer[x, y] = buffers[1][x, y];
				}
			}
		}

		public void SetCell(CellStade stade, int x, int y)
		{
			mainBuffer[x, y] = stade;
		}

		private CellStade[] GetNeighbors(int x, int y)
		{
			var result = new CellStade[8];

			CellStade get(int fx, int fy) 
			{
				try
				{ return mainBuffer[fx, fy]; }
				catch(IndexOutOfRangeException) { return null; }
			};

			result[0] = get(x - 1, y - 1);
			result[1] = get(x + 0, y - 1);
			result[2] = get(x + 1, y - 1);

			result[3] = get(x - 1, y + 0);
			//result[]= get(x + 0, y + 0);
			result[4] = get(x + 1, y + 0);

			result[5] = get(x - 1, y + 1);
			result[6] = get(x + 0, y + 1);
			result[7] = get(x + 1, y + 1);

			return result;
		}
		
		private int GetNeighborsCount(int x, int y)
		{
			var count = 0;

			int has(int fx, int fy) 
			{
				try
				{ return mainBuffer[fx, fy] == CellStade.Filled ? 1 : 0 ; }
				catch(IndexOutOfRangeException) { return 0; }
			};

			count += has(x - 1, y - 1);
			count += has(x + 0, y - 1);
			count += has(x + 1, y - 1);

			count += has(x - 1, y + 0);
			//count += has(x + 0, y + 0);
			count += has(x + 1, y + 0);

			count += has(x - 1, y + 1);
			count += has(x + 0, y + 1);
			count += has(x + 1, y + 1);

			return count;
		}
	}
}
