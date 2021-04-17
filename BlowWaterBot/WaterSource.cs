using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;

namespace BlowWaterBot
{
    public class WaterSource
    {
        private const string _fileName = "Water.csv";

        //private string _directory;
        private string _filePath;

        private List<WaterContent> _blowWaters;
        private Queue<WaterContent> _usedWater;
        private int _usedQueueLength;
        private const float _usedQueueLengthFraction = 0.6f;
        private Random _random;

        public async Task Initialization()
        {
            var directory = Directory.GetCurrentDirectory();

            LogContent($"Directory is : {directory}");
            directory = directory + "/" + _fileName;

            if (File.Exists(directory))
            {
                _filePath = directory;
                LogContent($"$File '{_fileName}' exists");

                await ReadCSV();
            }

            else
            {
                throw new Exception($"Unable to locate {directory}.");
                //LogContent($"Unable to locate {_fileName}");
            }
        }

        private async Task ReadCSV()
        {
            LogContent("Reading from CSV File...");

            using (StreamReader reader = new StreamReader(_filePath))
            using (CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                _blowWaters = csvReader.GetRecords<WaterContent>().ToList();
            }

            LogContent("Finished reading from CSV File.");
            LogContent("Contents from CSV File cached to memory");

            //Initialize usedQueue
            _usedQueueLength = (int) Math.Round(_blowWaters.Count * _usedQueueLengthFraction, 0);
            _usedWater = new Queue<WaterContent>(_usedQueueLength);

            //Initialize Random
            _random = new Random();
            
        }

        public void AddNewWater(string newWater)
        {
            using (StreamWriter writer = new StreamWriter(_filePath))
            using (CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                
            }
        }

        public string GetRandomWater()
        {
            if (_blowWaters == null || _blowWaters.Count == 0) return "No water here";

            int x = _random.Next(0, _blowWaters.Count);
            WaterContent content = _blowWaters[x];

            if (_usedWater.Count == _usedQueueLength)
            {
                WaterContent newcontent = _usedWater.Dequeue();
                _blowWaters.Add(newcontent);
            }

            _blowWaters.RemoveAt(x);
            _usedWater.Enqueue(content);


            return content.BlowWaterContent;
        }

        public void LogContent(string logmsg)
        {
            Console.WriteLine(logmsg);
        }
    }


    public struct WaterContent
    {
        public string BlowWaterContent { get; set; }
        public string Contributor { get; set; }
    }
}