using Assets.Scripts;
using Game;
using Game.Enums;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagerSkript : MonoBehaviour
{
     Player player;
    private void Start()
    {
        player = Player.GetInstance();
    }
    public void LoadNextScene()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Menu":
                //player.TeamInfo = (Team)Enum.Parse(typeof(Team), transform.Find("Canvas").Find("Dropdown").GetComponent<Dropdown>().options[transform.Find("Canvas").Find("Dropdown").GetComponent<Dropdown>().value].text);
                
                if (player.PlayerName == "")
                    return;

                player.PlayerName = transform.Find("Canvas").Find("InputName").GetComponent<InputField>().text;

                Debug.LogWarning(player.BlasterMacAdress);
                break;

            case "Weapons":
                if (player.WeaponType == WeaponType.None) return;
                break;

            case "Upgrades":
                player.ApplyUpgrades();
                break;

            default:
                break;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        player.DestroyInstance();
        SceneManager.LoadScene(0);
    }
}
