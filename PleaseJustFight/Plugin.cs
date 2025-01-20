using BepInEx;

namespace PleaseJustFight
{
    [BepInPlugin("Shibdib.PleaseJustFight", "PleaseJustFight", "1.0.0")]
    [BepInDependency("com.SPT.custom", "3.10.0")]
    public class PleaseJustFight : BaseUnityPlugin
    {
        private void Awake()
        {
            new Patches.SetHostilityPatch().Enable();
        }
    }
}