using System.Collections.Generic;
using Rocket.API;

namespace DarkerClear
{
    public class Config : IRocketPluginConfiguration
    {
        public bool AutoClear;
        public int CooldownClear;
        public string Icone;
        public List<Zone> Zones;
        public void LoadDefaults()
        {
            AutoClear = true;
            CooldownClear = 7200;
            Icone = "https://images-ext-1.discordapp.net/external/ayom3YTguVrKAthIAxa5jHDGOZjuqugLGc1ZSuzupus/%3Fsize%3D1024/https/cdn.discordapp.com/icons/936729851304488980/00a57bc5f2c4a57804d5ac83bc41d8f8.png";
            Zones = new List<Zone>()
            {
                new Zone(new UnityEngine.Vector3(0f,0f,0f),50)
            };
        }
    }
}
