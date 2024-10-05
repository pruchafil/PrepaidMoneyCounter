using PrepaidMoneyCounter.Model;
using PrepaidMoneyCounter.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace PrepaidMoneyCounter.ViewModel
{
    public class MainViewModel
    {
        public class RecordVm
        {
            public Guid Guid { get; set; }

            public string Date { get; set; }

            public string Cost { get; set; }

            public string Message { get; set; }
        }

        private IRecordRepository _recordRepository;
        private IReadOnlyList<Record> _records;

        public List<RecordVm> RecordsVm { get; set; }

        public decimal Balance { get; set; }

        public Brush BalanceColor
        {
            get
            {
                return Balance < 0 ? new SolidColorBrush(Colors.Red) : (Brush)new SolidColorBrush(Colors.Green);
            }
        }

        public MainViewModel(IRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
            Task.Run(async () => await Reload()).Wait();
        }

        public async Task AddRecord(Record record)
        {
            await _recordRepository.Add(record);
        }

        public async Task Reload()
        {
            _records = await _recordRepository.GetAll();
            RecordsVm = _records.Select(x => new RecordVm
            {
                Guid = x.Guid,
                Cost = $"{(x.RecordType == RecordType.Received ? '-' : '+')} {x.Cost}",
                Date = x.DateTime.ToShortDateString(),
                Message = x.Message
            }).ToList();
            Balance = _records.Sum(x => x.RecordType == RecordType.Cost ? x.Cost : -x.Cost);
        }

        public async Task RemoveRecord(Guid guid)
        {
            await _recordRepository.Remove(guid);
        }
    }
}