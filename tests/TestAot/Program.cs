using Google.Protobuf;

TestPackage.TestArrayMessage message = new TestPackage.TestArrayMessage()
{
    Items = { 1, 2, 3, 4, 5 },
};
var bytes = message.ToByteArray();
Console.WriteLine(Convert.ToHexString(bytes));
