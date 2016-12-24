public static class Service
{
    public static StoneState GetOpponent(StoneState state)
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
