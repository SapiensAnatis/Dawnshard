using System.Numerics;

namespace DragaliaAPI.Extensions;

public static class NumberExtensions
{
    extension<T>(T value) where T : IBinaryInteger<T>
    {
        /// <summary>
        /// Computes the result of adding <paramref name="amount"/> to the value, capped at <paramref name="cap"/>.
        /// </summary>
        /// <param name="amount">The amount to add. Must not be negative.</param>
        /// <param name="cap">The maximum value.</param>
        /// <param name="amountAdded">Out parameter containing the value difference.</param> 
        /// <returns>The lower of <paramref name="cap"/> or the addition result.</returns>
        public T AddWithCap(T amount, T cap, out T amountAdded)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            if (value >= cap)
            {
                amountAdded = T.Zero;
                return value;
            }

            amountAdded = T.Min(amount, cap - value);
            return value + amountAdded;
        }
    }
}

