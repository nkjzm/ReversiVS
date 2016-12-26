using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    BoardManager board;

    ReactiveProperty<StoneState> currentState = new ReactiveProperty<StoneState>(StoneState.BLACK);

    bool isPassed;

    void Start()
    {
        var cells = GetComponentsInChildren<Cell>().ToList();
        board = new BoardManager(cells);

        board.Tapped
            .Subscribe(p =>
            {
                if (board.TrySetStone(new Action(p, currentState.Value)))
                {
                    Debug.Log(string.Format("Set: {0},{1}", p.x, p.y));
                    currentState.Value = Service.GetOpponent(currentState.Value);
                }
            })
            .AddTo(gameObject);

        currentState
            .Skip(1)
            .Subscribe(state =>
            {
                if (CanAvailable())
                {
                    isPassed = false;
                }
                else
                {
                    // 2回連続のパス
                    if (isPassed)
                    {
                        GameSet();
                        return;
                    }
                    isPassed = true;
                    currentState.Value = Service.GetOpponent(state);
                }
                if (state.Equals(StoneState.WHITE))
                {
                    RandomSet();
                }
            })
            .AddTo(gameObject);
    }

    void GameSet()
    {
        Debug.Log("ゲーム終了");
    }

    // HACK: もっと枝切りできる
    bool CanAvailable()
    {
        for (int y = 0; y < 8; ++y)
        {
            for (int x = 0; x < 8; ++x)
            {
                var count = board.GetReversiblePoints(
                    new Action(new Point(x, y), currentState.Value)
                ).Count;
                if (count > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void RandomSet()
    {
        var list = new List<Point>();
        for (int y = 0; y < 8; ++y)
        {
            for (int x = 0; x < 8; ++x)
            {
                list.Add(new Point(x, y));
            }
        }
        list = list.OrderBy(i => Guid.NewGuid()).ToList();

        foreach (var p in list)
        {
            if (board.TrySetStone(new Action(p, currentState.Value)))
            {
                Debug.Log(string.Format("Set: {0},{1}", p.x, p.y));
                currentState.Value = Service.GetOpponent(currentState.Value);
                return;
            }
        }
    }
}
