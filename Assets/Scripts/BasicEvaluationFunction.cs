using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEvaluationFunction : EvaluationFunction
{
    public override float evaluate(State s)
    {
        return ((float)(0.95 * HPBalance(s) + 0.05 * MoveVal(s)) + 1 / (float)s.depth) / 2;
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

        return ((player_hp - enemy_hp) / (float)(player_hp + enemy_hp) + 1) / 2;
    }

    public float SurroundingVal(State state)
    {
        float acum = 0;
        foreach (var playerUnit in state.PlayersUnits)
        {
            float acum2 = 0;
            foreach (var enemyUnit in state.AdversaryUnits)
            {
                acum2 += enemyUnit.CanAttack(state, playerUnit) ? 1 : 0;
            }
            acum += 1 / (acum2 + 1);
        }
        return acum / state.PlayersUnits.Count;
    }

    public float AttackVal(State state)
    {
        float acum = 0;
        foreach (var playerUnit in state.PlayersUnits)
        {
            float acum2 = 0;
            foreach (var enemyUnit in state.AdversaryUnits)
            {
                acum2 += playerUnit.CanAttack(state, enemyUnit) ? 1 : 0;
            }
            acum += acum / state.AdversaryUnits.Count;
        }
        return acum / state.PlayersUnits.Count;
    }

    public float MoveVal(State state)
    {
        return (float)(0.5 * SurroundingVal(state) + 0.5 * AttackVal(state));
    }
}
