using Newtonsoft.Json;
using PrepaidMoneyCounter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace PrepaidMoneyCounter.Repository
{
    public interface IRecordRepository
    {
        Task<IReadOnlyList<Record>> GetAll();
        Task Remove(Guid guid);
        Task Add(Record record);
    }

    public class RecordRepository : IRecordRepository
    {
        private string FilePath
        {
            get
            {
                return "records.json";
            }
        }

        private async Task CreateNew()
        {
            IStorageItem check = await ApplicationData.Current.LocalFolder.TryGetItemAsync(FilePath);

            if (check != null)
            {
                return;
            }

            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(FilePath,
                CreationCollisionOption.FailIfExists);

            await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(new List<Record>(), Formatting.Indented));
        }

        public async Task Add(Record record)
        {
            await CreateNew();

            IReadOnlyList<Record> all = await GetAll();
            List<Record> list = all.Append(record).ToList();

            string str = JsonConvert.SerializeObject(list, Formatting.Indented);
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(FilePath);
            await FileIO.WriteTextAsync(file, str);
        }

        public async Task<IReadOnlyList<Record>> GetAll()
        {
            await CreateNew();

            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(FilePath);
            string content = await FileIO.ReadTextAsync(file);
            List<Record> list = JsonConvert.DeserializeObject<List<Record>>(content);

            return list;
        }

        public async Task Remove(Guid guid)
        {
            await CreateNew();

            IReadOnlyList<Record> all = await GetAll();
            List<Record> list = all.Where(x => x.Guid != guid).ToList();

            string str = JsonConvert.SerializeObject(list, Formatting.Indented);
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(FilePath);
            await FileIO.WriteTextAsync(file, str);
        }
    }
}