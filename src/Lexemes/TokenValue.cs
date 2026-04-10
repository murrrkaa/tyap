using System;
using System.Globalization;

namespace PsTiger.Lexemes;

public class TokenValue
{
    private readonly object _value;

    public TokenValue(string value)
    {
        _value = value;
    }

    public TokenValue(int value)
    {
        _value = value;
    }

    public TokenValue(double value)
    {
        _value = value;
    }

    public override string ToString()
    {
        return _value switch
        {
            string s => s,
            int d => d.ToString(CultureInfo.InvariantCulture),
            double f => f.ToString(CultureInfo.InvariantCulture),
            _ => throw new NotImplementedException(),
        };
    }

    public int ToInt()
    {
        return _value switch
        {
            string s => int.Parse(s, CultureInfo.InvariantCulture),
            int i => i,
            _ => throw new NotImplementedException(),
        };
    }

    public double ToDouble()
    {
        return _value switch
        {
            string s => double.Parse(s, CultureInfo.InvariantCulture),
            double f => f,
            int i => i,
            _ => throw new NotImplementedException(),
        };
    }

    public override bool Equals(object? obj)
    {
        if (obj is TokenValue other)
        {
            return _value switch
            {
                string s => other._value is string os && s == os,
                int i => other._value is int oi && i == oi,
                double f => other._value is double of && f == of,
                _ => throw new NotImplementedException(),
            };
        }

        return false;
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }
}