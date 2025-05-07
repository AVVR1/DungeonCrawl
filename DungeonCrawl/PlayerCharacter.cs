using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static DungeonCrawl.Map;

namespace DungeonCrawl
{
	enum PlayerTurnResult
	{
		TurnOver,
		NewTurn,
		OpenInventory,
		OpenShop,
		NextLevel,
		BackToGame
	}

	internal class PlayerCharacter
	{
		public string name;
		public int hitpoints;
		public int maxHitpoints;
		public Item weapon;
		public Item armor;
		public int gold;
		public Vector2 position;
		public List<Item> inventory;

		public void CreateCharacter()
		{
			name = "";
			hitpoints = 20;
			maxHitpoints = hitpoints;
			gold = 0;
			weapon = null;
			armor = null;
			inventory = new List<Item>();

			Console.Clear();
			Drawer.DrawBrickBg();

			// Draw entrance
			Console.BackgroundColor = ConsoleColor.Black;
			int doorHeight = (int)(Console.WindowHeight * (3.0f / 4.0f));
			int doorY = Console.WindowHeight - doorHeight;
			int doorWidth = (int)(Console.WindowWidth * (3.0f / 5.0f));
			int doorX = Console.WindowWidth / 2 - doorWidth / 2;

			Drawer.DrawRectangle(doorX, doorY, doorWidth, doorHeight, ConsoleColor.Black);
			Drawer.DrawRectangleBorders(doorX + 1, doorY + 1, doorWidth - 2, doorHeight - 2, ConsoleColor.Blue, "|");
			Drawer.DrawRectangleBorders(doorX + 3, doorY + 3, doorWidth - 6, doorHeight - 6, ConsoleColor.DarkBlue, "|");

			Console.SetCursorPosition(Console.WindowWidth / 2 - 8, Console.WindowHeight / 2);
			Printer.Print("Welcome Brave Adventurer!");
			Console.SetCursorPosition(Console.WindowWidth / 2 - 8, Console.WindowHeight / 2 + 1);
			Printer.Print("What is your name?", ConsoleColor.Yellow);
			while (string.IsNullOrEmpty(name))
			{
				name = Console.ReadLine();
			}
			Printer.Print($"Welcome {name}!", ConsoleColor.Yellow);
		}

		public void GiveItem(Item item)
		{
			// Inventory order
			// Weapons
			// Armors
			// Potions
			switch (item.type)
			{
				case ItemType.Weapon:
				if ((weapon != null && weapon.quality < item.quality)
					|| weapon == null)
				{
					weapon = item;
				}
				inventory.Insert(0, item);
				break;
				case ItemType.Armor:
				if ((armor != null && armor.quality < item.quality)
					|| armor == null)
				{
					armor = item;
				}
				int armorIndex = 0;
				while (armorIndex < inventory.Count && inventory[armorIndex].type == ItemType.Weapon)
				{
					armorIndex++;
				}
				inventory.Insert(armorIndex, item);
				break;
				case ItemType.Potion:
				inventory.Add(item);
				break;
				case ItemType.Treasure:
				gold += item.quality;
				break;
			}
		}

		public int GetCharacterDamage()
		{
			if (weapon != null)
			{
				return weapon.quality;
			}
			else
			{
				return 1;
			}
		}

		public int GetCharacterDefense()
		{
			if (armor != null)
			{
				return armor.quality;
			}
			else
			{
				return 0;
			}
		}

		public void UseItem(Item item, List<string> messages)
		{
			switch (item.type)
			{
				case ItemType.Weapon:
				weapon = item;
				messages.Add($"You are now wielding a {item.name} ");
				break;
				case ItemType.Armor:
				armor = item;
				messages.Add($"You equip {item.name} on yourself. ");
				break;
				case ItemType.Potion:
				hitpoints += item.quality;
				if (hitpoints > maxHitpoints)
				{
					maxHitpoints = hitpoints;
				}
				messages.Add($"You drink a potion and gain {item.quality} hitpoints ");
				inventory.Remove(item);
				break;
			}
		}

		//Player turns

