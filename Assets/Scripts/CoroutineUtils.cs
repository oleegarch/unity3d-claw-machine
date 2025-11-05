using System;
using System.Collections;
using UnityEngine;

public static class CoroutineUtils {
    /// <summary>
    /// provides a util to easily control the timing of a lerp over a duration
    /// </summary>
    /// <param name="duration">How long our lerp will take</param>
    /// <param name="action">The action to perform per frame of the lerp, is given the progress t in [0,1]</param>
    /// <param name="curve">If we want out time curve to follow a specific animation curve</param>
    /// <returns></returns>
    public static IEnumerator Lerp(float duration, Action<float> action, AnimationCurve curve = null) {
        float time = 0;

        // by default we use a linear evaluation
        Func<float, float> tEval = t => t;

        // If a curve is provided follow the curve for our t evaluations instead
        if(curve != null) tEval = t => curve.Evaluate(t);
        while(time < duration) {
            float delta = Time.deltaTime;
            float t = (time + delta > duration) ? 1 : (time / duration);
            action(tEval(t));
            time += delta;
            yield return null;
        }
        action(tEval(1));
    }
}