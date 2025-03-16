using System.Collections.Generic;
using Rocket.API;

namespace DarkerClear
{
    public class Config : IRocketPluginConfiguration
    {
        public bool AutoClear;
        public int CooldownClear;
        public List<Zone> Zones;
        public void LoadDefaults()
        {
            AutoClear = true;
            CooldownClear = 7200;
            Zones = new List<Zone>()
            {
                new Zone(new UnityEngine.Vector3(0f,0f,0f),50)
            };
        }
    }
}
