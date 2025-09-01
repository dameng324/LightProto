
using Dameng.Protobuf.Extension;
using Google.Protobuf;

TestPackage.Request request = new TestPackage.Request()
{
    Message = "Hello, World!"
};

byte[] data = Serialize(request);
var deserializedRequest = Deserialize<TestPackage.Request>(data);

Console.WriteLine(deserializedRequest.Message);

T Deserialize<T>(byte[] bytes) where T : IPbMessageParser<T>
{
    return T.Parser.ParseFrom(bytes);
}
byte[] Serialize<T>(T t) where T : IPbMessageParser<T>
{
    return t.ToByteArray();
}