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
     BluetoothManager bluetooth;
    private void Start()
    {
        player = Player.GetInstance();
        bluetooth = BluetoothManager.GetInstance();
    }
    public void LoadNextScene()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                player.TeamInfo = (Team)Enum.Parse(typeof(Team), transform.Find("Canvas").Find("Dropdown").GetComponent<Dropdown>().options[transform.Find("Canvas").Find("Dropdown").GetComponent<Dropdown>().value].text);
                break;

            case 1:
                if (player.WeaponType == WeaponType.None) return;
                break;

            case 2:
                bluetooth.WriteData(player.ToString() + "\n\n");
                Settings.Health = player.Health;
                Settings.Healing += (byte)(player.Upgrades[UpgradeType.Healing] * 2);
                break;

            default:
                break;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        bluetooth.StopConnection();
        player.ResetInstance();
        SceneManager.LoadScene(0);
    }
}
