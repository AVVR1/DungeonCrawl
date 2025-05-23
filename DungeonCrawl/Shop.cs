﻿using System;
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
		public List<Item> items = new List<Item>();

		public static Shop currentShop = null;
        public Shop(Vector2 position, int height, int width) : base(position, height, width)
		{
			
		}

		public void CreateShop(Random random)
		{
			for (int i = 0; i < Program.SHOP_ITEMS; i++)
			{
				Item shopItem;
				do
				{
					shopItem = Item.CreateRandomItem(random);
					shopItem.price = 2;
				} while (shopItem.type == ItemType.Treasure);
				items.Add(shopItem);
            }
        }
	}
}