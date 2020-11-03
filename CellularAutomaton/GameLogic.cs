using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellularAutomaton
{
	class GameLogic
	{
		private readonly CellStadeKey[][,] buffers = new CellStadeKey[2][,];
		private readonly CellStadeKey[,] mainBuffer;
		private readonly Random random = new Random();


		public GameLogic(int wight, int height)
		{
			buffers[0] = mainBuffer = new CellStadeKey[wight, height];
			buffers[1] = new CellStadeKey[wight, height];
		}


		public void GenerateRandomField()
		{
			for (int x = 0; x <= mainBuffer.GetUpperBound(0); x++)
			{
				for (int y = 0; y <= mainBuffer.GetUpperBound(1); y++)
				{
					mainBuffer[x, y] = random.Next(3) == 0 ? CellStadeKey.Filled : CellStadeKey.Empty;
				}
			}
		}

		public void ClearField(CellStadeKey clear)
		{
			for (int x = 0; x <= mainBuffer.GetUpperBound(0); x++)
			{
				for (int y = 0; y <= mainBuffer.GetUpperBound(1); y++)
				{
					mainBuffer[x, y] = clear;
				}
			}
		}

		public CellStadeKey[,] GetField()
		{
			return (CellStadeKey[,])mainBuffer.Clone();
		}

		public void Update()
		{
			for (int x = 0; x <= mainBuffer.GetUpperBound(0); x++)
			{
				for (int y = 0; y <= mainBuffer.GetUpperBound(1); y++)
				{
					var neightborsCount = GetNeighborsCount(x, y);
					var cell = mainBuffer[x, y];
					buffers[1][x, y] = mainBuffer[x, y];

					if(cell == CellStadeKey.Filled)
					{
						if(!(neightborsCount == 2 || neightborsCount == 3))
						{
							buffers[1][x, y] = CellStadeKey.Empty;
						}
					}
					else if(cell == CellStadeKey.Empty)
					{
						if(neightborsCount == 3)
						{
							buffers[1][x, y] = CellStadeKey.Filled;
						}
					}
					else
					{
						continue;
					}
				}
			}

			for (int x = 0; x <= mainBuffer.GetUpperBound(0); x++)
			{
				for (int y = 0; y <= mainBuffer.GetUpperBound(1); y++)
				{
					mainBuffer[x, y] = buffers[1][x, y];
				}
			}
		}

		public void SetCell(CellStadeKey stade, int x, int y)
		{
			mainBuffer[x, y] = stade;
		}

		private CellStadeKey?[] GetNeighbors(int x, int y)
		{
			var result = new CellStadeKey?[8];

			CellStadeKey? get(int fx, int fy) 
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
				if(fx > mainBuffer.GetUpperBound(0) || fx < 0 || fy > mainBuffer.GetUpperBound(1) || fy < 0) return 0;
				return mainBuffer[fx, fy] == CellStadeKey.Filled ? 1 : 0 ;
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
