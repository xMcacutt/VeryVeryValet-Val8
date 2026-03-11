using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using TemplatePlugin;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace VeryVeryValet_Val8
{
    [BepInPlugin(TemplatePluginInfo.PLUGIN_GUID, TemplatePluginInfo.PLUGIN_NAME, Version)]
    public class PluginMain : BaseUnityPlugin
    {
        public const string GameName = TemplatePluginInfo.GAME_NAME;
        private const string Version = "1.0.0";

        private readonly Harmony _harmony = new Harmony(TemplatePluginInfo.PLUGIN_GUID);
        public static ManualLogSource? logger;

        void Awake()
        {
            logger = Logger;
            _harmony.PatchAll();
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += (scene, mode) => { };
        }

        [HarmonyPatch(typeof(ScreenUI_Title), nameof(ScreenUI_Title.OnPlay))]
        private static class ScreenUI_Title_Patch
        {
            private static void Postfix(DefaultButtonUi button, string name)
            {
                PlayerMgr.ActiveMaxPlayerCount = 8;
            }
        }

        [HarmonyPatch(typeof(ResultsVoteUi), nameof(ResultsVoteUi.Init))]
        private static class ResultsVoteUi_Patch
        {
            private static void Prefix(ResultsVoteUi __instance, ref List<int> playerIds)
            {
                if (playerIds.Count > 4)
                    playerIds.RemoveRange(4, playerIds.Count - 4);
                __instance._playerFlowCount = Math.Min(playerIds.Count, 4);
            }
        }

        [HarmonyPatch(typeof(ScreenUI_Results), nameof(ScreenUI_Results._setWin))]
        private static class ScreenUI_Results_Patch
        {
            private static void Postfix(
                ScreenUI_Results __instance,
                LevelData levelData,
                int strikeCount,
                bool isStrikes,
                string topKey,
                string bottomKey,
                Transform[] photoHandles,
                Dictionary<int, TitleData> playersToTitles,
                LevelController levelController,
                PlayerMgr playerMgr,
                PlayerSave.SaveLevelResult saveResult)
            {
                int count = __instance._valetFacesUis.Count;
                if (count <= 4)
                    return;
                for (int i = 4; i < count; i++)
                    Destroy(__instance._valetFacesUis[i]);
                __instance._valetFacesUis.RemoveRange(4, count - 4);
            }
        }
    }
}