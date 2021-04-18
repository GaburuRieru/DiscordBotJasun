using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Discord;
using Discord.WebSocket;

namespace BlowWaterBot
{
    public class WaterSource
    {
        // public WaterSource()
        // {
        //     Initialize();
        // }
        
        private const string _fileName = "Water.csv";

        private string _filePath = "";

        private List<WaterContent> _blowWaters = new List<WaterContent>();
        private Queue<WaterContent> _usedWater = new Queue<WaterContent>();
        private int _usedQueueLength;
        private const float _usedQueueLengthFraction = 0.6f;
        private Random _random;

        public async Task Initialize()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), _fileName);

            if (!File.Exists(path))
            {
                throw new Exception($"Unable to locate {path}.");
            }

            _filePath = path;
            
            //Iniitalize usedQueue
            _usedWater = new Queue<WaterContent>();
            
            //LoadWater from CSV
            await ReadCSV();
            
            LogContent("Water source initialized.");
        }

        private async Task ReadCSV()
        {
            if (_blowWaters.Count > 0) _blowWaters.Clear();

            LogContent("Reading from CSV File...");

            CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                ShouldQuote = (quote) => true,
            };

            await using (FileStream stream = File.OpenRead(_filePath))
            using (StreamReader reader = new StreamReader(stream))
            using (CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // _blowWaters = csvReader.GetRecords<WaterContent>().ToList();
                var recordsAsync =  csvReader.GetRecordsAsync<WaterContent>();
                _blowWaters = await recordsAsync.ToListAsync();
            }

            LogContent("Finished reading from CSV File.");
            LogContent("Contents from CSV File cached to memory");

            //update usedqueue
            _usedWater.Clear();
            _usedQueueLength = (int) Math.Round(_blowWaters.Count * _usedQueueLengthFraction, 0);

            //Initialize Random
            _random = new Random();

        }


        public async Task AddLineToCsv(string water, string contributor, IMessageChannel channel)
        {
            string finalwater = $"{water}";
            
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                ShouldQuote = (quote) => true,
                HasHeaderRecord = false,
            };
            
            await using (FileStream stream = File.Open(_filePath, FileMode.Append))
            using (StreamWriter writer = new StreamWriter(stream))
            using (CsvWriter csvWriter = new CsvWriter(writer, configuration))
            {
                csvWriter.WriteRecord<WaterContent>(new WaterContent(finalwater, contributor));
            }
            
            
            LogContent("Added new water content into file Water.csv");
            await channel.SendMessageAsync($"{contributor} added \"{water}\"");
            
            await ReloadWater();
        }

        public async Task ReloadWater()
        {
            LogContent("Reloading water content.");
            await ReadCSV();
        }

        public async Task SayWater(ISocketMessageChannel channel)
        {
            if (_blowWaters == null || _blowWaters.Count == 0)
            {
                LogContent("No water available to blow.");
                return;
            }

            int x = _random.Next(0, _blowWaters.Count);
            WaterContent content = _blowWaters[x];

            if (_usedWater.Count == _usedQueueLength)
            {
                WaterContent newcontent = _usedWater.Dequeue();
                _blowWaters.Add(newcontent);
            }

            _blowWaters.RemoveAt(x);
            _usedWater.Enqueue(content);

            await channel.SendMessageAsync(content.BlowWaterContent);
            return;
        }

        private void LogContent(string logmsg)
        {
            Console.WriteLine(logmsg);
        }
    }


    public  struct WaterContent
    {
        public WaterContent(string water, string contributor)
        {
            BlowWaterContent = water;
            Contributor = contributor;
        }

        public string BlowWaterContent { get; set; }
        public string Contributor { get; set; }
    }
}