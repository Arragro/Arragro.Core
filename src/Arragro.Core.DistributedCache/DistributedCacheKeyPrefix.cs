namespace Arragro.Core.DistributedCache
{
    public class DistributedCacheKeyPrefix : IDistributedCacheKeyPrefix
    {
        public string GeneratePrefix()
        {
            return string.Empty;
        }
    }
}
