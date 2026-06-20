using Enaxos.MusicTheory.Formulas;
using Enaxos.MusicTheory.Scales;

namespace Enaxos.MusicTheory.Tests.Scales;

public sealed class FormulaAndDefinitionRulesTests
{
    [Fact]
    public void Default_formula_degree_is_the_unaltered_tonic()
    {
        var degree = default(FormulaDegree);

        Assert.Equal(1, degree.Number);
        Assert.Equal(0, degree.Alteration);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(3, -1)]
    [InlineData(5, 1)]
    [InlineData(9, 2)]
    [InlineData(int.MaxValue, int.MinValue)]
    public void Formula_degree_preserves_number_and_signed_alteration(int number, int alteration)
    {
        var degree = new FormulaDegree(number, alteration);

        Assert.Equal(number, degree.Number);
        Assert.Equal(alteration, degree.Alteration);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void Formula_degree_number_must_be_positive(int number)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new FormulaDegree(number));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Definition_id_must_be_stable_and_non_blank(string id)
    {
        Assert.Throws<ArgumentException>(() =>
            new ScaleDefinition(id, [new FormulaDegree(1)]));
    }

    [Fact]
    public void Definition_requires_at_least_one_degree()
    {
        Assert.Throws<ArgumentException>(() =>
            new ScaleDefinition("scale.empty", []));
    }

    [Theory]
    [MemberData(nameof(InvalidDegreeSequences))]
    public void Definition_degrees_must_start_at_natural_one_and_strictly_increase(
        FormulaDegree[] degrees)
    {
        Assert.Throws<ArgumentException>(() =>
            new ScaleDefinition("scale.invalid", degrees));
    }

    [Fact]
    public void Definition_copies_input_and_exposes_a_read_only_collection()
    {
        var input = new List<FormulaDegree>
        {
            new(1),
            new(2),
            new(3, -1),
        };
        var definition = new ScaleDefinition("scale.test", input);

        input[1] = new FormulaDegree(7);
        input.Add(new FormulaDegree(8));

        Assert.Equal([1, 2, 3], definition.Degrees.Select(degree => degree.Number));
        var mutableView = Assert.IsAssignableFrom<ICollection<FormulaDegree>>(definition.Degrees);
        Assert.Throws<NotSupportedException>(() => mutableView.Add(new FormulaDegree(4)));
    }

    [Fact]
    public void Aggregate_equality_uses_id_and_degree_sequence()
    {
        var first = new ScaleDefinition(
            "scale.test",
            [new FormulaDegree(1), new FormulaDegree(2), new FormulaDegree(3, -1)]);
        var second = new ScaleDefinition(
            "scale.test",
            [new FormulaDegree(1), new FormulaDegree(2), new FormulaDegree(3, -1)]);
        var otherId = new ScaleDefinition(
            "scale.other",
            [new FormulaDegree(1), new FormulaDegree(2), new FormulaDegree(3, -1)]);
        var otherDegrees = new ScaleDefinition(
            "scale.test",
            [new FormulaDegree(1), new FormulaDegree(2, -1), new FormulaDegree(3, -1)]);

        Assert.Equal(first, second);
        Assert.True(first == second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.NotEqual(first, otherId);
        Assert.NotEqual(first, otherDegrees);
    }

    public static TheoryData<FormulaDegree[]> InvalidDegreeSequences => new()
    {
        new[] { new FormulaDegree(2), new FormulaDegree(3) },
        new[] { new FormulaDegree(1, -1), new FormulaDegree(2) },
        new[] { new FormulaDegree(1), new FormulaDegree(3), new FormulaDegree(2) },
        new[] { new FormulaDegree(1), new FormulaDegree(2), new FormulaDegree(2, 1) },
    };
}
