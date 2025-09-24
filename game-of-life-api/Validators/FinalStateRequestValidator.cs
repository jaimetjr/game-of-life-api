using FluentValidation;
using game_of_life_api.DTOs;

namespace game_of_life_api.Validators;

public class FinalStateRequestValidator : AbstractValidator<FinalStateRequest>
{
    private const int MaxAttempts = 50_000;

    public FinalStateRequestValidator()
    {
        RuleFor(x => x.MaxAttempts)
            .GreaterThan(0).WithMessage("maxAttempts must be > 0")
            .LessThanOrEqualTo(MaxAttempts).WithMessage($"maxAttempts must be <= {MaxAttempts}");

        RuleFor(x => x.Cells)
            .NotNull().WithMessage("Cells cannot be null");
    }
}
