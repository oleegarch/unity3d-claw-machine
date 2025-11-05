using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Grabby;
using Grabby.UI;

public class CaughtLoadDialog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private CaughtLoadPreview previewer;
    private Modal modal;
    private Load currentLoad;
    private List<Load> nextLoads = new List<Load>();
    private Action onEnded;
    
    private void Awake() {
        modal = GetComponent<Modal>();
    }

    public void SetLoad(Load load) {
        if(currentLoad != null) {
            nextLoads.Add(load);
            return;
        }

        currentLoad = load;
        header.SetText(load.title);
        description.SetText(load.winDescription);
        previewer.SetLoad(load);
    }

    public void SetOnEnded(Action callback) {
        onEnded = callback;
    }

    public void OnClosed() {
        currentLoad = null;
        previewer.ClearLoad();
        if(nextLoads.Count > 0) {
            modal.Open();
            Load nextLoad = nextLoads[nextLoads.Count - 1];
            nextLoads.RemoveAt(nextLoads.Count - 1);
            SetLoad(nextLoad);
        }
        else {
            onEnded?.Invoke();
        }
    }
}