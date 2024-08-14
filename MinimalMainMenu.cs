using System.Collections.Generic;
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
        GameObject mainMenu = GameObject.Find("MainMenu");
        if (mainMenu is null)
        {
            return;
        }
        
        Transform s1 = mainMenu.transform.Find("stage 1");
        Transform s2 = mainMenu.transform.Find("stage 2");

        if (s1 is null || s2 is null)
        {
            return;
        }

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
        
        int numExpansionItems = 0;
        VerticalLayoutGroup vx = expansion.gameObject.GetComponent<VerticalLayoutGroup>();
        if (vx is not null)
        {
            vx.spacing = -2.5f;
            vx.childAlignment = TextAnchor.MiddleCenter;
            foreach(Transform child in expansion)
            {
                if (child.gameObject.activeSelf)
                {
                    numExpansionItems++;
                }
            }
        }
        
        // Hide misc menu items
        SetActive(startGame, false);
        SetActive(community, false);
        SetActive(addons, false);
        SetActive(merch, false);
        SetActive(header, false);
        SetActive(buildMissions, false);
        SetActive(back, false);
        
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


        const float offset = 0.005f;
        
        var startT = startGame.transform.position;

        logo.position = new(logo.position.x, 0.1794f + offset, logo.position.z);
        
        int index = 0;
        foreach (var menuItem in menuItems)
        {
            menuItem.transform.rotation = Quaternion.Euler(Vector3.zero);
            menuItem.transform.position =  new(startT.x, offset + startT.y - (0.05f * index), startT.z);
            
            if (menuItem == expansion)
            {
                menuItem.transform.position = new(startT.x, menuItem.transform.position.y - (numExpansionItems / 2.0f * 0.05f) + 0.025f, startT.z);
                index += numExpansionItems;
            }
            else
            {
                index++;
            }
        }
        
        
    }

    private static void SetActive(Component t, bool b)
    {
        t?.gameObject.SetActive(b);
    }
}

[HarmonyPatch(typeof(MainMenuEnvLogic))]
[HarmonyPatch("Update")]
public class MainMenuNoAnimationPatch
{
    // ReSharper disable InconsistentNaming
    public static void Postfix(ref MainMenuEnvLogic __instance, ref Vector3 ___tgtPos, ref Quaternion ___tgtRot)
    {
        __instance.landscapeCamera.transform.position = ___tgtPos;
        __instance.landscapeCamera.transform.rotation = ___tgtRot;
    }
}