using Caliburn.Micro;
using RemoteCompanyHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RemoteCompanyHelper.ViewModels
{
    public class JobsViewModel : Screen
    {
        private Data vmData;
        private List<Job> _jobs;
        private Job _selectedJob;
        private string _timeSpentStr;
        private string _description;
        private Time timeSpent;
        private BitmapSource _currentScreenPath;
        private int _screenNum;
        private bool _zoomInVis;
        private IWindowManager mng;
        private readonly IEventAggregator _eventAggregator;
        public bool ZoomInVis
        {
            get => _zoomInVis;
            set
            {
                _zoomInVis = value;
                NotifyOfPropertyChange(() => ZoomInVis);
            }
        }
        public MyTask CurTask { get; set; }
        public int ScreenNum
        {
            get => _screenNum;
            set
            {
                _screenNum = value;
                //CurrentScreenPath = SelectedJob.Screens[_screenNum].Path;
                CurrentScreenPath = SelectedJob.Screens[_screenNum].Source;
                NotifyOfPropertyChange(() => CanPreviousScr);
                NotifyOfPropertyChange(() => CanNextScr);
            }
        }
        public BitmapSource CurrentScreenPath
        {
            get => _currentScreenPath;
            set
            {
                _currentScreenPath = value;
                NotifyOfPropertyChange(() => CurrentScreenPath);
            }
        }
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                NotifyOfPropertyChange(() => Description);
            }
        }
        public string TimeSpentStr
        {
            get => _timeSpentStr;
            set
            {
                _timeSpentStr = value;
                NotifyOfPropertyChange(() => TimeSpentStr);
            }
        }
        public Job SelectedJob
        {
            get => _selectedJob;
            set
            {
                _selectedJob = value;
                NotifyOfPropertyChange(() => SelectedJob);
            }
        }
        public List<Job> Jobs
        {
            get => _jobs;
            set
            {
                _jobs = value;
                NotifyOfPropertyChange(() => Jobs);
            }
        }
        public JobsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.PublishOnUIThread("StartLoad");
            Init();
        }
        private async void Init()
        {
            await Task.Factory.StartNew(() =>
            {
                var task = Task.Factory.StartNew(() => { Task.Delay(500).Wait(); });
                vmData = Data.GetInstance();
                CurTask = vmData.SelectedTask;
                Jobs = vmData.GetJobsWithScreens(CurTask.TaskId);
                ZoomInVis = false;
                timeSpent = new Time(CurTask.TimeSpent, true);
                TimeSpentStr = timeSpent.StrTime;
                Time t = new Time(false);
                foreach (Job j in Jobs)
                {
                    t.SetTime(j.Duration);
                    j.StrDuration = t.StrTime;
                }
                if (Jobs.Count > 0)
                {
                    SelectedJob = Jobs[0];
                    if (SelectedJob.Screens.Count > 0)
                    {
                        ScreenNum = 0;
                        ZoomInVis = true;
                    }
                }
                mng = new WindowManager();
                Task.WaitAny(task);
                _eventAggregator.PublishOnUIThread("StopLoad");
            });
        }
        public void CompleteTask()
        {
            // TODO: save data to db, close page
        }
        public bool CanPreviousScr
        {
            get { return ScreenNum > 0; }
        }
        public void PreviousScr()
        {
            ScreenNum--;
        }
        public bool CanNextScr
        {
            get { return SelectedJob != null && SelectedJob.Screens != null && SelectedJob.Screens.Count-1 > ScreenNum; }
        }
        public void NextScr()
        {
            ScreenNum++;
        }
        public void JobSelectionChanged()
        {
            if (SelectedJob.Screens.Count > 0)
            {
                ScreenNum = 0;
                ZoomInVis = true;
            }
            else
            {
                CurrentScreenPath = null;
                ZoomInVis = false;
            }
        }
        public void ShowScreen()
        {
            mng.ShowDialog(new ScreenShowViewModel(SelectedJob.Screens, ScreenNum), null, null);
        }
    }
}
