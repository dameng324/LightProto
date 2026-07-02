using System.Runtime.CompilerServices;
using LightProto;

namespace LightProto.Tests.Parsers;

#if NET8_0_OR_GREATER
[InlineArray(10)]
public struct IntInlineArray10
{
    private int value;
}

[ProtoContract]
[ProtoBuf.ProtoContract]
public partial struct InlineArrayItem
{
    [ProtoMember(1)]
    [ProtoBuf.ProtoMember(1)]
    public int Value { get; set; }

    [ProtoMember(2)]
    [ProtoBuf.ProtoMember(2)]
    public long Version { get; set; }

    public override string ToString()
    {
        return $"{Value}:{Version}";
    }
}

[InlineArray(3)]
public struct CustomItemInlineArray3
{
    private InlineArrayItem value;
}

[InheritsTests]
public partial class InlineArrayTests : BaseTests<InlineArrayTests.Message, ArrayTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial class Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public IntInlineArray10 Property { get; set; } = new();

        public override string ToString()
        {
            return $"Property: {string.Join(", ", InlineArray10ToEnumerable(Property))}";
        }
    }

    protected override bool ProtoBuf_net_Serialize_Disabled => true;

    protected override bool ProtoBuf_net_Deserialize_Disabled => true;

    public IEnumerable<int[]> GetIntArrays()
    {
        yield return [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        yield return [-1, -2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
        yield return [-1, -2, -3, -4, -5];
        yield return [0, 0, 0, 0, 0];
        yield return [0];
        yield return [];
    }

    public override IEnumerable<Message> GetMessages()
    {
        return GetIntArrays().Select(x => new Message() { Property = FillInlineArray10(x) });
    }

    static IntInlineArray10 FillInlineArray10(int[] array)
    {
        var inlineArray = new IntInlineArray10();
        for (int i = 0; i < 10; i++)
        {
            if (i < array.Length)
            {
                inlineArray[i] = array[i];
            }
        }
        return inlineArray;
    }

    static IEnumerable<int> InlineArray10ToEnumerable(IntInlineArray10 inlineArray10)
    {
        for (var index = 0; index < 10; index++)
        {
            yield return inlineArray10[index];
        }
    }

    public override IEnumerable<ArrayTestsMessage> GetGoogleMessages()
    {
        return GetIntArrays()
            .Select(o =>
            {
                return new ArrayTestsMessage() { Property = { InlineArray10ToEnumerable(FillInlineArray10(o)) } };
            });
    }

    public override async Task AssertGoogleResult(ArrayTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToArray()).IsEquivalentTo(InlineArray10ToEnumerable(message.Property).ToArray());
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert
            .That(InlineArray10ToEnumerable(clone.Property).ToArray())
            .IsEquivalentTo(InlineArray10ToEnumerable(message.Property).ToArray());
    }
}

