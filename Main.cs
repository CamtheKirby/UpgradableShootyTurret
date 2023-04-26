﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Il2CppAssets.Scripts.Unity.Scenes;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Powers;
using Il2CppAssets.Scripts.Models.Profile;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Upgrades;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.StoreMenu;
using Il2CppAssets.Scripts.Unity.UI_New.Upgrade;
using Il2CppAssets.Scripts.Utils;
using Harmony;
using Il2CppSystem.Collections.Generic;
using MelonLoader;

using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using System.Net;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;

using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.TowerFilters;
using Il2CppAssets.Scripts.Unity.UI_New.Main.MonkeySelect;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Emissions;
using Il2CppAssets.Scripts.Unity.Towers.Emissions;
using Il2CppSystem.Dynamic.Utils;
using Il2Cpp;

[assembly: MelonInfo(typeof(UpgradableShootyTurret.Main), UpgradableShootyTurret.ModHelperData.Name, UpgradableShootyTurret.ModHelperData.Version, UpgradableShootyTurret.ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace UpgradableShootyTurret
{

    class Main : BloonsMod
    {
        //https://github.com/gurrenm3/BloonsTD6-Mod-Helper/releases

        public class UpgradableShootyTurret : ModTower
        {
            public override string Name => "UpgradableShootyTurret";
            public override string DisplayName => "Upgradable Shooty Turret";
            public override string Description => "This is Geraldo's Shooty Turret but now it is upgradable!";
            public override string BaseTower => "DartMonkey";
            public override int Cost => 500;
            public override int TopPathUpgrades => 5;
            public override int MiddlePathUpgrades => 0;
            public override int BottomPathUpgrades => 0;
            public override Il2CppAssets.Scripts.Models.TowerSets.TowerSet TowerSet => Il2CppAssets.Scripts.Models.TowerSets.TowerSet.Support;
            public override void ModifyBaseTowerModel(TowerModel towerModel)
            {
                //stuff
                towerModel.GetBehavior<DisplayModel>().ignoreRotation = true;
                towerModel.AddBehavior(new DisplayModel("DisplayModel_UST_Top", new PrefabReference() { guidRef = "6a8505a5c8dd849489c750e3b65047dc" }, 0, default, 1, false, 0));
                var attackModel = towerModel.GetBehavior<AttackModel>();
                attackModel.weapons[0].projectile.GetBehavior<TravelStraitModel>().Lifespan = 4;
                attackModel.weapons[0].projectile.display = new PrefabReference() { guidRef = "e57060793f03d3046a9f97b8cb24986a" };

                //pierce, damage, and range
                attackModel.weapons[0].projectile.pierce = 10;
                attackModel.weapons[0].projectile.GetDamageModel().damage = 2;
                attackModel.range = 50;
                towerModel.range = 50;

                //how many seconds until it shoots
                attackModel.weapons[0].Rate = 1.5f;

                //makes tower look like shooty turret.
                towerModel.display = new PrefabReference() { guidRef = "c834b6ab8cd5afc429065acb83992abb" };
                towerModel.GetBehavior<DisplayModel>().display = new PrefabReference() { guidRef = "c834b6ab8cd5afc429065acb83992abb" };
            }
            public override string Icon => "6dc10060b4cb6174992724ee4ff00d95";
            public override string Portrait => "6dc10060b4cb6174992724ee4ff00d95";
        }
        public class FasterMechanisms : ModUpgrade<UpgradableShootyTurret>
        {
            public override string Name => "FasterMechanisms";
            public override string DisplayName => "Faster Mechanisms";
            public override string Description => "Shooty Turrets shoot faster!";
            public override int Cost => 250;
            public override int Path => TOP;
            public override int Tier => 1;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                AttackModel attackModel = towerModel.GetBehavior<AttackModel>();
                attackModel.weapons[0].Rate *= 0.8f;
            }
            public override string Icon => "6dc10060b4cb6174992724ee4ff00d95";
            public override string Portrait => "6dc10060b4cb6174992724ee4ff00d95";
        }
        public class EnhancedMechanisms : ModUpgrade<UpgradableShootyTurret>
        {
            public override string Name => "EnhancedMechanisms";
            public override string DisplayName => "Enhanced Mechanisms";
            public override string Description => "Arrows do more damage and the Shooty Turret shoots them faster and can pop camo bloons!";
            public override int Cost => 500;
            public override int Path => TOP;
            public override int Tier => 2;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                AttackModel attackModel = towerModel.GetBehavior<AttackModel>();
                attackModel.weapons[0].Rate *= 0.7f;
                attackModel.weapons[0].projectile.GetDamageModel().damage += 2;
                towerModel.AddBehavior(new OverrideCamoDetectionModel("OverrideCamoDetectionModel_", true));
            }
            public override string Icon => "6dc10060b4cb6174992724ee4ff00d95";
            public override string Portrait => "6dc10060b4cb6174992724ee4ff00d95";
        }
        public class Catapult : ModUpgrade<UpgradableShootyTurret>
        {
            public override string Name => "Catapult";
            public override string DisplayName => "Catapult";
            public override string Description => "The Shooty Turret becomes a catapult that has more pierce and damage";
            public override int Cost => 1000;
            public override int Path => TOP;
            public override int Tier => 3;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                AttackModel attackModel = towerModel.GetBehavior<AttackModel>();
                attackModel.weapons[0].projectile.display = new PrefabReference() { guidRef = "fcddee8a92f5d2e4d8605a8924566620" };
                towerModel.display = new PrefabReference() { guidRef = "1f72b507ec539e84c84e25011d855974" };
                towerModel.GetBehavior<DisplayModel>().display = new PrefabReference() { guidRef = "1f72b507ec539e84c84e25011d855974" };
                towerModel.GetBehavior<DisplayModel>().ignoreRotation = false;
                towerModel.RemoveBehavior(towerModel.behaviors.First(a => a.name == "DisplayModel_UST_Top"));
                attackModel.weapons[0].projectile.GetDamageModel().damage += 3;
                attackModel.weapons[0].projectile.pierce += 5;
            }
            public override string Icon => "6dc10060b4cb6174992724ee4ff00d95";
            public override string Portrait => "6dc10060b4cb6174992724ee4ff00d95";
        }
        
        public class Tripapult : ModUpgrade<UpgradableShootyTurret>
        {
            public override string Name => "Tripapult";
            public override string DisplayName => "Tripapult";
            public override string Description => "Give the catapult 3 shots and can pop any bloon type.";
            public override int Cost => 5000;
            public override int Path => TOP;
            public override int Tier => 4;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                AttackModel attackModel = towerModel.GetBehavior<AttackModel>();
                attackModel.weapons[0].projectile.display = new PrefabReference() { guidRef = "ee74983d627954e4e9765d86e05b4500" };
                towerModel.display = new PrefabReference() { guidRef = "9a4bf86ce3861a64c9e118a693db992f" };
                towerModel.GetBehavior<DisplayModel>().display = new PrefabReference() { guidRef = "9a4bf86ce3861a64c9e118a693db992f" };
                attackModel.weapons[0].emission = new ArcEmissionModel("ArcEmissionModel_Tripapult", 3, 0, 15, null, false);
                attackModel.weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
                attackModel.weapons[0].projectile.GetDamageModel().damage += 2;
            }
            public override string Icon => "6dc10060b4cb6174992724ee4ff00d95";
            public override string Portrait => "6dc10060b4cb6174992724ee4ff00d95";
        }
        public class Megapult : ModUpgrade<UpgradableShootyTurret>
        {
            public override string Name => "Megapult";
            public override string DisplayName => "Megapult";
            public override string Description => "Each projectile clusters into 2 even clustered ones.";
            public override int Cost => 27500;
            public override int Path => TOP;
            public override int Tier => 5;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                AttackModel attackModel = towerModel.GetBehavior<AttackModel>();
                attackModel.weapons[0].projectile.display = new PrefabReference() { guidRef = "c4b8e7aa3e07d764fb9c3c773ceec2ab" };
                towerModel.display = new PrefabReference() { guidRef = "b194c58ed09f1aa468e935b453c6843c" };
                towerModel.GetBehavior<DisplayModel>().display = new PrefabReference() { guidRef = "b194c58ed09f1aa468e935b453c6843c" };
                attackModel.weapons[0].projectile.CanHitCamo();

                attackModel.weapons[0].projectile.AddBehavior<CreateProjectileOnContactModel>(new CreateProjectileOnContactModel("CreateProjectileOnContactModel_Megapult", ModelExt.Duplicate<ProjectileModel>(attackModel.weapons[0].projectile), new ArcEmissionModel("ArcEmissionModel_Megapult", 2, 0f, 0f, null, true), false, false, true));
            }
            public override string Icon => "6dc10060b4cb6174992724ee4ff00d95";
            public override string Portrait => "6dc10060b4cb6174992724ee4ff00d95";
        }
    }
}