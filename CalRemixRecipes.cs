using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalRemix.Items;
using CalRemix.Items.Accessories;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Fishing.AstralCatches;
using CalRemix.Items.Potions;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Placeables.Ores;
using CalRemix.Items.Materials;
using CalRemix.Items.Placeables;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Ammo;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Placeables.Furniture.CraftingStations;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Armor.Auric;
using CalRemix.Items.Weapons;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Weapons.DraedonsArsenal;

namespace CalRemix
{
    public class Recipes : ModSystem
    {
        public static RecipeGroup Blinkchid, Daychid, Moonchid, Deathchid, Waterchid, Firechid, Shiverchid;

        public override void Unload()
        {
            Blinkchid = null;
            Daychid = null;
            Moonchid = null;
            Deathchid = null;
            Waterchid = null;
            Firechid = null;
            Shiverchid = null;
        }
        public override void AddRecipeGroups()
        {
            CosmichidGroup(Blinkchid, "Blinkchid", ItemID.Blinkroot);
            CosmichidGroup(Daychid, "Daychid", ItemID.Daybloom);
            CosmichidGroup(Moonchid, "Moonchid", ItemID.Moonglow);
            CosmichidGroup(Deathchid, "Deathchid", ItemID.Deathweed);
            CosmichidGroup(Waterchid, "Waterchid", ItemID.Waterleaf);
            CosmichidGroup(Firechid, "Firechid", ItemID.Fireblossom);
            CosmichidGroup(Shiverchid, "Shiverchid", ItemID.Shiverthorn);
        }
        public override void AddRecipes() 
        {
            {
                Recipe feather = Recipe.Create(ModContent.ItemType<EffulgentFeather>(), 3);
                feather.AddIngredient<DesertFeather>(3)
                .AddIngredient<LifeAlloy>()
                .AddTile(TileID.LunarCraftingStation)
                .Register();
            }
            {
                Recipe alloy = Recipe.Create(ModContent.ItemType<LifeAlloy>());
                alloy.AddIngredient<LifeOre>(5)
                .AddTile(TileID.AdamantiteForge)
                .Register();
            }
            {
                Recipe coin = Recipe.Create(ItemID.PlatinumCoin, 100);
                coin.AddIngredient<CosmiliteCoin>(1);
                coin.Register();
            }

            #region DP stuff
            // Alcohol...
            AlcoholRecipe(ModContent.ItemType<CaribbeanRum>(), ModContent.ItemType<Vodka>(), ItemID.BambooBlock, ModContent.ItemType<LivingShard>(), 20, 20);
            AlcoholRecipe(ModContent.ItemType<CinnamonRoll>(), ModContent.ItemType<Everclear>(), ModContent.ItemType<Whiskey>(), ItemID.BeetleHusk, 5, 1);
            AlcoholRecipe(ModContent.ItemType<Everclear>(), ModContent.ItemType<Margarita>(), ItemID.Hay, ModContent.ItemType<AureusCell>(), 10, 1);
            AlcoholRecipe(ModContent.ItemType<EvergreenGin>(), ModContent.ItemType<Vodka>(), ItemID.PineTreeBlock, ModContent.ItemType<LivingShard>(), 20, 2);
            AlcoholRecipe(ModContent.ItemType<Fireball>(), ItemID.Ale, ModContent.ItemType<BloodOrange>(), ItemID.UnicornHorn, 40, 1);
            AlcoholRecipe(ModContent.ItemType<GrapeBeer>(), ItemID.Ale, ItemID.GrapeJuice, ItemID.UnicornHorn, 40, 1);
            AlcoholRecipe(ModContent.ItemType<Margarita>(), ModContent.ItemType<Vodka>(), ItemID.Starfruit, ModContent.ItemType<LivingShard>(), 20, 1);
            AlcoholRecipe(ModContent.ItemType<Moonshine>(), ModContent.ItemType<Everclear>(), ItemID.Ale, ItemID.BeetleHusk, 5, 1);
            AlcoholRecipe(ModContent.ItemType<MoscowMule>(), ModContent.ItemType<Everclear>(), ModContent.ItemType<Vodka>(), ItemID.BeetleHusk, 5, 1);
            AlcoholRecipe(ModContent.ItemType<RedWine>(), ItemID.Ale, ItemID.Grapes, ItemID.UnicornHorn, 40, 1);
            AlcoholRecipe(ModContent.ItemType<Rum>(), ItemID.Ale, ItemID.BambooBlock, ItemID.UnicornHorn, 40, 20);
            AlcoholRecipe(ModContent.ItemType<Screwdriver>(), ModContent.ItemType<NotFabsolVodka>(), ModContent.ItemType<BloodOrange>(), ModContent.ItemType<HallowedOre>(), 30, 1);
            AlcoholRecipe(ModContent.ItemType<StarBeamRye>(), ModContent.ItemType<Margarita>(), ItemID.Hay, ModContent.ItemType<AureusCell>(), 10, 20);
            AlcoholRecipe(ModContent.ItemType<Tequila>(), ItemID.Ale, ItemID.Hay, ItemID.UnicornHorn, 40, 20);
            AlcoholRecipe(ModContent.ItemType<TequilaSunrise>(), ModContent.ItemType<Everclear>(), ModContent.ItemType<Tequila>(), ItemID.BeetleHusk, 5, 1);
            AlcoholRecipe(ModContent.ItemType<Vodka>(), ModContent.ItemType<NotFabsolVodka>(), ItemID.Hay, ModContent.ItemType<HallowedOre>(), 30, 40);
            AlcoholRecipe(ModContent.ItemType<Whiskey>(), ItemID.Ale, ItemID.BottledWater, ItemID.UnicornHorn, 40, 1);
            AlcoholRecipe(ModContent.ItemType<WhiteWine>(), ModContent.ItemType<NotFabsolVodka>(), ItemID.Grapes, ModContent.ItemType<HallowedOre>(), 30, 1);
            // Candles
            CandleRecipe(ModContent.ItemType<ResilientCandle>(), ItemID.SoulofNight, 445, ModContent.ItemType<EssenceofSunlight>(), 444);
            CandleRecipe(ModContent.ItemType<SpitefulCandle>(), ModContent.ItemType<EssenceofSunlight>(), 1098, ModContent.ItemType<EssenceofChaos>(), 987);
            CandleRecipe(ModContent.ItemType<VigorousCandle>(), ItemID.SoulofLight, 277, ModContent.ItemType<EssenceofSunlight>(), 128);
            CandleRecipe(ModContent.ItemType<VigorousCandle>(), ItemID.SoulofFlight, 3422, ModContent.ItemType<EssenceofEleum>(), 357);
            // Bloody Mary exception
            {
                Recipe recipe = Recipe.Create(ModContent.ItemType<BloodyMary>(), 5);
                recipe.AddIngredient<Margarita>(5);
                recipe.AddIngredient<BloodOrb>(30);
                recipe.AddIngredient<AureusCell>(1);
                recipe.AddTile(TileID.AlchemyTable);
                recipe.Register();
            }
            #endregion
        }
        public override void PostAddRecipes()
        {
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];

