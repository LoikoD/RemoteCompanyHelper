using Caliburn.Micro;
using RemoteCompanyHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteCompanyHelper.ViewModels
{
    public class ProjectViewModel : Screen
    {
        private readonly IEventAggregator _eventAggregator;
        private List<MyTask> _allTasks;
        private List<MyTask> _myTasks;
        private MyTask _selectedTask;
        private MyTask _mSelectedTask;
        private MyTask _aSelectedTask;
        private Data vmData;
        private bool _isAdmin;
        public string ProjectName { get; set; }
        public string ProjectStatus { get; set; }
        public List<MyTask> AllTasks
        {
            get => _allTasks;
            set
            {
                _allTasks = value;
                NotifyOfPropertyChange(() => AllTasks);
            }
        }
        public List<MyTask> MyTasks
        {
            get => _myTasks;
            set
            {
                _myTasks = value;
                NotifyOfPropertyChange(() => MyTasks);
            }
        }
        public MyTask SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                NotifyOfPropertyChange(() => SelectedTask);
                NotifyOfPropertyChange(() => CanOpenTask);
                NotifyOfPropertyChange(() => CanOpenJobs);
            }
        }
        public MyTask MSelectedTask
        {
            get => _mSelectedTask;
            set
            {
                _mSelectedTask = value;
                NotifyOfPropertyChange(() => MSelectedTask);
            }
        }
        public MyTask ASelectedTask
        {
            get => _aSelectedTask;
            set
            {
                _aSelectedTask = value;
                NotifyOfPropertyChange(() => ASelectedTask);
            }
        }
        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                NotifyOfPropertyChange(() => IsAdmin);
                NotifyOfPropertyChange(() => CanOpenTask);
                NotifyOfPropertyChange(() => CanOpenJobs);
            }
        }
        public ProjectViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            vmData = Data.GetInstance();
            ProjectName = vmData.SelectedProject.ProjectName;
            ProjectStatus = vmData.SelectedProject.IsActive ? "Актвный" : "Завершен";

            AllTasks = vmData.GetTasks(vmData.SelectedProject.Id);
            MyTasks = new List<MyTask>();
            foreach (MyTask t in AllTasks)
            {
                if (t.Performer.Id == vmData.CurUser.Id)
                    MyTasks.Add(t);
            }
            
            IsAdmin = false;
            foreach (Role r in vmData.SelectedProject.ProjectRoles)
            {
                if (r.Users.Where(u => u.Id == vmData.CurUser.Id).FirstOrDefault() != null && r.IsAdmin == true)
                {
                    IsAdmin = true;
                    break;
                }
            }

        }

        public bool CanOpenTask
        {
            get
            {
                return IsAdmin && SelectedTask != null || MSelectedTask != null;
            }
        }
        public bool CanOpenJobs
        {
            get
            {
                return IsAdmin && SelectedTask != null;
            }
        }

        public void OpenTask()
        {
            vmData.SelectedTask = SelectedTask;
            _eventAggregator.PublishOnUIThread("OpenTask");
        }
        public void OpenJobs()
        {
            vmData.SelectedTask = SelectedTask;
            _eventAggregator.PublishOnUIThread("OpenJobs");
        }

        public void MyTaskSelectedCommand()
        {
            ASelectedTask = null;
            SelectedTask = MSelectedTask;
        }
        public void AllTaskSelectedCommand()
        {
            MSelectedTask = null;
            SelectedTask = ASelectedTask;
        }
    }
}
