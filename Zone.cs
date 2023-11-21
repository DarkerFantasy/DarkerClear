using UnityEngine;

namespace DarkerClear
{
    public class Zone
    {
        public Vector3 Position;
        public int Radius;
        public Zone(Vector3 pos, int rad)
        {
            Position = pos;
            Radius = rad;
        }
        public Zone() { }
    }
}
