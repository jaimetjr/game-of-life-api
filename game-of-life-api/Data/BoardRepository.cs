using game_of_life_api.Models;
using Microsoft.EntityFrameworkCore;

namespace game_of_life_api.Data;

public class BoardRepository : IBoardRepository
{
    private readonly DataContext _context;

    public BoardRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddAsync(Board entity, CancellationToken ct = default)
    {
        if (entity.Id == Guid.Empty)
            entity.Id = Guid.NewGuid();

        entity.CreatedAt = DateTime.UtcNow;

        _context.Boards.Add(entity);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<Board?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Boards.AsTracking().SingleOrDefaultAsync(x => x.Id == id, ct);
    }
}
