using game_of_life_api.DTOs;
using game_of_life_api.Helpers.Enum;
using System.Text;

namespace game_of_life_api.Services;

public class GameOfLifeService : IGameOfLifeService
{
    public bool[][] ComputeNext(bool[][] cells)
    {
        ValidateRectangular(cells);
        var rows = cells.Length;
        var cols = rows == 0 ? 0 : cells[0].Length;

        var next = CreateBoard(rows, cols);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int neighbors = CountNeighbors(cells, r, c, rows, cols);
                bool alive = cells[r][c];
                next[r][c] = alive ? (neighbors == 2 || neighbors == 3) : (neighbors == 3);
            }
        }

        return next;
    }

    public bool[][] Advance(bool[][] cells, int steps)
    {
        if (steps < 0) throw new ArgumentOutOfRangeException(nameof(steps), "steps must be >= 0");
        ValidateRectangular(cells);

        var current = CloneBoard(cells);
        for (int i = 0; i < steps; i++)
        {
            current = ComputeNext(current);
        }
        return current;
    }

    public FinalStateResult FindFinalState(bool[][] start, int maxAttempts)
    {
        if (maxAttempts <= 0) throw new ArgumentOutOfRangeException(nameof(maxAttempts), "maxAttempts must be > 0");
        ValidateRectangular(start);

        var seen = new HashSet<string>();
        var current = CloneBoard(start);
        var steps = 0;

        seen.Add(Serialize(current));

        while (steps < maxAttempts)
        {
            if (IsAllDead(current))
                return new FinalStateResult(current, TerminationReason.Extinct, steps);

            var next = ComputeNext(current);

            // Terminal: stable
            if (BoardsEqual(current, next))
                return new FinalStateResult(next, TerminationReason.Stable, steps + 1);

            var hash = Serialize(next);

            if (seen.Contains(hash))
                return new FinalStateResult(next, TerminationReason.Loop, steps + 1);

            seen.Add(hash);
            current = next;
            steps++;
        }
        return new FinalStateResult(current, TerminationReason.Unresolved, steps);
    }

    private static int CountNeighbors(bool[][] cells, int r, int c, int rows, int cols)
    {
        int count = 0;
        for (int dr = -1; dr <= 1; dr++)
        {
            for (int dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0) continue;
                int nr = r + dr;
                int nc = c + dc;
                if (nr >= 0 && nr < rows && nc >= 0 && nc < cols && cells[nr][nc])
                    count++;
            }
        }
        return count;
    }

    private static bool BoardsEqual(bool[][] a, bool[][] b)
    {
        if (a.Length != b.Length) return false;
        for (int r = 0; r < a.Length; r++)
        {
            if (a[r].Length != b[r].Length) return false;
            for (int c = 0; c < a[r].Length; c++)
            {
                if (a[r][c] != b[r][c]) return false;
            }
        }
        return true;
    }

    private static bool IsAllDead(bool[][] cells)
    {
        for (int r = 0; r < cells.Length; r++)
            for (int c = 0; c < cells[r].Length; c++)
                if (cells[r][c]) return false;
        return true;
    }

    private static string Serialize(bool[][] cells)
    {
        var sb = new StringBuilder(cells.Length * (cells.Length > 0 ? cells[0].Length : 0) + cells.Length);
        for (int r = 0; r < cells.Length; r++)
        {
            if (r > 0) sb.Append('|');
            for (int c = 0; c < cells[r].Length; c++)
                sb.Append(cells[r][c] ? '1' : '0');
        }
        return sb.ToString();
    }

    private static void ValidateRectangular(bool[][] cells)
    {
        if (cells is null)
            throw new ArgumentNullException(nameof(cells));
        if (cells.Length == 0) return;

        var cols = cells[0].Length;
        for (int r = 1; r < cells.Length; r++)
        {
            if (cells[r].Length != cols)
                throw new ArgumentException("Board must be rectangular (all rows same length).", nameof(cells));
        }
    }

    private static bool[][] CreateBoard(int rows, int cols)
    {
        var arr = new bool[rows][];
        for (int r = 0; r < rows; r++)
            arr[r] = new bool[cols];
        return arr;
    }

    private static bool[][] CloneBoard(bool[][] source)
    {
        var rows = source.Length;
        var cols = rows == 0 ? 0 : source[0].Length;
        var copy = CreateBoard(rows, cols);
        for (int r = 0; r < rows; r++)
            Array.Copy(source[r], copy[r], cols);
        return copy;
    }
}
