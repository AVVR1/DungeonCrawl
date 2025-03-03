using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
	}
}
