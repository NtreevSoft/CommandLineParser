using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ntreev;
using System.IO;
using Ntreev.Library;
using CommandLineParserTest.Options;

namespace CommandLineParserTest
{
    /// <summary>
    /// Test의 요약 설명
    /// </summary>
    [TestClass]
    public class Test
    {
        public Test()
        {
            //
            // TODO: 여기에 생성자 논리를 추가합니다.
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///현재 테스트 실행에 대한 정보 및 기능을
        ///제공하는 테스트 컨텍스트를 가져오거나 설정합니다.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 추가 테스트 특성
        //
        // 테스트를 작성할 때 다음 추가 특성을 사용할 수 있습니다.
        //
        // ClassInitialize를 사용하여 클래스의 첫 번째 테스트를 실행하기 전에 코드를 실행합니다.
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // ClassCleanup을 사용하여 클래스의 테스트를 모두 실행한 후에 코드를 실행합니다.
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 테스트를 작성할 때 다음 추가 특성을 사용할 수 있습니다. 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // TestInitialize를 사용하여 각 테스트를 실행하기 전에 코드를 실행합니다.
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void BaseTypeSwitchesTest()
        {
            BaseTypeSwitches options = new BaseTypeSwitches();
            CommandLineParser parser = new CommandLineParser();

            string path = Path.GetTempFileName();
            parser.Parse(@"Test.exe /Text ""this is a string"" /Number 5 /Boolean /Path """ + path + @""" /AttributeTargets ""Assembly,Constructor""" , options);

            Assert.AreEqual("this is a string", options.Text);
            Assert.AreEqual(5, options.Number);
            Assert.AreEqual(true, options.Boolean);
            Assert.AreEqual(path, options.Path.FullName);
            Assert.AreEqual(AttributeTargets.Assembly | AttributeTargets.Constructor, options.AttributeTargets);
        }

        [TestMethod]
        public void SwitchDelimiterTest()
        {
            try
            {
                SwitchAttribute.SwitchDelimiter = 'a';
                Assert.Inconclusive();
            }
            catch (Exception)
            {

            }

            try
            {
                SwitchAttribute.SwitchDelimiter = '1';
                Assert.Inconclusive();
            }
            catch (Exception)
            {

            }

            try
            {
                SwitchAttribute.SwitchDelimiter = ' ';
                Assert.Inconclusive();
            }
            catch (Exception)
            {

            }

            try
            {
                SwitchAttribute.SwitchDelimiter = '\"';
                Assert.Inconclusive();
            }
            catch (Exception)
            {

            }
        }

        [TestMethod]
        public void RequiredSwitchesTest()
        {
            RequiredSwitches options = new RequiredSwitches();
            StringBuilder arg = new StringBuilder();
            CommandLineParser parser = new CommandLineParser();

            arg.Append("Test.exe");

            // 인자가 하나도 없을때의 예외 테스트
            try
            {
                parser.Parse(arg.ToString(), options);
                Assert.Inconclusive("예외가 발생하지 않았습니다.");
            }
            catch (ArgumentException)
            {
                
            }
            arg.Append(" /Index 5");    

            // text 인자 테스트
            try
            {
                parser.Parse(arg.ToString(), options);
                Assert.Inconclusive("예외가 발생하지 않았습니다.");
            }
            catch (MissingSwitchException e)
            {
                Assert.AreEqual("Text", e.SwitchName);
            }
            arg.Append(@" /Text ""this is a string""");

            // number 인자 테스트
            try
            {
                parser.Parse(arg.ToString(), options);
                Assert.Inconclusive("예외가 발생하지 않았습니다.");
            }
            catch (MissingSwitchException e)
            {
                Assert.AreEqual("Number", e.SwitchName);
            }

            // 최종적으로 필요한 인자를 모두 추가후 마지막 테스트
            arg.Append(" /Number 4");
            try
            {
                parser.Parse(arg.ToString(), options);
            }
            catch (Exception)
            {
                Assert.Inconclusive("예외가 발생하지 말아야 합니다.");
            }

            // 마지막 값 비교 테스트
            Assert.AreEqual(5, options.Index);
            Assert.AreEqual("this is a string", options.Text);
            Assert.AreEqual(4, options.Number);
        }

        [TestMethod]
        public void ArgSeperatorSwitchesTest()
        {
            ArgSeperatorSwitches options = new ArgSeperatorSwitches();

            CommandLineParser parser = new CommandLineParser();

            parser.Parse("Test.exe /Level5 /IsAlive:true", options);

            Assert.AreEqual(5, options.Level);
            Assert.AreEqual(true, options.IsAlive);

            parser.Parse("Test.exe /Level-1 /IsAlive:false", options);

            Assert.AreEqual(-1, options.Level);
            Assert.AreEqual(false, options.IsAlive);
        }

        [TestMethod]
        public void DuplicatedOptionsTest()
        {
            DuplicatedOptions options = new DuplicatedOptions();
            CommandLineParser parser = new CommandLineParser();

            try
            {
                parser.Parse("Test.exe /index 5", options);
                Assert.Inconclusive("예외가 발생하지 않았습니다.");
            }
            catch (SwitchException e)
            {
                Assert.AreEqual("index", e.SwitchName);
            }
        }

        [TestMethod]
        public void SwitchTypeArgTest()
        {

        }
    }
}
