using System;
using Microsoft.Extensions.FileProviders;

namespace Arragro.Providers.InMemoryStorageProvider.Directory
{
    internal interface IFileDirectoryItem : IDirectoryItem
    {
        void Update(IFileInfo newFileInfo);
        //void Delete();

    }
}