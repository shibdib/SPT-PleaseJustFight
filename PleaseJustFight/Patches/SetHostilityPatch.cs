using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Comfort.Common;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace PleaseJustFight.Patches
{
    public class SetHostilityPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(BotOwner), "method_10");
        }
        
        [PatchPostfix]
        private static void PatchPostfix(BotOwner __instance)
        {
            if (__instance.IsRole(WildSpawnType.pmcBEAR) || __instance.IsRole(WildSpawnType.pmcUSEC))
            {
                SetAllEnemies(__instance);
            } else if (__instance.IsRole(WildSpawnType.assault) || __instance.IsRole(WildSpawnType.assaultGroup) || 
                       __instance.IsRole(WildSpawnType.marksman))
            {
                SetPmcAsEnemies(__instance);
            } else if (__instance.IsRole(WildSpawnType.bossKnight) || __instance.IsRole(WildSpawnType.followerBirdEye) || 
                       __instance.IsRole(WildSpawnType.followerBigPipe))
            {
                SetPmcAsEnemies(__instance);
            }
        }

        // Set pmc's as hostile towards all other bots with some boss exceptions
        private static void SetAllEnemies(BotOwner newBot)
        {
            IEnumerable<IPlayer> humanPlayers = Singleton<GameWorld>.Instance.AllAlivePlayersList
                .Where(p => !p.IsAI);
            
            foreach (IPlayer humanPlayer in humanPlayers)
            {
                newBot.BotsGroup.AddEnemy(humanPlayer, EBotEnemyCause.initial);
            }

            IEnumerable<BotOwner> activatedBots = Singleton<IBotGame>.Instance.BotsController.Bots.BotOwners
                .Where(b => b.BotState == EBotState.Active && b.Profile.Id != newBot.Profile.Id);
            foreach (BotOwner bot in activatedBots)
            {
                if (bot.IsRole(WildSpawnType.gifter) || bot.IsRole(WildSpawnType.shooterBTR) || 
                    bot.IsRole(WildSpawnType.bossZryachiy) || bot.IsRole(WildSpawnType.peacefullZryachiyEvent) 
                    || bot.IsRole(WildSpawnType.followerZryachiy))
                {
                    continue;
                }
                
                bot.BotsGroup.AddEnemy(newBot, EBotEnemyCause.initial);
                newBot.BotsGroup.AddEnemy(bot, EBotEnemyCause.initial);
            }
        } 

        // Set scavs and goons as hostile towards all pmc's
        private static void SetPmcAsEnemies(BotOwner newBot)
        {
            IEnumerable<IPlayer> humanPlayers = Singleton<GameWorld>.Instance.AllAlivePlayersList
                .Where(p => !p.IsAI);
            
            foreach (IPlayer humanPlayer in humanPlayers)
            {
                // Scav run check
                if (humanPlayer.Profile.Info.Side != EPlayerSide.Savage)
                {
                    newBot.BotsGroup.AddEnemy(humanPlayer, EBotEnemyCause.initial);
                }
            }

            IEnumerable<BotOwner> activatedBots = Singleton<IBotGame>.Instance.BotsController.Bots.BotOwners
                .Where(b => b.BotState == EBotState.Active && (b.IsRole(WildSpawnType.pmcBEAR) || b.IsRole(WildSpawnType.pmcUSEC)));
            foreach (BotOwner bot in activatedBots)
            {
                bot.BotsGroup.AddEnemy(newBot, EBotEnemyCause.initial);
                newBot.BotsGroup.AddEnemy(bot, EBotEnemyCause.initial);
            }
        }
    }
}