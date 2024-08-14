using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace MinimalMainMenu;

[KSPAddon(KSPAddon.Startup.MainMenu, true)]
public class NoMainMenuAnimations : MonoBehaviour
{
    public void Start()
    {
        Debug.Log("NoMainMenuAnimations: Patching...");

        Harmony harmony = new Harmony("com.coldrifting.NoMainMenuAnimations");
        harmony.PatchAll();

        Debug.Log("NoMainMenuAnimations: Patched");
    }
}


[KSPAddon(KSPAddon.Startup.MainMenu, false)]
public class MinimalMainMenu : MonoBehaviour
{
    public void Start()
    {
        const float offset = 0.005f;
        
        GameObject mainMenu = GameObject.Find("MainMenu");
        Transform s1 = mainMenu.transform.Find("stage 1");
        Transform s2 = mainMenu.transform.Find("stage 2");

        s2.position = s1.position;
        s2.rotation = s1.rotation;

        Transform logo = s1.Find("logo");
        Transform startGame = s1.Find("Start Game");
        Transform settings = s1.Find("Settings");
        Transform community = s1.Find("Community");
        Transform addons = s1.Find("Addons");
        Transform credits = s1.Find("Credits");
        Transform merch = s1.Find("Merch");
        Transform quit = s1.Find("Quit");
        
        Transform header = s2.Find("Header");
        Transform newGame = s2.Find("New Game");
        Transform continueGame = s2.Find("Continue Game");
        Transform training = s2.Find("Training");
        Transform scenarios = s2.Find("Scenarios");

        Transform expansion = s2.Find("ExpansionDependentOptions");
        Transform buildMissions = s2.Find("ExpansionDependentOptions/MissionBuilder");
        Transform back = s2.Find("Back");
        
        var vx = expansion.gameObject.GetComponent<VerticalLayoutGroup>();
        vx.spacing = -2.125f;
        vx.childAlignment = TextAnchor.MiddleCenter;
        int numExpansionItems = 0;
        foreach(Transform child in expansion){
            if (child.gameObject.activeSelf)
            {
                numExpansionItems++;
            }
        }
        
        // Hide misc menu items
        startGame.gameObject.SetActive(false);
        community.gameObject.SetActive(false);
        addons.gameObject.SetActive(false);
        merch.gameObject.SetActive(false);
        header.gameObject.SetActive(false);
        buildMissions.gameObject.SetActive(false);
        back.gameObject.SetActive(false);
        
        List<Transform> menuItems =
        [
            newGame,
            continueGame,
            training,

            training,
            scenarios,
            expansion,

            settings,

            settings,
            credits,
            quit
        ];


        var startT = startGame.transform.position;

        logo.position = new(logo.position.x, 0.1794f + offset, logo.position.z);
        
        int index = 0;
        foreach (var t1 in menuItems)
        {
            t1.transform.rotation = Quaternion.Euler(Vector3.zero);
            t1.transform.position =  new(startT.x, offset + startT.y - (0.05f * index), startT.z);
            
            if (t1 == expansion)
            {
                t1.transform.position = new(startT.x, t1.transform.position.y - (numExpansionItems / 2.0f * 0.05f) + 0.025f, startT.z);
                index += numExpansionItems;
            }
            else
            {
                index++;
            }
        }
        
        
    }
}

[HarmonyPatch(typeof(MainMenuEnvLogic))]
[HarmonyPatch("Update")]
public class MainMenuNoAnimationPatch
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static void Postfix(ref MainMenuEnvLogic __instance, ref Vector3 ___tgtPos, ref Quaternion ___tgtRot)
    {
        __instance.landscapeCamera.transform.position = ___tgtPos;
        __instance.landscapeCamera.transform.rotation = ___tgtRot;
    }
}