using System.Linq;
using System.Reflection;
using Comfort.Common;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace PleaseJustFight.Patches;

public class SetHostilityPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(BotOwner), nameof(BotOwner.method_10));
    }

    [PatchPostfix]
    private static void PatchPostfix(BotOwner __instance)
    {
        var role = __instance.Profile.Info?.Settings?.Role;
        if (role is null) {
            return;
        }

        switch (role) {
            // pmc
            case WildSpawnType.pmcBEAR:
            case WildSpawnType.pmcUSEC:
                SetAllAsEnemies(__instance);
                break;
            case WildSpawnType.assault:
            case WildSpawnType.assaultGroup:
            case WildSpawnType.cursedAssault:
            case WildSpawnType.crazyAssaultEvent:
            case WildSpawnType.marksman:
            // golden TT
            case WildSpawnType.bossBully:
            // mall dude
            case WildSpawnType.bossKilla:
            // streets couple
            case WildSpawnType.bossBoar:
            case WildSpawnType.followerBoar:
            case WildSpawnType.bossBoarSniper:
            case WildSpawnType.followerBoarClose1:
            case WildSpawnType.followerBoarClose2:
            case WildSpawnType.bossKolontay:
            case WildSpawnType.followerKolontayAssault:
            case WildSpawnType.followerKolontaySecurity:
            // SVDS camper
            case WildSpawnType.bossKojaniy:
            case WildSpawnType.followerKojaniy:
            // boy gang leader
            case WildSpawnType.bossGluhar:
            case WildSpawnType.followerGluharAssault:
            case WildSpawnType.followerGluharSecurity:
            case WildSpawnType.followerGluharScout:
            case WildSpawnType.followerGluharSnipe:
            // orange bags
            case WildSpawnType.bossSanitar:
            case WildSpawnType.followerSanitar:
            // orange bag wannabe
            case WildSpawnType.bossPartisan:
            // rogues
            case WildSpawnType.exUsec:
            // men in black
            case WildSpawnType.sectantWarrior:
            case WildSpawnType.sectantPriest:
            case WildSpawnType.sectantPredvestnik:
            case WildSpawnType.sectantPrizrak:
            case WildSpawnType.sectantOni:
            // hammer guy
            case WildSpawnType.bossTagilla:
            case WildSpawnType.followerTagilla:
            // threesome
            case WildSpawnType.bossKnight:
            case WildSpawnType.followerBigPipe:
            case WildSpawnType.followerBirdEye:
            // zombies
            case WildSpawnType.infectedAssault:
            case WildSpawnType.infectedPmc:
            case WildSpawnType.infectedCivil:
            case WildSpawnType.infectedLaborant:
            case WildSpawnType.infectedTagilla:
                SetPmcAsEnemies(__instance);
                break;
        }
    }

    // Set pmc as hostile towards all other bots with some boss exceptions
    private static void SetAllAsEnemies(BotOwner newBot)
    {
        var humanPlayers = Singleton<GameWorld>.Instance.AllAlivePlayersList
            .Where(p => !p.IsAI);
        foreach (var humanPlayer in humanPlayers) {
            newBot.BotsGroup.AddEnemy(humanPlayer, EBotEnemyCause.initial);
        }

        var activatedBots = Singleton<IBotGame>.Instance.BotsController.Bots.BotOwners
            .Where(b => b.BotState == EBotState.Active && b.Profile.Id != newBot.Profile.Id);
        foreach (var bot in activatedBots) {
            switch (bot.Profile.Info?.Settings?.Role) {
                case WildSpawnType.bossZryachiy:
                case WildSpawnType.followerZryachiy:
                case WildSpawnType.peacefullZryachiyEvent:
                case WildSpawnType.shooterBTR:
                case WildSpawnType.gifter:
                case null:
                    continue;
            }

            bot.BotsGroup.AddEnemy(newBot, EBotEnemyCause.initial);
            newBot.BotsGroup.AddEnemy(bot, EBotEnemyCause.initial);
        }
    }

    // Set scavs and goons as hostile towards all pmc's
    private static void SetPmcAsEnemies(BotOwner newBot)
    {
        var humanPlayers = Singleton<GameWorld>.Instance.AllAlivePlayersList
            .Where(p => !p.IsAI);
        foreach (var humanPlayer in humanPlayers) {
            // scavs run check
            if (humanPlayer.Profile.Info?.Side is EPlayerSide.Usec or EPlayerSide.Bear) {
                newBot.BotsGroup.AddEnemy(humanPlayer, EBotEnemyCause.initial);
            }
        }

        var activatedBots = Singleton<IBotGame>.Instance.BotsController.Bots.BotOwners
            .Where(b => b.BotState == EBotState.Active);
        foreach (var bot in activatedBots) {
            var role = bot.Profile.Info?.Settings?.Role;
            if (role is not (WildSpawnType.pmcUSEC or WildSpawnType.pmcBEAR)) {
                continue;
            }

            bot.BotsGroup.AddEnemy(newBot, EBotEnemyCause.initial);
            newBot.BotsGroup.AddEnemy(bot, EBotEnemyCause.initial);
        }
    }
}