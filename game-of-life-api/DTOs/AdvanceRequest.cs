namespace game_of_life_api.DTOs;

public class AdvanceRequest
{
    public bool[][] Cells { get; set; } = Array.Empty<bool[]>();
    public int Steps { get; set; }
}
