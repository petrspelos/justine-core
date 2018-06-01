using System;
using JustineCore.Storage;

namespace JustineCore.Discord.Providers
{
    public class AdventureGroupProvider
    {
        private IDataStorage _s;

        public AdventureGroupProvider(IDataStorage storage)
        {
            _s = storage;
        }
    }
}