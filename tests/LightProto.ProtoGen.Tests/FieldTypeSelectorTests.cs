using LightProto.ProtoGen;

namespace LightProto.ProtoGen.Tests;

public class FieldTypeSelectorTests
{
    [Test]
    public async Task RepeatedTypeParser_IsCaseInsensitive()
    {
        var rule = RepeatedFieldTypeRuleParser.ParseAssignment("pkg.Message.tags=hashset");

        await Assert.That(rule.Pattern).IsEqualTo("pkg.Message.tags");
        await Assert.That(rule.Type).IsEqualTo(RepeatedFieldType.HashSet);
    }

    [Test]
    public async Task MapTypeParser_IsCaseInsensitive()
    {
        var rule = MapFieldTypeRuleParser.ParseAssignment("pkg.Message.items=immutabledictionary");

        await Assert.That(rule.Pattern).IsEqualTo("pkg.Message.items");
        await Assert.That(rule.Type).IsEqualTo(MapFieldType.ImmutableDictionary);
    }

    [Test]
    public async Task SelectorResolver_UsesCaseStyleSpecificityRules()
    {
        var resolver = new SelectorResolver<RepeatedFieldType>(
            RepeatedFieldType.List,
            [
                ("pkg.**", RepeatedFieldType.Array),
                ("pkg.*.items", RepeatedFieldType.HashSet),
                ("pkg.Message.*", RepeatedFieldType.IReadOnlyList),
            ]
        );

        await Assert.That(resolver.Resolve("pkg.Message.items")).IsEqualTo(RepeatedFieldType.IReadOnlyList);
        await Assert.That(resolver.Resolve("pkg.Other.items")).IsEqualTo(RepeatedFieldType.HashSet);
        await Assert.That(resolver.Resolve("other.Message.items")).IsEqualTo(RepeatedFieldType.List);
    }

    [Test]
    public async Task SelectorResolver_NullRulesUsesDefaultValue()
    {
        var resolver = new SelectorResolver<MapFieldType>(MapFieldType.Dictionary, null);

        await Assert.That(resolver.Resolve("pkg.Message.items")).IsEqualTo(MapFieldType.Dictionary);
    }

    [Test]
    public async Task SelectorResolver_SameSpecificityLaterRuleWins()
    {
        var resolver = new SelectorResolver<MapFieldType>(
            MapFieldType.Dictionary,
            [("pkg.Message.items", MapFieldType.IDictionary), ("pkg.Message.items", MapFieldType.IReadOnlyDictionary)]
        );

        await Assert.That(resolver.Resolve("pkg.Message.items")).IsEqualTo(MapFieldType.IReadOnlyDictionary);
    }

    [Test]
    public async Task SelectorResolver_LessSpecificLaterRuleDoesNotWin()
    {
        var resolver = new SelectorResolver<MapFieldType>(
            MapFieldType.Dictionary,
            [("pkg.Message.items", MapFieldType.IDictionary), ("pkg.**", MapFieldType.IReadOnlyDictionary)]
        );

        await Assert.That(resolver.Resolve("pkg.Message.items")).IsEqualTo(MapFieldType.IDictionary);
    }

    [Test]
    public async Task RepeatedTypeParser_EmptyRuleThrowsFriendlyError()
    {
        await Assert
            .That(() => RepeatedFieldTypeRuleParser.ParseAssignment(" "))
            .Throws<ArgumentException>()
            .WithMessage("Repeated-field type rule cannot be empty. Expected '<Pattern>=<Type>'.");
    }

    [Test]
    public async Task RepeatedTypeParser_MissingSeparatorThrowsFriendlyError()
    {
        await Assert
            .That(() => RepeatedFieldTypeRuleParser.ParseAssignment("pkg.Message.tags"))
            .Throws<ArgumentException>()
            .WithMessage("Invalid --repeated-type 'pkg.Message.tags'. Expected '<Pattern>=<Type>'.");
    }

    [Test]
    public async Task RepeatedTypeParser_EmptyValueThrowsFriendlyError()
    {
        await Assert
            .That(() => RepeatedFieldTypeRuleParser.ParseAssignment("pkg.Message.tags= "))
            .Throws<ArgumentException>()
            .WithMessage("Invalid --repeated-type 'pkg.Message.tags= '. Expected '<Pattern>=<Type>'.");
    }

    [Test]
    public async Task MapTypeParser_EmptyPatternThrowsFriendlyError()
    {
        await Assert
            .That(() => MapFieldTypeRuleParser.ParseAssignment(" =Dictionary"))
            .Throws<ArgumentException>()
            .WithMessage("Invalid --map-type ' =Dictionary'. Expected '<Pattern>=<Type>'.");
    }

    [Test]
    public async Task RepeatedTypeParser_InvalidTypeThrowsFriendlyError()
    {
        await Assert
            .That(() => RepeatedFieldTypeRuleParser.ParseAssignment("pkg.Message.tags=UnknownType"))
            .Throws<ArgumentException>()
            .WithMessage(
                "Invalid repeated-field type 'UnknownType'. Supported values: List, Array, HashSet, IList, IReadOnlyList, ICollection, IReadOnlyCollection, IEnumerable, ISet, Queue, Stack, LinkedList, SortedSet, Collection, ReadOnlyCollection, ObservableCollection, ReadOnlyObservableCollection, ConcurrentQueue, ConcurrentStack, ConcurrentBag, BlockingCollection, ImmutableArray, ImmutableList, ImmutableHashSet, ImmutableSortedSet, ImmutableQueue, ImmutableStack, FrozenSet."
            );
    }

    [Test]
    public async Task MapTypeParser_InvalidTypeThrowsFriendlyError()
    {
        await Assert
            .That(() => MapFieldTypeRuleParser.ParseAssignment("pkg.Message.items=UnknownType"))
            .Throws<ArgumentException>()
            .WithMessage(
                "Invalid map-field type 'UnknownType'. Supported values: Dictionary, IDictionary, IReadOnlyDictionary, SortedDictionary, SortedList, ConcurrentDictionary, ReadOnlyDictionary, ImmutableDictionary, ImmutableSortedDictionary, FrozenDictionary."
            );
    }

    [Test]
    public async Task MapTypeParser_InvalidPatternThrowsFriendlyError()
    {
        await Assert
            .That(() => MapFieldTypeRuleParser.ParseAssignment("pkg.ab**cd=Dictionary"))
            .Throws<ArgumentException>()
            .WithMessage("Invalid pattern 'pkg.ab**cd': '**' must occupy an entire segment (for example 'pkg.**.Type').");
    }
}
