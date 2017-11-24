namespace Arragro.Providers.InMemoryStorageProvider.Directory
{
    internal interface IVisitable<T>
    {
        void Accept(T Visitor);
    }
}
