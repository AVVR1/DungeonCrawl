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


		public void GiveItem(PlayerCharacter character, Item item)
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

		public int GetCharacterDamage(PlayerCharacter character)
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

		public int GetCharacterDefense(PlayerCharacter character)
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

		public void UseItem(PlayerCharacter character, Item item, List<string> messages)
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
