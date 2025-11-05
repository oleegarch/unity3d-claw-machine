using UnityEngine;
using Grabby;

public class MachineDesigner : MonoBehaviour
{
    [SerializeField] private MeshRenderer machineRenderer;
    [SerializeField] private Material metalBlueMaterial;
    [SerializeField] private Material metalBlueGreyMaterial;

    private void Awake() {
        SetMachineColor();
    }

    public void SetMachineColor() {
        Material[] materials = machineRenderer.materials;

        if(Store.settings.liveGame) {
            materials[0] = metalBlueMaterial;
        }
        else {
            materials[0] = metalBlueGreyMaterial;
        }

        machineRenderer.materials = materials;
    }
}