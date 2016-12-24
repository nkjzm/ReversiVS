using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class Cell : MonoBehaviour
{
    [SerializeField]
    Image Stone;

    public ReactiveProperty<StoneState> ObservableStoneState
    = new ReactiveProperty<StoneState>(StoneState.NONE);

    void Start()
    {
        ObservableStoneState.Subscribe(UpdateState).AddTo(gameObject);

        GetComponent<Button>().OnClickAsObservable()
                              .Subscribe(_ =>
        {
            ObservableStoneState.Value = (StoneState)Random.Range(1, 3);
        }).AddTo(gameObject);
    }

    void UpdateState(StoneState state)
    {
        Debug.Log(state);
        switch (state)
        {
            case StoneState.NONE:
                Stone.color = new Color(0, 0, 0, 0);
                break;
            case StoneState.BLACK:
                Stone.color = Color.black;
                break;
            case StoneState.WHITE:
                Stone.color = Color.white;
                break;
        }
    }
}

public enum StoneState
{
    NONE,
    WHITE,
    BLACK
}
