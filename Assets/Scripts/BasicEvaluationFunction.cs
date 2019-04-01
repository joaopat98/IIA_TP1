using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEvaluationFunction : EvaluationFunction
{
    public override float evaluate(State s)
    {
        float player_hp = 0;
        float enemy_hp = 0;
        foreach (Unit u in s.PlayersUnits)
        {
            player_hp += u.hp + u.hpbonus;
        }


        foreach (Unit u in s.AdversaryUnits)
        {
            enemy_hp += u.hp + u.hpbonus;
        }


        return (player_hp / Mathf.Max(1, enemy_hp) / s.depth);
    }
}
