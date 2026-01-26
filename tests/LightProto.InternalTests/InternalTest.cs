using System.Collections;
using System.Numerics;
using LightProto.Parser;

namespace LightProto.InternalTests;

public class InternalTest
{
    [Test]
    public async Task PlaneNullFloatsArray_Should_ParseToDefault()
    {
        var protoParser = new PlaneProtoParser() { Floats = null! };

        Plane result = protoParser;
        await Assert.That(result).IsEqualTo(default(Plane));
    }

    [Test]
    public async Task PlaneFloatsArrayWithInvalidLength_Should_ThrowException()
    {
        var protoParser = new PlaneProtoParser()
        {
            Floats = new float[] { 1, 2, 3 }, // Invalid length
        };

        var exception = await Assert
            .That(() =>
            {
                Plane result = protoParser;
            })
            .Throws<ArgumentException>();
        await Assert.That(exception!.Message).Contains("Input array must contain 4 elements for Plane conversion.");
    }

    [Test]
    public async Task QuaternionNullFloatsArray_Should_ParseToDefault()
    {
        var protoParser = new QuaternionProtoParser() { Floats = null! };

        Quaternion result = protoParser;
        await Assert.That(result).IsEqualTo(default(Quaternion));
    }

    [Test]
    public async Task QuaternionFloatsArrayWithInvalidLength_Should_ThrowException()
    {
        var protoParser = new QuaternionProtoParser()
        {
            Floats = new float[] { 1, 2, 3 }, // Invalid length
        };

        var exception = await Assert
            .That(() =>
            {
                Quaternion result = protoParser;
            })
            .Throws<ArgumentException>();
        await Assert.That(exception!.Message).Contains("Input array must contain 4 elements for Quaternion conversion.");
    }

    [Test]
    public async Task Matrix3x2NullFloatsArray_Should_ParseToDefault()
    {
        var protoParser = new Matrix3x2ProtoParser() { Floats = null! };

        Matrix3x2 result = protoParser;
        await Assert.That(result).IsEqualTo(default(Matrix3x2));
    }

    [Test]
    public async Task Matrix3x2FloatsArrayWithInvalidLength_Should_ThrowException()
    {
        var protoParser = new Matrix3x2ProtoParser()
        {
            Floats = new float[] { 1, 2, 3 }, // Invalid length
        };

        var exception = await Assert
            .That(() =>
            {
                Matrix3x2 result = protoParser;
            })
            .Throws<ArgumentException>();
        await Assert.That(exception!.Message).Contains("Input array must contain 6 elements for Matrix3x2 conversion.");
    }

    [Test]
    public async Task Matrix4x4NullFloatsArray_Should_ParseToDefault()
    {
        var protoParser = new Matrix4x4ProtoParser() { Floats = null! };

        Matrix4x4 result = protoParser;
        await Assert.That(result).IsEqualTo(default(Matrix4x4));
    }

    [Test]
    public async Task Matrix4x4FloatsArrayWithInvalidLength_Should_ThrowException()
    {
        var protoParser = new Matrix4x4ProtoParser()
        {
            Floats = new float[] { 1, 2, 3 }, // Invalid length
        };

        var exception = await Assert
            .That(() =>
            {
                Matrix4x4 result = protoParser;
            })
            .Throws<ArgumentException>();
        await Assert.That(exception!.Message).Contains("Input array must contain 16 elements for Matrix4x4 conversion.");
    }

    [Test]
    public async Task BitArrayProtoParser_ShouldBeEmpty_WhenBitArrayIsNull()
    {
        BitArray bitArray = null!;
        BitArrayProtoParser parser = bitArray;
        await Assert.That(parser.Bits).IsEmpty();
    }

    [Test]
    public async Task BitArrayProtoParser_ShouldHandleNullBits_WhenDeserializing()
    {
        BitArrayProtoParser parser = new BitArrayProtoParser() { Bits = null! };
        BitArray bitArray = parser;
        await Assert.That(bitArray.Length).IsEqualTo(0);
    }

    [Test]
    public async Task IsNormalizedTest()
    {
        DateTime240ProtoParser parser = new() { Nanos = -1 };
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            DateTime dateTime = parser;
        });
        await Assert.That(ex.Message).Contains("contains invalid values");
    }

    [Test]
    public async Task MessageWrapperTest()
    {
        var protoWriter = MessageWrapper<int>.ProtoWriter.From(Int32ProtoParser.ProtoWriter);
        int a = 1234;
        var longSize = protoWriter.CalculateLongSize(a);
        var size = protoWriter.CalculateSize(a);
        await Assert.That(longSize).IsEqualTo(size);
    }
}
