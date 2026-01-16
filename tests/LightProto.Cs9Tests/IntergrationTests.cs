using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightProto.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightProto.Cs9Tests
{
    [TestClass]
    public class IntergrationTests
    {
        private readonly Random random = new Random(31);

        private CsTestMessage NewCsMessage() =>
            new CsTestMessage
            {
                RequiredIntField = 10,
                StringField = "hello",
                Int32Field = 20,
                Int32ArrayFields = new List<int>() { 0, 13123 },
                StringArrayFields = { string.Empty, Guid.NewGuid().ToString() },
                BytesField = Enumerable.Range(0, random.Next(100)).Select(_ => (byte)20).ToArray(),
                BoolField = random.Next() % 2 == 0,
                DoubleField = random.NextDouble(),
                FloatField = (float)random.NextDouble(),
                Int64Field = random.Next(),
                UInt32Field = (uint)random.Next(),
                UInt64Field = (ulong)random.Next(),
                SInt32Field = random.Next(),
                SInt64Field = random.Next(),
                Fixed32Field = (uint)random.Next(),
                Fixed64Field = (ulong)random.Next(),
                SFixed32Field = random.Next(),
                SFixed64Field = random.Next(),
                EnumField = CsTestEnum.OptionB,
                EnumArrayFields = { CsTestEnum.OptionB, CsTestEnum.None, CsTestEnum.OptionA },
                NestedMessageField = new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
                NestedMessageArrayFields = new List<CsTestMessage>
                {
                    new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
                    new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
                },
                TimestampField = DateTime.UtcNow,
                DurationField = DateTime.UtcNow.TimeOfDay,
                MapField2s = { ["hello"] = "hello", ["hello1"] = "hello" },
                IntArrayFieldTests = new[] { 20, 20, 20, 20 },
                StringListFieldTests = { "hello", "hello", "hello", "hello" },
                StringArrayFieldTests = { "hello", "hello" },
                IntListFieldTests = new[] { 20, 20, 20 },
                StringSetFieldTests = { "hello", "hello2" },
                StringQueueFieldTests = new(new List<string> { "hello", "hello" }.AsReadOnly()),
                StringStackFieldTests = new(new List<string> { "hello", "hello" }.AsReadOnly()),
                ConcurrentStringQueueFieldTests = new(new List<string> { "hello", "hello" }.AsReadOnly()),
                ConcurrentStringStackFieldTests = new(new List<string> { "hello", "hello" }.AsReadOnly()),
                NullableIntField = 10,
                IntLists = new[] { 20, 20 },
                StringISets = { "hello", "hello2" },
                TimeSpanField = DateTime.Now.TimeOfDay,
#if NET6_0_OR_GREATER
                DateOnlyField = DateOnly.FromDateTime(DateTime.Now.Date),
                TimeOnlyField = TimeOnly.FromDateTime(DateTime.Now),
#else
                DateOnlyField = (int)((ulong)DateTime.Now.Date.Ticks / 864000000000UL),
                TimeOnlyField = DateTime.Now.TimeOfDay.Ticks,
#endif
                GuidField = Guid.NewGuid(),
                StringBuilderField = new StringBuilder("hello"),
                DateTimeField = DateTime.UtcNow,
                MapField4s = { [20] = 20 },
                MapFields =
                {
                    ["key1"] = new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
                    ["key2"] = new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
                },

                MapField5s = { ["hello"] = "hello", ["hello2"] = "hello" },
                MapField6s = { ["hello"] = "hello", ["hello2"] = "hello" },
                MapField7s =
                {
                    ["hello"] = new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
                    ["hello2"] = new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
                },
            };

        [TestMethod]
        public void CsMessageDeepCloneTest()
        {
            var origin = NewCsMessage();
            var clone = Serializer.DeepClone(origin, CsTestMessage.ProtoReader, CsTestMessage.ProtoWriter);
            Assert.AreEqual(origin, clone);
        }
    }
}
