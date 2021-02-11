using Caliburn.Micro;
using RemoteCompanyHelper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RemoteCompanyHelper.ViewModels
{
    public class MainViewModel : Conductor<object>, IHandle<string>
    {
        public IEventAggregator EventAggregator { get; private set; }
        private LoginViewModel loginVm;
        private TeamsViewModel teamsVm;
        private RegisterViewModel registerVm;
        private User _curUser;
        private MenuObject _selectedMenuItem;
        private Data data;
        private bool _menuChecked;
        private double _mainPageOpacity;
        private bool _toolTipVisibility;
        private Visibility _vis;
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
        public Visibility Vis
        {
            get { return _vis; }
            set
            {
                _vis = value;
                NotifyOfPropertyChange(() => Vis);
            }
        }
        public bool ToolTipVisibility
        {
            get { return _toolTipVisibility; }
            set
            {
                _toolTipVisibility = value;
                NotifyOfPropertyChange(() => ToolTipVisibility);
            }
        }
        public double MainPageOpacity
        {
            get { return _mainPageOpacity; }
            set
            {
                _mainPageOpacity = value;
                NotifyOfPropertyChange(() => MainPageOpacity);
            }
        }
        public bool MenuChecked
        {
            get { return _menuChecked; }
            set
            {
                _menuChecked = value;
                NotifyOfPropertyChange(() => MenuChecked);
            }
        }
        public MenuObject SelectedMenuItem
        {
            get { return _selectedMenuItem; }
            set
            {
                _selectedMenuItem = value;
                NotifyOfPropertyChange(() => SelectedMenuItem);
            }
        }

        private ObservableCollection<MenuObject> _menuItems;

        public ObservableCollection<MenuObject> MenuItems
        {
            get { return _menuItems; }
            set
            {
                _menuItems = value;
                NotifyOfPropertyChange(() => MenuItems);
            }
        }

        public User CurUser
        {
            get { return _curUser; }
            set
            {
                _curUser = value;
                NotifyOfPropertyChange(() => CurUser);
            }
        }

        public bool IsLoggedIn { get; set; }

        public MainViewModel()
        {
            IsLoading = false;
            EventAggregator = new EventAggregator();
            EventAggregator.Subscribe(this);
            IsLoggedIn = false;
            data = Data.GetInstance();
            CurUser = new User();
            MenuItems = data.GetLoginMenuItems();
            MenuToggleOff();
        }
        public void MenuToggleOn()
        {
            MenuChecked = true;
            MainPageOpacity = 0.3;
            ToolTipVisibility = false;
            Vis = Visibility.Collapsed;
        }
        public void MenuToggleOff()
        {
            MenuChecked = false;
            MainPageOpacity = 1;
            ToolTipVisibility = true;
            Vis = Visibility.Visible;
        }

        public void LoginPage()
        {
            loginVm = new LoginViewModel();
            ActivateItem(loginVm);
            loginVm.Deactivated += LoginViewModel_Deactivated;
        }
        public void RegisterPage()
        {
            registerVm = new RegisterViewModel();
            ActivateItem(registerVm);
            registerVm.Deactivated += RegisterViewModel_Deactivated;
        }

        public void TeamsPage()
        {
            teamsVm = new TeamsViewModel(EventAggregator);
            ActivateItem(teamsVm);
        }
        
        private void LoginViewModel_Deactivated(object sender, DeactivationEventArgs e)
        {
            CurUser = loginVm.CurrentUser;

            if (CurUser?.Id != null)
            {
                LogIn();
            }
        }
        private void RegisterViewModel_Deactivated(object sender, DeactivationEventArgs e)
        {
            CurUser = registerVm.CurUser;

            if (CurUser != null && CurUser.Id != 0)
            {
                Console.WriteLine(CurUser.Id);
                LogIn();
            }
        }

        private void LogIn()
        {
            data.CurUser = CurUser;
            List<int> ids = data.GetMyTeamsIds(CurUser);
            data.MyTeams = data.GetTeams(ids);
            ids = data.GetOtherTeamsIds(CurUser).Except(ids).ToList();
            data.OtherTeams = data.GetTeams(ids);
            if (!IsLoggedIn)
            {
                IsLoggedIn = true;
                MenuItems = data.GetDefaultMenuItems();
            }
        }

        private void LogOut()
        {
            if (IsLoggedIn)
            {
                IsLoggedIn = false;
                CurUser = null;
                MenuItems = data.GetLoginMenuItems();
            }
        }
        
        public void MenuItemCommand()
        {
            switch (SelectedMenuItem.Name)
            {
                case ("Вход"):
                    {
                        LoginPage();
                        break;
                    }
                case ("Регистрация"):
                    {
                        RegisterPage();
                        break;
                    }
                case ("Выйти"):
                    {
                        LogOut();
                        LoginPage();
                        break;
                    }
                case ("Команды"):
                    {
                        TeamsPage();
                        break;
                    }
            }
        }
        public void Handle(string message)
        {
            switch (message)
            {
                case ("OpenProject"):
                    {
                        var instance = new ProjectViewModel(EventAggregator);
                        ActivateItem(instance);
                        break;
                    }
                case ("OpenTask"):
                    {
                        var instance = new TaskViewModel();
                        ActivateItem(instance);
                        break;
                    }
                case ("OpenJobs"):
                    {
                        var instance = new JobsViewModel(EventAggregator);
                        ActivateItem(instance);
                        break;
                    }
                case ("StartLoad"):
                    {
                        IsLoading = true;
                        break;
                    }
                case ("StopLoad"):
                    {
                        IsLoading = false;
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Unknown action: " + message);
                        break;
                    }
            }
        }

    }
}
