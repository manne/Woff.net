using System;
using System.Diagnostics.Contracts;

namespace Blocker
{
    public class Block : IEquatable<Block>
    {
        public Block(uint start, uint end)
        {
            Contract.Requires(start < end);

            Start = start;
            End = end;
        }

        public static Block CreateFromStartAndDistance(uint start, uint distance)
        {
            Contract.Requires(distance > 0);
            Contract.Ensures(Contract.Result<Block>() != null);

            return new Block(start, start + distance);
        }

        public uint Start { get; private set; }

        public uint End { get; private set; }

        #region IEquatable

        public bool Equals(Block other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Start == other.Start && End == other.End;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is Block && Equals((Block)obj);
        }

        public override int GetHashCode()
        {
            return ((int)Start * 397) ^ (int)End;
        }

        public static bool operator ==(Block left, Block right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Block left, Block right)
        {
            return !Equals(left, right);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Start: {0}, End: {1}", Start, End);
        }
    }
}
