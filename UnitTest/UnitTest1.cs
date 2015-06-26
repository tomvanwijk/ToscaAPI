using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using BKR.Test.ToscaAPI.Shared;

namespace BKR.Test.ToscaAPI.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CompareFilesSame()
        {
            string expected = Path.Combine(Environment.CurrentDirectory, @"Files\TG1.controle.txt");
            string actual = Path.Combine(Environment.CurrentDirectory, @"Files\TG1.out.txt");
            string result = FileComparer.Compare(expected, actual);
            Assert.AreEqual("",result);
        }

        [TestMethod]
        public void CompareFilesExpectedDifferent()
        {
            string expected = Path.Combine(Environment.CurrentDirectory, @"Files\Anders.txt");
            string actual = Path.Combine(Environment.CurrentDirectory, @"Files\TG1.out.txt");
            string result = FileComparer.Compare(expected, actual);
            Assert.AreEqual("ActualFile contains the following differences:" + Environment.NewLine
                                +  "1,3,3,2015-11-26,3,2014-12-01,2,2014-08-24,3,2015-03-19", result);

        }

        [TestMethod]
        public void CompareFilesActualDifferent()
        {
            string expected = Path.Combine(Environment.CurrentDirectory, @"Files\TG1.controle.txt");
            string actual = Path.Combine(Environment.CurrentDirectory, @"Files\Anders.txt");
            string result = FileComparer.Compare(expected, actual);
            Assert.AreEqual("ActualFile contains the following differences:" + Environment.NewLine
                                + "1,3,3,2015-11-26,3,2015-12-01,2,2014-08-24,3,2015-03-19", result);

        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void CompareFilesActualNotExists()
        {
            string expected = Path.Combine(Environment.CurrentDirectory, @"Files\TG1.controle.txt");
            string actual = Path.Combine(Environment.CurrentDirectory, @"Files\bestaatniet.txt");
            string result = FileComparer.Compare(expected, actual);
            Assert.AreEqual("", result);

        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void CompareFilesExpectedNotExists()
        {
            string expected = Path.Combine(Environment.CurrentDirectory, @"Files\bestaatniet.txt");
            string actual = Path.Combine(Environment.CurrentDirectory, @"Files\TG1.out.txt");
            string result = FileComparer.Compare(expected, actual);
            Assert.AreEqual("", result);
        }

    }
}
