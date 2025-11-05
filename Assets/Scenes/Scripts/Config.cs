using UnityEngine;

namespace Grabby
{
    public static class Config
    {
        public static string SCHEME = "https://";
        public static string DOMAIN => Debug.isDebugBuild ? "grabbytest.oleegarch.com" : "oleegarch.com";
        public static string APP_ROUTE = "/grabby";
        public static string API_ROUTE => APP_ROUTE + "/api";

        public static string BASE_URL => SCHEME + DOMAIN;
        public static string APP_URL => SCHEME + DOMAIN + APP_ROUTE;
        public static string API_URL => SCHEME + DOMAIN + API_ROUTE;
    }
}