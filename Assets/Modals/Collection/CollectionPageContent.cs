using UnityEngine;
using Grabby;

public class CollectionPageContent : MonoBehaviour
{
    [SerializeField] private Loads loads;
    [SerializeField] private GameObject card;

    private void Awake() {
        foreach(Load load in loads.items) {
            GameObject cardGO = Instantiate(card, transform);
            cardGO.GetComponent<CollectionPageCard>().SetUp(load);
        }
    }
}