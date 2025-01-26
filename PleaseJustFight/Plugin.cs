using BepInEx;

namespace PleaseJustFight
{
    [BepInPlugin("com.shibdib.pjf", "PleaseJustFight", "1.0.4")]
    [BepInDependency("com.SPT.custom", "3.10.0")]
    public class PleaseJustFight : BaseUnityPlugin
    {
        private void Awake()
        {
            new Patches.SetHostilityPatch().Enable();
        }
    }
}