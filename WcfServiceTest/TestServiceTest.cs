using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WcfServiceTest.Utils;
using WcfServiceLibrary.Utils;
using WcfServiceLibrary;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WcfServiceTest
{
    [TestClass]
    public class TestServiceTest
    {
        private const string TestString = "TestString";
        private const string ServerName = "Server#1";

        private static TestService _testService;

        [ClassInitialize]
        public static void TestServiceTestInitialize(TestContext context)
        {
            _testService = new TestService(ServerName);
        }

        [TestMethod]
        public void GetStringDataTest()
        {
            //Arrange
            const int TestStringsCount = 100;
            const int RandomStringSize = 100;
            string[] testStrings = RandomUtils.GetArrayOfRandomStrings(TestStringsCount, RandomStringSize);
            string[] expectedResultStrings = EncryptionUtils.HashSHA1StringArray(testStrings);
            
            //Act
            string[] resultStrings = new string[TestStringsCount];
            for (int i = 0; i < TestStringsCount; i++)
            {
                resultStrings[i] = _testService.GetStringData(testStrings[i]);
            }

            //Assert
            for (int i = 0; i < TestStringsCount; i++)
            {
                Assert.AreEqual(ServerName + "_" + expectedResultStrings[i], resultStrings[i]);
            }
        }

        [TestMethod]
        public void GetDataUsingDataContractTestPutFalse()
        {
            //Arrange
            CompositeType testCompositeType = new CompositeType();
            testCompositeType.BoolValue = true;
            testCompositeType.StringValue = TestString;
            
            //Act
            CompositeType resultCompositeType = _testService.GetDataUsingDataContract(testCompositeType);
            
            //Assert
            Assert.AreEqual("TRUE", resultCompositeType.StringValue);
        }

        [TestMethod]
        public void GetDataUsingDataContractTestPutTrue()
        {
            //Arrange
            CompositeType testCompositeType = new CompositeType();
            testCompositeType.BoolValue = false;
            testCompositeType.StringValue = TestString;

            //Act
            CompositeType resultCompositeType = _testService.GetDataUsingDataContract(testCompositeType);

            //Assert
            Assert.AreEqual("FALSE", resultCompositeType.StringValue);
        }

        [TestMethod]
        public void GetDataUsingDataContractTestInnerType()
        {
            //Arrange
            CompositeType testCompositeType = new CompositeType();
            testCompositeType.BoolValue = false;
            testCompositeType.StringValue = TestString;

            const int TestStringsCount = 100;
            const int RandomStringSize = 100;
            string[] testStrings = RandomUtils.GetArrayOfRandomStrings(TestStringsCount, RandomStringSize);
            string[] expectedResultStrings = EncryptionUtils.HashSHA1StringArray(testStrings);

            InnerCompositeType testInnerCompositeType = new InnerCompositeType();
            for (int i = 0; i < testStrings.Length; i++)
            {
                testInnerCompositeType.ListOfStrings.Add(testStrings[i]);
            }
            testInnerCompositeType.ServerName = TestString;
            testCompositeType.InnerCompositeType = testInnerCompositeType;

            //Act
            CompositeType resultCompositeType = _testService.GetDataUsingDataContract(testCompositeType);

            //Assert
            Assert.AreEqual(ServerName, resultCompositeType.InnerCompositeType.ServerName);
            CollectionAssert.AreEqual(expectedResultStrings, (List<string>)resultCompositeType.InnerCompositeType.ListOfStrings);
        }

        [TestMethod]
        public void GetLargeDataTest()
        {
            //Arrange
            byte[] testBytes = Encoding.UTF8.GetBytes(TestString);
            byte[] nullBytes = new byte[TestString.Length];
            for (int i = 0; i < TestString.Length; i++) {
                nullBytes[i] = 0;
            }

            using (Stream testStream = new MemoryStream(testBytes))
            {
                InputStreamMessage testMessage = new InputStreamMessage();
                testMessage.Data = testStream;
                testMessage.NullCount = TestString.Length;

            //Act
                OutputStreamMessage resultStreamMessage = _testService.GetLargeData(testMessage);
            
            //Assert
                Assert.AreEqual(ServerName, resultStreamMessage.ServerName);
                using (Stream resultStream = resultStreamMessage.Data)
                {
                    byte[] buffer = new byte[TestString.Length];
                    resultStream.Read(buffer, 0, buffer.Length);
                    CollectionAssert.AreEqual(testBytes, buffer);
                    resultStream.Read(buffer, 0, buffer.Length);
                    CollectionAssert.AreEqual(nullBytes, buffer);
                }
            }
                      
        }
    }
}
