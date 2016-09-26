using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ModelLauncher.Class
{
    public class BrowserDialog
    {
        public static String FirmPath { get; set; }
        public static String TeklaPath { get; set; }
        private static FolderBrowserDialog _fbddDialog;

        public static void BrowseFolder()
        {
            _fbddDialog = new FolderBrowserDialog();

            _fbddDialog.ShowNewFolderButton = false;            

            _fbddDialog.SelectedPath = GlobalObj.RootCurrentModelFolder;
            
            var ds = _fbddDialog.ShowDialog();
            
            if (ds != DialogResult.OK) return;
            
            if (XmlUtilities.CheckifValidPath(_fbddDialog.SelectedPath))
            {
                XmlUtilities.SaveXmlFile(XmlUtilities.XmlName, XmlUtilities.AddElemeltsXDoc());
                XmlUtilities.ViewedModelList.Clear();
                XmlUtilities.SaveXmlFile(XmlUtilities.XmlName, XmlUtilities.RemoveXnodeGreaterThan5XDocument());
                XmlUtilities.GetViewModel();
            }
        }
    }
}
