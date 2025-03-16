using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core.Plugins;
using Rocket.Core.Commands;
using SDG.Unturned;
using Rocket.Unturned.Player;
using UnityEngine;
using Rocket.API;
using Steamworks;
using Rocket.Core;

namespace DarkerClear
{
    public class Plugin : RocketPlugin<Config>
    {
        public static Plugin Instance;
        public float LastClear;
        protected override void Load()
        {
            Instance = this;
            LastClear = Time.realtimeSinceStartup + Configuration.Instance.CooldownClear;
            Rocket.Core.Logging.Logger.Log("#####   ####  #####  ##  ## ##### ##### ");
            Rocket.Core.Logging.Logger.Log("##  ## ##  ## ##  ## ## ##  ##    ##  ##");
            Rocket.Core.Logging.Logger.Log("##  ## ###### #####  ####   ####  ##### ");
            Rocket.Core.Logging.Logger.Log("##  ## ##  ## ##  ## ## ##  ##    ##  ##");
            Rocket.Core.Logging.Logger.Log("#####  ##  ## ##  ## ##  ## ##### ##  ##");
        }

        [RocketCommand("dv", "")]
        public void DeleteVehicle(IRocketPlayer caller, string[] command)
        {
            if (caller is ConsolePlayer)
            {
                return;
            }
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length < 1)
            {
                //Получаем то, на что смотрит игрок
                if (Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out RaycastHit raycastHit, 4f, 201326592))
                {
                    //Получаем машину, из того на что смотрит игрок
                    InteractableVehicle iv = DamageTool.getVehicle(raycastHit.transform);
                    if(iv != null)
                    {
                        VehicleManager.askVehicleDestroy(iv);
                        ChatManager.serverSendMessage($"Машина {iv.asset.name}(ID:{iv.asset.id}) удалена!", Color.yellow, null, player.SteamPlayer(),EChatMode.LOCAL);
                    }
                    else
                    {
                        ChatManager.serverSendMessage("Вы не смотрите на машину!", Color.red, null, player.SteamPlayer(), EChatMode.LOCAL);
                    }
                }
                else
                {
                    ChatManager.serverSendMessage("Вы не смотрите на машину!", Color.red, null, player.SteamPlayer(), EChatMode.LOCAL);
                }
                return;
            }
            if (int.TryParse(command[0], out int radius))
            {
                List<InteractableVehicle> vehicles = VehicleManager.vehicles.Where(p => Vector3.Distance(p.transform.position,player.Position) <= radius).ToList();     //Получаем машины в радиусе
                if(vehicles.IsEmpty())
                {
                    ChatManager.serverSendMessage("Машины не найдены!", Color.red, null, player.SteamPlayer(), EChatMode.LOCAL);
                    return;
                }
                foreach(var p in vehicles) 
                {
                    VehicleManager.askVehicleDestroy(p);
                }
                ChatManager.serverSendMessage("Машина удалены!", Color.yellow, null, player.SteamPlayer(), EChatMode.LOCAL);
            }
            else
            {
                ChatManager.serverSendMessage("/dv (radius) для удаления машин.", Color.red, null, player.SteamPlayer(), EChatMode.LOCAL);
            }
        }











        public void ClearMap(bool vehicle,bool item)
        {
            LastClear = Time.realtimeSinceStartup + Configuration.Instance.CooldownClear;
            if(vehicle)
            {
                int count = 0;
                List<InteractableVehicle> array = VehicleManager.vehicles.Where(pz => pz != null && !Configuration.Instance.Zones.Exists(p => Vector3.Distance(p.Position, pz.transform.position) <= p.Radius) &&
                    !pz.isInsideSafezone && !pz.anySeatsOccupied).ToList();
                foreach (var v in array)     //Поиск в машинах
                {
                    try
                    {
                        VehicleManager.askVehicleDestroy(v);
                        count++;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in DarkerClear.ClearMap:\n" + e);
                    }
                }
                ChatManager.serverSendMessage("Было удалено " + count + " машин!", Color.green, null, null, EChatMode.GLOBAL);
                Console.WriteLine("Было удалено " + count + " машин!");
            }
            if (item)
            {
                ItemManager.askClearAllItems();
                foreach (var p in Provider.clients)
                {
                    ChatManager.serverSendMessage("Все предметы были удалены!", Color.green, null, p, EChatMode.LOCAL);
                }
            }
        }











        [RocketCommand("clear", "")]
        public void Clear(IRocketPlayer caller, string[] command)
        {
            if(command.Length < 1)
            {
                ClearMap(true,true);
            }
            if (command[0] == "vehicles" || command[0] == "v") 
            {
                ClearMap(true, false);
            }
            if (command[0] == "items" || command[0] == "i")
            {
                ClearMap(false, true);
            }
        }




        [RocketCommand("createzone", "")]
        public void CreateZone(IRocketPlayer caller, string[] command)
        {
            if(caller is ConsolePlayer)
            {
                return;
            }
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length != 1)
            {
                ChatManager.serverSendMessage("/createzone Radius", Color.green, null, player.SteamPlayer(), EChatMode.LOCAL);
                return;
            }
            if (!int.TryParse(command[0], out int radius))
            {
                ChatManager.serverSendMessage("/createzone Radius", Color.green, null, player.SteamPlayer(), EChatMode.LOCAL);
                return;
            }
            Configuration.Instance.Zones.Add(new Zone(player.Position, radius));
            Configuration.Save();
        }




        public void FixedUpdate()
        {
            if(Time.realtimeSinceStartup > LastClear)
            {
                LastClear = Time.realtimeSinceStartup + Configuration.Instance.CooldownClear;
                ClearMap(true,true);
            }
        }
        protected override void Unload()
        {
            
        }

    }
}