using CsvManager.Properties;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Text;

namespace CsvManager.Tests
{
    [TestClass()]
    public class CsvParserTests
    {
        private Mock<ILogger<CsvParser>> mockLog;

        public CsvParserTests()
        {
            mockLog = new Mock<ILogger<CsvParser>>();
        }

        #region ReadTest
        [TestMethod()]
        public void ReadTest_param_null()
        {
            var parser = new CsvParser(mockLog.Object);
            var actual = parser.Read(null);

            // assert
            Assert.IsFalse(actual);
            mockLog.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == string.Format(Resources.MsgErrFileNotFound, "(null)")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        [TestMethod()]
        public void ReadTest_param_blank()
        {
            // test data
            var path = string.Empty;

            var parser = new CsvParser(mockLog.Object);
            var actual = parser.Read(path);

            // assert
            Assert.IsFalse(actual);
            mockLog.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == string.Format(Resources.MsgErrFileNotFound, string.Empty)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        [TestMethod()]
        public void ReadTest_not_exist_file()
        {
            // test data
            var path = @"csv\test_not_exist.csv";

            var parser = new CsvParser(mockLog.Object);
            var actual = parser.Read(path);

            // assert
            Assert.IsFalse(actual);
            mockLog.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == string.Format(Resources.MsgErrFileNotFound, path)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        [TestMethod()]
        public void ReadTest_blank_csv()
        {
            // test data
            var path = @"csv\test_blank.csv";

            var parser = new CsvParser(mockLog.Object);
            var actual = parser.Read(path);

            // assert
            Assert.IsTrue(actual);
            Assert.IsTrue(string.IsNullOrEmpty(parser.CsvText));
        }

        [TestMethod()]
        public void ReadTest_single_line_csv()
        {
            // test data
            var path = @"csv\test_single_line.csv";
            // expected data
            var expected = File.ReadAllText(path, Encoding.UTF8);

            var parser = new CsvParser(mockLog.Object);
            var actual = parser.Read(path);

            // assert
            Assert.IsTrue(actual);
            Assert.AreEqual(expected, parser.CsvText);
        }

        [TestMethod()]
        public void ReadTest_multiple_lines_csv()
        {
            // test data
            var path = @"csv\test_multiple_lines.csv";
            // expected data
            var expected = File.ReadAllText(path, Encoding.UTF8);

            var parser = new CsvParser(mockLog.Object);
            var actual = parser.Read(path);

            // assert
            Assert.IsTrue(actual);
            Assert.AreEqual(expected, parser.CsvText);
        }
        #endregion ReadTest
    }
}
