using CsvManager.Properties;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
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

        #region getColumnItemsTest TODO: 後でプライベートメソッドに変更するので削除予定。
        [TestMethod()]
        public void getColumnItemsTest_param_null()
        {
            var parser = new CsvParser(mockLog.Object);
            var actual = parser.getColumnItems(null);

            // assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.result);
            Assert.IsNull(actual.value);
            mockLog.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == Resources.MsgInfoTextLineBlank),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        [TestMethod()]
        public void getColumnItemsTest_param_blank()
        {
            // test data
            var line = string.Empty;

            var parser = new CsvParser(mockLog.Object);
            var actual = parser.getColumnItems(line);

            // assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.result);
            Assert.IsNull(actual.value);
            mockLog.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == Resources.MsgInfoTextLineBlank),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        [TestMethod()]
        public void getColumnItemsTest_param_no_separator()
        {
            // test data
            var line = "col1col2col3";

            var parser = new CsvParser(mockLog.Object);
            var actual = parser.getColumnItems(line);

            // assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.result);
            Assert.AreEqual(1, actual.value.Length);
            Assert.AreEqual(line, actual.value[0]);
        }

        [TestMethod()]
        public void getColumnItemsTest_param_cols3()
        {
            // test data
            var line = "col1,col2,col3";
            // expected data
            var expected = new List<string> { "col1", "col2", "col3" };

            var parser = new CsvParser(mockLog.Object);
            var actual = parser.getColumnItems(line);

            // assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.result);
            Assert.AreEqual(3, actual.value.Length);
            Assert.AreEqual(expected[0], actual.value[0]);
            Assert.AreEqual(expected[1], actual.value[1]);
            Assert.AreEqual(expected[2], actual.value[2]);
        }
        #endregion getColumnItemsTest

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
            Assert.AreEqual(0, parser.CsvData.Count);
        }

        [TestMethod()]
        public void ReadTest_single_line_csv()
        {
            // test data
            var path = @"csv\test_single_line.csv";
            // expected data
            var expected = new string[] { "column1", "column2", "column3" };

            var parser = new CsvParser(mockLog.Object);
            var actual = parser.Read(path);

            // assert
            Assert.IsTrue(actual);
            Assert.AreEqual(1, parser.CsvData.Count);
            CollectionAssert.AreEqual(expected, parser.CsvData[0]);
        }

        [TestMethod()]
        public void ReadTest_multiple_lines_csv()
        {
            // test data
            var path = @"csv\test_multiple_lines.csv";
            // expected data
            var expected = new List<string[]> {
                new string[] { "row1-col1", "row1-col2", "row1-col3", "row1-col4", "row1-col5" },
                new string[] { "row2-col1", "row2-col2", "row2-col3", "row2-col4", "row2-col5" },
                new string[] { "row3-col1", "row3-col2", "row3-col3", "row3-col4", "row3-col5" }
            };

            var parser = new CsvParser(mockLog.Object);
            var actual = parser.Read(path);

            // assert
            Assert.IsTrue(actual);
            Assert.AreEqual(3, parser.CsvData.Count);
            CollectionAssert.AreEqual(expected[0], parser.CsvData[0]);
            CollectionAssert.AreEqual(expected[1], parser.CsvData[1]);
            CollectionAssert.AreEqual(expected[2], parser.CsvData[2]);
        }
        #endregion ReadTest
    }
}
