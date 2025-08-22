using AwesomeAssertions;
using Dameng.Protobuf.Extension;
using Google.Protobuf;
using SGD.EasyTcp.Test;

namespace Dameng.SepEx.Tests;

public class Tests
{
    [Test]
    public void Test()
    {
        Run(new SubscribeRequest(){ Parameter = Guid.NewGuid().ToString()});
    }

    void Run<T>(T obj) where T : IPbMessageParser<T>, IMessage<T>
    {
        var bytes = obj.ToByteArray();
        var parsed = T.Parser.ParseFrom(bytes);

        var originalJson= JsonFormatter.Default.Format(obj);
        var parsedJson = JsonFormatter.Default.Format(parsed);
        originalJson.Should().Be(parsedJson);
    }
}