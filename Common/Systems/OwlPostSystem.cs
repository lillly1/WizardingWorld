using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace WizardingWorld.Common.Systems
{
    /// <summary>
    /// Owl Post System -- daily delivery requests for Hogsmeade.
    /// Each in-game day, a new request is posted. Complete it by having the
    /// requested item in inventory and talking to any town NPC.
    /// Rewards: Essence of Magic + random buff.
    /// Canon-inspired: Owl Post is the wizarding mail service.
    /// </summary>
    public class OwlPostSystem : ModSystem
    {
        public static int currentRequestItemType;
        public static int currentRequestStack;
        public static bool requestCompleted;
        public static int lastRequestDay;

        private static readonly (int itemId, int stack, string descKey)[] PossibleRequests = new (int itemId, int stack, string descKey)[]
        {
            (ItemID.Mushroom, 10, "Mods.WizardingWorld.OwlPost.DescMushroom"),
            (ItemID.Daybloom, 5, "Mods.WizardingWorld.OwlPost.DescDaybloom"),
            (ItemID.FallenStar, 3, "Mods.WizardingWorld.OwlPost.DescFallenStar"),
            (ItemID.Cobweb, 20, "Mods.WizardingWorld.OwlPost.DescCobweb"),
            (ItemID.Lens, 5, "Mods.WizardingWorld.OwlPost.DescLens"),
            (ItemID.Book, 3, "Mods.WizardingWorld.OwlPost.DescBook"),
            (ItemID.Bottle, 10, "Mods.WizardingWorld.OwlPost.DescBottle"),
            (ItemID.GoldBar, 2, "Mods.WizardingWorld.OwlPost.DescGoldBar"),
            (ItemID.Feather, 5, "Mods.WizardingWorld.OwlPost.DescFeather"),
            (ItemID.Silk, 5, "Mods.WizardingWorld.OwlPost.DescSilk"),
        };

        public override void PreUpdateWorld()
        {
            int today = (int)(Main.time / 86400.0);
            if (today != lastRequestDay)
            {
                GenerateNewRequest(today);
            }
        }

        private static void GenerateNewRequest(int daySeed)
        {
            var rand = new Random(daySeed * 7919 + Main.ActiveWorldFileData?.Seed ?? 0);
            int idx = rand.Next(PossibleRequests.Length);
            var req = PossibleRequests[idx];

            currentRequestItemType = req.itemId;
            currentRequestStack = req.stack;
            requestCompleted = false;
            lastRequestDay = daySeed;
        }

        public static bool TryCompleteRequest(Player player)
        {
            if (requestCompleted || currentRequestItemType == 0)
                return false;

            // Check if player has the items
            int count = 0;
            foreach (var item in player.inventory)
            {
                if (item.active && item.type == currentRequestItemType)
                    count += item.stack;
            }

            if (count < currentRequestStack)
                return false;

            // Consume items
            int remaining = currentRequestStack;
            for (int i = 0; i < player.inventory.Length && remaining > 0; i++)
            {
                if (player.inventory[i].active && player.inventory[i].type == currentRequestItemType)
                {
                    int take = Math.Min(player.inventory[i].stack, remaining);
                    player.inventory[i].stack -= take;
                    remaining -= take;
                    if (player.inventory[i].stack <= 0)
                        player.inventory[i].TurnToAir();
                }
            }

            // Reward
            requestCompleted = true;
            int essenceAmount = Main.rand.Next(5, 15);
            player.QuickSpawnItem(player.GetSource_GiftOrReward(),
                ModContent.ItemType<Content.Items.Consumables.EssenceOfMagic>(), essenceAmount);

            // Random buff reward
            int[] rewardBuffs = { BuffID.Wrath, BuffID.Rage, BuffID.Endurance, BuffID.Lifeforce, BuffID.ManaRegeneration };
            player.AddBuff(rewardBuffs[Main.rand.Next(rewardBuffs.Length)], 18000); // 5 min

            Main.NewText(Language.GetTextValue("Mods.WizardingWorld.OwlPost.DeliveryComplete", essenceAmount), new Color(200, 180, 100));

            var wp = player.GetModPlayer<Players.WizardPlayer>();
            if (wp.houseSet > 0)
                GreatHallSystem.AwardHousePoints(wp.houseSet, 10, Language.GetTextValue("Mods.WizardingWorld.GreatHall.SourceOwlPost"));

            return true;
        }

        public static string GetCurrentRequestText()
        {
            if (requestCompleted)
                return Language.GetTextValue("Mods.WizardingWorld.OwlPost.CompletedToday");
            if (currentRequestItemType == 0)
                return Language.GetTextValue("Mods.WizardingWorld.OwlPost.NoDeliveries");

            string itemName;
            try
            {
                var item = new Item();
                item.SetDefaults(currentRequestItemType);
                itemName = item.Name;
            }
            catch
            {
                itemName = $"Item #{currentRequestItemType}";
            }

            return Language.GetTextValue("Mods.WizardingWorld.OwlPost.RequestFormat", currentRequestStack, itemName);
        }

        public override void ClearWorld()
        {
            currentRequestItemType = 0;
            currentRequestStack = 0;
            requestCompleted = false;
            lastRequestDay = -1;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["owlpost_item"] = currentRequestItemType;
            tag["owlpost_stack"] = currentRequestStack;
            tag["owlpost_done"] = requestCompleted;
            tag["owlpost_day"] = lastRequestDay;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            currentRequestItemType = tag.GetInt("owlpost_item");
            currentRequestStack = tag.GetInt("owlpost_stack");
            requestCompleted = tag.GetBool("owlpost_done");
            lastRequestDay = tag.GetInt("owlpost_day");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(currentRequestItemType);
            writer.Write(currentRequestStack);
            writer.Write(requestCompleted);
            writer.Write(lastRequestDay);
        }

        public override void NetReceive(BinaryReader reader)
        {
            currentRequestItemType = reader.ReadInt32();
            currentRequestStack = reader.ReadInt32();
            requestCompleted = reader.ReadBoolean();
            lastRequestDay = reader.ReadInt32();
        }
    }
}
