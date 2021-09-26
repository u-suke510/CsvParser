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
        private List<string> lineText = new List<string>();
        public string CsvText
        {
            get
            {
                if (!lineText.Any())
                {
                    return string.Empty;
                }
                return string.Join(Environment.NewLine, lineText);
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

            using (var sr = new StreamReader(path, Encoding.UTF8))
            {
                while (sr.Peek() > -1)
                {
                    lineText.Add(sr.ReadLine());
                }
            }
            return true;
        }
    }
}
