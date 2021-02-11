using Caliburn.Micro;
using RemoteCompanyHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RemoteCompanyHelper.ViewModels
{
    class LoginViewModel : Screen
    {
        private string _userName;
        private bool _isPasswordEmpty;
        private string _pbTag = "*****";
        private bool _isLoading;
        private string _loadingText;

        public string LoadingText
        {
            get => _loadingText;
            set
            {
                _loadingText = value;
                NotifyOfPropertyChange(() => LoadingText);
            }
        }
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(() => IsLoading);
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }
        public bool IsPasswordEmpty
        {
            get { return _isPasswordEmpty; }
            set
            {
                _isPasswordEmpty = value;
                if (!_isPasswordEmpty)
                {
                    PbTag = "";
                }
                else
                {
                    PbTag = "*****";
                }
                NotifyOfPropertyChange(() => CanLogin);
            }
        }
        public User CurrentUser { get; set; }
        public string PbTag
        {
            get { return _pbTag; }
            set
            {
                _pbTag = value;
                NotifyOfPropertyChange(() => PbTag);
            }
        }
        public bool CanLogin
        {
            get
            {
                return !string.IsNullOrWhiteSpace(UserName) && !IsPasswordEmpty;
            }
        }
        public LoginViewModel()
        {
            IsLoading = false;
            IsPasswordEmpty = true;
            LoadingText = "Загрузка...";
            CurrentUser = null;
        }
        public void Login(object parameter)
        {
            LoadingText = "Загрузка...";
            IsLoading = true;
            LoginExecAsync(parameter);
        }
        public async void LoginExecAsync(object p)
        {
            await Task.Factory.StartNew(() =>
            {
                var task = Task.Factory.StartNew(() => { Task.Delay(500).Wait(); });
                CurrentUser = Data.GetInstance().Authenticate(UserName, (p as PasswordBox).Password);
                if (CurrentUser == null)
                {
                    Task.WaitAny(task);
                    LoadingText = "Ошибка";
                    Task.Delay(500).Wait();
                    IsLoading = false;
                }
                else
                {
                    Task.WaitAny(task);
                    TryClose();
                }
            });
            
        }
        public void ExecPasswordChanged(object param)
        {
            if (param is PasswordBox pb && pb.Password != null && pb.Password.Length > 0)
            {
                IsPasswordEmpty = false;
            }
            else
            {
                IsPasswordEmpty = true;
            }
        }
    }
}
