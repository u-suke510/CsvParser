using CsvManager.Properties;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CsvManager
{
    public interface ICsvParser
    {
        bool Read(string path);
    }

    public class CsvParser : ICsvParser
    {
        private readonly ILogger<CsvParser> logger;
        private List<string[]> csvData;
        public List<string[]> CsvData
        {
            get
            {
                return csvData;
            }
        }

        public CsvParser(ILogger<CsvParser> logger)
        {
            this.logger = logger;
        }

        public bool Read(string path)
        {
            if (!File.Exists(path))
            {
                logger.LogError(Resources.MsgErrFileNotFound, path);
                return false;
            }

            var items = new List<string[]>();
            using (var sr = new StreamReader(path, Encoding.UTF8))
            {
                while (sr.Peek() > -1)
                {
                    var item = getColumnItems(sr.ReadLine());
                    if (item.result)
                    {
                        items.Add(item.value);
                    }
                }
            }

            csvData = items;
            return true;
        }

        public (bool result, string[] value) getColumnItems(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                logger.LogInformation(Resources.MsgInfoTextLineBlank);
                return (false, null);
            }

            return (true, line.Split(","));
        }
    }
}
