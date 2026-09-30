using System.Collections;

namespace TitanFitness.Domain.Common;

public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public bool Equals(ValueObject? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return GetEqualityComponents()
            .SequenceEqual(
                other.GetEqualityComponents(),
                new ValueObjectEqualityComparer());
    }

    public override bool Equals(object? obj)
    {
        return obj is ValueObject other && Equals(other);
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(
                0,
                (current, obj) => HashCode.Combine(current, obj));
    }

    public static bool operator ==(
        ValueObject? left,
        ValueObject? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(
        ValueObject? left,
        ValueObject? right)
    {
        return !(left == right);
    }

    private sealed class ValueObjectEqualityComparer
        : IEqualityComparer<object?>
    {
        public new bool Equals(object? x, object? y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x is null || y is null)
                return false;

            if (x is IEnumerable xEnumerable &&
                y is IEnumerable yEnumerable &&
                x is not string &&
                y is not string)
            {
                return xEnumerable
                    .Cast<object?>()
                    .SequenceEqual(
                        yEnumerable.Cast<object?>());
            }

            return x.Equals(y);
        }

        public int GetHashCode(object? obj)
        {
            return obj?.GetHashCode() ?? 0;
        }
    }
}