namespace game_of_life_api.DTOs;

public class UploadBoardRequest
{
    public string? Name { get; set; }
    public int Rows { get; set; }
    public int Cols { get; set; }
    public bool[][] Cells { get; set; } = Array.Empty<bool[]>();
}
