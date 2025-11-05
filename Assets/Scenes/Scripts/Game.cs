using System;
using UnityEngine;
using Grabby;

namespace Grabby
{
    [Serializable]
    public struct Game
    {
        public int seed;
        public int won;
        public LoadsCollection wonLoads;
        public int lose;
        public double nextGenerateAt;
        public GameScene scene;
    }
    [Serializable]
    public struct GameScene
    {
        public GameSceneLoad[] loads;
        public Vector3 ironPosition;
        public Vector3 ironRotation;
    }
    [Serializable]
    public struct GameSceneLoad
    {
        public string name;
        public int index;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }
}