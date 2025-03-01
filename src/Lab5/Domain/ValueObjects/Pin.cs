namespace Domain.Pins;

public sealed class Pin
{
    public string Value { get; }

    public Pin(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("PIN cannot be empty or whitespace.");
        }

        Value = value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Pin other)
            return Value == other.Value;
        return false;
    }

    public override int GetHashCode()
    {
        return StringComparer.Ordinal.GetHashCode(Value);
    }
}