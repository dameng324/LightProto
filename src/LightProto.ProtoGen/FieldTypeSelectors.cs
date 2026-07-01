namespace LightProto.ProtoGen;

internal static class RepeatedFieldTypeRuleParser
{
    public static RepeatedFieldTypeRule ParseAssignment(string assignment)
    {
        var (pattern, typeText) = ParseAssignmentCore(assignment, "--repeated-type", "Repeated-field type rule");

        if (!Enum.TryParse<RepeatedFieldType>(typeText, ignoreCase: true, out var type))
            throw new ArgumentException(
                $"Invalid repeated-field type '{typeText}'. Supported values: {string.Join(", ", Enum.GetNames<RepeatedFieldType>())}."
            );

        CaseStyleResolver.GlobPattern.Parse(pattern);
        return new RepeatedFieldTypeRule(pattern, type);
    }

    internal static (string Pattern, string Value) ParseAssignmentCore(string assignment, string optionName, string ruleName)
    {
        if (string.IsNullOrWhiteSpace(assignment))
            throw new ArgumentException($"{ruleName} cannot be empty. Expected '<Pattern>=<Type>'.");

        var separatorIndex = assignment.IndexOf('=');
        if (separatorIndex <= 0 || separatorIndex == assignment.Length - 1)
            throw new ArgumentException($"Invalid {optionName} '{assignment}'. Expected '<Pattern>=<Type>'.");

        var pattern = assignment[..separatorIndex].Trim();
        var value = assignment[(separatorIndex + 1)..].Trim();

        if (pattern.Length == 0 || value.Length == 0)
            throw new ArgumentException($"Invalid {optionName} '{assignment}'. Expected '<Pattern>=<Type>'.");

        return (pattern, value);
    }
}

internal static class MapFieldTypeRuleParser
{
    public static MapFieldTypeRule ParseAssignment(string assignment)
    {
        var (pattern, typeText) = RepeatedFieldTypeRuleParser.ParseAssignmentCore(assignment, "--map-type", "Map-field type rule");

        if (!Enum.TryParse<MapFieldType>(typeText, ignoreCase: true, out var type))
            throw new ArgumentException(
                $"Invalid map-field type '{typeText}'. Supported values: {string.Join(", ", Enum.GetNames<MapFieldType>())}."
            );

        CaseStyleResolver.GlobPattern.Parse(pattern);
        return new MapFieldTypeRule(pattern, type);
    }
}

internal sealed class SelectorResolver<TValue>
    where TValue : struct, Enum
{
    private readonly TValue _defaultValue;
    private readonly List<CompiledSelectorRule> _rules;

    public SelectorResolver(TValue defaultValue, IEnumerable<(string Pattern, TValue Value)>? rules)
    {
        _defaultValue = defaultValue;
        _rules = [];

        if (rules is null)
            return;

        foreach (var rule in rules)
        {
            var pattern = CaseStyleResolver.GlobPattern.Parse(rule.Pattern);
            _rules.Add(new CompiledSelectorRule(rule.Value, pattern, pattern.Specificity));
        }
    }

    public TValue Resolve(string fullName)
    {
        var targetSegments = CaseStyleResolver.SplitSegments(fullName);
        CompiledSelectorRule? winner = null;

        foreach (var rule in _rules)
        {
            if (!rule.Pattern.IsMatch(targetSegments))
                continue;

            if (winner is null || rule.IsMoreSpecificThan(winner))
                winner = rule;
        }

        return winner?.Value ?? _defaultValue;
    }

    private sealed record CompiledSelectorRule(
        TValue Value,
        CaseStyleResolver.GlobPattern Pattern,
        CaseStyleResolver.Specificity Specificity
    )
    {
        public bool IsMoreSpecificThan(CompiledSelectorRule other)
        {
            var score = Specificity.CompareTo(other.Specificity);
            return score >= 0;
        }
    }
}
