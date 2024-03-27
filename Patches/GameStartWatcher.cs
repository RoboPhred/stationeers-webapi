
using Assets.Scripts;
using HarmonyLib;

namespace StationeersWebApi.Patches
{

    [HarmonyPatch(typeof(GameManager), "StartGame")]
    sealed class GameStartWatcher
    {
        static void Postfix()
        {
            StationeersWebApiPlugin.Instance.StartServer();
        }
    }
}