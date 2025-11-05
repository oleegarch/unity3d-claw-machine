using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Grabby;

using Random = UnityEngine.Random;

public class LoadsController : MonoBehaviour
{
    public static LoadsController currentController;
    
    [SerializeField] private MachineController machine;
    [SerializeField] private Loads loads;
    [SerializeField] private GameObject antiWinCollider;
    [SerializeField] private Transform[] spawnMarkers;
    [NonSerialized] public List<GameSceneLoad> gameSceneLoads = new List<GameSceneLoad>();
    [NonSerialized] public bool antiWinColliderShowed = false;
    private List<GameObject> prefabs = new List<GameObject>();

    private void Awake() {
        currentController = this;
    }

    private void Clear() {
        foreach(GameObject prefab in prefabs) {
            Destroy(prefab);
        }
        prefabs.Clear();
    }
    public void Generate() {
        Clear();
        StartCoroutine(ShowAntiWinCollider());

        int seed = Store.currentGameSeed;
        Random.InitState(seed);

        for(int i = 0; i < 48; i++) {
            float rand = Random.value;
            int index = (int)(loads.items.Length * rand);

            Load load = loads.items[index];
            Transform marker = spawnMarkers[i % spawnMarkers.Length];
            GameObject prefab = Instantiate(load.prefab, marker.position, marker.rotation, transform);
            prefab.GetComponent<LoadManager>().index = i;
            prefabs.Add(prefab);
        }
    }
    public void SetUp(GameScene scene) {
        Clear();
        foreach(GameSceneLoad gameSceneLoad in scene.loads) {
            Load load = Array.Find(loads.items, load => load.name == gameSceneLoad.name);
            GameObject prefab = Instantiate(load.prefab, gameSceneLoad.position, Quaternion.Euler(gameSceneLoad.rotation), transform);
            prefab.GetComponent<LoadManager>().index = gameSceneLoad.index;
            prefabs.Add(prefab);
        }
    }
    private IEnumerator ShowAntiWinCollider() {
        antiWinColliderShowed = true;
        antiWinCollider.SetActive(antiWinColliderShowed);
        yield return new WaitForSeconds(1f);
        antiWinColliderShowed = false;
        antiWinCollider.SetActive(antiWinColliderShowed);
    }
    public List<GameSceneLoad> SetGameSceneLoads() {
        gameSceneLoads.Clear();

        foreach(GameObject prefab in prefabs) {
            if(prefab == null) continue;

            LoadManager loadManager = prefab.GetComponent<LoadManager>();

            // на серверной части всё происходит с помощью синхронной мгновенной симуляции
            // и метод Destroy срабатывает в следующем update loop
            // поэтому я проверяю дополнительно что префаб удалён
            if(loadManager.isCaught == true || machine.wonLoads.ContainsKey(loadManager.index)) continue;

            GameSceneLoad gameSceneLoad;
            gameSceneLoad.name = loadManager.load.name;
            gameSceneLoad.index = loadManager.index;
            gameSceneLoad.position = prefab.transform.position;
            gameSceneLoad.rotation = prefab.transform.eulerAngles;
            gameSceneLoad.scale = prefab.transform.localScale;
            gameSceneLoads.Add(gameSceneLoad);
        }

        return gameSceneLoads;
    }
}