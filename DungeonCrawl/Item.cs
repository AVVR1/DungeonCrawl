using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static DungeonCrawl.Map;

namespace DungeonCrawl
{
	internal enum ItemType
	{
		Weapon,
		Armor,
		Potion,
		Treasure
	}
	internal class Item
	{
		public string name;
		public int quality; // means different things depending on the type
		public Vector2 position;
		public ItemType type;

		public int price;

        public Item(string name, ItemType type, int quality)
        {
            this.name = name;
            this.type = type;
            this.quality = quality;
        }

        public static Item CreateRandomItem(Random random)
        {
            ItemType type = Enum.GetValues<ItemType>()[random.Next(4)];
			Item i = type switch
			{
				ItemType.Treasure => new Item("Book", type, 2),
				ItemType.Weapon => new Item("Sword", type, 3),
				ItemType.Armor => new Item("Helmet", type, 1),
				ItemType.Potion => new Item("Apple Juice", type, 1)
			};
			return i;
        }
    }
}
