using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEvaluationFunction : EvaluationFunction
{
    public override float evaluate(State s)
    {
        return ((float)(0.8 * HPBalance(s) + 0.2 * MoveVal(s)));
    }

    private float HPBalance(State s)
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
        if(s.isAttack){
            return 0.7f * (0.6f * (((player_hp - enemy_hp) / (float)(player_hp + enemy_hp) + 1) / 2) + 0.4f *(s.PlayersUnits.Count / (s.PlayersUnits.Count + s.AdversaryUnits.Count))) + 0.3f * (s.unitAttacked.attack/100);
        }
        return  0.6f * (((player_hp - enemy_hp) / (float)(player_hp + enemy_hp) + 1) / 2) + 0.4f *(s.PlayersUnits.Count / (s.PlayersUnits.Count + s.AdversaryUnits.Count));
    }

    public float SurroundingVal(State state)
    {
        float acum = 0;
        foreach (var enemyUnit in state.AdversaryUnits)
        {
            acum += 1 / (float)(enemyUnit.GetAttackable(state, state.PlayersUnits).Count + 1);
        }
        return acum / state.AdversaryUnits.Count;
    }

    public float AttackVal(State state)
    {
        float acum = 0;
        foreach (var playerUnit in state.PlayersUnits)
        {
            acum += (playerUnit.GetAttackable(state, state.AdversaryUnits).Count) / (float)state.AdversaryUnits.Count;
        }
        return acum / state.PlayersUnits.Count;
    }

    private float Dist(Unit a, Unit b)
    {
        return Mathf.Abs(b.x - a.x) + Mathf.Abs(b.y - a.y);
    }

    public float ProximityVal(State state)
    {
        float acum = 0;
        foreach (var playerUnit in state.PlayersUnits)
        {
            float acum2 = 0;
            foreach (var enemyUnit in state.AdversaryUnits)
            {
                //if(Mathf.Ceil((playerUnit.hp + playerUnit.hpbonus) / (float)(enemyUnit.attack + enemyUnit.attackbonus )) > Mathf.Ceil((enemyUnit.hp + enemyUnit.hpbonus) / (float)(playerUnit.attack + playerUnit.attackbonus )))
                
                acum2 += 1/(Dist(playerUnit, enemyUnit)+1);
            }
            acum += acum2 / state.AdversaryUnits.Count;
        }
        return acum / state.PlayersUnits.Count;
    }

    public float MoveVal(State state)
    {
        return (float)(0.60 * SurroundingVal(state) + 0.30 * AttackVal(state) + 0.05 * ProximityVal(state));
    }
}
