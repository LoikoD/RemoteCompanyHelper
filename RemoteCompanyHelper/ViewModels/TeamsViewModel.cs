using Caliburn.Micro;
using RemoteCompanyHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteCompanyHelper.ViewModels
{
    class TeamsViewModel : Screen
    {
        private readonly IEventAggregator _eventAggregator;
        private List<Team> _myTeamsList;
        private List<Team> _otherTeamsList;
        private List<Project> _projectsList;
        private Team _selectedTeam;
        private bool _isMyTeamSelected;
        private Project _selectedProject;
        private User _selectedUser;
        private bool _isAdmin;
        private Data vmData;
        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                NotifyOfPropertyChange(() => IsAdmin);
            }
        }
        public Team SelectedTeam
        {
            get => _selectedTeam;
            set
            {
                if (_selectedTeam != null)
                {
                    _selectedTeam = null;
                    NotifyOfPropertyChange(() => SelectedTeam);
                }
                _selectedTeam = value;
                NotifyOfPropertyChange(() => SelectedTeam);
            }
        }
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (_selectedUser != null)
                {
                    _selectedUser = null;
                    NotifyOfPropertyChange(() => SelectedUser);
                }
                _selectedUser = value;
                NotifyOfPropertyChange(() => SelectedUser);
            }
        }
        public bool IsMyTeamSelected
        {
            get => _isMyTeamSelected;
            set
            {
                _isMyTeamSelected = value;
                NotifyOfPropertyChange(() => CanEditTeam);
            }
        }
        public Project SelectedProject
        {
            get => _selectedProject;
            set
            {
                if (_selectedProject != null)
                {
                    _selectedProject = null;
                    NotifyOfPropertyChange(() => SelectedProject);
                }
                _selectedProject = value;
                NotifyOfPropertyChange(() => SelectedProject);
                NotifyOfPropertyChange(() => CanOpenProject);
            }
        }
        public List<Team> MyTeamsList
        {
            get => _myTeamsList;
            set
            {
                _myTeamsList = value;
                NotifyOfPropertyChange(() => MyTeamsList);
            }
        }
        public List<Team> OtherTeamsList
        {
            get => _otherTeamsList;
            set
            {
                _otherTeamsList = value;
                NotifyOfPropertyChange(() => OtherTeamsList);
            }
        }
        public List<Project> ProjectsList
        {
            get => _projectsList;
            set
            {
                _projectsList = value;
                NotifyOfPropertyChange(() => ProjectsList);
            }
        }
        public TeamsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            vmData = Data.GetInstance();
            MyTeamsList = vmData.MyTeams;
            OtherTeamsList = vmData.OtherTeams;
            SelectedTeam = null;
            SelectedProject = null;
            SelectedUser = null;
            IsMyTeamSelected = false;
        }

        public void MyTeamSelectedCommand()
        {
            IsMyTeamSelected = true;
        }
        public void OthTeamSelectedCommand()
        {
            IsMyTeamSelected = false;
        }

        public void OpenProject()
        {
            vmData.SelectedProject = SelectedProject;
            _eventAggregator.PublishOnUIThread("OpenProject");
        }

        public bool CanOpenProject
        {
            get
            {
                return SelectedProject?.ProjectName != null;
            }
        }
        public void EditTeam()
        {
            Console.WriteLine("EditTeam");
        }
        public bool CanEditTeam
        {
            get
            {
                return IsMyTeamSelected;
            }
        }
        public void CreateTeam()
        {
            Console.WriteLine("CreateTeam");
        }
    }
}
