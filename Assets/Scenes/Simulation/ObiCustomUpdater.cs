using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Obi;

public class ObiCustomUpdater : ObiUpdater
{
    /// <summary>
    /// Each FixedUpdate() call will be divided into several substeps. Performing more substeps will greatly improve the accuracy/convergence speed of the simulation. 
    /// Increasing the amount of substeps is more effective than increasing the amount of constraint iterations.
    /// </summary>
    [Tooltip("Amount of substeps performed per FixedUpdate. Increasing the amount of substeps greatly improves accuracy and convergence speed.")]
    public int substeps = 4;
    
    public void Step(float fixedDeltaTime) {
        ObiProfiler.EnableProfiler();

        PrepareFrame();

        BeginStep(fixedDeltaTime);

        float substepDelta = fixedDeltaTime / (float)substeps;

        // Divide the step into multiple smaller substeps:
        for (int i = 0; i < substeps; ++i)
            Substep(fixedDeltaTime, substepDelta, substeps-i);

        EndStep(substepDelta);

        ObiProfiler.DisableProfiler();
    }
}