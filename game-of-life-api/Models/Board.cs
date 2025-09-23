namespace game_of_life_api.Models;

public sealed class Board
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int Rows { get; set; }
    public int Cols { get; set; }
    public string CellsJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}