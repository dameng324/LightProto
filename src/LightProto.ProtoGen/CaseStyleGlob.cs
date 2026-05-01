namespace LightProto.ProtoGen;

internal static class CaseStyleRuleParser
{
    public static CaseStyleRule ParseAssignment(string assignment)
    {
        if (string.IsNullOrWhiteSpace(assignment))
            throw new ArgumentException("Case-style rule cannot be empty. Expected '<Pattern>=<Style>'.");

        var separatorIndex = assignment.IndexOf('=');
        if (separatorIndex <= 0 || separatorIndex == assignment.Length - 1)
            throw new ArgumentException($"Invalid --case-style '{assignment}'. Expected '<Pattern>=<Style>'.");

        var pattern = assignment[..separatorIndex].Trim();
        var styleText = assignment[(separatorIndex + 1)..].Trim();

        if (pattern.Length == 0 || styleText.Length == 0)
            throw new ArgumentException($"Invalid --case-style '{assignment}'. Expected '<Pattern>=<Style>'.");

        if (!Enum.TryParse<CaseStyle>(styleText, ignoreCase: true, out var style))
            throw new ArgumentException(
                $"Invalid case style '{styleText}'. Supported values: {string.Join(", ", Enum.GetNames<CaseStyle>())}."
            );

        CaseStyleResolver.GlobPattern.Parse(pattern);
        return new CaseStyleRule(pattern, style);
    }
}

internal sealed class CaseStyleResolver
{
    private readonly CaseStyle _defaultStyle;
    private readonly List<CompiledCaseStyleRule> _rules;

    public CaseStyleResolver(CaseStyle defaultStyle, IEnumerable<CaseStyleRule>? rules)
    {
        _defaultStyle = defaultStyle;
        _rules = [];

        if (rules is null)
            return;

        int order = 0;
        foreach (var rule in rules)
        {
            var pattern = GlobPattern.Parse(rule.Pattern);
            _rules.Add(new CompiledCaseStyleRule(order++, rule.Pattern, rule.Style, pattern, pattern.Specificity));
        }
    }

    public CaseStyle Resolve(string fullName) => Explain(fullName).SelectedStyle;

