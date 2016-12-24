using UnityEngine;
using System.Linq;
using UniRx;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    Cell[][] Board = new Cell[8][];

    public Subject<Point> Tapped = new Subject<Point>();

    void Start()
    {
        var cells = GetComponentsInChildren<Cell>();

        for (int i = 0; i < 8; ++i)
        {
            Board[i] = cells.Skip(i * 8).Take(8).ToArray();
        }

        for (int y = 0; y < 8; ++y)
        {
            for (int x = 0; x < 8; ++x)
            {
                var point = new Point(x, y);    // スコープ的にここで定義
                Board[y][x].Tapped
                    .Select(_ => point)
                    .Subscribe(Tapped.OnNext)
                    .AddTo(gameObject);
            }
        }

        SetStone(new Action(new Point(3, 3), StoneState.WHITE));
        SetStone(new Action(new Point(3, 4), StoneState.BLACK));
        SetStone(new Action(new Point(4, 3), StoneState.BLACK));
        SetStone(new Action(new Point(4, 4), StoneState.WHITE));
        SetStone(new Action(new Point(5, 4), StoneState.WHITE));
    }

    public StoneState GetStone(Point point)
    {
        return Board[point.y][point.x].ObservableStoneState.Value;
    }

    void SetStone(Action action)
    {
        Board[action.point.y][action.point.x].ObservableStoneState.Value = action.state;
    }

    public bool TrySetStone(Action action)
    {
        var points = GetReversiblePoints(action);
        foreach (var p in points)
        {
            Debug.Log("reversible:" + p.x + "," + p.y);
        }

        if (points.Count > 0)
        {
            SetStone(action);
            ReversePoints(points, action.state);
            return true;
        }
        return false;
    }

    public void ReversePoints(List<Point> points, StoneState state)
    {
        foreach (var p in points)
        {
            SetStone(new Action(p, state));
        }
    }

    public List<Point> GetReversiblePoints(Action action)
    {
        var points = new List<Point>();

        for (int y = -1; y <= 1; ++y)
        {
            for (int x = -1; x <= 1; ++x)
            {
                // 中心判定しない
                if (x == 0 && y == 0) { continue; }

                points.AddRange(
                    GetReversibleDirectionalPoints(action.point, action.state, x, y)
                );
            }
        }

        return points;
    }

    List<Point> GetReversibleDirectionalPoints(Point p, StoneState selfState, int x_dir, int y_dir)
    {
        var i = 0;
        var points = new List<Point>();

        while (true)
        {
            ++i;
            var nextPoint = new Point(p.x + (x_dir * i), p.y + (y_dir * i));
            if (!nextPoint.IsValid) { return new List<Point>(); }

            var nextStone = GetStone(nextPoint);
            if (nextStone.Equals(StoneState.NONE)) { return new List<Point>(); }

            if (nextStone.Equals(selfState))
            {
                return points;
            }
            points.Add(nextPoint);
        }
    }

    StoneState GetOpponent(StoneState state)
    {
        switch (state)
        {
            case StoneState.BLACK:
                return StoneState.WHITE;
            case StoneState.WHITE:
                return StoneState.BLACK;
            default:
                return StoneState.NONE;
        }
    }
}

public enum StoneState
{
    NONE,
    WHITE,
    BLACK
}

public class Point
{
    public int x;
    public int y;
    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public bool IsValid
    {
        get { return 0 <= x && x < 8 && 0 <= y && y < 8; }
    }
}

public struct Action
{
    public Point point;
    public StoneState state;
    public Action(Point point, StoneState state)
    {
        this.point = point;
        this.state = state;
    }
}