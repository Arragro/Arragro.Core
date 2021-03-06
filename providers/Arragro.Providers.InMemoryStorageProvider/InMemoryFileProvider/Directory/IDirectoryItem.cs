﻿using System;
using Microsoft.Extensions.FileProviders;

namespace Arragro.Providers.InMemoryStorageProvider.Directory
{
    internal interface IDirectoryItem : IVisitable<BaseDirectoryVisitor>
    {
        IFolderDirectoryItem ParentFolder { get; set; }

        string Name { get; }

        bool IsFolder { get; }

        IFileInfo FileInfo { get; }

        string Path { get; }

        event EventHandler<DirectoryItemUpdatedEventArgs> Updated;
        event EventHandler<DirectoryItemDeletedEventArgs> Deleted;

        void OnRemoved();

        /// <summary>
        /// Deletes the item from the directory.
        /// </summary>
        void Delete();
        void ApplyUpdate(IFileInfo newFileInfo);
    }
}