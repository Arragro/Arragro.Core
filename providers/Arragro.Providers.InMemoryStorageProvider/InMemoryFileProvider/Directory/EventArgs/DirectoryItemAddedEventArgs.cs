using System;

namespace Arragro.Providers.InMemoryStorageProvider.Directory
{
    internal class DirectoryItemAddedEventArgs : EventArgs
    {
        public DirectoryItemAddedEventArgs(IDirectoryItem newItem)
        {
            NewItem = newItem;
        }
        public IDirectoryItem NewItem { get; private set; }
    }
}