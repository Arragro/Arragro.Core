using Microsoft.Extensions.FileProviders;
using System.Collections;
using System.Collections.Generic;

namespace Arragro.Providers.InMemoryStorageProvider
{
    internal class EnumerableDirectoryContents : IDirectoryContents
    {
        private readonly IFileInfo[] _files;

        public EnumerableDirectoryContents(params IFileInfo[] files)
        {
            _files = files;
        }

        public bool Exists => true;

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            foreach (var entry in _files)
            {
                yield return entry;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
