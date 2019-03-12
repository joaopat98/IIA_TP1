using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicUtilityFunction : UtilityFunction
{
    public override float evaluate(State s)
    {
        return s.PlayersUnits.Count - s.AdversaryUnits.Count;
    }
}
