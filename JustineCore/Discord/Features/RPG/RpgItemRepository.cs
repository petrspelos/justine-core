using JustineCore.Storage;
using System.Collections.Generic;
using System.Linq;

namespace JustineCore.Discord.Features.RPG
{
    public class RpgRepository
    {
        private const string DataStorageKey = "RpgItemRepository";
        private readonly List<RpgItem> _items;

        public RpgRepository(IDataStorage dataStorage)
        {
            try
            {
                _items = dataStorage.Get<List<RpgItem>>(DataStorageKey);
            }
            catch (DataStorageKeyDoesNotExistException)
            {
                _items = new List<RpgItem>
                {
                    new RpgItem
                    {
                        Id = 1,
                        Name = "Gold",
                        IconUrl = "?"
                    }
                };
                dataStorage.Store(_items, DataStorageKey);
            }
        }

        /// <summary>
        /// Returns an RPG Item based on its ID. Null if not found.
        /// </summary>
        public RpgItem GetItemById(uint itemId)
        {
            return _items.FirstOrDefault(i => i.Id == itemId);
        }

        /// <summary>
        /// Returns an RPG Item based on its name. Null if not found. First if multiple found.
        /// Is not case sensitive.
        /// </summary>
        public RpgItem GetItemByName(string name)
        {
            return _items.FirstOrDefault(i => i.Name.ToLower() == name.ToLower());
        }
    }
}