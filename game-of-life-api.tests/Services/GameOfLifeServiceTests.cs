using FluentAssertions;
using game_of_life_api.Helpers.Enum;
using game_of_life_api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_of_life_api.tests.Services;

public class GameOfLifeServiceTests
{
    private readonly GameOfLifeService _sut = new();

    [Fact]
    public void ComputeNext_Should_ApplyRulesCorrectly()
    {
        bool[][] start =
        [
            [false, true, false],
            [false, true, false],
            [false, true, false]
        ];

        var next = _sut.ComputeNext(start);

        next.Should().BeEquivalentTo(new bool[][]
        {
            new [] { false, false, false },
            new [] { true,  true,  true  },
            new [] { false, false, false }
        });
    }

    [Fact]
    public void Advance_Should_RunMultipleSteps()
    {
        bool[][] start =
        [
            [false, true, false],
            [false, true, false],
            [false, true, false]
        ];

        var advanced = _sut.Advance(start, 2);
        advanced.Should().BeEquivalentTo(start);
    }

    [Fact]
    public void FindFinalState_Should_DetectStable()
    {
        bool[][] block =
        [
            [true, true],
            [true, true]
        ];

        var result = _sut.FindFinalState(block, 10);

        result.Reason.Should().Be(TerminationReason.Stable);
    }

    [Fact]
    public void FindFinalState_Should_DetectExtinct()
    {
        bool[][] empty = [];

        var result = _sut.FindFinalState(empty, 10);

        result.Reason.Should().Be(TerminationReason.Extinct);
    }

    [Fact]
    public void FindFinalState_Should_DetectLoop()
    {
        bool[][] blinker =
        [
            [false, true, false],
            [false, true, false],
            [false, true, false]
        ];

        var result = _sut.FindFinalState(blinker, 10);

        result.Reason.Should().Be(TerminationReason.Loop);
    }
}
