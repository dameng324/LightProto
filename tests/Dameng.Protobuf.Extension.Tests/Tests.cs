using AwesomeAssertions;
using Dameng.Protobuf.Extension;
using Google.Protobuf;
using SGD.EasyTcp.Test;

namespace Dameng.Protobuf.Extension.Tests;

public class Tests
{
    [Test]
    public void Test()
    {
        Run(new SubscribeRequest(){ Parameter = Guid.NewGuid().ToString()});
    }

    void Run<T>(T obj) where T : IPbMessageParser<T>
    {
        var bytes = obj.ToByteArray();
        var parsed = T.Parser.ParseFrom(bytes);

        var originalJson= JsonFormatter.Default.Format(obj);
        var parsedJson = JsonFormatter.Default.Format(parsed);
        originalJson.Should().Be(parsedJson);
    }
    T Deserialize<T>(byte[] bytes) where T : IPbMessageParser<T>
    {
        return T.Parser.ParseFrom(bytes);
    }
}