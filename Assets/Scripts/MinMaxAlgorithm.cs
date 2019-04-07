using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeepCopyExtensions;

public class MinMaxAlgorithm : MoveMaker
{
    public EvaluationFunction evaluator;
    private UtilityFunction utilityfunc;
    public int depth = 0;
    private PlayerController MaxPlayer;
    private PlayerController MinPlayer;

    private State nextState;
    private float bestVal;
    private static int maxDepth = 5;

    public MinMaxAlgorithm(PlayerController MaxPlayer, EvaluationFunction eval, UtilityFunction utilf, PlayerController MinPlayer)
    {
        this.MaxPlayer = MaxPlayer;
        this.MinPlayer = MinPlayer;
        this.evaluator = eval;
        this.utilityfunc = utilf;
    }

    public override State MakeMove()
    {
        // The move is decided by the selected state
        return GenerateNewState();
    }

    private State GenerateNewState()
    {
        // Creates initial state
        State newState = new State(this.MaxPlayer, this.MinPlayer);
        // Call the MinMax implementation
        State bestMove = MinMax(newState);
        // returning the actual state. You should modify this
        return bestMove;
    }

    private float valMin(State state, float alfa, float beta)
    {
        if (state.AdversaryUnits.Count == 0 || state.PlayersUnits.Count == 0)
        {
            return utilityfunc.evaluate(state);
        }

        if (state.depth >= maxDepth || this.MaxPlayer.ExpandedNodes >= this.MaxPlayer.MaximumNodesToExpand)
        {
            return evaluator.evaluate(state);
        }

        float v = Mathf.Infinity;

        state = new State(state);
        var possibleStates = GeneratePossibleStates(state);
        var stateSorter = new StateSorter(evaluator, false);
        //possibleStates.Sort(stateSorter);
        foreach (var possibleState in possibleStates)
        {
            var newV = valMax(possibleState, alfa, beta, false);
            if (newV < v)
            {
                v = newV;
            }
            if (v <= alfa)
                return v;
            beta = Math.Min(beta, v);
        }
        return v;
    }

    private float valMax(State state, float alfa, float beta, bool isTop)
    {
        if (state.AdversaryUnits.Count == 0 || state.PlayersUnits.Count == 0)
        {
            return utilityfunc.evaluate(state);
        }

        if (state.depth >= maxDepth || this.MaxPlayer.ExpandedNodes >= this.MaxPlayer.MaximumNodesToExpand)
        {
            return evaluator.evaluate(state);
        }

        float v = -Mathf.Infinity;

        state = new State(state);
        var possibleStates = GeneratePossibleStates(state);
        var stateSorter = new StateSorter(evaluator, true);
        //possibleStates.Sort(stateSorter);
        foreach (var possibleState in possibleStates)
        {
            var newV = valMin(possibleState, alfa, beta);
            if (newV > v)
            {
                v = newV;
                if (isTop)
                    nextState = possibleState;
            }
            if (v >= beta)
                return v;
            alfa = Math.Max(alfa, v);
        }
        return v;
    }

    public State MinMax(State state)
    {
        valMax(state, -Mathf.Infinity, Mathf.Infinity, true);
        this.MaxPlayer.ExpandedNodes = 0;
        return nextState;
    }


    private List<State> GeneratePossibleStates(State state)
    {
        List<State> states = new List<State>();
        //Generate the possible states available to expand
        foreach (Unit currentUnit in state.PlayersUnits)
        {
            // Movement States
            List<Tile> neighbours = currentUnit.GetFreeNeighbours(state);
            foreach (Tile t in neighbours)
            {
                State newState = new State(state, currentUnit, true);
                newState = MoveUnit(newState, t);
                states.Add(newState);
            }
            // Attack states
            List<Unit> attackOptions = currentUnit.GetAttackable(state, state.AdversaryUnits);
            foreach (Unit t in attackOptions)
            {
                State newState = new State(state, currentUnit, false);
                newState = AttackUnit(newState, t);
                states.Add(newState);
            }

        }

        // YOU SHOULD NOT REMOVE THIS
        // Counts the number of expanded nodes;
        this.MaxPlayer.ExpandedNodes += states.Count;
        //

        return states;
    }

    private State MoveUnit(State state, Tile destination)
    {
        Unit currentUnit = state.unitToPermormAction;
        //First: Update Board
        state.board[(int)destination.gridPosition.x, (int)destination.gridPosition.y] = currentUnit;
        state.board[currentUnit.x, currentUnit.y] = null;
        //Second: Update Players Unit Position
        currentUnit.x = (int)destination.gridPosition.x;
        currentUnit.y = (int)destination.gridPosition.y;
        state.isMove = true;
        state.isAttack = false;
        return state;
    }

    private State AttackUnit(State state, Unit toAttack)
    {
        Unit currentUnit = state.unitToPermormAction;
        Unit attacked = toAttack.DeepCopyByExpressionTree();

        Tuple<float, float> currentUnitBonus = currentUnit.GetBonus(state.board, state.PlayersUnits);
        Tuple<float, float> attackedUnitBonus = attacked.GetBonus(state.board, state.AdversaryUnits);


        attacked.hp += Math.Min(0, (attackedUnitBonus.Item1)) - (currentUnitBonus.Item2 + currentUnit.attack);
        state.unitAttacked = attacked;

        state.board[attacked.x, attacked.y] = attacked;
        int index = state.AdversaryUnits.IndexOf(attacked);
        state.AdversaryUnits[index] = attacked;



        if (attacked.hp <= 0)
        {
            //Board update by killing the unit!
            state.board[attacked.x, attacked.y] = null;
            index = state.AdversaryUnits.IndexOf(attacked);
            state.AdversaryUnits.RemoveAt(index);

        }
        state.isMove = false;
        state.isAttack = true;

        return state;

    }
}
