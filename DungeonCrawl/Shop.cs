using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawl
{
    class Shop : Room
    {
        public const int SHOP_COUNT = 2;

		public Shop(Vector2 position, int height, int width) : base(position, height, width)
		{

		}
	}
}