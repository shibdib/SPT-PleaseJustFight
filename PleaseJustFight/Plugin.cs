using BepInEx;

namespace PleaseJustFight
{
    [BepInPlugin("com.shibdib.pjf", "PleaseJustFight", "1.1.0")]
    [BepInDependency("com.SPT.custom", "3.11.0")]
    public class PleaseJustFight : BaseUnityPlugin
    {
        private void Awake()
        {
            new Patches.SetHostilityPatch().Enable();
        }
    }
}