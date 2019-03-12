using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEvaluationFunction : EvaluationFunction
{
    public override float evaluate(State s)
    {
        return s.PlayersUnits.Count - s.AdversaryUnits.Count;
    }
}
