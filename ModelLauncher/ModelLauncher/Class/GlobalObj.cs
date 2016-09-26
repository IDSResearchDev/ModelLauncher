using System;
using System.Linq;
using System.Windows.Forms;
using ModelLauncher.UserControls;
using ModelLauncher.WinForms;
using ModelLauncher.Model;
using System.IO;
using Common = Rnd.Common;

namespace ModelLauncher.Class
{
    public class GlobalObj
    {
        public static string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string LocalAppModelLauncherFolder = Path.Combine(LocalAppData, "ModelLauncher");
        public static string LocalUpdateConfigurationFile = Path.Combine(LocalAppModelLauncherFolder, "updater.bin");
        public static string LocalUpdaterFile = Path.Combine(LocalAppModelLauncherFolder, "updater.ini");
        public static UpdateConfigurationModel UpdateConfigModel;
       
        public static MainDashboard MainDash;

        public static string RootCurrentModelFolder { get; set; }
        public static Rnd.Common.Utilities Utilities = new Rnd.Common.Utilities();
        public static string AppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        public static string Port = "1238";

        public static void CreateFabControls()
        {
            try
            {
                var vmList = XmlUtilities.ViewedModelList.OrderByDescending(id => Convert.ToInt32(id.Viewid));

                int x = 0;
                foreach (var item in vmList)
                {
                    if (x == 0)
                    { RootCurrentModelFolder = item.Path; }
                    CopyConfigFiles(item.Path);
                    var fabctrl = new FabricatorControl
                    {
                        Name = "fab" + x,
                        JobNumber = item.Jobnumber,
                        JobCode = item.Jobcode,
                        FabricatorName = item.Name,
                        ModelPath = item.Path,
                        UserCtrlViewId = Convert.ToInt32(item.Viewid),
                        UserCtrlConfiguration = item.Configuration,
                        ServerName = item.ServerName,
                        UserCtrlRole = item.Role
                    };
                    
                    fabctrl.InitializeModelControl();
                    MainDash.StackPopulate.Children.Add(fabctrl);
                    x++;
                }
                if (MainDash.StackPopulate.Children.Count <= 0)
                {
                    MainDash.StackPopulate.Children.Add(new StackAddModelTextControl());
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Error retrieving data");
            }
            
        }

        private static void CopyConfigFiles(string path)
        {
            var helper = new Rnd.TeklaStructure.Helper.Utilities();
            string source = helper.GetModelFirm(path);
            string destination = Path.Combine(helper.GetTeklaroot(path).Trim(Environment.NewLine.ToCharArray()), helper.GetVersion(path).Trim(Environment.NewLine.ToCharArray()), @"environments\usimp");

            helper.TransferConfigFiles(source, destination);
        }
    }
}
