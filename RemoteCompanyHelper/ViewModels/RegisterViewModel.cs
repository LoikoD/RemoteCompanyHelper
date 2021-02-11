using Caliburn.Micro;
using RemoteCompanyHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RemoteCompanyHelper.ViewModels
{
    public class RegisterViewModel :Screen
    {
        private bool _userNameErrVisible;
        private bool _passwordErrVisible;
        private bool _confPasswordErrVisible;
        private string _pbTag = "*****";
        private string _confPbTag = "*****";
        private string _userName;
        private string _password = "";
        private string _confPassword = "";
        public User CurUser;

        public string PbTag
        {
            get => _pbTag;
            set
            {
                _pbTag = value;
                NotifyOfPropertyChange(() => PbTag);
            }
        }
        public string ConfPbTag
        {
            get => _confPbTag;
            set
            {
                _confPbTag = value;
                NotifyOfPropertyChange(() => ConfPbTag);
            }
        }
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                UserNameErrVisible = false;
                NotifyOfPropertyChange(() => UserName);
                NotifyOfPropertyChange(() => CanRegister);
                if (value != "")
                    CheckUsername();
            }
        }
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                if (!string.IsNullOrWhiteSpace(_password))
                {
                    PbTag = "";
                }
                else
                {
                    PbTag = "*****";
                }
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanRegister);
                CheckPassword();
            }
        }
        public string ConfPassword
        {
            get => _confPassword;
            set
            {
                _confPassword = value;
                if (!string.IsNullOrWhiteSpace(_confPassword))
                {
                    ConfPbTag = "";
                }
                else
                {
                    ConfPbTag = "*****";
                }
                NotifyOfPropertyChange(() => ConfPassword);
                NotifyOfPropertyChange(() => CanRegister);
                CheckPassword();
            }
        }

        public bool UserNameErrVisible
        {
            get => _userNameErrVisible;
            set
            {
                _userNameErrVisible = value;
                NotifyOfPropertyChange(() => UserNameErrVisible);
            }
        }
        public bool PasswordErrVisible
        {
            get => _passwordErrVisible;
            set
            {
                _passwordErrVisible = value;
                NotifyOfPropertyChange(() => PasswordErrVisible);
            }
        }
        public bool ConfPasswordErrVisible
        {
            get => _confPasswordErrVisible;
            set
            {
                _confPasswordErrVisible = value;
                NotifyOfPropertyChange(() => ConfPasswordErrVisible);
            }
        }
        public RegisterViewModel()
        {
            UserNameErrVisible = false;
            PasswordErrVisible = false;
            ConfPasswordErrVisible = false;
            CurUser = new User();

        }
        public bool CanRegister
        {
            get
            {
                return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password) && Password == ConfPassword;
            }
        }
        public void Register()
        {
            Console.WriteLine(UserName);
            Console.WriteLine(Password);
            Console.WriteLine(ConfPassword);
            if (CheckLogin())
            {
                CurUser = new User(UserName, Password);
                int id = Data.GetInstance().InsertNewUser(CurUser);
                if (id > 0)
                {
                    CurUser.Id = id;
                    TryClose();
                }
                else
                {
                    Console.WriteLine("Error Register(): Can't get new id");
                }
            }

        }
        public bool CheckLogin()
        {
            if (Data.GetInstance().DuplicateUsernameCheckExist(UserName))
            {
                UserNameErrVisible = true;
                return false;
            }
            return true;
        }
        public void CheckUsername()
        {
            var checkTask = Task.Run(async () => UserNameErrVisible = await Data.GetInstance().AsyncDuplicateUsernameCheckExist(UserName));
        }
        public void ScrTimerCallback(object state)
        {

        }
        
        public void CheckPassword()
        {
            if (Password.Length <= ConfPassword.Length && Password != ConfPassword)
            {
                PasswordErrVisible = true;
                ConfPasswordErrVisible = true;
            }
            else if (Password == ConfPassword)
            {
                PasswordErrVisible = false;
                ConfPasswordErrVisible = false;
            }
        }
        
        public void ExecPasswordChanged(PasswordBox param)
        {
            if (param != null)
            {
                Password = param.Password;
            }
        }
        public void ExecConfPasswordChanged(PasswordBox param)
        {
            if (param != null)
            {
                ConfPassword = param.Password;
            }
        }
    }
}
