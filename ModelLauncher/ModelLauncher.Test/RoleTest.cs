using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rnd.TeklaStructure.Helper;

namespace ModelLauncher.Test
{
    [TestClass]
    public class RoleTest
    {
        [TestMethod]
        public void LoadRoleTest()
        {
            Utilities utilities = new Utilities();
            utilities.RoleList(@"D:\LATEST MODELS\15830_PTB_BeauceAtlas_TS21.0SR1USImp");
        }

        [TestMethod]
        public void CopyRoleTest()
        {
            Utilities utilities = new Utilities();
            utilities.CopyRole(@"D:\LATEST MODELS\15830_PTB_BeauceAtlas_TS21.0SR1USImp");
        }


        [TestMethod]
        public void OpenModelTest()
        {
            Utilities teklaUtil = new Utilities();
            teklaUtil.OpenTekla(@"D:\LATEST MODELS\15830_PTB_BeauceAtlas_TS21.0SR1USImp", "Developer", "IDS Standards");
        }
    }
}
