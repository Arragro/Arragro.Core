using System;

namespace Arragro.Providers.InMemoryStorageProvider.Directory
{
    internal class DirectoryItemUpdatedEventArgs : EventArgs
    {
        public DirectoryItemUpdatedEventArgs(IDirectoryItem oldItem, IDirectoryItem newItem)
        {
            OldItem = oldItem;
            NewItem = newItem;
        }
        public IDirectoryItem OldItem { get; private set; }
        public IDirectoryItem NewItem { get; private set; }
    }
}