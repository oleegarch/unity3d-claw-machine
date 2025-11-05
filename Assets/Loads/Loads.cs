using UnityEngine;

namespace Grabby
{
    [CreateAssetMenu(fileName = "New Loads", menuName = "Grabby/Loads")]
    public class Loads : ScriptableObject
    {
        public Load[] items;
    }

    public struct LoadsCollection
    {
        public int turtle;
        public int shark;
        public int teddy;
        public int octopus;
    }
}