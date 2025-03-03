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

		public PlayerCharacter CreateCharacter()
	{
		PlayerCharacter character = new PlayerCharacter();
		character.name = "";
		character.hitpoints = 20;
		character.maxHitpoints = character.hitpoints;
		character.gold = 0;
		character.weapon = null;
		character.armor = null;
		character.inventory = new List<Item>();

		Console.Clear();
		DrawBrickBg();

		// Draw entrance
		Console.BackgroundColor = ConsoleColor.Black;
		int doorHeight = (int)(Console.WindowHeight * (3.0f / 4.0f));
		int doorY = Console.WindowHeight - doorHeight;
		int doorWidth = (int)(Console.WindowWidth * (3.0f / 5.0f));
		int doorX = Console.WindowWidth / 2 - doorWidth / 2;

		DrawRectangle(doorX, doorY, doorWidth, doorHeight, ConsoleColor.Black);
		DrawRectangleBorders(doorX + 1, doorY + 1, doorWidth - 2, doorHeight - 2, ConsoleColor.Blue, "|");
		DrawRectangleBorders(doorX + 3, doorY + 3, doorWidth - 6, doorHeight - 6, ConsoleColor.DarkBlue, "|");

		Console.SetCursorPosition(Console.WindowWidth / 2 - 8, Console.WindowHeight / 2);
		Print("Welcome Brave Adventurer!");
		Console.SetCursorPosition(Console.WindowWidth / 2 - 8, Console.WindowHeight / 2 + 1);
		Print("What is your name?", ConsoleColor.Yellow);
		while (string.IsNullOrEmpty(character.name))
		{
			character.name = Console.ReadLine();
		}
		Print($"Welcome {character.name}!", ConsoleColor.Yellow);

		return character;
	}

		static void GiveItem(PlayerCharacter character, Item item)
		{
			// Inventory order
			// Weapons
			// Armors
			// Potions
			switch (item.type)
			{
				case ItemType.Weapon:
				if ((character.weapon != null && character.weapon.quality < item.quality)
					|| character.weapon == null)
				{
					character.weapon = item;
				}
				character.inventory.Insert(0, item);
				break;
				case ItemType.Armor:
				if ((character.armor != null && character.armor.quality < item.quality)
					|| character.armor == null)
				{
					character.armor = item;
				}
				int armorIndex = 0;
				while (armorIndex < character.inventory.Count && character.inventory[armorIndex].type == ItemType.Weapon)
				{
					armorIndex++;
				}
				character.inventory.Insert(armorIndex, item);
				break;
				case ItemType.Potion:
				character.inventory.Add(item);
				break;
				case ItemType.Treasure:
				character.gold += item.quality;
				break;
			}
		}

		static int GetCharacterDamage(PlayerCharacter character)
		{
			if (character.weapon != null)
			{
				return character.weapon.quality;
			}
			else
			{
				return 1;
			}
		}

		static int GetCharacterDefense(PlayerCharacter character)
		{
			if (character.armor != null)
			{
				return character.armor.quality;
			}
			else
			{
				return 0;
			}
		}

		static void UseItem(PlayerCharacter character, Item item, List<string> messages)
		{
			switch (item.type)
			{
				case ItemType.Weapon:
				character.weapon = item;
				messages.Add($"You are now wielding a {item.name}");
				break;
				case ItemType.Armor:
				character.armor = item;
				messages.Add($"You equip {item.name} on yourself.");
				break;
				case ItemType.Potion:
				character.hitpoints += item.quality;
				if (character.hitpoints > character.maxHitpoints)
				{
					character.maxHitpoints = character.hitpoints;
				}
				messages.Add($"You drink a potion and gain {item.quality} hitpoints");
				character.inventory.Remove(item);
				break;
			}
		}
	}
}
