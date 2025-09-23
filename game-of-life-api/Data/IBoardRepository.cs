using game_of_life_api.Models;

namespace game_of_life_api.Data;

public interface IBoardRepository
{
    Task<Guid> AddAsync(Board entity, CancellationToken ct = default);
    Task<Board?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
