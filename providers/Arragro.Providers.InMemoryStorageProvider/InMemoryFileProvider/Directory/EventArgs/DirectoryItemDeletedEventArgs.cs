using System;

namespace Arragro.Providers.InMemoryStorageProvider.Directory
{
    internal class DirectoryItemDeletedEventArgs : EventArgs
    {
        public DirectoryItemDeletedEventArgs(IDirectoryItem deletedItem)
        {
            DeletedItem = deletedItem;
        }
        public IDirectoryItem DeletedItem { get; private set; }
    }
}