using UnityEngine;

public class TrainingHandler : MonoBehaviour
{
    public void NextSlide() {
        TrainingManager.currentTraining.NextSlide();
    }
    public void Skip() {
        TrainingManager.currentTraining.Skip();
    }
}