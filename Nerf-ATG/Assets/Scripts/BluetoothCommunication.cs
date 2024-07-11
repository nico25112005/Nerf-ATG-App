using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Game;

public class BluetoothCommunication
{
    Player player = Player.GetInstance();

    public void OnDataReceived(object sender, string data)
    {
        // Processing data in a separate thread
        ThreadPool.QueueUserWorkItem(ProcessData, data);
    }

    void ProcessData(object state)
    {
        BluetoothManager threadBluetooth = BluetoothManager.GetInstance();
        string data = (string)state;

        try
        {
            if (data.Length != 4) throw new Exception("Wrong data sent");

            short hexData = Convert.ToInt16(data, 16);

            Debug.Log($"Data: {hexData:X}");
            if ((hexData & 0xFF00) != 0)
            {
                if (player.Health - (byte)(hexData >> 8) >= 0)
                {
                    MainThreadDispatcher.Execute(() => player.Health -= (byte)(hexData >> 8));
                }
                else throw new Exception("To less hp");
            }

            if ((hexData & 0x00F0) != 0)
            {
                if (player.Ammo - (byte)((hexData & 0x00F0) >> 4) >= 0)
                {
                    MainThreadDispatcher.Execute(() => player.Ammo -= (byte)((hexData & 0x00F0) >> 4));
                }
                else throw new Exception("To less ammo");
            }

            if ((hexData & 0x00F) != 0)
            {

                if (player.MaxAmmo - (Settings.weaponInfo[player.WeaponType].AmmoPerMag - player.Ammo) >= 0)
                {
                    MainThreadDispatcher.Execute(() =>
                    {
                        player.MaxAmmo -= (Settings.weaponInfo[player.WeaponType].AmmoPerMag - player.Ammo);
                        player.Ammo = Settings.weaponInfo[player.WeaponType].AmmoPerMag;
                    });
                }
                else
                {
                    MainThreadDispatcher.Execute(() =>
                    {
                        player.Ammo += (byte)player.MaxAmmo;
                        player.MaxAmmo = 0;
                    });
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e.StackTrace);
        }

        // Use MainThreadDispatcher to call WriteData on the main thread
        MainThreadDispatcher.Execute(() => threadBluetooth.WriteData("Received Data: " + data + "\n"));
    }
}
