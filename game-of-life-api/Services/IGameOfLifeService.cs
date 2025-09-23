using game_of_life_api.DTOs;

namespace game_of_life_api.Services;

public interface IGameOfLifeService
{
    /// <summary>Computes the next generation for the given board (no wrap-around).</summary>
    bool[][] ComputeNext(bool[][] cells);

    /// <summary>Advances the board by the specified number of generations.</summary>
    /// <exception cref="ArgumentOutOfRangeException">If steps is negative.</exception>
    bool[][] Advance(bool[][] cells, int steps);

    /// <summary>
    /// Finds a terminal state starting from the given board.
    /// Terminal = Stable (no change), Extinct (all dead), or Loop (repeats a prior state).
    /// Returns Unresolved if no terminal state is found within maxAttempts.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If maxAttempts is not positive.</exception>
    FinalStateResult FindFinalState(bool[][] start, int maxAttempts);

}
