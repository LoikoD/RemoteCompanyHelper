using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RemoteCompanyHelper.Models
{
    public class Role
    {
        public List<User> Users { get; set; }
        public string RoleName { get; set; }
        public bool IsAdmin { get; set; }
        public Role() { }
        public Role(List<User> users, string roleName, bool isAdmin)
        {
            Users = users;
            RoleName = roleName;
            IsAdmin = isAdmin;
        }
    }

    public class Participant
    {
        public List<User> Users { get; set; }
        public string RoleName { get; set; }
        public bool IsAdmin { get; set; }
        public Participant() { }
        public Participant(List<User> users, string roleName, bool isAdmin)
        {
            Users = users;
            RoleName = roleName;
            IsAdmin = isAdmin;
        }
    }

    public class Project
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public List<Role> ProjectRoles { get; set; }
        public bool IsActive { get; set; }
        public Project() { }
        public Project(int id, string projectName, List<Role> projectRoles, bool isActive)
        {
            Id = id;
            ProjectName = projectName;
            ProjectRoles = projectRoles;
            IsActive = isActive;
        }
    }

    public class MyTask
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public User Creator { get; set; }
        public User Performer { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public int TimeSpent { get; set; }
        public MyTask() { }
        public MyTask(string taskName, User creator, User performer, bool isActive)
        {
            TaskName = taskName;
            Creator = creator;
            Performer = performer;
            IsActive = isActive;
            Status = IsActive ? "Активна" : "Завершена";
        }
        public MyTask(string taskName, User creator, User performer, bool isActive, int timeSpent)
        {
            TaskName = taskName;
            Creator = creator;
            Performer = performer;
            IsActive = isActive;
            Status = IsActive ? "Активна" : "Завершена";
            TimeSpent = timeSpent;
        }
        public MyTask(int taskId, string taskName, User creator, User performer, bool isActive, int timeSpent)
        {
            TaskId = taskId;
            TaskName = taskName;
            Creator = creator;
            Performer = performer;
            IsActive = isActive;
            Status = IsActive ? "Активна" : "Завершена";
            TimeSpent = timeSpent;
        }
    }

    public class Job
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; }
        public string StrDuration { get; set; }
        public List<Screenshot> Screens { get; set; }
        public Job() { }
        public Job(DateTime startDate, int duration, string description)
        {
            StartDate = startDate;
            Duration = duration;
            Description = description;
        }
        public Job(int id, DateTime startDate, int duration, string description)
        {
            Id = id;
            StartDate = startDate;
            Duration = duration;
            Description = description;
        }
        public Job(int id, DateTime startDate, int duration, string description, List<Screenshot> screens)
        {
            Id = id;
            StartDate = startDate;
            Duration = duration;
            Description = description;
            Screens = screens;
        }
    }
    public class Screenshot
    {
        private int _id;
        public BitmapSource Source;
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                Path = "pack://siteoforigin:,,,/Screens/SCR" + _id.ToString("000000") + ".jpg";
            }
        }
        public string Path { get; set; }
        public Screenshot() { }
        public Screenshot(int id)
        {
            Id = id;
        }
        public Screenshot(int id, BitmapSource source)
        {
            Id = id;
            Source = source;
        }
    }
    public class Team
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<Participant> Participants { get; set; }
        public List<Project> Projects { get; set; }
        public List<Project> ActiveProjects { get; set; }
        public List<Project> CompletedProjects { get; set; }
        public Team() { }
        public Team(string name, int id, List<Participant> participants, List<Project> projects)
        {
            Name = name;
            Id = id;
            Participants = participants;
            Projects = projects;
        }
    }

}
