using System;
using System.Collections.Generic;

namespace lobe
{
    public class Classification : IEquatable<Classification>
    {
        private sealed class LabelConfidenceEqualityComparer : IEqualityComparer<Classification>
        {
            public bool Equals(Classification x, Classification y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Label == y.Label && x.Confidence.Equals(y.Confidence);
            }

            public int GetHashCode(Classification obj)
            {
                unchecked
                {
                    return ((obj.Label != null ? obj.Label.GetHashCode() : 0) * 397) ^ obj.Confidence.GetHashCode();
                }
            }
        }

        public static IEqualityComparer<Classification> LabelConfidenceComparer { get; } = new LabelConfidenceEqualityComparer();

        public bool Equals(Classification other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Label == other.Label && Confidence.Equals(other.Confidence);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Classification) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Label != null ? Label.GetHashCode() : 0) * 397) ^ Confidence.GetHashCode();
            }
        }

        public static bool operator ==(Classification left, Classification right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Classification left, Classification right)
        {
            return !Equals(left, right);
        }

        public Classification(string label, double confidence)
        {
            Label = label;
            Confidence = confidence;
        }

        public string Label { get;}
        public double Confidence { get; }
        public override string ToString()
        {
            return $"{Label} [{Confidence:F4}]";
        }
    }
}