using FluentAssertions;
using game_of_life_api.DTOs;
using game_of_life_api.Validators;

namespace game_of_life_api.tests.Validators;

public class FinalStateRequestValidatorTests
{
    private readonly FinalStateRequestValidator _sut = new();

    [Fact]
    public void Should_Fail_When_MaxAttemptsIsZero()
    {
        var req = new FinalStateRequest { MaxAttempts = 0, Cells = new bool[1][] { new bool[1] } };
        var result = _sut.Validate(req);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Fail_When_MaxAttemptsTooLarge()
    {
        var req = new FinalStateRequest { MaxAttempts = 100_000, Cells = new bool[1][] { new bool[1] } };
        var result = _sut.Validate(req);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var req = new FinalStateRequest { MaxAttempts = 500, Cells = new bool[1][] { new bool[1] } };
        var result = _sut.Validate(req);
        result.IsValid.Should().BeTrue();
    }

}
