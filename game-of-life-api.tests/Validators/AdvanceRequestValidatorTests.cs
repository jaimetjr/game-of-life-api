using FluentAssertions;
using game_of_life_api.DTOs;
using game_of_life_api.Validators;

namespace game_of_life_api.tests.Validators;

public class AdvanceRequestValidatorTests
{
    private readonly AdvanceRequestValidator _sut = new();

    [Fact]
    public void Should_Fail_When_StepsIsZero()
    {
        var req = new AdvanceRequest { Steps = 0, Cells = new bool[1][] { new bool[1] } };
        var result = _sut.Validate(req);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Fail_When_StepsTooLarge()
    {
        var req = new AdvanceRequest { Steps = 20_000, Cells = new bool[1][] { new bool[1] } };
        var result = _sut.Validate(req);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var req = new AdvanceRequest { Steps = 5, Cells = new bool[1][] { new bool[1] } };
        var result = _sut.Validate(req);
        result.IsValid.Should().BeTrue();
    }
}
