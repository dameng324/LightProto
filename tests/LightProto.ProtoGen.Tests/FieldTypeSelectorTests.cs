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
    public async Task MapTypeParser_InvalidPatternThrowsFriendlyError()
    {
        await Assert
            .That(() => MapFieldTypeRuleParser.ParseAssignment("pkg.ab**cd=Dictionary"))
            .Throws<ArgumentException>()
            .WithMessage("Invalid pattern 'pkg.ab**cd': '**' must occupy an entire segment (for example 'pkg.**.Type').");
    }
}
