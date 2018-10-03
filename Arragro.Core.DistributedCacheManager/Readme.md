## Distributed Cache

To use:

```csharp
serviceCollection.AddDistributedMemoryCache();
serviceCollection.AddSingleton<DistributedCacheManager>();
serviceCollection.AddSingleton(new DistributedCacheEntryOptions { SlidingExpiration = new TimeSpan(0, 20, 0) });
```