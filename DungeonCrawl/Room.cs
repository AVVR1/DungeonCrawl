using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawl
{
	public class Room
	{
		public Vector2 position;
		public int height;
		public int width;

		public Room(Vector2 position, int height, int width)
		{
			this.position = position;
			this.height = height;
			this.width = width;
		}
	}
}