		public bool DoPlayerTurnVsEnemies(List<Monster> enemies, Map level, Vector2 destinationPlace, List<int> dirtyTiles, List<string> messages)
		{
			// Check enemies
			bool hitEnemy = false;
			Monster toRemoveMonster = null;
			foreach (Monster enemy in enemies)
			{
				if (enemy.position == destinationPlace)
				{
					int damage = GetCharacterDamage();
					messages.Add($"You hit {enemy.name} for {damage}! ");
					enemy.hitpoints -= damage;
					hitEnemy = true;
					if (enemy.hitpoints <= 0)
					{
						toRemoveMonster = enemy;
					}
				}
			}
			if (toRemoveMonster != null)
			{
				int dirtyTile = level.PositionToTileIndex(destinationPlace);
				dirtyTiles.Add(dirtyTile);
				enemies.Remove(toRemoveMonster);
			}
			return hitEnemy;
		}
		public bool DoPlayerTurnVsItems(List<Item> items, Vector2 destinationPlace, List<string> messages)
		{
			// Check items
			Item toRemoveItem = null;
			foreach (Item item in items)
			{
				if (item.position == destinationPlace)
				{
					string itemMessage = $"You find a ";
					switch (item.type)
					{
						case ItemType.Armor:
						itemMessage += $"{item.name}, it fits you well";
						break;
						case ItemType.Weapon:
						itemMessage += $"{item.name} to use in battle";
						break;
						case ItemType.Potion:
						itemMessage += $"potion of {item.name}";
						break;
						case ItemType.Treasure:
						itemMessage += $"valuable {item.name} and get {item.quality} gold!";
						break;
					};
					messages.Add(itemMessage);
					toRemoveItem = item;
					GiveItem(item);
					break;
				}
			}
			if (toRemoveItem != null)
			{
				items.Remove(toRemoveItem);
			}
			return false;
		}
		public bool DoPlayerTurnVsShop(List<Room> rooms, Vector2 destinationPlace, List<string> messages)
		{
			foreach (Room room in rooms)
			{
				if (room.GetType() == typeof(Shop))
				{
					if (destinationPlace.X > room.position.X && destinationPlace.X < room.position.X + room.width - 1 && destinationPlace.Y > room.position.Y && destinationPlace.Y < room.position.Y + room.height - 1)
					{
						messages.Add("You enter a shop! ");
						Shop.currentShop = (Shop)room;
						return true;
					}
				}
			}
			Shop.currentShop = null;
			return false;
		}
		public PlayerTurnResult DoTurn(Map level, List<Monster> enemies, List<Item> items, List<int> dirtyTiles, List<string> messages)
		{
			Vector2 playerMove = new Vector2(0, 0);
			while (true)
			{
				while (Console.KeyAvailable)
				{
					Console.ReadKey(false);
				}
				ConsoleKeyInfo key = Console.ReadKey();
				if (key.Key == ConsoleKey.W || key.Key == ConsoleKey.UpArrow)
				{
					playerMove.Y = -1;
					break;
				}
				else if (key.Key == ConsoleKey.S || key.Key == ConsoleKey.DownArrow)
				{
					playerMove.Y = 1;
					break;
				}
				else if (key.Key == ConsoleKey.A || key.Key == ConsoleKey.LeftArrow)
				{
					playerMove.X = -1;
					break;
				}
				else if (key.Key == ConsoleKey.D || key.Key == ConsoleKey.RightArrow)
				{
					playerMove.X = 1;
					break;
				}
				// Other commands
				else if (key.Key == ConsoleKey.I)
				{
					return PlayerTurnResult.OpenInventory;
				}
			}

			int startTile = level.PositionToTileIndex(position);
			Vector2 destinationPlace = position + playerMove;

			if (DoPlayerTurnVsEnemies(enemies, level, destinationPlace, dirtyTiles, messages))
			{
				return PlayerTurnResult.TurnOver;
			}

			if (DoPlayerTurnVsItems(items, destinationPlace, messages))
			{
				return PlayerTurnResult.TurnOver;
			}

			if (DoPlayerTurnVsShop(level.rooms, destinationPlace, messages))
			{
				return PlayerTurnResult.OpenShop;
			}

			// Check movement
			Tile destination = level.GetTileAtMap(destinationPlace);
			if (destination == Tile.Floor)
			{
				position = destinationPlace;
				dirtyTiles.Add(startTile);
			}
			else if (destination == Tile.Door)
			{
				messages.Add("You open a door. ");
				position = destinationPlace;
				dirtyTiles.Add(startTile);
			}
			else if (destination == Tile.Wall)
			{
				messages.Add("You hit a wall. ");
			}
			else if (destination == Tile.Stairs)
			{
				messages.Add("You find stairs leading down. ");
				return PlayerTurnResult.NextLevel;
			}
			return PlayerTurnResult.TurnOver;
		}
	}
}
