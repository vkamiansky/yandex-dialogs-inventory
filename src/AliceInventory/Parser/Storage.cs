using System.Collections.Generic;

namespace ConsoleApp
{
    class Storage: IStorageInterface
    {
        HashSet<Entry> storage; //contains unique elements of Entries of user list
        public void Add(Entry newEntry)
        {
            if(storage.Contains(newEntry)){
                Entry oldEntry = new Entry();
                storage.TryGetValue(newEntry, out oldEntry);
                Entry EntryToAdd = new Entry
                (
                    newEntry.ItemName, 
                    oldEntry.ItemCount + newEntry.ItemCount, 
                    oldEntry.Unit
                );
                storage.Remove(oldEntry);
                storage.Add(EntryToAdd);
            } 
            else
            {
                storage.Add(newEntry);
            }
        }
        public void DeleteEntry(Entry entry)
        {
            if(storage.Contains(entry))
            {
                storage.Remove(entry);
            }
        }
        public HashSet<Entry> ReadAll()
        {
            return storage;
        }
        public void ClearAll()
        {
            storage.Clear();
        }
    }
}
