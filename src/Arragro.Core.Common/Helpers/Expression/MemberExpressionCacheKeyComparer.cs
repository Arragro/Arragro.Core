using Microsoft.DotNet.PlatformAbstractions;
using System.Collections.Generic;

namespace Arragro.Core.Common.Helpers
{
    internal class MemberExpressionCacheKeyComparer : IEqualityComparer<MemberExpressionCacheKey>
    {
        public static readonly MemberExpressionCacheKeyComparer Instance = new MemberExpressionCacheKeyComparer();

        public bool Equals(MemberExpressionCacheKey x, MemberExpressionCacheKey y)
        {
            if (x.ModelType != y.ModelType)
            {
                return false;
            }

            var xEnumerator = x.GetEnumerator();
            var yEnumerator = y.GetEnumerator();

            while (xEnumerator.MoveNext())
            {
                if (!yEnumerator.MoveNext())
                {
                    return false;
                }

                // Current is a MemberInfo instance which has a good comparer.
                if (xEnumerator.Current != yEnumerator.Current)
                {
                    return false;
                }
            }

            return !yEnumerator.MoveNext();
        }

        public int GetHashCode(MemberExpressionCacheKey obj)
        {
            var hashCodeCombiner = new HashCodeCombiner();
            hashCodeCombiner.Add(obj.ModelType);

            foreach (var member in obj)
            {
                hashCodeCombiner.Add(member);
            }

            return hashCodeCombiner.CombinedHash;
        }
    }
}
