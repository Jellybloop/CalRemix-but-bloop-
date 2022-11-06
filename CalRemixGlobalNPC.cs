using Terraria;
using Terraria.ModLoader;
using CalamityMod.NPCs.TownNPCs;
using CalRemix.Items.Materials;
using CalRemix.Tiles;
using Microsoft.Xna.Framework;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.AdultEidolonWyrm;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.SulphurousSea;
using CalRemix.NPCs;
using CalRemix.Items;
using CalRemix.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.Bumblebirb;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace CalRemix
{
    public class CalRemixGlobalNPC : GlobalNPC
    {
        bool SlimeBoost = false;
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }

        public List<int> BossSlimes = new List<int> 
        { 
            NPCID.KingSlime,
            NPCID.QueenSlimeBoss,
            ModContent.NPCType<AstrumAureus>(),
            ModContent.NPCType<EbonianSlimeGod>(),
            ModContent.NPCType<CrimulanSlimeGod>(),
            ModContent.NPCType<SplitEbonianSlimeGod>(),
            ModContent.NPCType<SplitCrimulanSlimeGod>(),
            ModContent.NPCType<SlimeGodCore>(),
            ModContent.NPCType<LifeSlime>(),
            ModContent.NPCType<CragmawMire>()
        };

        public List<int> Slimes = new List<int>
        {
            1, 16, 59, 71, 81, 138, 121, 122, 141, 147, 183, 184, 204, 225, 244, 302, 333, 335, 334, 336, 537,
            NPCID.SlimeSpiked,
            NPCID.QueenSlimeMinionBlue,
            NPCID.QueenSlimeMinionPink,
            NPCID.QueenSlimeMinionPurple,
            ModContent.NPCType<AeroSlime>(),
            ModContent.NPCType<CalamityMod.NPCs.Astral.AstralSlime>(),
            ModContent.NPCType<CalamityMod.NPCs.PlagueEnemies.PestilentSlime>(),
            ModContent.NPCType<BloomSlime>(),
            ModContent.NPCType<CalamityMod.NPCs.Crags.CharredSlime>(),
            ModContent.NPCType<PerennialSlime>(),
            ModContent.NPCType<CryoSlime>(),
            ModContent.NPCType<GammaSlime>(),
            ModContent.NPCType<IrradiatedSlime>(),
            ModContent.NPCType<AuricSlime>(),
            ModContent.NPCType<CorruptSlimeSpawn>(),
            ModContent.NPCType<CorruptSlimeSpawn2>(),
            ModContent.NPCType<CrimsonSlimeSpawn>(),
            ModContent.NPCType<CrimsonSlimeSpawn2>()
        };

        public override bool PreAI(NPC npc)
        {
            if (Slimes.Contains(npc.type) && Main.LocalPlayer.GetModPlayer<CalRemixPlayer>().assortegel)
            {
                if (!npc.GetGlobalNPC<CalRemixGlobalNPC>().SlimeBoost)
                {
                    npc.lifeMax = (int)(npc.lifeMax * 6f);
                    npc.damage = (int)(npc.damage * 12f);
                    npc.life = npc.lifeMax;
                    npc.chaseable = false;
                    npc.GetGlobalNPC<CalRemixGlobalNPC>().SlimeBoost = true;
                }
                if (Main.LocalPlayer.MinionAttackTargetNPC > 0 && !Slimes.Contains(Main.npc[Main.LocalPlayer.MinionAttackTargetNPC].type))
                {
                    Vector2 direction = Main.npc[Main.LocalPlayer.MinionAttackTargetNPC].position - npc.position;
                    direction.Normalize();
                    npc.velocity = direction * 20;
                    npc.noTileCollide = true;
                }
                else
                {
                    npc.noTileCollide = false;
                }
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    Rectangle thisrect = npc.getRect();
                    Rectangle theirrect = target.getRect();
                    if (thisrect.Intersects(theirrect) && target.whoAmI != npc.whoAmI && npc.active && target.active && !target.dontTakeDamage && !Slimes.Contains(target.type))
                    {
                        target.StrikeNPC(npc.damage, 0, 0);
                        npc.StrikeNPC(target.damage, 0, 0);
                    }

                }
            }
            if (BossSlimes.Contains(npc.type) && Main.LocalPlayer.GetModPlayer<CalRemixPlayer>().assortegel && !CalamityMod.Events.BossRushEvent.BossRushActive)
            {
                npc.damage = 0;
                if (npc.type == ModContent.NPCType<SlimeGodCore>() && NPC.CountNPCS(ModContent.NPCType<CrimulanSlimeGod>()) < 1
                    && NPC.CountNPCS(ModContent.NPCType<SplitCrimulanSlimeGod>()) < 1
                    && NPC.CountNPCS(ModContent.NPCType<EbonianSlimeGod>()) < 1
                    && NPC.CountNPCS(ModContent.NPCType<SplitEbonianSlimeGod>()) < 1)
                {
                    return true;
                }
                return false;
            }
            return true;
        }
        public override void AI(NPC npc)
        {
            CalRemixPlayer modPlayer = Main.LocalPlayer.GetModPlayer<CalRemixPlayer>();
            if (npc.type == ModContent.NPCType<MicrobialCluster>() && npc.catchItem == 0)
            {
                npc.catchItem = ModContent.ItemType<DisgustingSeawater>();
            }
            if (npc.type == ModContent.NPCType<FAP>()) // MURDER the drunk princess
            {
                npc.active = false;
            }
            if (npc.type == ModContent.NPCType<Bumblefuck>() && Main.LocalPlayer.ZoneDesert)
            {
                npc.localAI[1] = 0;
            }
            if (npc.type == ModContent.NPCType<AureusSpawn>() && modPlayer.nuclegel && !CalamityMod.Events.BossRushEvent.BossRushActive)
            {
                npc.active = false;
            }
        }
        public override void ModifyTypeName(NPC npc, ref string typeName)
        {
            if (npc.type == ModContent.NPCType<WITCH>())
            {
                typeName = "Calamity Witch";
            }
            else if (npc.type == ModContent.NPCType<BrimstoneElemental>())
            {
                typeName = "Calamity Elemental";
            }
            else if (npc.type == ModContent.NPCType<BrimstoneHeart>())
            {
                typeName = "Calamity Heart";
            }
        }
        public override void SetDefaults(NPC npc)
        {
            if (npc.type == ModContent.NPCType<BrimstoneElemental>())
            {
                npc.GivenName = "Calamity Elemental";
            }
            else if (npc.type == ModContent.NPCType<BrimstoneHeart>())
            {
                npc.GivenName = "Calamity Heart";
            }
            else if (npc.type == ModContent.NPCType<Bumblefuck>())
            {
                npc.damage = 80;
                npc.lifeMax = 58500;
                npc.defense = 20;
                npc.value = Item.buyPrice(gold: 10);
            }
            else if (npc.type == ModContent.NPCType<Bumblefuck2>())
            {
                npc.damage = 60;
                npc.lifeMax = 3375;
            }
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == ModContent.NPCType<DesertScourgeHead>())
            {
                npcLoot.Add(DropHelper.PerPlayer(ModContent.ItemType<ParchedScale>(), 1, 25, 30));
            }
            else if (npc.type == ModContent.NPCType<AdultEidolonWyrmHead>())
            {
                npcLoot.Add(DropHelper.PerPlayer(ModContent.ItemType<SubnauticalPlate>(), 1, 22, 34));
            }
            else if (npc.type == ModContent.NPCType<Bumblefuck>())
            {
                npcLoot.Add(DropHelper.PerPlayer(ModContent.ItemType<DesertFeather>(), 11, 17, 34));
            }
            else if (npc.type == ModContent.NPCType<MirageJelly>())
            {
                npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<MirageJellyItem>(), 7, 5));
            }
            else if (npc.type == ModContent.NPCType<CragmawMire>())
            {
                LeadingConditionRule postPolter = npcLoot.DefineConditionalDropSet(() => DownedBossSystem.downedPolterghast);
                postPolter.Add(ModContent.ItemType<NucleateGello>(), 10, hideLootReport: !DownedBossSystem.downedPolterghast);
                postPolter.AddFail(ModContent.ItemType<NucleateGello>(), hideLootReport: DownedBossSystem.downedPolterghast);
            }
            else if (npc.type == ModContent.NPCType<NuclearTerror>())
            {
                npcLoot.Add(ModContent.ItemType<Microxodonta>(), 3);
            }
        }

        public override bool PreKill(NPC npc)
        {
            if (!CalamityMod.DownedBossSystem.downedRavager && npc.type == ModContent.NPCType<RavagerBody>())
            {
                CalamityUtils.SpawnOre(ModContent.TileType<LifeOreTile>(), 0.25E-05, 0.45f, 0.65f, 30, 40);

                Color messageColor = Color.Lime;
                CalamityUtils.DisplayLocalizedText("Vitality sprawls throughout the underground.", messageColor);
            }
            if (npc.type == Terraria.ID.NPCID.WallofFlesh && !Main.hardMode)
            {
                CalRemixWorld.ShrineTimer = 3000;
            }
            return true;
        }
    }
}
