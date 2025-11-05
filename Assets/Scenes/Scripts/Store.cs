using System;
using System.Reflection;
using Grabby;

namespace Grabby
{
    [Serializable]
    public struct Settings
    {
        public bool sounds;
        public bool music;
        public bool vibration;
        public bool hiddenInLeaders;
        public bool liveGame;
    }

    [Serializable]
    public struct User
    {
        public string _id;
        public string firstName;
        public string lastName;
        public string gender;
        public string locale;
        public int timezone;
        public Settings settings;
        public UserTraining training;
        public Authorization authorization;
        public UserGame game;
        public int coins;
        public int score;
        public int loadsCount;
        public LoadsCollection loadsCollection;
        public int lastRequest;
    }
    [Serializable]
    public struct UserTraining
    {
        public bool mainScene;
        public bool gameScene;
    }
    [Serializable]
    public struct UserGame
    {
        public int testTries;
        public int liveTries;
        public double nextFreeTestTriesAt;
        public Game test;
        public Game live;
    }

    public static class Store
    {
        public static Settings settings = new Settings();
        public static User user = new User();
        public static int currentGameTries => settings.liveGame ? user.game.liveTries : user.game.testTries;
        public static int currentGameSeed => settings.liveGame ? user.game.live.seed : user.game.test.seed;
    }
}