[InheritsTests]
public partial class FixedSizeInlineArrayTests : BaseTests<FixedSizeInlineArrayTests.Message, FixedSizeArrayTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial class Message
    {
        [ProtoMember(1, DataFormat = DataFormat.FixedSize, IsPacked = true)]
        [ProtoBuf.ProtoMember(1, DataFormat = ProtoBuf.DataFormat.FixedSize, IsPacked = true)]
        public IntInlineArray10 Property { get; set; } = new();

        public override string ToString()
        {
            return $"Property: {string.Join(", ", InlineArray10ToEnumerable(Property))}";
        }
    }

    protected override bool ProtoBuf_net_Serialize_Disabled => true;

    protected override bool ProtoBuf_net_Deserialize_Disabled => true;

    public IEnumerable<int[]> GetIntArrays()
    {
        yield return [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        yield return [-1, -2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
        yield return [-1, -2, -3, -4, -5];
        yield return [0, 0, 0, 0, 0];
        yield return [0];
        yield return [];
    }

    public override IEnumerable<Message> GetMessages()
    {
        return GetIntArrays().Select(x => new Message() { Property = FillInlineArray10(x) });
    }

    static IntInlineArray10 FillInlineArray10(int[] array)
    {
        var inlineArray = new IntInlineArray10();
        for (int i = 0; i < 10; i++)
        {
            if (i < array.Length)
            {
                inlineArray[i] = array[i];
            }
        }
        return inlineArray;
    }

    static IEnumerable<int> InlineArray10ToEnumerable(IntInlineArray10 inlineArray10)
    {
        for (var index = 0; index < 10; index++)
        {
            yield return inlineArray10[index];
        }
    }

    public override IEnumerable<FixedSizeArrayTestsMessage> GetGoogleMessages()
    {
        return GetIntArrays()
            .Select(o =>
            {
                return new FixedSizeArrayTestsMessage() { Property = { InlineArray10ToEnumerable(FillInlineArray10(o)) } };
            });
    }

    public override async Task AssertGoogleResult(FixedSizeArrayTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToArray()).IsEquivalentTo(InlineArray10ToEnumerable(message.Property).ToArray());
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert
            .That(InlineArray10ToEnumerable(clone.Property).ToArray())
            .IsEquivalentTo(InlineArray10ToEnumerable(message.Property).ToArray());
    }
}

[InlineArray(4)]
public struct GenericInlineArray<T>
{
    private T value;
}

[InheritsTests]
public partial class GenericInlineArrayTests : BaseTests<GenericInlineArrayTests.Message, ArrayTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial class Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public GenericInlineArray<int> Property { get; set; } = new();

        public override string ToString()
        {
            return $"Property: {string.Join(", ", InlineArrayToEnumerable(Property))}";
        }
    }

    protected override bool ProtoBuf_net_Serialize_Disabled => true;

    protected override bool ProtoBuf_net_Deserialize_Disabled => true;

    public IEnumerable<int[]> GetIntArrays()
    {
        yield return [1, 2, 3, 4];
        yield return [-1, -2, 3, 4, 5, 6];
        yield return [-1, -2];
        yield return [0, 0, 0, 0];
        yield return [];
    }

    public override IEnumerable<Message> GetMessages()
    {
        return GetIntArrays().Select(x => new Message() { Property = FillInlineArray(x) });
    }

    static GenericInlineArray<int> FillInlineArray(int[] array)
    {
        var inlineArray = new GenericInlineArray<int>();
        for (int i = 0; i < 4; i++)
        {
            if (i < array.Length)
            {
                inlineArray[i] = array[i];
            }
        }
        return inlineArray;
    }

    static IEnumerable<int> InlineArrayToEnumerable(GenericInlineArray<int> inlineArray)
    {
        for (var index = 0; index < 4; index++)
        {
            yield return inlineArray[index];
        }
    }

    public override IEnumerable<ArrayTestsMessage> GetGoogleMessages()
    {
        return GetIntArrays()
            .Select(o =>
            {
                return new ArrayTestsMessage() { Property = { InlineArrayToEnumerable(FillInlineArray(o)) } };
            });
    }

    public override async Task AssertGoogleResult(ArrayTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToArray()).IsEquivalentTo(InlineArrayToEnumerable(message.Property).ToArray());
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert
            .That(InlineArrayToEnumerable(clone.Property).ToArray())
            .IsEquivalentTo(InlineArrayToEnumerable(message.Property).ToArray());
    }
}

