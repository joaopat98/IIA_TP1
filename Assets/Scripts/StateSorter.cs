using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSorter : IComparer<State>
{
    private EvaluationFunction func;
    public StateSorter(EvaluationFunction func)
    {
        this.func = func;
    }
    public int Compare(State x, State y)
    {
        float e1 = func.evaluate(x);
        float e2 = func.evaluate(y);
        if (e1 > e2)
        {
            return 1;
        }
        else if (e1 < e2)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}
