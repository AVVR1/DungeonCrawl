using System.Numerics;

namespace DungeonCrawl
{
	internal class Monster
	{
		public string name;
		public Vector2 position;
		public int hitpoints;
		public int damage;
		public char symbol;
		public ConsoleColor color;

		public Monster(string name, int hitpoints, int damage, char symbol, ConsoleColor color, Vector2 position)
		{
			this.name = name;
			this.hitpoints = hitpoints;
			this.damage = damage;
			this.symbol = symbol;
			this.color = color;
			this.position = position;
		}
		public static Monster CreateRandomMonster(Random random, Vector2 position)
		{
			int type = random.Next(4);
			return type switch
			{
				0 => new Monster("Goblin", 5, 2, 'g', ConsoleColor.Green, position),
				1 => new Monster("Bat Man", 2, 1, 'M', ConsoleColor.Magenta, position),
				2 => new Monster("Orc", 4, 3, 'o', ConsoleColor.Red, position),
				3 => new Monster("Bunny", 1, 0, 'B', ConsoleColor.Yellow, position)
			};
		}
	}
}
