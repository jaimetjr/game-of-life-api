namespace game_of_life_api.DTOs;

public class FinalStateRequest
{
    public bool[][] Cells { get; set; } = Array.Empty<bool[]>();
    public int MaxAttempts { get; set; }
}
