using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TentaclesCollider : MonoBehaviour
{
    [SerializeField] private MachineController machine;
    [SerializeField] private string toysLayerName = "Toy";
    private int toysLayer;
    private List<Rigidbody> collidingToys = new List<Rigidbody>();

    private void Awake() {
        toysLayer = LayerMask.NameToLayer(toysLayerName);
    }
    async private Task OnTriggerExit(Collider other) {
        if(
            collidingToys.Contains(other.attachedRigidbody) &&
            other.transform.gameObject.layer == toysLayer
        ) {
            collidingToys.Remove(other.attachedRigidbody);

            other.attachedRigidbody.transform.parent = null;

            if(collidingToys.Count == 0) {
                await machine.OnToyDropped();
            }
        }
    }
    public void SetToysColliding(Collider[] toys) {
        foreach(Collider toy in toys) {
            if(toy != null) {
                toy.attachedRigidbody.transform.parent = transform;
                if(!collidingToys.Contains(toy.attachedRigidbody)) {
                    collidingToys.Add(toy.attachedRigidbody);
                }
            }
        }
    }
}