using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using LogicLayer;

namespace UnitTest
{


    [TestClass]
    public class XMLDALTests
    {

        string TestFolder = "";
        [TestInitialize]
        public void Initialize()
        {
            string binPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string rootFolder = binPath.Substring(0, binPath.IndexOf(@"\bin\"));
            TestFolder = rootFolder + "\\TestFiles";
            if (!Directory.Exists(TestFolder))
                Directory.CreateDirectory(TestFolder);
            HouseKeeping.RootFolder = TestFolder;
        }

        [TestMethod]
        public void GetSystemFolderTest()
        {
            string SystemFolder = HouseKeeping.GetSystemFolder();
            Assert.AreEqual(TestFolder + "\\System", SystemFolder);
        }

        [TestMethod]
        public void CreateDirectory()
        {
            string SystemFolder = HouseKeeping.GetSystemFolder();
            HouseKeeping.CreateDirectory(TestFolder + HouseKeeping.Slash + "NewProject\\Documents");
            HouseKeeping.CreateDirectory(TestFolder + HouseKeeping.Slash + "NewProject\\Code");
            HouseKeeping.CreateDirectory(TestFolder + HouseKeeping.Slash + "NewProject\\Communication\\Email");

            Assert.IsTrue(Directory.Exists(TestFolder + HouseKeeping.Slash + "NewProject\\Documents"));
            Assert.IsTrue(Directory.Exists(TestFolder + HouseKeeping.Slash + "NewProject\\Code"));
            Assert.IsTrue(Directory.Exists(TestFolder + HouseKeeping.Slash + "NewProject\\Communication\\Email"));


            Directory.Delete(TestFolder + HouseKeeping.Slash + "NewProject",true);
            
        }

        [TestMethod]
        public void GetProjectSpecificFolder()
        {
            
        }
    }
}
