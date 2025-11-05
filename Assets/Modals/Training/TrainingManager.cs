using System;
using UnityEngine;
using UnityEngine.Events;

public class TrainingManager : MonoBehaviour
{
    public static TrainingManager currentTraining;

    [SerializeField] private Transform spawnAt;
    [SerializeField] private GameObject[] slides;
    public UnityEvent onShowed = new UnityEvent();

    private int currentSlideIndex;
    private GameObject currentSlide;

    public void ShowTraining(int slideIndex = 0) {
        if(currentSlide != null || currentTraining != null) {
            throw new Exception("Training already showing");
        }
        currentSlideIndex = slideIndex;
        currentSlide = Instantiate(slides[slideIndex], spawnAt);
        currentTraining = this;
    }
    public void NextSlide() {
        if(currentSlideIndex + 1 < slides.Length) {
            Destroy(currentSlide);
            currentSlideIndex++;
            currentSlide = Instantiate(slides[currentSlideIndex], spawnAt);
        }
        else {
            Skip();
        }
    }
    public void Skip() {
        Destroy(currentSlide);
        currentSlideIndex = 0;
        currentTraining = null;
        onShowed.Invoke();
    }
}