[InheritsTests]
public partial class InlineArrayCustomStructItemTests : BaseProtoBufTests<InlineArrayCustomStructItemTests.Message>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial class Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public CustomItemInlineArray3 Property { get; set; } = new();

        public override string ToString()
        {
            return $"Property: {string.Join(", ", InlineArrayToEnumerable(Property))}";
        }
    }

    protected override bool ProtoBuf_net_Serialize_Disabled => true;

    protected override bool ProtoBuf_net_Deserialize_Disabled => true;

    public IEnumerable<InlineArrayItem[]> GetInlineArrayItems()
    {
        yield return
        [
            new InlineArrayItem { Value = 1, Version = 101 },
            new InlineArrayItem { Value = 2, Version = 102 },
            new InlineArrayItem { Value = 3, Version = 103 },
        ];
        yield return
        [
            new InlineArrayItem { Value = -1, Version = 201 },
            new InlineArrayItem { Value = -2, Version = 202 },
            new InlineArrayItem { Value = -3, Version = 203 },
            new InlineArrayItem { Value = -4, Version = 204 },
        ];
        yield return [new InlineArrayItem { Value = 7, Version = 301 }];
        yield return [default];
        yield return [];
    }

    public override IEnumerable<Message> GetMessages()
    {
        return GetInlineArrayItems().Select(x => new Message() { Property = FillInlineArray(x) });
    }

    private static CustomItemInlineArray3 FillInlineArray(InlineArrayItem[] array)
    {
        var inlineArray = new CustomItemInlineArray3();
        for (int i = 0; i < 3; i++)
        {
            if (i < array.Length)
            {
                inlineArray[i] = array[i];
            }
        }
        return inlineArray;
    }

    private static IEnumerable<InlineArrayItem> InlineArrayToEnumerable(CustomItemInlineArray3 inlineArray)
    {
        for (var index = 0; index < 3; index++)
        {
            yield return inlineArray[index];
        }
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert
            .That(InlineArrayToEnumerable(clone.Property).ToArray())
            .IsEquivalentTo(InlineArrayToEnumerable(message.Property).ToArray());
    }
}

[InheritsTests]
public partial class GenericInlineArrayCustomStructItemTests : BaseProtoBufTests<GenericInlineArrayCustomStructItemTests.Message>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial class Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public GenericInlineArray<InlineArrayItem> Property { get; set; } = new();

        public override string ToString()
        {
            return $"Property: {string.Join(", ", InlineArrayToEnumerable(Property))}";
        }
    }

    protected override bool ProtoBuf_net_Serialize_Disabled => true;

    protected override bool ProtoBuf_net_Deserialize_Disabled => true;

    public IEnumerable<InlineArrayItem[]> GetInlineArrayItems()
    {
        yield return
        [
            new InlineArrayItem { Value = 11, Version = 1001 },
            new InlineArrayItem { Value = 12, Version = 1002 },
            new InlineArrayItem { Value = 13, Version = 1003 },
            new InlineArrayItem { Value = 14, Version = 1004 },
        ];
        yield return
        [
            new InlineArrayItem { Value = -11, Version = 2001 },
            new InlineArrayItem { Value = -12, Version = 2002 },
            new InlineArrayItem { Value = -13, Version = 2003 },
            new InlineArrayItem { Value = -14, Version = 2004 },
            new InlineArrayItem { Value = -15, Version = 2005 },
        ];
        yield return [new InlineArrayItem { Value = 17, Version = 3001 }, new InlineArrayItem { Value = 18, Version = 3002 }];
        yield return [default];
        yield return [];
    }

    public override IEnumerable<Message> GetMessages()
    {
        return GetInlineArrayItems().Select(x => new Message() { Property = FillInlineArray(x) });
    }

    private static GenericInlineArray<InlineArrayItem> FillInlineArray(InlineArrayItem[] array)
    {
        var inlineArray = new GenericInlineArray<InlineArrayItem>();
        for (int i = 0; i < 4; i++)
        {
            if (i < array.Length)
            {
                inlineArray[i] = array[i];
            }
        }
        return inlineArray;
    }

    private static IEnumerable<InlineArrayItem> InlineArrayToEnumerable(GenericInlineArray<InlineArrayItem> inlineArray)
    {
        for (var index = 0; index < 4; index++)
        {
            yield return inlineArray[index];
        }
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert
            .That(InlineArrayToEnumerable(clone.Property).ToArray())
            .IsEquivalentTo(InlineArrayToEnumerable(message.Property).ToArray());
    }
}
#endif
