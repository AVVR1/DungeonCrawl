using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

		public Item(string name, ItemType type, int quality, Vector2 position)
		{
			this.name = name;
			this.type = type;
			this.quality = quality;
			this.position = position;
		}
		public static Item CreateRandomItem(Random random, Vector2 position)
		{
			ItemType type = Enum.GetValues<ItemType>()[random.Next(4)];
			Item i = type switch
			{
				ItemType.Treasure => new Item("Book", type, 2, position),
				ItemType.Weapon => new Item("Sword", type, 3, position),
				ItemType.Armor => new Item("Helmet", type, 1, position),
				ItemType.Potion => new Item("Apple Juice", type, 1, position)
			};
			return i;
		}

        public static List<Item> CreateItems(Map level, Random random)
		{
            List<Item> items = new List<Item>();

            for (int y = 0; y < level.height; y++)
            {
                for (int x = 0; x < level.width; x++)
                {
                    int ti = y * level.width + x;
                    if (level.Tiles[ti] == Tile.Item)
                    {
                        Item m = Item.CreateRandomItem(random, new Vector2(x, y));
                        items.Add(m);
                        level.Tiles[ti] = (sbyte)Tile.Floor;
                    }
                }
            }
            return items;
        }
    }
}
