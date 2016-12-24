using UnityEngine;
using System.Linq;
using UniRx;

public class BoardManager : MonoBehaviour
{
    Cell[][] Board = new Cell[8][];

    public Subject<Point> Tapped;

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
                Board[y][x].Tapped
                           .Subscribe(_ => Tapped.OnNext(new Point(x, y)))
                           .AddTo(gameObject);
            }
        }
    }

    public StoneState GetStone(Point p)
    {
        return Board[p.x][p.y].ObservableStoneState.Value;
    }

    public void SetStone(Point p, StoneState state)
    {
        Board[p.x][p.y].ObservableStoneState.Value = state;
    }
}

public enum StoneState
{
    NONE,
    WHITE,
    BLACK
}

public struct Point
{
    public int x;
    public int y;
    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}