                CosmichidChange(recipe, "Blinkchid", ItemID.Blinkroot);
                CosmichidChange(recipe, "Daychid", ItemID.Daybloom);
                CosmichidChange(recipe, "Moonchid", ItemID.Moonglow);
                CosmichidChange(recipe, "Deathchid", ItemID.Deathweed);
                CosmichidChange(recipe, "Waterchid", ItemID.Waterleaf);
                CosmichidChange(recipe, "Firechid", ItemID.Fireblossom);
                CosmichidChange(recipe, "Shiverchid", ItemID.Shiverthorn);

                if (recipe.HasResult(ModContent.ItemType<FabsolsVodka>()))
                {
                    recipe.DisableRecipe();
                }
                if (recipe.HasResult(ModContent.ItemType<GrandScale>()))
                {
                    recipe.DisableRecipe();
                }
                if (recipe.HasResult(ModContent.ItemType<ShadowspecBar>()))
                {
                    recipe.AddIngredient<SubnauticalPlate>();
                }
                if (recipe.TryGetIngredient(ModContent.ItemType<PearlShard>(), out Item shard) && recipe.HasResult(ModContent.ItemType<SeaRemains>()) || recipe.HasResult(ModContent.ItemType<MonstrousKnives>()) || recipe.HasResult(ModContent.ItemType<FirestormCannon>()) || recipe.HasResult(ModContent.ItemType<SuperballBullet>()))
		        {
			        shard.type = ModContent.ItemType<ParchedScale>();
                }
                if (recipe.HasIngredient(ModContent.ItemType<PearlShard>()) && recipe.HasResult(ModContent.ItemType<NavyFishingRod>()) || recipe.HasResult(ModContent.ItemType<EutrophicShelf>()) || recipe.HasResult(ModContent.ItemType<AquamarineStaff>()) || recipe.HasResult(ModContent.ItemType<Riptide>()) || recipe.HasResult(ModContent.ItemType<SeashineSword>()) || recipe.HasResult(ModContent.ItemType<StormSurge>()) || recipe.HasResult(ModContent.ItemType<SeafoamBomb>()))
                {
                    recipe.RemoveIngredient(ModContent.ItemType<PearlShard>());
                }
                if (recipe.HasResult(ModContent.ItemType<Elderberry>()))
                {
                    recipe.DisableRecipe();
                }
                if (recipe.HasResult(ModContent.ItemType<MiracleFruit>()))
                {
                    recipe.DisableRecipe();
                }
                if (recipe.HasResult(ModContent.ItemType<Dragonfruit>()))
                {
                    recipe.DisableRecipe();
                }
                if (recipe.HasResult(ModContent.ItemType<BloodOrange>()))
                {
                    recipe.DisableRecipe();
                }
                if (recipe.HasResult(ModContent.ItemType<CometShard>()))
                {
                    recipe.DisableRecipe();
                }
                if (recipe.HasResult(ModContent.ItemType<EtherealCore>()))
                {
                    recipe.DisableRecipe();
                }
                if (recipe.HasResult(ModContent.ItemType<PhantomHeart>()))
                {
                    recipe.DisableRecipe();
                }
                if (recipe.HasResult(ModContent.ItemType<ExoticPheromones>()))
                {
                    recipe.RemoveIngredient(ModContent.ItemType<LifeAlloy>());
                    recipe.RemoveIngredient(ItemID.FragmentSolar);
                    recipe.RemoveTile(TileID.LunarCraftingStation);
                    recipe.AddIngredient(ModContent.ItemType<UnholyCore>(), 5);
                    recipe.AddIngredient(ItemID.SoulofLight, 5);
                    recipe.AddIngredient(ItemID.SoulofNight, 5);
                    recipe.AddIngredient(ItemID.PinkPricklyPear);
                    recipe.AddTile(TileID.MythrilAnvil);
                }
                if (recipe.HasResult(ModContent.ItemType<GrandGelatin>()))
                {
                    recipe.AddIngredient<MirageJellyItem>();
                }
                if (recipe.HasResult(ModContent.ItemType<TheAbsorber>()))
                {
                    recipe.AddIngredient<Regenator>();
                }
                if (recipe.HasResult(ModContent.ItemType<TheSponge>()))
                {
                    recipe.AddIngredient<AquaticHeart>();
                    recipe.AddIngredient<FlameLickedShell>();
                    recipe.AddIngredient<TrinketofChi>();
                    recipe.AddIngredient<AmidiasSpark>();
                    recipe.AddIngredient<UrsaSergeant>();
                    recipe.AddIngredient<PermafrostsConcoction>();
                }
                if (recipe.HasResult(ModContent.ItemType<RampartofDeities>()))
                {
                    recipe.AddIngredient<RustyMedallion>();
                    recipe.AddIngredient<AmidiasPendant>();
                }
                if (recipe.HasResult(ModContent.ItemType<ElysianTracers>()))
                {
                    recipe.AddIngredient<GravistarSabaton>();
                    recipe.AddIngredient<Microxodonta>();
                }
                if (recipe.HasResult(ModContent.ItemType<AmbrosialAmpoule>()))
                {
                    recipe.AddIngredient<ArchaicPowder>();
                    recipe.AddIngredient<HoneyDew>();
                }
                if (recipe.HasResult(ModContent.ItemType<AbyssalDivingGear>()))
                {
                    recipe.AddIngredient<OceanCrest>();
                }
                if (recipe.HasResult(ModContent.ItemType<AbyssalDivingSuit>()))
                {
                    recipe.AddIngredient<AquaticEmblem>();
                    recipe.AddIngredient<SpelunkersAmulet>();
                    recipe.AddIngredient<AlluringBait>();
                    recipe.AddIngredient<LumenousAmulet>();
                }
                if (recipe.HasResult(ModContent.ItemType<TheAmalgam>()))
                {
                    recipe.AddIngredient<GiantPearl>();
                    recipe.AddIngredient<ManaPolarizer>();
                    recipe.AddIngredient<FrostFlare>();
                    recipe.AddIngredient<CorrosiveSpine>();
                    recipe.AddIngredient<VoidofExtinction>();
                    recipe.AddIngredient<LeviathanAmbergris>();
                    recipe.AddIngredient(ItemID.SporeSac);
                    recipe.AddIngredient<TheCamper>();
                    recipe.AddIngredient<PlagueHive>();
                    recipe.AddIngredient<AstralArcanum>();
                    recipe.AddIngredient<DynamoStemCells>();
                    recipe.AddIngredient<BlazingCore>();
                    recipe.AddIngredient<TheEvolution>();
                    recipe.AddIngredient<Affliction>();
                    recipe.AddIngredient<OldDukeScales>();
                }
                if (recipe.HasResult(ModContent.ItemType<PlagueHive>()))
                {
                    recipe.AddIngredient<ToxicHeart>();
                }
                if (recipe.HasResult(ModContent.ItemType<PhantomicArtifact>()))
                {
                    recipe.RemoveIngredient(ModContent.ItemType<HallowedRune>());
                    recipe.RemoveIngredient(ModContent.ItemType<RuinousSoul>());
                    recipe.RemoveIngredient(ModContent.ItemType<BloodOrb>());
                    recipe.RemoveIngredient(ModContent.ItemType<ExodiumCluster>());
                    recipe.RemoveTile(TileID.LunarCraftingStation);
                    recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Placeables.Plates.Navyplate>(), 25);
                    recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 5);
                    recipe.AddIngredient(ModContent.ItemType<ExodiumCluster>(), 25);
                    recipe.AddTile(TileID.DemonAltar);
                }
                if (recipe.HasResult(ModContent.ItemType<ResilientCandle>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<EssenceofBabil>(), 444);
                }
                if (recipe.HasResult(ModContent.ItemType<StormfrontRazor>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<EssenceofBabil>(), 4);
                }
                if (recipe.HasResult(ModContent.ItemType<TearsofHeaven>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<AeroBolt>());
                    recipe.AddIngredient(ModContent.ItemType<ThunderBolt>());
                }
                if (recipe.HasResult(ModContent.ItemType<Voidragon>()))
                {
                    recipe.RemoveIngredient(ModContent.ItemType<Seadragon>());
                    recipe.AddIngredient(ModContent.ItemType<Megaskeet>());
                }
                #region Yharim Bar Recipes
                if (recipe.HasResult(ModContent.ItemType<AsgardianAegis>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 4);
                }
                if (recipe.HasResult(ModContent.ItemType<AuricBar>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>());
                }
                if (recipe.HasResult(ModContent.ItemType<AuricTeslaBodyArmor>()) || recipe.HasResult(ModContent.ItemType<AuricTeslaCuisses>()) || recipe.HasResult(ModContent.ItemType<AuricTeslaHoodedFacemask>()) || recipe.HasResult(ModContent.ItemType<AuricTeslaPlumedHelm>()) || recipe.HasResult(ModContent.ItemType<AuricTeslaRoyalHelm>()) || recipe.HasResult(ModContent.ItemType<AuricTeslaSpaceHelmet>()) || recipe.HasResult(ModContent.ItemType<AuricTeslaWireHemmedVisage>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 35);
                }
                if (recipe.HasResult(ModContent.ItemType<CoreOfTheBloodGod>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 4);
                }
                if (recipe.HasResult(ModContent.ItemType<EclipseMirror>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 4);
                }
                if (recipe.HasResult(ModContent.ItemType<EcologicalCollapse>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 5);
                }
                if (recipe.HasResult(ModContent.ItemType<EidolonStaff>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 17);
                }
                if (recipe.HasResult(ModContent.ItemType<ElementalQuiver>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 4);
                }
                if (recipe.HasResult(ModContent.ItemType<ElysianTracers>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 4);
                }

                if (recipe.HasResult(ModContent.ItemType<GazeOfCrysthamyr>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 50);
                }
                if (recipe.HasResult(ModContent.ItemType<GodSlayerSlug>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 4);
                }
                if (recipe.HasResult(ModContent.ItemType<GrandReef>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 30);
                }
                if (recipe.HasResult(ModContent.ItemType<HadopelagicEcho>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 2);
                }
                if (recipe.HasResult(ModContent.ItemType<HalibutCannon>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 99);
                }
                if (recipe.HasResult(ModContent.ItemType<HolyMantle>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>());
                }

                if (recipe.HasResult(ModContent.ItemType<MagnaCore>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 49);
                }
                if (recipe.HasResult(ModContent.ItemType<Megaskeet>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 99);
                }
                if (recipe.HasResult(ModContent.ItemType<Nanotech>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 4);
                }

                if (recipe.HasResult(ModContent.ItemType<Nucleogenesis>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 4);
                }
                if (recipe.HasResult(ModContent.ItemType<OmegaHealingPotion>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 90);
                }

                if (recipe.HasResult(ModContent.ItemType<PlasmaGrenade>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>());
                }
                if (recipe.HasResult(ModContent.ItemType<QuiverofMadness>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 4);
                }
                if (recipe.HasResult(ModContent.ItemType<RampartofDeities>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 4);
                }
                if (recipe.HasResult(ModContent.ItemType<RoguesLootbox>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 22);
                }
                if (recipe.HasResult(ModContent.ItemType<ScorchedEarth>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 99);
                }
                if (recipe.HasResult(ModContent.ItemType<SearedPan>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 99);
                }
                if (recipe.HasResult(ModContent.ItemType<Slimelgamation>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 4);
                }
                if (recipe.HasResult(ModContent.ItemType<TheAmalgam>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 99);
                }
                if (recipe.HasResult(ModContent.ItemType<TheDevourerofCods>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 10);
                }
                /*
                if (recipe.HasResult(ModContent.ItemType<TheDreamingGhost>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 4);
                }
                */
                if (recipe.HasResult(ModContent.ItemType<TheSponge>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 100);
                }
                if (recipe.HasResult(ModContent.ItemType<ThrowersGauntlet>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>());
                }
                /*
                if (recipe.HasResult(ModContent.ItemType<TyrantShield>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 124);
                }
                */
                if (recipe.HasResult(ModContent.ItemType<UniversalStone>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 100);
                }

                if (recipe.HasResult(ItemID.Zenith))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 193);
                }

                if (recipe.HasResult(ModContent.ItemType<ZenithArcanum>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<YharimBar>(), 99);
                }
                #endregion
            }
        }

        public void AlcoholRecipe(int result, int drinkingredient, int midgredient, int lastgredient, int blorbcount, int midnum = 5)
        {
            int lastnum = 1;
            if (lastgredient == ModContent.ItemType<HallowedOre>())
            {
                lastnum = 5;
            }
            Recipe norm = Recipe.Create(result, 5);
            norm.AddIngredient(drinkingredient);
            norm.AddIngredient(midgredient, midnum);
            norm.AddIngredient(lastgredient, lastnum);
            norm.AddTile(TileID.Kegs);
            norm.Register();
            Recipe blood = Recipe.Create(result, 5);
            blood.AddIngredient(drinkingredient);
            blood.AddIngredient(ModContent.ItemType<BloodOrb>(), blorbcount);
            blood.AddIngredient(lastgredient, lastnum);
            blood.AddTile(TileID.AlchemyTable);
            blood.Register();
        }
        public void CandleRecipe(int result, int soul, int soulnum, int essence, int essencenum)
        {
            Recipe recipe = Recipe.Create(result);
            recipe.AddIngredient(ItemID.PeaceCandle);
            recipe.AddIngredient(essence, essencenum);
            recipe.AddIngredient(soul, soulnum);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
        public void CosmichidGroup(RecipeGroup group, string name, int herb)
        {
            group = new RecipeGroup(() => $"{Lang.GetItemNameValue(herb)} or Cosmichid",
            herb, ModContent.ItemType<Cosmichid>());
            RecipeGroup.RegisterGroup("CalRemix:" + name, group);
        }
        public void CosmichidChange(Recipe recipe, string group, int herb)
        {
            if (recipe.TryGetIngredient(herb, out Item item))
            {
                recipe.AddRecipeGroup("CalRemix:" + group, item.stack);
                recipe.RemoveIngredient(item);
            }
        }
    }
}
