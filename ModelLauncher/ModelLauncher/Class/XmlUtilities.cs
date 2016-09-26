using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using MessageBox = System.Windows.Forms.MessageBox;

namespace ModelLauncher.Class
{
    public class XmlUtilities
    {

        private static XDocument _mainXmlDocument;
        public static XElement RootElement;
        public static String LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static String XmlName = Path.Combine(LocalAppData, "ModelLauncher", "HistoryModel.xml");
        public static List<RecentViewedModels> ViewedModelList = new List<RecentViewedModels>();
        public static int LastId { get; set; }

        private static string _viewId;
        private static string _jobnumber;
        private static string _jobcode;
        private static string _name;
        private static string _version;
        private static string _path;
        private static string _servername;
        private static string _date;
        private static string _configuration;
        private static string _role;

        private static List<string> lastFolderXElements;
        private static string JobCode { get; set; }
        private static string ModelPath { get; set; }
        private static string FabricatorName { get; set; }

        //XML methods
        public static void LoadXml()
        {
            if (!File.Exists(XmlName))
            {
                //var ds = MessageBox.Show("XML not exist.", "Missing XML file.", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                //if (ds != MessageBoxResult.Yes) return;
                CreateXml();
                SaveXmlFile(XmlName, CreateXml());
                _mainXmlDocument = XDocument.Load(XmlName);
                RootElement = _mainXmlDocument.Root;
            }
            else
            {
                _mainXmlDocument = XDocument.Load(XmlName);
                RootElement = _mainXmlDocument.Root;
                GetViewModel();
            }
        }
        public static void SaveXmlFile(string filename, XDocument xmlDoc)
        {
            xmlDoc.Save(filename);
        }
        public static XDocument AddElemeltsXDoc()
        {
            LoadXml();

            //if ((lastFolderXElements.Count() != 6)) return _mainXmlDocument;

            var id = GetLastViewId() + 1;

            _viewId = id.ToString();
            _jobcode = JobCode; //lastFolderXElements[1];
            _path = ModelPath; //lastFolderXElements[4];
            _name = FabricatorName; //lastFolderXElements[5];

            /// -- not included
            _jobnumber = string.Empty; //lastFolderXElements[0];
            _version = string.Empty; //lastFolderXElements[3];
            /// -- 

            var newXElements = new XElement
            (
                "viewmodel",
                new XAttribute("viewid", _viewId),
                new XElement("jobnumber", _jobnumber),
                new XElement("jobcode", _jobcode),
                new XElement("name", _name),
                new XElement("version", _version),
                new XElement("path", _path),
                new XElement("servername", ""),
                new XElement("date", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()),
                new XElement("configuration", ""),
                new XElement("role", "")
            );
            RootElement.Add(newXElements);

            return _mainXmlDocument;
        }
        public static XDocument RemoveXnodeGreaterThan5XDocument()
        {
            LoadXml();
            var itemCount = RootElement.Descendants("viewmodel").Count();
            if (itemCount > 5)
            {
                var firstitem = RootElement.Descendants("viewmodel").OrderBy(id => Convert.ToInt32(id.Attribute("viewid").Value)).First();
                firstitem.Remove();
            }

            return _mainXmlDocument;
        }
        public static XDocument CreateXml()
        {
            _mainXmlDocument = new XDocument
            (
                new XDeclaration("1.0", "utf-8", "yes"),
                new XComment("Client recent model Records"),
                new XElement("recentfilesdata", new XAttribute("Title", "RecentFiles"))
            );
            return _mainXmlDocument;
        }

