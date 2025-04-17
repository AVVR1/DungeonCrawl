using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
				messages.Add($"You are now wielding a {item.name}");
				break;
				case ItemType.Armor:
				armor = item;
				messages.Add($"You equip {item.name} on yourself.");
				break;
				case ItemType.Potion:
				hitpoints += item.quality;
				if (hitpoints > maxHitpoints)
				{
					maxHitpoints = hitpoints;
				}
				messages.Add($"You drink a potion and gain {item.quality} hitpoints");
				inventory.Remove(item);
				break;
			}
		}
	}
}
