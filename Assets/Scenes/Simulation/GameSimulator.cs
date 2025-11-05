using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Grabby;

public struct GameSimulatorResponse
{
    public string error;
    public string[] simulatedWonLoads;
    public GameScene endScene;
}
public struct GameSimulatorGameScenesOnServer
{
    public GameScene[] scenes;
}

public class GameSimulator : MonoBehaviour
{
    [SerializeField] private GameObject machinePrefab;
    private MachineController controller;
    private List<GameScene> gameScenesOnServer = new List<GameScene>();

    private void Awake() {
        GameObject machine = Instantiate(machinePrefab);
        controller = machine.GetComponent<MachineController>();
        controller.SetSimulate(Physics.defaultPhysicsScene);
        Physics.simulationMode = SimulationMode.Script;
    }

    async public Task<GameSimulatorResponse> Simulate(APIMachineControlStateRequest request) {
        controller.loadsController.SetUp(request.startScene);
        for(int i = 0; i < 1000; i++) {
            Physics.Simulate(Time.fixedDeltaTime);
            controller.ropeUpdater.Step(Time.fixedDeltaTime);
            await Task.Yield();
        }
        foreach(MachineControlState state in request.value) {
            controller.Step();
            controller.SetState(state);
            Physics.Simulate(state.fixedDeltaTime);
            controller.ropeUpdater.Step(state.fixedDeltaTime);
            gameScenesOnServer.Add(controller.GetCurrentGameScene());
        }
        SaveGameScenesFile();
        GameSimulatorResponse response;
        response.error = null;
        response.simulatedWonLoads = controller.wonLoads.Select(wonLoad => wonLoad.Value.name).ToArray();
        response.endScene = controller.GetCurrentGameScene();
        controller.ClearStates();
        return response;
    }
	private void SaveGameScenesFile() {
        GameSimulatorGameScenesOnServer fileData;
        fileData.scenes = gameScenesOnServer.ToArray();
        File.WriteAllText(Application.persistentDataPath + "/gameScenes.json", JsonUtility.ToJson(fileData));
        Debug.Log($"Writed control state to {Application.persistentDataPath + "/gameScenes.json"}");
	}
}