namespace Arragro.Core.DistributedCache
{
    public interface IDistributedCacheKeyPrefix
    {
        string GeneratePrefix();
    }
}
