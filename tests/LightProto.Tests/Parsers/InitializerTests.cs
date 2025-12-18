using System.Net;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class InitializerTests : BaseProtoBufTests<InitializerTests.Message>
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public partial record Message
    {
        public string Value { get; set; } = "";

        public string Value2 { get; set; } = string.Empty;

        public string Value3 { get; set; } =
            new Func<int, string>((int a) => "default" + a.ToString())(1);

        public string[] Value4 { get; set; } = new[] { "one", "two" };

        public string Value5 { get; set; } = nameof(HttpStatusCode.OK);
        public string? Value6 { get; set; } = default;
        public string? Value7 { get; set; } = default(string);
        public HttpStatusCode Value8 { get; set; } = default(HttpStatusCode);
        public IEnumerable<int> Value9 { get; set; } = Enumerable.Repeat<int>(0, 10).ToArray();
        public HttpStatusCode[] Value10 { get; set; } = new[] { HttpStatusCode.OK };
        public List<HttpStatusCode> Value11 { get; set; } =
            Enumerable.Repeat<HttpStatusCode>(HttpStatusCode.OK, 10).ToList();
        public HttpStatusCode[] Value12 { get; set; } = [HttpStatusCode.OK];
        public List<HttpStatusCode> Value13 { get; set; } = new(10) { HttpStatusCode.OK };
        public List<HttpStatusCode> Value14 { get; set; } =
            new List<HttpStatusCode>(10) { HttpStatusCode.OK };
        public List<HttpStatusCode> Value15 { get; set; } = new[] { HttpStatusCode.OK }.ToList();
        public string Value16 { get; set; } = Initializer.GetDefaultValue();
        public string Value17 { get; set; } = Initializer.GetInitializer().ToString()!;
        public string Value18 { get; set; } = new Initializer().ToString()!;
        public string Value19 = new Initializer().ToString()!;
        public Initializer Value20 = new Initializer();
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public partial class Initializer
    {
        public string Value { get; set; } = "";

        public static string GetDefaultValue()
        {
            return "defaultValueFromMethod";
        }

        public static Initializer GetInitializer()
        {
            return new Initializer();
        }
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new Message { Value = "base" };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Value).IsEqualTo(message.Value);
        await Assert.That(clone.Value2).IsEqualTo(message.Value2);
        await Assert.That(clone.Value3).IsEqualTo(message.Value3);
        await Assert.That(clone.Value4).IsEquivalentTo(message.Value4);
        await Assert.That(clone.Value5).IsEqualTo(message.Value5);
        await Assert.That(clone.Value6).IsEqualTo(message.Value6);
        await Assert.That(clone.Value7).IsEqualTo(message.Value7);
        await Assert.That(clone.Value8).IsEqualTo(message.Value8);
        await Assert.That(clone.Value9).IsEquivalentTo(message.Value9);
        await Assert.That(clone.Value10).IsEquivalentTo(message.Value10);
        await Assert.That(clone.Value11).IsEquivalentTo(message.Value11);
        await Assert.That(clone.Value12).IsEquivalentTo(message.Value12);
        await Assert.That(clone.Value13).IsEquivalentTo(message.Value13);
        await Assert.That(clone.Value14).IsEquivalentTo(message.Value14);
        await Assert.That(clone.Value15).IsEquivalentTo(message.Value15);
        await Assert.That(clone.Value16).IsEqualTo(message.Value16);
        await Assert.That(clone.Value17).IsEqualTo(message.Value17);
        await Assert.That(clone.Value18).IsEqualTo(message.Value18);
        await Assert.That(clone.Value19).IsEqualTo(message.Value19);
    }
}
