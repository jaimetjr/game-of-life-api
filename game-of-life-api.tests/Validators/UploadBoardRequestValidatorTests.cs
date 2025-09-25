using FluentAssertions;
using game_of_life_api.DTOs;
using game_of_life_api.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_of_life_api.tests.Validators;

public class UploadBoardRequestValidatorTests
{
    private readonly UploadBoardRequestValidator _sut = new();

    [Fact]
    public void Should_Fail_When_RowsTooLarge()
    {
        var req = new UploadBoardRequest
        {
            Rows = 201,
            Cols = 5,
            Cells = Enumerable.Range(0, 201).Select(_ => new bool[5]).ToArray()
        };
        var result = _sut.Validate(req);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Fail_When_DimensionsMismatch()
    {
        var req = new UploadBoardRequest
        {
            Rows = 2,
            Cols = 2,
            Cells = new bool[][]
            {
                new [] { true, false },
                new [] { true }
            }
        };
        var result = _sut.Validate(req);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var req = new UploadBoardRequest
        {
            Rows = 2,
            Cols = 2,
            Cells = new bool[][]
            {
                new [] { true, false },
                new [] { false, true }
            }
        };
        var result = _sut.Validate(req);
        result.IsValid.Should().BeTrue();
    }

}
