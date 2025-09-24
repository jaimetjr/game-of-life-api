using FluentValidation;
using game_of_life_api.DTOs;

namespace game_of_life_api.Validators;

public class UploadBoardRequestValidator : AbstractValidator<UploadBoardRequest>
{
    public UploadBoardRequestValidator()
    {
        RuleFor(x => x.Rows)
            .GreaterThan(0).WithMessage("Rows must be > 0")
            .LessThanOrEqualTo(200).WithMessage("Rows must be <= 200");

        RuleFor(x => x.Cols)
            .GreaterThan(0).WithMessage("Cols must be > 0")
            .LessThanOrEqualTo(200).WithMessage("Cols must be <= 200");

        RuleFor(x => x.Cells)
            .NotNull().WithMessage("Cells cannot be null")
            .Must((req, cells) =>
                cells.Length == req.Rows &&
                cells.All(r => r.Length == req.Cols))
            .WithMessage("Cells must match Rows × Cols dimensions.");
    }
}

