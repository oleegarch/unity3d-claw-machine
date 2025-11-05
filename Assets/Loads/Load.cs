using UnityEngine;

namespace Grabby
{
    [CreateAssetMenu(fileName = "New Load", menuName = "Grabby/Load")]
    public class Load : ScriptableObject
    {
        public new string name;
        public string title;
        public string winDescription;
        public GameObject prefab;
        public GameObject prefabPreview;
        public RenderTexture renderTexture;
    }
}