        public static List<RecentViewedModels> GetRecentViewedModel(string viewId, string jnum, string jcode, string name, string ver, string path, string svrname, string date, string configuration, string role)
        {
            ViewedModelList.Add(
                new RecentViewedModels()
                {
                    Viewid = viewId,
                    Jobnumber = jnum,
                    Jobcode = jcode,
                    Name = name,
                    Version = ver,
                    Path = path,
                    ServerName = svrname,
                    Date = date,
                    Configuration = configuration,
                    Role = role
                });

            return ViewedModelList;
        }
        public static void GetViewModel()
        {
            var viewModelXElements = RootElement.Descendants("viewmodel");
            ViewedModelList.Clear();
            foreach (var vmattrb in viewModelXElements)
            {
                _viewId = vmattrb.Attribute("viewid").Value;
                foreach (var itemElements in vmattrb.Elements())
                {
                    if (itemElements.Name == "jobnumber") _jobnumber = itemElements.Value;
                    if (itemElements.Name == "jobcode") _jobcode = itemElements.Value;
                    if (itemElements.Name == "name") _name = itemElements.Value;
                    if (itemElements.Name == "version") _version = itemElements.Value;
                    if (itemElements.Name == "path") _path = itemElements.Value;
                    if (itemElements.Name == "servername") _servername = itemElements.Value;
                    if (itemElements.Name == "date") _date = itemElements.Value;
                    if (itemElements.Name == "configuration") _configuration = itemElements.Value;
                    if (itemElements.Name == "role") _role = itemElements.Value;
                }
                GetRecentViewedModel(_viewId, _jobnumber, _jobcode, _name, _version, _path, _servername, _date, _configuration, _role);
            }
        }
        public static int GetLastViewId()
        {
            var lastid = 0;
            var checkElements = RootElement.Elements().Any();
            if (checkElements)
            {
                var LastXviewId =
                    RootElement.Descendants("viewmodel")
                        .OrderByDescending(s => Convert.ToInt32(s.Attribute("viewid").Value)).First();
                //.First(); //first because ordered by descending

                var id = LastXviewId.Attribute("viewid").Value;
                lastid = Convert.ToInt32(id);
                LastId = Convert.ToInt32(id);
                return lastid;
            }
            else
            {
                LastId = 0;
                lastid = 0;
                return lastid;
            }
        }
        public static bool CheckifValidPath(string path)
        {
            var locationsplit = path.Split('\\');
            var splitCount = locationsplit.Count();
            var modelFabricator = locationsplit[splitCount - 2];
            var lastFolder = locationsplit[splitCount - 1];
            var splitLastFolderXElements = lastFolder.Split('_');

            if (splitLastFolderXElements.Count() >= 2)
            {
                JobCode = splitLastFolderXElements[1];
            }

            ModelPath = path;
            FabricatorName = modelFabricator;
            return true;

            //if ((splitLastFolderXElements.Count() == 4) /*&& rootModel=="IDS_Model"*/)
            //{
            //    FolderXElementsList(path, splitLastFolderXElements, modelFabricator);
            //    return true;
            //}
            //MessageBox.Show("Invalid path!. Please check folder format.", "Incorrect folder format.",
            //    MessageBoxButtons.OK, MessageBoxIcon.Error);
            //return false;
        }
        private static List<string> FolderXElementsList(string path, string[] splitLastFolderXElements, string modelFabricatorFolder)
        {
            lastFolderXElements = new List<string>();
            foreach (var elements in splitLastFolderXElements)
            {
                lastFolderXElements.Add(elements);
            }
            lastFolderXElements.Add(path);
            lastFolderXElements.Add(modelFabricatorFolder);
            return lastFolderXElements;
        }
        public static XDocument UpdateXElements(int viewIdvalue, string config, string server, string role)
        {
            var _attrib = RootElement.Descendants("viewmodel");

            foreach (var attrib in _attrib)
            {
                foreach (var itemElements in attrib.Elements())
                {
                    if (Convert.ToInt32(attrib.Attribute("viewid").Value) == viewIdvalue)
                    {
                        if (itemElements.Name == "configuration") itemElements.Value = config;
                        if (itemElements.Name == "servername") itemElements.Value = server;
                        if (itemElements.Name == "role") itemElements.Value = role;
                    }
                }
                var newid = GetLastViewId() + 1;
                if (Convert.ToInt32(attrib.Attribute("viewid").Value) == viewIdvalue) attrib.Attribute("viewid").Value = newid.ToString();
            }
            Console.Read();
            //tawag SaveXmlFile then pasa MainXmlDocument para masave .. :p
            //SavenXmlFile(XmlName,MainXmlDocument);
            return _mainXmlDocument;
        }
        public static void CheckifXmlPathsExists()
        {
            SaveXmlFile(XmlName, RemoveNonExistingPath());
            GetViewModel();
        }
        public static XDocument RemoveNonExistingPath()
        {
            LoadXml();
            var Xelements = RootElement.Descendants("viewmodel");
            var idList = new List<int>();

            foreach (var elements in Xelements)
            {
                var xElement = elements.Element("path");
                if (xElement != null && !Rnd.Common.Utilities.CheckIfDirectoryExists(xElement.Value))
                {
                    idList.Add(Convert.ToInt32(elements.Attribute("viewid").Value));
                }
                #region Commented
                //foreach (var items in elements.Elements())
                //{
                //    if (items.Name == "path")
                //    {
                //        if (!Rnd.Common.Utilities.CheckIfDirectoryExists(items.Value))
                //        {
                //            idList.Add(Convert.ToInt32(elements.Attribute("viewid").Value));
                //        }
                //    }
                //} 
                #endregion

            }

            foreach (var id in idList)
            {
                var i = id;
                var removeXElement = RootElement.Descendants("viewmodel").Where(e => Convert.ToInt32(e.Attribute("viewid").Value) == i);
                removeXElement.Remove();
            }

            return _mainXmlDocument;
        }


    }
}
