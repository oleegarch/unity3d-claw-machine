using System.Collections;
using UnityEngine;
using TMPro;
using Grabby;

public class TimerController : MonoBehaviour
{
    [SerializeField] private MachineController machine;
    [SerializeField] private float grabbyDuration = 40f;
    [SerializeField] private float releaseDuration = 30f;

    private TextMeshPro text;
    private Coroutine grabbyCoroutine;
    private Coroutine releaseCoroutine;

    private void Awake() {
        text = GetComponent<TextMeshPro>();
        ShowCurrentTries();
    }

    public void StartGrabby() {
        if(grabbyCoroutine == null) {
            Stop();
            grabbyCoroutine = StartCoroutine(StartGrabbyCoroutine());
        }
    }
    public void StartRelease() {
        if(releaseCoroutine == null) {
            Stop();
            releaseCoroutine = StartCoroutine(StartReleaseCoroutine());
        }
    }
    public void Stop() {
        if(grabbyCoroutine != null) {
            StopCoroutine(grabbyCoroutine);
            grabbyCoroutine = null;
        }
        if(releaseCoroutine != null) {
            StopCoroutine(releaseCoroutine);
            releaseCoroutine = null;
        }
    }

    public void ShowCurrentTries() {
        Stop();
        text.SetText($"{FillZero(Store.currentGameTries)}");
    }

    private IEnumerator StartGrabbyCoroutine() {
        yield return StartTimer(grabbyDuration);
        grabbyCoroutine = null;
        yield return machine.Grab();
    }
    private IEnumerator StartReleaseCoroutine() {
        yield return StartTimer(releaseDuration);
        releaseCoroutine = null;
        yield return machine.Release();
    }

    private IEnumerator StartTimer(float duration) {
        for(float spent = 0f; spent < duration; spent += Time.deltaTime) {
            text.SetText(FillZero(Mathf.Ceil(duration - spent)));
            yield return null;
        }
        text.SetText("00");
        yield break;
    }

    static public string FillZero(float num, float digits = 2) {
        string str = num.ToString();
        if(str.Length >= digits) {
            return str;
        }
        while(str.Length != digits) {
            str = "0" + str;
        }
        return str;
    }
}