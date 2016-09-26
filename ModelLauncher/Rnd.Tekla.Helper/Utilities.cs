using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Rnd.TeklaStructure.Helper
{
    public class Utilities
    {

        /// <summary>
        ///  Open a new instance of Tekla Structure with initialization 
        /// </summary>
        /// <param name="modelfolder">Model folder directory</param>
        /// <param name="configuration">Current selected configuration</param>
        public void OpenTekla(string modelfolder,string configuration,string selectedrole)
        {
            string root = GetTeklaroot(modelfolder).Trim();
            string version = GetVersion(modelfolder).Trim();
            string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            UpdateBypass(configuration);
            string bypass = Path.Combine(LocalAppData,"ModelLauncher","Bypass.ini");
            string environment = string.Format(@"{0}\{1}\Environments\USimp\env_US_imperial.ini", root, version);
            string role = string.Format(@"{0}\{1}\Environments\USimp\Role_{2}.ini", root, version,selectedrole.Replace(" ","_"));

            string arguments = string.Format(@"""{0}""  -I   ""{1}"" -i ""{2}"" -i ""{3}"" ", modelfolder, bypass, environment, role);
            string tekla = string.Format(@"  ""{0}\{1}\nt\bin\TeklaStructures.exe""  ",root,version);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = tekla;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = arguments;
            Process.Start(startInfo);
        }

        private void UpdateBypass(string configuration)
        {
            var value = "set XS_DEFAULT_LICENSE=";
            string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //string path = Assembly.GetExecutingAssembly().Location;
            var strreplace = configuration.Replace(' ', '_');

            var tempfile = Path.GetTempFileName();
            using (var writer = new StreamWriter(tempfile))
            using (var reader = new StreamReader(Path.Combine(LocalAppData,"ModelLauncher", "Bypass.ini")))
            {
                while (!reader.EndOfStream)
                {
                    var readLine = reader.ReadLine();
                    if (readLine != null)
                    {
                        writer.WriteLine((!readLine.Contains(value) ? readLine : value+strreplace));
                    }
                }
            }
            File.Copy(tempfile, Path.Combine(LocalAppData,"ModelLauncher", "Bypass.ini"), true);
        }

        /// <summary>
        /// Provides information Tekla version.
        /// </summary>
        /// <param name="path">Model folder directory</param>
        /// <returns>Version</returns>
        public string GetVersion(string path)
        {
            var utils = new Rnd.Common.Utilities();
            var value = utils.GetSingleXElementXml(path + "\\TeklaStructuresModel.xml", "Version");
            var splitstr = value.Split(' ');
            return (value!=string.Empty) ? splitstr[0] : value;
        }

        /// <summary>
        ///  Provides information on Tekla root directory.
        /// </summary>
        /// <param name="path">Model folder directory</param>
        /// <returns>Tekla Installation directory</returns>
        public string GetTeklaroot(string path)
        {
            var utils = new Rnd.Common.Utilities();
            var value = utils.GetSingleXElementXml(path + "\\TeklaStructuresModel.xml", "XS_SYSTEM");
            var splitstr = value.Split('\\');

            return (value != string.Empty) ? splitstr[0] + "\\" + splitstr[1] : value;
        }

        /// <summary>
        ///  Provides information on Model Firm location.
        /// </summary>
        /// <param name="path">Model folder directory</param>
        /// <returns>Tekla Installation directory</returns>
        public string GetModelFirm(string path)
        {
            var utils = new Rnd.Common.Utilities();
            var value = utils.GetSingleXElementXml(path + "\\TeklaStructuresModel.xml", "XS_FIRM");            

            return value;
        }

        public List<string> RoleList(string modelfolder)
        {
            List<string> role = new List<string>();
            string root = GetTeklaroot(modelfolder).Trim();
            string version = GetVersion(modelfolder).Trim();
            string environment = string.Format(@"{0}\{1}\Environments\USimp\", root, version);
            
            var environmentDir = new DirectoryInfo(environment);

            foreach (var file in environmentDir.GetFiles())
            {
                if (file.Name.Contains("Role_"))
                {
                    role.Add(file.Name.Replace("Role_",string.Empty).Replace("_"," ").Replace(".ini",string.Empty));
                }
            }

            return role;

        }

        public void CopyRole(string modelfolder)
        {
            string root = GetTeklaroot(modelfolder).Trim();
            string version = GetVersion(modelfolder).Trim();
            string Role_IDS_Standards = string.Format(@"{0}\{1}\Environments\USimp\Role_IDS_Standards.ini", root, version);

            var appdata = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ModelLauncher");
            var source = Path.Combine(appdata, "Role_IDS_Standards.ini");
            File.Copy(source , Role_IDS_Standards,true);
        }

        public void TransferConfigFiles(string firmDirectory, string teklaRootDirectory)
        {
            var util = new Rnd.Common.Utilities();

            var firm = firmDirectory.Trim('\n');

            if (Directory.Exists(firm))
            {
                var source = System.IO.Path.Combine(firm, "Roles");
                if (Directory.Exists(source))
                {
                    util.CopyFilesToLocation(source, teklaRootDirectory, "*");
                }
            }
        }

    }
}