    public CaseStyleMatchExplanation Explain(string fullName)
    {
        var targetSegments = SplitSegments(fullName);
        var matches = new List<CaseStyleMatchCandidate>();
        CompiledCaseStyleRule? winner = null;

        foreach (var rule in _rules)
        {
            if (!rule.Pattern.IsMatch(targetSegments))
                continue;

            var isBetter = winner is null || rule.IsMoreSpecificThan(winner);
            if (isBetter)
                winner = rule;

            matches.Add(new CaseStyleMatchCandidate(rule.Order, rule.RawPattern, rule.Style, rule.Specificity, IsWinner: false));
        }

        if (winner is null)
            return new CaseStyleMatchExplanation(fullName, _defaultStyle, null, matches);

        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].RuleOrder == winner.Order)
            {
                matches[i] = matches[i] with { IsWinner = true };
                break;
            }
        }

        return new CaseStyleMatchExplanation(fullName, winner.Style, winner.RawPattern, matches);
    }

    internal static string[] SplitSegments(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be empty.");

        var segments = value.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
            throw new ArgumentException($"Invalid dotted name '{value}'.");
        return segments;
    }

    internal readonly record struct Specificity(int LiteralCount, int SegmentGlobCount, int SingleStarCount, int DoubleStarCount, int Depth)
    {
        public int CompareTo(Specificity other)
        {
            if (LiteralCount != other.LiteralCount)
                return LiteralCount.CompareTo(other.LiteralCount);
            if (SegmentGlobCount != other.SegmentGlobCount)
                return SegmentGlobCount.CompareTo(other.SegmentGlobCount);
            if (SingleStarCount != other.SingleStarCount)
                return SingleStarCount.CompareTo(other.SingleStarCount);
            if (DoubleStarCount != other.DoubleStarCount)
                return other.DoubleStarCount.CompareTo(DoubleStarCount);
            return Depth.CompareTo(other.Depth);
        }
    }

    internal sealed class GlobPattern
    {
        private readonly GlobSegment[] _segments;

        private GlobPattern(GlobSegment[] segments, Specificity specificity)
        {
            _segments = segments;
            Specificity = specificity;
        }

        public Specificity Specificity { get; }

        public static GlobPattern Parse(string pattern)
        {
            var segments = SplitSegments(pattern);
            var compiled = new GlobSegment[segments.Length];

            int literal = 0;
            int segmentGlob = 0;
            int singleStar = 0;
            int doubleStar = 0;

            for (int i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];
                if (segment == "**")
                {
                    compiled[i] = new GlobSegment(segment, GlobSegmentType.DoubleStar);
                    doubleStar++;
                    continue;
                }

                if (segment.Contains("**", StringComparison.Ordinal))
                {
                    throw new ArgumentException(
                        $"Invalid pattern '{pattern}': '**' must occupy an entire segment (for example 'pkg.**.Type')."
                    );
                }

                if (segment == "*")
                {
                    compiled[i] = new GlobSegment(segment, GlobSegmentType.SingleStar);
                    singleStar++;
                    continue;
                }

                if (segment.Contains('*'))
                {
                    compiled[i] = new GlobSegment(segment, GlobSegmentType.SegmentGlob);
                    segmentGlob++;
                    continue;
                }

                compiled[i] = new GlobSegment(segment, GlobSegmentType.Literal);
                literal++;
            }

            return new GlobPattern(compiled, new Specificity(literal, segmentGlob, singleStar, doubleStar, segments.Length));
        }

        public bool IsMatch(string[] targetSegments)
        {
            var memo = new Dictionary<(int PatternIndex, int TargetIndex), bool>();
            return IsMatchCore(0, 0);

            bool IsMatchCore(int patternIndex, int targetIndex)
            {
                var key = (patternIndex, targetIndex);
                if (memo.TryGetValue(key, out var cached))
                    return cached;

                bool result;
                if (patternIndex == _segments.Length)
                {
                    result = targetIndex == targetSegments.Length;
                }
                else
                {
                    var segment = _segments[patternIndex];
                    if (segment.Type == GlobSegmentType.DoubleStar)
                    {
                        result = false;
                        for (int i = targetIndex; i <= targetSegments.Length; i++)
                        {
                            if (IsMatchCore(patternIndex + 1, i))
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                    else if (targetIndex < targetSegments.Length && SegmentMatches(segment, targetSegments[targetIndex]))
                    {
                        result = IsMatchCore(patternIndex + 1, targetIndex + 1);
                    }
                    else
                    {
                        result = false;
                    }
                }

                memo[key] = result;
                return result;
            }
        }

        private static bool SegmentMatches(GlobSegment segment, string value)
        {
            return segment.Type switch
            {
                GlobSegmentType.Literal => string.Equals(segment.Text, value, StringComparison.Ordinal),
                GlobSegmentType.SingleStar => true,
                GlobSegmentType.SegmentGlob => SegmentGlobMatcher.IsMatch(segment.Text, value),
                _ => false,
            };
        }

        private readonly record struct GlobSegment(string Text, GlobSegmentType Type);
    }

    private enum GlobSegmentType
    {
        Literal = 0,
        SegmentGlob,
        SingleStar,
        DoubleStar,
    }

    private static class SegmentGlobMatcher
    {
        public static bool IsMatch(string pattern, string value)
        {
            int patternIndex = 0;
            int valueIndex = 0;
            int starIndex = -1;
            int matchIndex = 0;

            while (valueIndex < value.Length)
            {
                if (patternIndex < pattern.Length && pattern[patternIndex] == value[valueIndex])
                {
                    patternIndex++;
                    valueIndex++;
                    continue;
                }

                if (patternIndex < pattern.Length && pattern[patternIndex] == '*')
                {
                    starIndex = patternIndex++;
                    matchIndex = valueIndex;
                    continue;
                }

                if (starIndex != -1)
                {
                    patternIndex = starIndex + 1;
                    valueIndex = ++matchIndex;
                    continue;
                }

                return false;
            }

            while (patternIndex < pattern.Length && pattern[patternIndex] == '*')
                patternIndex++;

            return patternIndex == pattern.Length;
        }
    }

    private sealed record CompiledCaseStyleRule(int Order, string RawPattern, CaseStyle Style, GlobPattern Pattern, Specificity Specificity)
    {
        public bool IsMoreSpecificThan(CompiledCaseStyleRule other)
        {
            var score = Specificity.CompareTo(other.Specificity);
            return score > 0 || (score == 0 && Order > other.Order);
        }
    }
}

internal sealed record CaseStyleMatchCandidate(
    int RuleOrder,
    string Pattern,
    CaseStyle Style,
    CaseStyleResolver.Specificity Specificity,
    bool IsWinner
);

internal sealed record CaseStyleMatchExplanation(
    string FullName,
    CaseStyle SelectedStyle,
    string? SelectedPattern,
    IReadOnlyList<CaseStyleMatchCandidate> Candidates
);
