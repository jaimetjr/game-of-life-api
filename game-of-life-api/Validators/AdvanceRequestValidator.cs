using FluentValidation;
using game_of_life_api.DTOs;

namespace game_of_life_api.Validators;

public class AdvanceRequestValidator : AbstractValidator<AdvanceRequest>
{
    private const int MaxSteps = 10_000;

    public AdvanceRequestValidator()
    {
        RuleFor(x => x.Steps)
            .GreaterThan(0).WithMessage("Steps must be > 0")
            .LessThanOrEqualTo(MaxSteps).WithMessage($"Steps must be <= {MaxSteps}");

        RuleFor(x => x.Cells)
            .NotNull().WithMessage("Cells cannot be null");
    }
}

