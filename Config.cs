using System.Collections.Generic;
using Rocket.API;

namespace DarkerClear
{
    public class Config : IRocketPluginConfiguration
    {
        public bool AutoClear;
        public bool AutoClearI;
        public bool AutoClearV;
        public int CooldownClear;
        public List<Zone> Zones;
        public void LoadDefaults()
        {
            AutoClear = true;
            AutoClearI = true; 
            AutoClearV = true;
            CooldownClear = 7200;
            Zones = new List<Zone>();
        }
    }
}
