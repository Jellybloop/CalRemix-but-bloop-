﻿using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;
using CalamityMod.World;
using CalamityMod.Sounds;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.NormalNPCs;
using CalRemix.Items;
using CalRemix.Projectiles.WulfrumExcavator;

using System;
using CalRemix.Items.Materials;

namespace CalRemix.NPCs.Bosses
{
    [AutoloadBossHead]
    public class WulfrumExcavatorHead : ModNPC
    {


        public enum WulfrumExcavatorAIState
        {
            BasicLaserShots,
            CircleWithLiterallyNothingElse,
            RectangularHoverWithLaserBursts,
            LemniscateWithLaserCircles
        }

        // Define specific attack properties.
        public int LaserBurstCount
        {
            get;
            set;
        }

        public float CurrentSpinOffsetAngle
        {
            get;
            set;
        }

        public Vector2 RectangleMoveDirection
        {
            get;
            set;
        }

        public Vector2 TargetScreenResolution
        {
            get;
            set;
        }

        public int RectangleRedirectCounter
        {
            get;
            set;
        }

        public int LemniscateCircleCounter
        {
            get;
            set;
        }

        // Define universal attack properties. The 0th AI slot is reserved for the index of the next segment.
        public Player Target => Main.player[NPC.target];

        public WulfrumExcavatorAIState AIState
        {
            get => (WulfrumExcavatorAIState)NPC.ai[1];
            set => NPC.ai[1] = (int)value;
        }

        public ref float AttackTimer => ref NPC.ai[2];

        // Define useful properties for convenience of calculations.
        public float LifeRatio => NPC.life / (float)NPC.lifeMax;

        // Define phase variables.
        public static float BaseDifficultyScale => MathHelper.Clamp((CalamityWorld.revenge.ToInt() + Main.expertMode.ToInt()) * 0.5f, 0f, 1f);

        public static float Phase2LifeRatio => MathHelper.Lerp(0.75f, 0.6f, BaseDifficultyScale);

        public static float Phase3LifeRatio => MathHelper.Lerp(0.5f, 0.4f, BaseDifficultyScale);

        public static float Phase4LifeRatio => MathHelper.Lerp(0.25f, 0.15f, BaseDifficultyScale);

        public int SegmentCount = 20;

        public bool InitializedChargeState = false;

        public int ChargeState = 0;

        public int DefaultChargeState = 0;

        public int PylonCharge = 4;

        public bool PylonCharged = false;

        public bool DeathCharge = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Excavator");
            Main.npcFrameCount[NPC.type] = 2;
        }

