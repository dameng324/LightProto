using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightProto.Tests.Parsers
{
    [TestClass]
    public partial class InheritanceAbstractTests
    {
        [ProtoContract(SkipConstructor = true)]
        [ProtoInclude(3, typeof(Message1))]
        [ProtoInclude(4, typeof(Message2))]
        public abstract partial class Base { }

        [ProtoContract(SkipConstructor = true)]
        public partial class Message1 : Base
        {
            [ProtoMember(1)]
            public string Value { get; set; } = "";
        }

        [ProtoContract(SkipConstructor = true)]
        public partial class Message2 : Base
        {
            [ProtoMember(1)]
            public string Value { get; set; } = "";
        }

        public static IEnumerable<object[]> GetMessages()
        {
            yield return new object[] { new Message1 { Value = Guid.NewGuid().ToString() } };
            yield return new object[] { new Message2 { Value = Guid.NewGuid().ToString() } };
        }

        [TestMethod]
        [DynamicData(nameof(GetMessages))]
        public void InheritanceAbstract_Test(Base message)
        {
            var clone = Serializer.DeepClone(message, Base.ProtoReader, Base.ProtoWriter);

            Assert.AreEqual(clone.GetType(), message.GetType());
            if (message is Message1 message1)
            {
                Assert.IsTrue(clone is Message1);
                var cloneMessage = (clone as Message1)!;
                Assert.AreEqual(message1.Value, cloneMessage.Value);
            }
            if (message is Message2 message2)
            {
                Assert.IsTrue(clone is Message2);
                var cloneMessage = (Message2)(clone);
                Assert.AreEqual(message2.Value, cloneMessage.Value);
            }
        }
    }
}
