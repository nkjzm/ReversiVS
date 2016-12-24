using UnityEngine;
using System.Linq;

public class BoardManager : MonoBehaviour
{
    Cell[][] Board = new Cell[8][];

    void Start()
    {
        var cells = GetComponentsInChildren<Cell>();
        for (int i = 0; i < 8; ++i)
        {
            Board[i] = cells.Skip(i * 8).Take(8).ToArray();
        }
    }

    public StoneState GetStone(int x, int y)
    {
        return Board[x][y].ObservableStoneState.Value;
    }

    public void SetStone(int x, int y, StoneState state)
    {
        Board[x][y].ObservableStoneState.Value = state;
    }
}
