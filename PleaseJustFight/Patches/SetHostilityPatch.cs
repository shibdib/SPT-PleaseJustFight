using System.Reflection;
using Comfort.Common;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace PleaseJustFight.Patches
{
	public class SetHostilityPatch : ModulePatch
	{
		private static readonly FieldInfo WildSpawnTypeField = AccessTools.Field(typeof(BotDifficultySettingsClass), "wildSpawnType_0");
		
		protected override MethodBase GetTargetMethod()
		{
			return AccessTools.Method(typeof(BotDifficultySettingsClass), "IsPlayerEnemy");
		}

		[PatchPrefix]
		public bool PatchPrefix(IPlayer player)
		{
			WildSpawnType wildSpawnType = (WildSpawnType)WildSpawnTypeField.GetValue(this);
			
			AdditionalHostilitySettings[] additionalHostilitySettings = Singleton<IBotGame>.Instance.BotsController
				.BotLocationModifier.AdditionalHostilitySettings;
			AdditionalHostilitySettings additionalHostilitySettings2 = null;
			if (additionalHostilitySettings != null)
			{
				foreach (AdditionalHostilitySettings additionalHostilitySettings3 in additionalHostilitySettings)
				{
					if (additionalHostilitySettings3.BotRole == wildSpawnType)
					{
						additionalHostilitySettings2 = additionalHostilitySettings3;
						break;
					}
				}
			}

			switch (player.Side)
			{
				case EPlayerSide.Usec:
					if (additionalHostilitySettings2 != null)
					{
						if (additionalHostilitySettings2.UsecPlayerBehaviour.HasFlag(EWarnBehaviour.AlwaysFriends) ||
						    additionalHostilitySettings2.UsecPlayerBehaviour.HasFlag(EWarnBehaviour.Neutral) ||
						    additionalHostilitySettings2.UsecPlayerBehaviour.HasFlag(EWarnBehaviour.Warn))
						{
							return false;
						}
					}
					return true;
				case EPlayerSide.Bear:
					if (additionalHostilitySettings2 != null)
					{
						if (additionalHostilitySettings2.BearPlayerBehaviour.HasFlag(EWarnBehaviour.AlwaysFriends) ||
						    additionalHostilitySettings2.BearPlayerBehaviour.HasFlag(EWarnBehaviour.Neutral) ||
						    additionalHostilitySettings2.BearPlayerBehaviour.HasFlag(EWarnBehaviour.Warn))
						{
							return false;
						}
					}
					return true;
				case EPlayerSide.Savage:
					if (additionalHostilitySettings2 != null)
					{
						if (additionalHostilitySettings2.SavagePlayerBehaviour.HasFlag(EWarnBehaviour.AlwaysFriends) ||
						    additionalHostilitySettings2.SavagePlayerBehaviour.HasFlag(EWarnBehaviour.Neutral) ||
						    additionalHostilitySettings2.SavagePlayerBehaviour.HasFlag(EWarnBehaviour.Warn))
						{
							return false;
						}
					}
					return true;
				default:
					return false;
			}
		}
	}
}