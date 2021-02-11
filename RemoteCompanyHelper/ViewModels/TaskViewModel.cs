using Caliburn.Micro;
using RemoteCompanyHelper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteCompanyHelper.ViewModels
{
    public class TaskViewModel : Screen
    {
        private Data vmData;
        private Job _selectedJob;
        private ObservableCollection<Job> _jobs;
        private string _timeSpentStr;
        private bool _isWorking;
        private string _description;
        private Timer scrTimer;
        private Timer strTimer;
        private Time currentTime;
        private Time timeSpent;
        private Random _random = new Random();
        private string _timerStr;
        public MyTask CurTask { get; set; }
        public string TimerStr
        {
            get => _timerStr;
            set
            {
                _timerStr = value;
                NotifyOfPropertyChange(() => TimerStr);
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
        public bool IsWorking
        {
            get => _isWorking;
            set
            {
                _isWorking = value;
                NotifyOfPropertyChange(() => IsWorking);
                NotifyOfPropertyChange(() => CanStartJob);
                NotifyOfPropertyChange(() => CanStopJob);
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
        public ObservableCollection<Job> Jobs
        {
            get => _jobs;
            set
            {
                _jobs = value;
                NotifyOfPropertyChange(() => Jobs);
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
        public TaskViewModel()
        {
            vmData = Data.GetInstance();
            CurTask = vmData.SelectedTask;
            timeSpent = new Time(vmData.SelectedTask.TimeSpent, true);
            TimeSpentStr = timeSpent.StrTime;

            IsWorking = false;
            currentTime = new Time(false);
            TimerStr = currentTime.StrTime;

            Jobs = new ObservableCollection<Job>(vmData.GetJobs(CurTask.TaskId));
            Time t = new Time(false);
            foreach (Job j in Jobs)
            {
                t.SetTime(j.Duration);
                j.StrDuration = t.StrTime;
            }

            // test only //
            /*Jobs = new ObservableCollection<Job>();
            Jobs.Add(new Job(DateTime.Now, 5000, "qwerty uiop"));
            Jobs.Add(new Job(DateTime.Now, 5000, "qwerty uiop"));*/
        }
        public bool CanStartJob
        {
            get { return !IsWorking; }
        }
        public bool CanStopJob
        {
            get { return IsWorking; }
        }


        public void StartJob()
        {
            IsWorking = true;
            Job newJob = new Job(DateTime.Now, currentTime.ToInt32(), Description);
            vmData.CurrentJob = vmData.CreateNewJob(newJob);
            //scrTimer = new Timer(ScrTimerCallback, null, _random.Next(1800000, 3600000), Timeout.Infinite);
            scrTimer = new Timer(ScrTimerCallback, null, _random.Next(3000, 5000), Timeout.Infinite);
            strTimer = new Timer(StrTimerCallback, null, 1000, Timeout.Infinite);
        }
        public void StopJob()
        {
            vmData.CurrentJob.Description = Description;
            vmData.CurrentJob.Duration = currentTime.ToInt32();
            vmData.UpdateJobInfo(vmData.CurrentJob);
            IsWorking = false;
            scrTimer.Dispose();
            strTimer.Dispose();
            currentTime.Reset();
            TimerStr = currentTime.StrTime;
            Description = "";
        }
        public void CompleteTask()
        {
            // TODO: save data to db, close page
        }
        private void ScrTimerCallback(object state)
        {
            try
            {
                vmData.GetScreenshot();
            }
            finally
            {
                //scrTimer.Change(_random.Next(1800000, 3600000), Timeout.Infinite);
                scrTimer.Change(_random.Next(3000, 5000), Timeout.Infinite);
            }
        }
        private void StrTimerCallback(object state)
        {
            try
            {
                currentTime++;
                TimerStr = currentTime.StrTime;
            }
            finally
            {
                strTimer?.Change(1000, Timeout.Infinite);
            }
        }
    }
}