        public override void SetDefaults()
        {
            WulfrumExcavatorBody.InitializeSegment(NPC);
            NPC.damage = 20;
            Music = MusicLoader.GetMusicSlot("CalRemix/Sounds/Music/scourge of the scrapyard");
        }


        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("An aged project of an esteemed scientist. It appears to be barely functional.")
            });
        }
        
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.chaseable);
            writer.Write(NPC.dontTakeDamage);
            writer.Write(LaserBurstCount);
            writer.WriteVector2(RectangleMoveDirection);
            writer.WriteVector2(TargetScreenResolution);
            writer.Write(RectangleRedirectCounter);
            writer.Write(LemniscateCircleCounter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.chaseable = reader.ReadBoolean();
            NPC.dontTakeDamage = reader.ReadBoolean();
            LaserBurstCount = reader.ReadInt32();
            RectangleMoveDirection = reader.ReadVector2();
            TargetScreenResolution = reader.ReadVector2();
            RectangleRedirectCounter = reader.ReadInt32();
            LemniscateCircleCounter = reader.ReadInt32();
        }

        public override void AI()
        {
            if (Main.getGoodWorld)
            {
                this.SegmentCount *= 3;
            }

            if (!InitializedChargeState)
            {
                if (BaseDifficultyScale >= 0.5f)
                {
                    DefaultChargeState += 1;
                }

                if (BaseDifficultyScale >= 0.9f)
                {
                    DefaultChargeState += 1;
                }

                if (CalamityWorld.death)
                {
                    DefaultChargeState += 1;
                }
                InitializedChargeState = true;
            }

            // Reset things every frame. They may be temporarily changed by AI states below.
            NPC.dontTakeDamage = false;
            NPC.damage = NPC.defDamage;

            // Adds Boss Zen to the player.
            Main.player[Main.myPlayer].Calamity().isNearbyBoss = true;
            Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<CalamityMod.Buffs.StatBuffs.BossEffects>(), 10, true);

            // Pick a new target if the current one is invalid.
            if (NPC.target <= -1 || NPC.target >= Main.maxPlayers || Target.dead || !Target.active || !NPC.WithinRange(Target.Center, CalamityGlobalNPC.CatchUpDistance350Tiles))
            {
                NPC.TargetClosest();

                // Despawn if the closest target is still invalid. A netupdate is fired by vanilla's default AI loop method following this, ensuring that this is synced.
                if (NPC.target <= -1 || NPC.target >= Main.maxPlayers || Target.dead || !Target.active || !NPC.WithinRange(Target.Center, CalamityGlobalNPC.CatchUpDistance350Tiles))
                    NPC.active = false;
            }

            // Create segments on the first frame.
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[0] == 0f)
            {
                SpawnSegments();
                NPC.localAI[0] = 1f;
            }

            if (ChargeState == 4)
            {
                DeathCharge = true;
            }

            DeathCharge = false;

                switch (AIState)
            {
                case WulfrumExcavatorAIState.BasicLaserShots:
                    DoBehavior_BasicLaserShots();
                    break;
                case WulfrumExcavatorAIState.CircleWithLiterallyNothingElse:
                    DoBehavior_CircleWithLiterallyNothingElse();
                    break;
                case WulfrumExcavatorAIState.RectangularHoverWithLaserBursts:
                    DoBehavior_RectangularHoverWithLaserBursts();
                    break;
                case WulfrumExcavatorAIState.LemniscateWithLaserCircles:
                    DoBehavior_LemniscateWithLaserCircles();
                    break;
            }

            AttackTimer++;
        }

        public void SpawnSegments()
        {
            int previousSegment = NPC.whoAmI;



            for (int i = 0; i < SegmentCount; i++)
            {


                int nextSegmentIndex;
                if (i < SegmentCount - 1)
                    nextSegmentIndex = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WulfrumExcavatorBody>(), NPC.whoAmI);
                else
                    nextSegmentIndex = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WulfrumExcavatorTail>(), NPC.whoAmI);
                Main.npc[nextSegmentIndex].realLife = NPC.whoAmI;
                Main.npc[nextSegmentIndex].ai[2] = NPC.whoAmI;
                Main.npc[nextSegmentIndex].ai[1] = previousSegment;
                Main.npc[previousSegment].ai[0] = nextSegmentIndex;

                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, nextSegmentIndex, 0f, 0f, 0f, 0);

                previousSegment = nextSegmentIndex;
            }
        }

        public void DoBehavior_BasicLaserShots()
        {
            int laserBurstCount = 3;
            int segmentSpacingPerLaser = 3;
            int delayPerBurst = 360;
            int damageDelay = 200;
            int slowdownTime = 30;
            int timeSpentFiringLasers = 60;
            int laserShootTime = slowdownTime + timeSpentFiringLasers;
            int laserDamage = 24;
            float flySpeed = MathHelper.Lerp(5.6f, 8f, 1f - LifeRatio);
            float laserShootSpeed = 7f;

            if (PylonCharged)
            {
                ChargeState += 1;
            }

            if (BaseDifficultyScale >= 0.5f || ChargeState >= 1)
            {
                laserBurstCount--;
                delayPerBurst -= 30;
                flySpeed += 2.5f;
                laserDamage = 22;
            }
            if (BaseDifficultyScale >= 0.9f || ChargeState >= 2)
            {
                segmentSpacingPerLaser--;
                delayPerBurst -= 30;
                flySpeed += 2.5f;
            }
            if (CalamityWorld.death || ChargeState >= 3)
                laserShootSpeed *= 1.2f;

            if (ChargeState >= 4) // This would normally be in Malice as well, but that doesn't exist, so it's exclusive to being charged on Death. If we bring it back, this will probably need new changes.
            {
                flySpeed += 4f;
            }

            // Fly towards the target.
            // If currently occupied shooting lasers, slow down.
            float wrappedAttackTimer = AttackTimer % delayPerBurst;
            float slowDownInterpolant = Utils.GetLerpValue(-48f, 0f, wrappedAttackTimer - delayPerBurst + laserShootTime, true);
            flySpeed *= MathHelper.SmoothStep(1f, 0.01f, slowDownInterpolant);

            // Release lasers across all segments, with even spacing.
            if (wrappedAttackTimer == delayPerBurst - laserShootTime)
            {
                // Play a sound to accompany the laser creation.
                if (NPC.WithinRange(Target.Center, 1400f))
                    SoundEngine.PlaySound(SoundID.Item33, Target.Center);

                // Create lasers.
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Loop through the linked list of worm body segments until the tail is reached.
                    List<NPC> segments = new();
                    NPC previousSegment = NPC;
                    int laserID = ModContent.ProjectileType<ExcavatorLaser>();

                    while (Main.npc.IndexInRange((int)previousSegment.ai[0]) && Main.npc[(int)previousSegment.ai[0]].active)
                    {
                        NPC nextSegment = Main.npc[(int)previousSegment.ai[0]];
                        if (nextSegment.type != NPC.type && nextSegment.type != ModContent.NPCType<WulfrumExcavatorBody>())
                            break;

                        segments.Add(nextSegment);
                        previousSegment = nextSegment;
                    }

                    for (int i = 1; i < segments.Count; i += segmentSpacingPerLaser)
                    {
                        int laser = Projectile.NewProjectile(segments[i].GetSource_FromAI(), segments[i].Center, segments[i].SafeDirectionTo(Target.Center) * laserShootSpeed, laserID, laserDamage, 0f);
                        if (Main.projectile.IndexInRange(laser))
                            Main.projectile[laser].ai[1] = segments[i].whoAmI;
                    }
                }
            }

            // Increment the laser burst count. Once it exceeds the threshold, select a new attack.
            if (wrappedAttackTimer == delayPerBurst - 1f)
            {
                LaserBurstCount++;
                if (LaserBurstCount >= laserBurstCount)
                    SelectNextAttack();

                NPC.netUpdate = true;
            }

            DefaultMovementTowards(Target.Center, flySpeed * 0.4f, flySpeed * 1.1f, flySpeed / 296f);
        }

        public void DoBehavior_CircleWithLiterallyNothingElse() //name is no longer accurate but it's a good historical note for how the attack used to be
        {
            int circleTime = 360;
            int damageDelay = 56;
            float spinRadius = 320f;
            float spinSpeed = 50f;
            float spinAngularVelocity = MathHelper.Pi / circleTime * 4f;
            int laserCount = 4;
            int delayPerBurst = 180;
            int slowdownTime = 30;
            int segmentSpacingPerLaser = 3;
            int timeSpentFiringLasers = 60;
            int laserShootTime = slowdownTime + timeSpentFiringLasers;
            int laserDamage = 24;
            float laserShootSpeed = 7f;


            if (PylonCharged)
            {
                ChargeState += 1;
            }

            if (BaseDifficultyScale >= 0.5f || ChargeState >= 1)
            {
                laserCount++;
                delayPerBurst -= 30;
                spinSpeed += 1f;
                laserDamage = 22;
            }
            if (BaseDifficultyScale >= 0.9f || ChargeState >= 2)
            {
                laserCount++;
                delayPerBurst -= 30;
                spinSpeed += 1f;
            }
            if (CalamityWorld.death || ChargeState >= 3)
                laserShootSpeed *= 1.2f;

            if (ChargeState >= 4) // This would normally be in Malice as well, but that doesn't exist, so it's exclusive to being charged on Death. If we bring it back, this will probably need new changes.
            {
                spinSpeed += 1f;
            }



            // Don't do damage at first, to prevent cheap hits.
            if (AttackTimer < damageDelay)
                NPC.damage = 0;

            if (Main.getGoodWorld) // Radius tweaks on FTW to ensure the boss isn't unavoidable.
            {
                spinRadius *= 3;
            }

            // Initialize the spin offset angle.
            if (AttackTimer == 1f)
            {
                CurrentSpinOffsetAngle = NPC.AngleTo(Target.Center) + MathHelper.Pi;
                NPC.netUpdate = true;
            }

            CurrentSpinOffsetAngle += spinAngularVelocity;
            Vector2 flyDestination = Target.Center + CurrentSpinOffsetAngle.ToRotationVector2() * spinRadius;
            NPC.velocity = Utils.MoveTowards(NPC.velocity, flyDestination - NPC.Center, Utils.GetLerpValue(0f, 40f, AttackTimer, true) * spinSpeed);
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

            float wrappedAttackTimer = AttackTimer % delayPerBurst;
            float slowDownInterpolant = Utils.GetLerpValue(-48f, 0f, wrappedAttackTimer - delayPerBurst + laserShootTime, true);

            if (wrappedAttackTimer == delayPerBurst - laserShootTime)
            {
                // Play a sound to accompany the laser creation.
                if (NPC.WithinRange(Target.Center, 1400f))
                    SoundEngine.PlaySound(SoundID.Item33, Target.Center);

                // Create lasers.
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Loop through the linked list of worm body segments until the tail is reached.
                    List<NPC> segments = new();
                    NPC previousSegment = NPC;
                    int laserID = ModContent.ProjectileType<ExcavatorLaser>();
                    float laserShootOffsetAngle = Main.rand.NextFloat(MathHelper.TwoPi);

                    while (Main.npc.IndexInRange((int)previousSegment.ai[0]) && Main.npc[(int)previousSegment.ai[0]].active)
                    {
                        NPC nextSegment = Main.npc[(int)previousSegment.ai[0]];
                        if (nextSegment.type != NPC.type && nextSegment.type != ModContent.NPCType<WulfrumExcavatorBody>())
                            break;

                        segments.Add(nextSegment);
                        previousSegment = nextSegment;
                    }

                    for (int i = 0; i < laserCount; i++)
                    {
                        Vector2 laserShootVelocity = (MathHelper.TwoPi * i / laserCount + laserShootOffsetAngle).ToRotationVector2() * laserShootSpeed;
                        int laser = Projectile.NewProjectile(segments[i].GetSource_FromAI(), NPC.Center, laserShootVelocity, laserID, laserDamage, 0f);
                        if (Main.projectile.IndexInRange(laser))
                            Main.projectile[laser].ai[1] = segments[i].whoAmI;
                    }
                }
            }


            // Lol!
            if (AttackTimer >= circleTime)
                SelectNextAttack();
        }

        public void DoBehavior_RectangularHoverWithLaserBursts()
        {
            int rectangleRedirectCount = 6;
            int laserCount = 7;
            int laserDamage = 24;
            float laserSpread = 0.71f;
            float laserShootSpeed = 7f;
            float flySpeed = 13f;

            if (BaseDifficultyScale >= 0.5f || ChargeState >= 1)
                laserDamage = 22;
            if (BaseDifficultyScale >= 1f || ChargeState >= 2)
                laserCount += 2;
            if (CalamityWorld.death || ChargeState >= 3)
            {
                flySpeed += 3f;
                laserShootSpeed *= 1.2f;
            }

            if (ChargeState >= 4)
            {
                flySpeed += 4f;
            }

            // Disable contact damage altogether.
            NPC.damage = 0;

            // Initialize the hover offset.
            if (RectangleMoveDirection == Vector2.Zero)
            {
                RectangleMoveDirection = new(Main.rand.NextBool().ToDirectionInt(), Main.rand.NextBool().ToDirectionInt());
                NPC.netUpdate = true;
            }

            // Resolution is non-deterministic, as it varies between clients. As such, it must be synced.
            if (Main.myPlayer == NPC.target && TargetScreenResolution == Vector2.Zero)
            {
                TargetScreenResolution = new(Main.screenWidth, Main.screenHeight);
                NPC.netUpdate = true;
            }

            Vector2 flyDestination = Target.Center + RectangleMoveDirection * TargetScreenResolution * 0.35f;
            DefaultMovementTowards(flyDestination, flySpeed * 0.5f, flySpeed, MathHelper.PiOver4);

            // Switch directions amd fire lasers if near the ideal destination.
            if (NPC.WithinRange(flyDestination, flySpeed * 8f))
            {
                // Play a sound to accompany the laser creation.
                if (NPC.WithinRange(Target.Center, 1400f))
                    SoundEngine.PlaySound(SoundID.Item33, Target.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int laserID = ModContent.ProjectileType<ExcavatorShot>();
                    for (int i = 0; i < laserCount; i++)
                    {
                        float laserShootOffsetAngle = MathHelper.Lerp(-laserSpread, laserSpread, i / (float)(laserCount - 1f));
                        Vector2 laserShootVelocity = NPC.SafeDirectionTo(Target.Center).RotatedBy(laserShootOffsetAngle) * laserShootSpeed;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, laserShootVelocity, laserID, laserDamage, 0f);
                    }
                }

                NPC.velocity *= 0.6f;
                RectangleRedirectCounter++;
                RectangleMoveDirection = RectangleMoveDirection.RotatedBy(MathHelper.PiOver2);
                if (RectangleRedirectCounter >= rectangleRedirectCount)
                {
                    NPC.velocity *= 0.5f;
                    SelectNextAttack();
                }
            }
        }

        public void DoBehavior_LemniscateWithLaserCircles()
        {
            int cyclePeriod = 75;
            int cycleCount = 4;
            int laserCount = 6;
            int laserDamage = 24;
            int damageDelay = 100;
            float laserShootSpeed = 9f;

            if (PylonCharged)
            {
                ChargeState += 1;
            }

            if (LaserBurstCount >= 0.5f)
            {
                laserDamage = 22;
                laserCount += 2;
            }
            if (LaserBurstCount >= 1f)
            {
                cyclePeriod -= 8;
                laserCount += 2;
            }
            if (CalamityWorld.death || ChargeState >= 2)
            {
                laserCount += 2;
                laserShootSpeed *= 1.2f;
            }

            if (ChargeState >= 4)
            {
                laserCount += 2;
                laserShootSpeed *= 1.2f;
            }

            // Ensures the boss shouldn't telefrag the player
            NPC.damage = 0;

            // Teleport above the player on the first frame.
            if (AttackTimer == 1f)
            {

                NPC.Center = Target.Center + new Vector2(750f, -500f);

                int bodyID = ModContent.NPCType<WulfrumExcavatorBody>();
                int tailID = ModContent.NPCType<WulfrumExcavatorTail>();

                // Bring the segments to the head position.
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (!Main.npc[i].active || Main.npc[i].type != bodyID || Main.npc[i].type != tailID || Main.npc[i].realLife != NPC.whoAmI)
                        continue;
                    
                    Main.npc[i].Center = NPC.Center;
                    Main.npc[i].netUpdate = true;
                }

                NPC.netUpdate = true;
            }               

            // The parametric form of a lemniscate of bernoulli is as follows:
            // x = r * cos(t) / (1 + sin^2(t))
            // y = r * sin(t) * cos(t) / (1 + sin^2(t))
            // Given that these provide positions, we can determine the velocity path that Deus must follow to move in this pattern
            // via taking derivatives of both components.

            // Quotient rule:
            // (g(x) / h(x))' = (g'(x) * h(x) - g(x) * h'(x)) / h(x)^2

            // Shorthands:
            // g(t) = cos(t) --- cos(t) * sin(t)
            // g'(t) = -sin(t) --- cos(2t)
            // h(t) = 1 + sin^2(t)
            // h'(t) = sin(2t)

            // Calculations for dx/dt:
            // (g'(t) * h(t) - g(t) * h'(t)) / h(t)^2 = 
            // r * ((-sin(t) * (1 + sin^2(t)) - cos(t) * sin(2t)) / (1 + sin^2(t))^2 =
            // r * (-sin(t) - sin^3(t) - cos(t) * sin(2t)) / (1 + 2sin^2(t) + sin^4(t))

            // Calculations for dy/dt:
            // (g'(t) * h(t) - g(t) * h'(t)) / h(t)^2 = 
            // r * ((cos(2t) * (1 + sin^2(t)) - sin(t) * cos(t) * sin(2t)) / (1 + sin^2(t))^2 =
            // r * (cos^2(t) - 2sin^4(t) - sin(t) * cos(t) * sin(2t)) / (1 + 2sin^2(t) + sin^4(t))
            float flySpeed = 32f;
            float t = MathHelper.TwoPi * AttackTimer / (cyclePeriod * 2f);
            float sinT = (float)Math.Sin(t);
            float sin2T = (float)Math.Sin(2D * t);
            float cosT = (float)Math.Cos(t);
            float denominator = (float)(1D + 2D * Math.Pow(Math.Sin(t), 2D) + Math.Pow(sinT, 4D));

            if (CalamityWorld.death && NPC.life < 0.05f)
                flySpeed += 12f;

            float speedX = flySpeed * (float)(-sinT - Math.Pow(sinT, 3D) - cosT * sin2T) / denominator;
            float speedY = flySpeed * (float)(Math.Pow(cosT, 2D) - 2D * Math.Pow(sinT, 4D) - sinT * cosT * sin2T) / denominator;
            NPC.velocity = new(speedX, speedY);
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;



            // Release bursts of lasers in an even spread.
            if (AttackTimer % cyclePeriod == cyclePeriod - 1f)
            {
                // Play a sound to accompany the laser creation.
                if (NPC.WithinRange(Target.Center, 1400f))
                    SoundEngine.PlaySound(SoundID.Item33, Target.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int laserID = ModContent.ProjectileType<ExcavatorShot>();
                    float laserShootOffsetAngle = Main.rand.NextFloat(MathHelper.TwoPi);
                    for (int i = 0; i < laserCount; i++)
                    {
                        Vector2 laserShootVelocity = (MathHelper.TwoPi * i / laserCount + laserShootOffsetAngle).ToRotationVector2() * laserShootSpeed;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, laserShootVelocity, laserID, laserDamage, 0f);
                    }
                    LemniscateCircleCounter++;
                    if (LemniscateCircleCounter >= cycleCount)
                        SelectNextAttack();
                    NPC.netUpdate = true;
                }
            }
        }

        public void DefaultMovementTowards(Vector2 destination, float maxSpeed, float minSpeed, float angularTurnSpeed)
        {
            float newSpeed = NPC.velocity.Length();
            Vector2 directionToDestination = NPC.SafeDirectionTo(destination);

            // Slow down and focus on turning around if pointing away from the destination.
            if (Vector2.Dot(NPC.velocity, directionToDestination) < 0f)
            {
                newSpeed *= 0.9f;
                angularTurnSpeed *= 1.5f;
            }

            // Speed up if noticeable close or far away from the destination. If close, do not rotate.
            bool closeToDestination = NPC.WithinRange(destination, 250f);
            if (closeToDestination || !NPC.WithinRange(destination, 750f))
            {
                if (closeToDestination)
                    angularTurnSpeed = 0f;
                newSpeed *= 1.125f;
            }

            Vector2 newVelocity = NPC.velocity.ToRotation().AngleLerp(directionToDestination.ToRotation(), angularTurnSpeed).ToRotationVector2() * newSpeed;
            newVelocity = Utils.MoveTowards(newVelocity, directionToDestination * newSpeed, newSpeed * 0.02f);

            // Ensure that the velocity does not leave its determined magnitude range.
            NPC.velocity = newVelocity.ClampMagnitude(minSpeed, maxSpeed);

            // Apply rotation based on the new velocity.
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public void SelectNextAttack()
        {
            int summonCount = 2;
            ChargeState = 0;

            if (PylonCharged)
            {
                PylonCharge -= 1;
            }

            if (PylonCharge == 0)
            {
                PylonCharge = 4;
                ChargeState = DefaultChargeState;
                PylonCharged = false;
            }

            if (BaseDifficultyScale >= 0.5f)
            {
                ChargeState += 1;
                summonCount += 1;

            }
            if (BaseDifficultyScale >= 0.9f)
            {
                ChargeState += 1;
                summonCount += 1;
            }
            if (CalamityWorld.death)
            {
                ChargeState += 1;
                summonCount += 1;
            }

            if (Main.expertMode)
            {
                for (int i = 0; i < summonCount; i++) // Randomly chooses a wulfrum droid to spawn.
                {
                    int choice = Main.rand.Next(9);
                    switch (choice)
                    {
                        case 0 or 1:
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WulfrumDrone>(), NPC.whoAmI);
                            break;
                        case 2 or 3 or 4 or 5 or 6 or 7 or 8: // Gyrator is weighted to spawn more than the rest due to the original wiki page.
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WulfrumGyrator>(), NPC.whoAmI);
                            break;
                        case 9: // 1/10 since 0 counts.
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WulfrumAmplifier>(), NPC.whoAmI);
                            break;
                        default:
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WulfrumGyrator>(), NPC.whoAmI);
                            break;
                    }
                }

                // Cycle through attacks based on current HP.
                switch (AIState)
                {
                    case WulfrumExcavatorAIState.BasicLaserShots:
                        AIState = LifeRatio < Phase2LifeRatio ? WulfrumExcavatorAIState.CircleWithLiterallyNothingElse : WulfrumExcavatorAIState.BasicLaserShots;
                        break;
                    case WulfrumExcavatorAIState.CircleWithLiterallyNothingElse:
                        AIState = LifeRatio < Phase3LifeRatio ? WulfrumExcavatorAIState.RectangularHoverWithLaserBursts : WulfrumExcavatorAIState.BasicLaserShots;
                        break;
                    case WulfrumExcavatorAIState.RectangularHoverWithLaserBursts:
                        AIState = LifeRatio < Phase4LifeRatio ? WulfrumExcavatorAIState.LemniscateWithLaserCircles : WulfrumExcavatorAIState.BasicLaserShots;
                        break;
                    case WulfrumExcavatorAIState.LemniscateWithLaserCircles:
                        AIState = WulfrumExcavatorAIState.BasicLaserShots;
                        break;
                }

                // Reset attack-specific variables.
                LaserBurstCount = 0;
                RectangleMoveDirection = Vector2.Zero;
                TargetScreenResolution = Vector2.Zero;
                RectangleRedirectCounter = 0;
                LemniscateCircleCounter = 0;

                AttackTimer = 0f;
                NPC.netUpdate = true;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
            if (PylonCharged == true) // Handles Pylon charge sprites.
            {
                NPC.frame.Y = 1 * frameHeight;
            }
            else
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            LeadingConditionRule normalOnly = new LeadingConditionRule(new Conditions.NotExpert());
            npcLoot.Add(normalOnly);
           
            if (Main.rand.NextBool(Main.expertMode ? 2 : 1, 5))
            {
                npcLoot.Add(ModContent.ItemType<WulfrumMetalScrap>(), 20 + Main.rand.Next(6));
            }

            else
            {
                npcLoot.Add(ModContent.ItemType<WulfrumMetalScrap>(), 16 + Main.rand.Next(4));
            }
            npcLoot.Add(ModContent.ItemType<EnergyCore>(), 1 + Main.rand.Next(1));
            npcLoot.Add(ModContent.ItemType<EnergyOrb>());
            int choice = Main.rand.Next(4);
            switch (choice)
            {
                case 0:
                    npcLoot.Add(ModContent.ItemType<WulfrumScrewdriver>(), 1);
                    break;
                case 1:
                    npcLoot.Add(ModContent.ItemType<WulfrumBlunderbuss>(), 1);
                    break;
                case 2:
                    npcLoot.Add(ModContent.ItemType<WulfrumProsthesis>(), 1);
                    break;
                case 3:
                    npcLoot.Add(ModContent.ItemType<WulfrumController>(), 1);
                    break;
                case 4:
                    npcLoot.Add(ModContent.ItemType<WulfrumKnife>(), 100 + Main.rand.Next(50));
                    break;
                default:
                    npcLoot.Add(ModContent.ItemType<WulfrumScrewdriver>(), 1);
                    break;
            }

            if (PylonCharged)
            {
                npcLoot.Add(ModContent.ItemType<WulfrumRod>(), 1);
            }
            
            // Lore item
            npcLoot.Add(ItemDropRule.ByCondition(DropHelper.If(() => !CalRemixWorld.downedExcavator), ModContent.ItemType<KnowledgeExcavator>()));
        }

        public override bool SpecialOnKill()
        {
            CalRemixWorld.downedExcavator = true;
            // Position to head such that it's at the closest worm segment to the target on the last frame of its existence.
            // This is done to make loot more convenient to pick up.
            int closestSegmentID = DropHelper.FindClosestWormSegment(NPC,
                ModContent.NPCType<WulfrumExcavatorHead>(),
                ModContent.NPCType<WulfrumExcavatorBody>(),
                ModContent.NPCType<WulfrumExcavatorTail>());
    


            NPC.position = Main.npc[closestSegmentID].position;
            if (Main.netMode != NetmodeID.Server)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("WulfrumExcavatorHead").Type, 1f);
            }
                return false;
        }
        
        public override bool CheckActive() => false;

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

    }
}
