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
            SetPmcEnemies(__instance);
        }

        private static void SetPmcEnemies(BotOwner newBot)
        {
            // Only PMC's
            if (!newBot.IsRole(WildSpawnType.pmcBEAR) && !newBot.IsRole(WildSpawnType.pmcUSEC))
            {
                return;
            }
            
            IEnumerable<IPlayer> humanPlayers = Singleton<GameWorld>.Instance.AllAlivePlayersList
                .Where(p => !p.IsAI);
            
            // Add all players to enemy list
            foreach (IPlayer humanPlayer in humanPlayers)
            {
                newBot.BotsGroup.AddEnemy(humanPlayer, EBotEnemyCause.initial);
            }

            IEnumerable<BotOwner> activatedBots = Singleton<IBotGame>.Instance.BotsController.Bots.BotOwners
                .Where(b => b.BotState == EBotState.Active && b.Profile.Id != newBot.Profile.Id);

            foreach (BotOwner bot in activatedBots)
            {
                // Special cases like santa, zyrachiy and btr
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
    }
}