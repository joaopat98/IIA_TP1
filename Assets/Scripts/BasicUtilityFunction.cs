using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicUtilityFunction : UtilityFunction
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

        return (((player_hp - enemy_hp) / (float)(player_hp + enemy_hp) + 1) / 2 + 1 / (float)s.depth) * 1000;

    }
}
