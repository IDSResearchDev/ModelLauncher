﻿using ModelLauncher.BaseClass;
using Common = Rnd.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ModelLauncher.Class;
using System.Diagnostics;
using System.IO;

namespace ModelLauncher.ViewModel
{
    public class UpdateSettingViewModel : ViewModelBase, IDataErrorInfo
    {

        public UpdateSettingViewModel()
        {
            CanValidate = false;
            if (GlobalObj.UpdateConfigModel != null)
            {
                RNDServer = GlobalObj.UpdateConfigModel.RNDServer.Equals(string.Empty)
                    ? ModelLauncher.Properties.Resources.ServerPath
                    : GlobalObj.UpdateConfigModel.RNDServer;
                FTPServer = GlobalObj.UpdateConfigModel.FTPServer;
                IsRNDServer = GlobalObj.UpdateConfigModel.IsRndServer;
                IsFTPServer = GlobalObj.UpdateConfigModel.IsFtpServer;
            }
        }

        #region Properties

        private ImageSource _icon;
        public ImageSource Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                OnPropertyChanged("Icon");
            }
        }

        private string _rndServer;

        public string RNDServer
        {
            get { return _rndServer; }
            set
            {
                _rndServer = value;
                OnPropertyChanged("RNDServer");
            }
        }

        private string _ftpServer;

        public string FTPServer
        {
            get { return _ftpServer; }
            set
            {
                _ftpServer = value;
                OnPropertyChanged("FTPServer");
            }
        }

        private bool _isRndServer;

        public bool IsRNDServer
        {
            get { return _isRndServer; }
            set
            {
                _isRndServer = value;
                OnPropertyChanged("IsRNDServer");
            }
        }

        private bool _isFtpServer;

        public bool IsFTPServer
        {
            get { return _isFtpServer; }
            set
            {
                _isFtpServer = value;
                OnPropertyChanged("IsFTPServer");
            }
        }

        private string _server;

        public string Server
        {
            get { return _server; }
            set
            {
                _server = value;
                OnPropertyChanged("Server");
            }
        }

        #endregion

        #region Command
        public ICommand BtnOk_OnClick
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    CanValidate = true;
                    
                    string updater = Path.Combine(GlobalObj.LocalAppModelLauncherFolder, @"updater.exe");
                    try
                    {
                        RNDServer = ModelLauncher.Properties.Resources.ServerPath;
                        Server = IsRNDServer ? RNDServer : FTPServer;
                        if (String.IsNullOrEmpty(this.Server)) return;

                        var util = new Common.Utilities();

                        if(ModifyUpdaterFile())
                        {
                            GlobalObj.UpdateConfigModel = new Model.UpdateConfigurationModel
                            {
                                FTPServer = this.FTPServer,
                                RNDServer = this.RNDServer,
                                IsRndServer = this.IsRNDServer,
                                IsFtpServer = this.IsFTPServer
                            };

                            util.SerializeBinFile(GlobalObj.LocalUpdateConfigurationFile, GlobalObj.UpdateConfigModel);
                            //Process.Start(updater);
                            this.Close();
                        }
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(this.GetCurrentWindow(), ex.Message, "Error updating", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }
        }

        private bool ModifyUpdaterFile()
        {
            bool isModified = false;
            try
            {
                var util = new Common.Utilities();
                string[] stringSeparators = new string[] { "://" };
                if (Server.Contains(stringSeparators[0]))
                {
                    var temp = Server.Split(stringSeparators, StringSplitOptions.None);
                    Server = temp[1];
                }

                string updaterFilePath = GlobalObj.LocalUpdaterFile;//AppDomain.CurrentDomain.BaseDirectory + @"updater.ini";
                string attribute = "url";
                char delimiter = '=';
                string updateTextFile = "model_launcher_update.txt";

                /// --- 
                string protocol = "http://";
                string iniNewValue;
                Server = Server.TrimEnd('/');

                if (IsFTPServer)
                {
                    System.Net.IPHostEntry host = new System.Net.IPHostEntry();
                    protocol = "ftp://";
                    Server = Server.Replace("/", "");
                    host = System.Net.Dns.GetHostEntry(Server);

                    // format: \\{host.HostName}\Dropbox\Update\{app}_update.txt
                    // ex. \\ORT070\Dropbox\Update\model_launcher_update.txt

                    string updateTextFilePath = System.IO.Path.Combine(@"\\" + host.HostName + "\\Dropbox\\Update", updateTextFile);
                    string exeFile = util.GetTextFileValue(updateTextFilePath, delimiter, "ServerFileName");
                    string updateTextNewValue = string.Concat(protocol, Server, "/", exeFile);

                    /// -- Update package_tool_update.txt
                    util.UpdateTextFileValue(updateTextFilePath, delimiter, attribute, updateTextNewValue);
                }

                iniNewValue = string.Concat(protocol, Server, "/", updateTextFile);
                /// -- Update localapp updater.ini
                util.UpdateTextFileValue(updaterFilePath, delimiter, attribute, iniNewValue);

                isModified = true;
            }
            catch (Exception ex)
            {
                isModified = false;
                MessageBox.Show(this.GetCurrentWindow(), ex.Message, "Error updating", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return isModified;
        }

        public ICommand BtnCancel_OnClick
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    this.Close();
                });
            }
        }
        #endregion

        #region Validation

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get
            {
                if (CanValidate)
                {
                    if (columnName == "RNDServer" && IsRNDServer)
                    {
                        if (string.IsNullOrEmpty(this.RNDServer))
                        {
                            return "RND Server cannot be blank or empty.";
                        }
                    }

                    if (columnName == "FTPServer" && IsFTPServer)
                    {
                        if (string.IsNullOrEmpty(this.FTPServer))
                        {
                            return "Local FTP cannot be blank or empty.";
                        }
                    }
                }
                return string.Empty;
            }
        }

        private bool canValidate;

        public bool CanValidate
        {
            get { return canValidate; }
            set
            {
                canValidate = value;
                OnPropertyChanged("CanValidate");
                OnPropertyChanged("RNDServer");
                OnPropertyChanged("FTPServer");
            }
        }
        #endregion        



    }
}
