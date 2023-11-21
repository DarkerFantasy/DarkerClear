using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Core.Plugins;
using Rocket.Core.Commands;
using SDG.Unturned;
using Rocket.Unturned.Player;
using UnityEngine;
using Rocket.API;

namespace DarkerClear
{
    public class Plugin : RocketPlugin<Config>
    {
        public static Plugin Instance;
        public DateTime LastClear;
        public bool Said;
        protected override void Load()
        {
            Instance = this;
            LastClear = DateTime.Now;
            Said = false;
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
                bool flag3 = Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out RaycastHit raycastHit, 4f, 201326592);
                if (flag3)
                {
                    //Получаем машину, из того на что смотрит игрок
                    InteractableVehicle iv = DamageTool.getVehicle(raycastHit.transform);
                    if(iv != null)
                    {
                        VehicleManager.askVehicleDestroy(iv);
                        ChatManager.serverSendMessage($"Машина {iv.asset.name}(ID:{iv.asset.id}) удалена!", Color.yellow, null, player.SteamPlayer(),EChatMode.LOCAL, Configuration.Instance.Icone);
                    }
                    else
                    {
                        ChatManager.serverSendMessage("Вы не смотрите на машину!", Color.red, null, player.SteamPlayer(), EChatMode.LOCAL, Configuration.Instance.Icone);
                    }
                }
                else
                {
                    ChatManager.serverSendMessage("Вы не смотрите на машину!", Color.red, null, player.SteamPlayer(), EChatMode.LOCAL, Configuration.Instance.Icone);

                }
                return;
            }
            if (int.TryParse(command[0], out int radius))
            {
                List<InteractableVehicle> vehicles = VehicleManager.vehicles.Where(p => Vector3.Distance(p.transform.position,player.Position) <= radius).ToList();     //Получаем машины в радиусе
                if(vehicles.IsEmpty())
                {
                    ChatManager.serverSendMessage("Машины не найдены!", Color.red, null, player.SteamPlayer(), EChatMode.LOCAL, Configuration.Instance.Icone);
                    return;
                }
                foreach(var p in vehicles) 
                {
                    VehicleManager.askVehicleDestroy(p);
                }
                ChatManager.serverSendMessage("Машина удалены!", Color.yellow, null, player.SteamPlayer(), EChatMode.LOCAL, Configuration.Instance.Icone);
            }
            else
            {
                ChatManager.serverSendMessage("/dv (radius) для удаления машин.", Color.red, null, player.SteamPlayer(), EChatMode.LOCAL, Configuration.Instance.Icone);
            }
        }











        public void ClearMap(bool vehicle,bool item)
        {
            LastClear = DateTime.Now;
            Said = false;
            if(vehicle)
            {
                int count = 0;
                foreach (var v in VehicleManager.vehicles)     //Поиск в машинах
                {
                    if (!v.isInsideSafezone)
                    {
                        if (!v.anySeatsOccupied)
                        {
                            try
                            {
                                if (!Configuration.Instance.Zones.Exists(p => Vector3.Distance(p.Position, ((UnityEngine.Component)v).transform.position) <= p.Radius))
                                {
                                    VehicleManager.askVehicleDestroy(v);
                                    count++;
                                }
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine("Ошибочка в DarkerClear.ClearMap:\n" + e);
                            }
                        }
                    }
                }
                foreach (var p in Provider.clients)
                {
                    ChatManager.serverSendMessage("Было удалено " + count + " машин!", Color.green, null, p, EChatMode.LOCAL, Configuration.Instance.Icone);
                }
                Console.WriteLine("Было удалено " + count + " машин!");
            }
            if (item)
            {
                ItemManager.askClearAllItems();
                foreach (var p in Provider.clients)
                {
                    ChatManager.serverSendMessage("Все предметы были удалены!", Color.green, null, p, EChatMode.LOCAL, Configuration.Instance.Icone);
                }
                LastClear = DateTime.Now;
                Said = false;
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
                ChatManager.serverSendMessage("/createzone Radius", Color.green, null, player.SteamPlayer(), EChatMode.LOCAL, Configuration.Instance.Icone);
                return;
            }
            if (!int.TryParse(command[0], out int radius))
            {
                ChatManager.serverSendMessage("/createzone Radius", Color.green, null, player.SteamPlayer(), EChatMode.LOCAL, Configuration.Instance.Icone);
                return;
            }
            Configuration.Instance.Zones.Add(new Zone(player.Position, radius));
            Configuration.Save();
        }




        public void FixedUpdate()
        {
            if((DateTime.Now - LastClear).TotalSeconds >= Configuration.Instance.CooldownClear)
            {
                LastClear = DateTime.Now;
                Said = false;
                ClearMap(true,true);
            }
            if (!Said)
            {
                if ((DateTime.Now - LastClear).TotalSeconds >= (Configuration.Instance.CooldownClear - 60))
                {
                    Said = true;
                    foreach (var p in Provider.clients)
                    {
                        ChatManager.serverSendMessage("До очистки машин и предметов 1 минута.", Color.green, null, p, EChatMode.LOCAL, Configuration.Instance.Icone);
                    }
                }
            }
        }
        protected override void Unload()
        {
            
        }

    }
}