using game_of_life_api.Helpers.Enum;

namespace game_of_life_api.DTOs;

public sealed class FinalStateResult
{
    public FinalStateResult(bool[][] state, TerminationReason reason, int stepsTaken)
    {
        State = state;
        Reason = reason;
        StepsTaken = stepsTaken;
    }

    public bool[][] State { get; }
    public TerminationReason Reason { get; }
    public int StepsTaken { get; }
}
