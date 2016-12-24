using UnityEngine;
using UniRx;

public class GameManager : MonoBehaviour
{
    BoardManager board;

    ReactiveProperty<StoneState> currentState = new ReactiveProperty<StoneState>(StoneState.BLACK);

    bool isPassed = false;

    void Start()
    {
        board = GetComponentInChildren<BoardManager>();

        board.Tapped
            .Subscribe(p =>
            {
                if (board.TrySetStone(new Action(p, currentState.Value)))
                {
                    Debug.Log(string.Format("Set:({0},{1}", p.x, p.y));
                    currentState.Value = Service.GetOpponent(currentState.Value);
                }
            })
            .AddTo(gameObject);

        currentState
            .Skip(1)
            .Subscribe(_ =>
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
                    currentState.Value = Service.GetOpponent(currentState.Value);
                }
            })
            .AddTo(gameObject);
    }

    void GameSet()
    {
        Debug.Log("ゲーム終了");
    }

    // HACK: 枝切りできる
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
}
