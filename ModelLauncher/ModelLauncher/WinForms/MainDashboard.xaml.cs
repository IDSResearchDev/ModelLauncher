using System.Windows;
using ModelLauncher.Class;
using ModelLauncher.Model;
using System.IO;
using System;
using System.Diagnostics;

namespace ModelLauncher.WinForms
{
    /// <summary>
    /// Interaction logic for MainDashboard.xaml
    /// </summary>
    public partial class MainDashboard : Window
    {
        public MainDashboard()
        {
            InitializeComponent();
        }        
        
        private void BtnSettings_OnClick(object sender, RoutedEventArgs e)
        {
            //this.OpenFirmLocationSetting();
        }        

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BrowserDialog.BrowseFolder();
                RefreshPanel();
            }
            catch (Exception x)
            {

                MessageBox.Show(this, x.Message, "Error retrieving data");
            }
            
        }        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeControls();
        }

        private void InitializeControls()
        {
            this.Title += GlobalObj.AppVersion;
            GlobalObj.MainDash = this;

            var util = new Rnd.Common.Utilities();
            GlobalObj.UpdateConfigModel = util.DeserializeBinFile<UpdateConfigurationModel>(GlobalObj.LocalUpdateConfigurationFile);

            XmlUtilities.CheckifXmlPathsExists();

            CheckLatestUpdate();

            RefreshPanel();
        }

        public void RefreshPanel()
        {
            this.StackPopulate.Children.Clear();
            XmlUtilities.LoadXml();
            GlobalObj.CreateFabControls();
        }

        private void BtnCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            string updater = Path.Combine(GlobalObj.LocalAppModelLauncherFolder, @"updater.exe");
            if (!File.Exists(updater))
            {
                MessageBox.Show(this, "Updater not found.", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (File.Exists(GlobalObj.LocalUpdaterFile))
            {
                if (CheckLatestUpdate())
                {
                    UpdateSettingView update = new UpdateSettingView();
                    update.Owner = this;
                    update.ShowDialog(); 
                }
                else
                {
                    Process.Start(updater);
                }          
            }
        }

        private bool CheckLatestUpdate()
        {
            bool value = false;
            if (File.Exists(GlobalObj.LocalUpdaterFile))
            {
                var aiuFile = "model_launcher_update.aiu";
                var util = new Rnd.Common.Utilities();
                var updatePath = Path.Combine(util.GetTextFileValue(GlobalObj.LocalUpdaterFile, '=', "DownloadsFolder"), aiuFile);
                if(File.Exists(updatePath))
                {
                    var updateVersion = new Version(util.GetTextFileValue(updatePath, '=', "Version")).ToString(3);

                    if (VersionComparer.IsUptoDate(updateVersion, GlobalObj.AppVersion))
                    {
                        value = true;
                        TxtGetUpdate.Text = string.Empty;
                        BtnCheckUpdate.Content = "Check for Update";
                    }
                    else
                    {
                        TxtGetUpdate.Text = "Get latest version ";
                        BtnCheckUpdate.Content = updateVersion;
                    }
                }
                else
                {                    
                    MessageBox.Show("Model Launcher update file (" + aiuFile + ") doesn't exist.", "Update not found", MessageBoxButton.OK, MessageBoxImage.Information);
                    value = true;                   
                }                
            }
            return value;
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            if (!File.Exists(appDir + @"ModelLauncher_Help.pdf"))
            {
                MessageBox.Show(this, "Help file doesn't exist.", "Help file not found", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            Process.Start(Path.Combine(appDir + @"ModelLauncher_Help.pdf"));
        }

        

    }
}
