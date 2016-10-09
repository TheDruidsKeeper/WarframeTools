using NLog;
using System.Collections.Generic;

namespace WarframeTools {
	public class PlayerInventory {
		private static Logger logger = LogManager.GetCurrentClassLogger();
		// Warframes
		public List<string> Warframes { get; set; }
		public List<string> PrimaryWeapons { get; set; }
		public List<string> SecondaryWeapons { get; set; }
		public List<string> MeleeWeapons { get; set; }
		// Archwings
		public List<string> Archwings { get; set; }
		public List<string> ArchwingGuns { get; set; }
		public List<string> ArchwingMelees { get; set; }
		// Companions
		public List<string> Sentinels { get; set; }
		public List<string> SentinelGuns { get; set; }
		public List<string> Kubrows { get; set; }
		public List<string> Kavats { get; set; }

		public PlayerInventory(dynamic inventory) {
			// Warframe
			this.Warframes = ParseItems(inventory.Suits);
			this.PrimaryWeapons = ParseItems(inventory.LongGuns);
			this.SecondaryWeapons = ParseItems(inventory.Pistols);
			this.MeleeWeapons = ParseItems(inventory.Melee);
			// Archwing
			this.Archwings = ParseItems(inventory.SpaceSuits);
			this.ArchwingGuns = ParseItems(inventory.SpaceGuns);
			this.ArchwingMelees = ParseItems(inventory.SpaceMelee);
			// Companion
			this.Sentinels = ParseItems(inventory.Sentinels);
			this.SentinelGuns = ParseItems(inventory.SentinelWeapons);
			this.Kubrows = ParseItems(inventory.KubrowPets);
			this.Kavats = new List<string>(); //ParseItems(inventory.?);
			logger.Debug($"Loaded player inventory");
		}
		
		private static List<string> ParseItems(dynamic items) {
			var itemsList = new List<string>();
			foreach (var item in items) {
				itemsList.Add((string)item.ItemType);
			}
			return itemsList;
		}
	}
}
