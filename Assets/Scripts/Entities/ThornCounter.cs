using System;
using System.Collections;
using UnityEngine;

public class ThornCounter : MonoBehaviour
{
    Coroutine counter;
    public static Action onCountChange;

    private void Start()
    {
        onCountChange = null;
        counter = StartCoroutine(Counter());
    }

    private IEnumerator Counter()
    {
        while (enabled)
        {
            Thorn.currentStep = ((Thorn.currentStep) % Thorn.stepCount)+1;
            onCountChange?.Invoke();
            yield return new WaitForSeconds(1);
        }
    }
}
