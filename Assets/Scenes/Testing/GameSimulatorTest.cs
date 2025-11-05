using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Grabby;

public class GameSimulatorTest : MonoBehaviour
{
    [SerializeField] private GameObject machinePrefab;
    private MachineController controller;

    private void Awake() {
        GameObject machine = Instantiate(machinePrefab);
        controller = machine.GetComponent<MachineController>();
        controller.SetSimulate(Physics.defaultPhysicsScene);
        Physics.simulationMode = SimulationMode.Script;
    }

    private void Start() {
        // string path = "./Assets/Scenes/Testing/controlState.json";
        // Simulate(File.ReadAllText(path));
        
        string path = "./Assets/Scenes/Testing/gameScenes.json";
        StartCoroutine(SimulateGameScenes(File.ReadAllText(path)));
    }

    async public Task<GameSimulatorResponse> Simulate(string json) {
        APIMachineControlStateRequest request = JsonUtility.FromJson<APIMachineControlStateRequest>(json);
        controller.loadsController.SetUp(request.startScene);
        for(int i = 0; i < 1000; i++) {
            Physics.Simulate(0.02f);
            controller.ropeUpdater.Step(0.02f);
            await Task.Yield();
        }
        foreach(MachineControlState state in request.value) {
            controller.Step();
            controller.SetState(state);
            Physics.Simulate(state.fixedDeltaTime);
            controller.ropeUpdater.Step(state.fixedDeltaTime);
        }
        GameSimulatorResponse response;
        response.error = null;
        response.simulatedWonLoads = controller.wonLoads.Select(wonLoad => wonLoad.Value.name).ToArray();
        response.endScene = controller.GetCurrentGameScene();;
        return response;
    }

    public IEnumerator SimulateGameScenes(string json) {
        GameSimulatorGameScenesOnServer gameScenes = JsonUtility.FromJson<GameSimulatorGameScenesOnServer>(json);
        foreach(GameScene scene in gameScenes.scenes) {
            controller.SetCurrentGameScene(scene);
            yield return new WaitForFixedUpdate();
        }
    }
}