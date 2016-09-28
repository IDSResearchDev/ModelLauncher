using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ModelLauncher.Class;
using Rnd.TeklaStructure.Helper;
using System.Collections.Generic;

namespace ModelLauncher.UserControls
{
    /// <summary>
    /// Interaction logic for FabricatorControl.xaml
    /// </summary>
    public partial class FabricatorControl : UserControl
    {
        public string FabricatorName { get; set; }
        public string JobNumber { get; set; }
        public string JobCode { get; set; }
        public string ModelPath { get; set; }
        public string FirmPath { get; set; }
        public string ServerName { get; set; }
        public string Error { get; set; }
        public int UserCtrlViewId { get; set; }
        public string UserCtrlConfiguration { get; set; }
        public string UserCtrlRole { get; set; }
               
        
        public FabricatorControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void InitializeModelControl()
        {
            this.TxtBlockFabricatorName.Text = this.FabricatorName;
            this.BtnModelIcon.Content = this.JobCode;
            this.BtnModelIcon.Tag = this.TxtBlockModelPath.Text = this.ModelPath;            
            this.CmbConfiguration.SelectedIndex = !String.IsNullOrWhiteSpace(this.UserCtrlConfiguration) ? (int)Enum.Parse(typeof(ConfigurationType), this.UserCtrlConfiguration) : -1;
            this.CmbRole.SelectedValue = !String.IsNullOrWhiteSpace(UserCtrlRole) ? UserCtrlRole : "IDS Standards";
            this.SetOpenButtonProperty();
            this.TxtBoxServerName.Text = GlobalObj.Utilities.GetServerName(this.ModelPath);
            this.TxtBlockModelDetail.Text = this.GetModelFolderName(this.ModelPath);
            this.TxtBlockErr.Text = Error;
            this.RefreshUserType();
        }

        private string GetModelFolderName(string path)
        {
            string[] temp = path.Split('\\');

            return temp[temp.Count() - 1];
        }                                         

        private void RefreshUserType()
        {
            //if (this.TxtBoxServerName.Text != string.Empty /*GlobalObj.Utilities.IsMultiUser(this.ModelPath)*/)
            if (GlobalObj.Utilities.IsMultiUser(this.ModelPath))
            {
                this.ImgUser.Source = new BitmapImage(new Uri("pack://application:,,,/ModelLauncher;component/Images/multiuser_icon.png"));
                this.TxtBlockUserType.Text = "multi-user ";
                //this.StackActiveUser.Visibility = Visibility.Visible;
                //this.TxtBlockActiveUser.Text = "10"/*Dynamic*/;
            }
            else
            {
                this.ImgUser.Source = new BitmapImage(new Uri("pack://application:,,,/ModelLauncher;component/Images/singleuser_icon.png"));
                this.TxtBlockUserType.Text = "single-user ";
                //this.StackActiveUser.Visibility = Visibility.Hidden;
            }
        }
        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            PerformOpenAction();
        }

        private void PerformOpenAction()
        {
            try
            {
                if (!this.UpdateMultiServerFile())
                {
                    MessageBox.Show("The right Server format is : [Computer Name];[Server Name];[32 or 64]", "Invalid Server Parameter", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                XmlUtilities.GetViewModel();
                XmlUtilities.SaveXmlFile(XmlUtilities.XmlName, XmlUtilities.UpdateXElements(UserCtrlViewId, CmbConfiguration.Text.Replace(" ", "_"), TxtBoxServerName.Text, CmbRole.Text));
                XmlUtilities.GetViewModel();

                Utilities teklaUtil = new Utilities();
                teklaUtil.OpenTekla(this.ModelPath, CmbConfiguration.Text.ToUpper(), CmbRole.Text);
                Application.Current.Shutdown();
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Error opening TeklaStructure");
            }
        }

        private bool UpdateMultiServerFile()
        {
            bool flag = true;
            var serverDetails = this.TxtBoxServerName.Text.Trim().Split(';');
            if (serverDetails.Count() > 2)
            {
                int sys;
                if (string.IsNullOrWhiteSpace(serverDetails[0]) || string.IsNullOrWhiteSpace(serverDetails[1]) || string.IsNullOrWhiteSpace(serverDetails[2]))
                { flag = false; }
                else if(int.TryParse(serverDetails[2],out sys))
                {
                    if(sys != 32 && sys != 64)
                    { flag = false; }
                    else
                    {
                        GlobalObj.Utilities.SetServerName
                        (
                            this.ModelPath,
                            computerName: serverDetails[0],
                            serverName: serverDetails[1],
                            port: GlobalObj.Port,
                            targetArch: serverDetails[2]
                        );
                    }
                }
                else
                { flag = false; }              
            }
            else
            { 
                if(!String.IsNullOrWhiteSpace(this.TxtBoxServerName.Text))
                { flag = false; }
                else
                { if (GlobalObj.Utilities.IsMultiUser(this.ModelPath)) { flag = false; } }
            }          
            return flag;
        }  

        private void Cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SetOpenButtonProperty();
        }

        private void SetOpenButtonProperty()
        {
            this.BtnOpen.IsEnabled = this.CmbConfiguration.SelectedIndex >= 0 && this.CmbRole.SelectedIndex >= 0;
        }

        public List<string> GetRoles
        {
            get { return new Utilities().RoleList(ModelPath); }
        }

        
    }    

}
