using System.Numerics;
using System.Runtime.CompilerServices;
using Google.Protobuf;

namespace Dameng.Protobuf.Extension;

public static class Check
{
    public static bool IsEmpty(string value)
    {
        return value is null || value.Length == 0;
    }

    public static bool IsEmpty(bool? value)
    {
        return value != true;
    }
    public static bool IsEmpty(bool value)
    {
        return value != true;
    }

    public static bool IsEmpty<T>(T? value)
        where T : struct, INumber<T>
    {
        return value.HasValue == false || T.IsZero(value.Value);
    }

    public static bool IsEmpty<T>(T t)
    {
        if (typeof(T) == typeof(bool))
        {
            var value= Unsafe.As<T, bool>(ref t);
            return value!=true;
        }

        if (typeof(T) == typeof(string))
        {
            var value= Unsafe.As<T, string>(ref t);
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            return value is null||value.Length<=0;
        }

        if (typeof(T) == typeof(int))
        {
            var value= Unsafe.As<T, int>(ref t);
            return value ==0;
        }
        if (typeof(T) == typeof(double))
        {
            var value= Unsafe.As<T, double>(ref t);
            return value ==0;
        }

        if (typeof(T) == typeof(ByteString))
        {
            var value= Unsafe.As<T, ByteString>(ref t);
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            return value is null||value.Length<=0;
        }
        if (typeof(T) == typeof(long))
        {
            var value= Unsafe.As<T, long>(ref t);
            return value ==0;
        }
        
        if (typeof(T) == typeof(float))
        {
            var value= Unsafe.As<T, float>(ref t);
            return value ==0;
        }
        if (typeof(T) == typeof(uint))
        {
            var value= Unsafe.As<T, uint>(ref t);
            return value ==0;
        }

        if (typeof(T) == typeof(ulong))
        {
            var value= Unsafe.As<T, ulong>(ref t);
            return value ==0;
        }
        if (typeof(T).IsEnum)
        {
            var value= Unsafe.As<T, int>(ref t);
            return value ==0;
        }

        return t is null;
    }
}
