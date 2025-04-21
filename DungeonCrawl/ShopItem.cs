using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawl
{
	internal class ShopItem : Item
	{
		int price;

		public ShopItem(string name, ItemType type, int quality, int price) : base(name, type, quality)
		{
			this.price = price
		}
	}
}
