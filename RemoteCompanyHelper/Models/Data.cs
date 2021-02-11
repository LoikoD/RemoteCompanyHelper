using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace RemoteCompanyHelper.Models
{
    public class Data
    {
        private static Data instance;

        private SqlConnection conn;

        public List<Team> MyTeams { get; set; }
        public List<Team> OtherTeams { get; set; }
        public Project SelectedProject { get; set; }
        public User CurUser { get; set; }
        public MyTask SelectedTask { get; set; }
        public Job CurrentJob { get; set; }
        public Team SelectedTeam { get; set; }

        private Data()
        {
            if (conn == null || conn.State != ConnectionState.Open)
            {
                conn = new SqlConnection
                {
                    ConnectionString = ConfigurationManager.ConnectionStrings["ConnString"].ToString()
                };
                conn.Open();
            }
        }
        public static Data GetInstance()
        {
            if (instance == null)
                instance = new Data();
            return instance;
        }
        public void GetScreenshot()
        {
            Bitmap bmp = new Bitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, bmp.Size);
            }
            ImageConverter converter = new ImageConverter();
            byte[] byteArray = new byte[0];
            byteArray = (byte[])converter.ConvertTo(bmp, typeof(byte[]));
            GetNewScreenId(byteArray);
        }
        private int GetNewScreenId(byte[] img)
        {
            SqlCommand command = new SqlCommand(@"insert Screenshots (User_Id, Job_ID, Screen) output inserted.id values (@userid, @jobid, @screen);", conn);
            command.Parameters.AddWithValue("@userid", CurUser.Id);
            command.Parameters.AddWithValue("@jobid", CurrentJob.Id);
            command.Parameters.AddWithValue("@screen", img);
            try
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (int)reader[0];
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Error GetNewScreenId(): " + e);
                return 0;
            }
        }
        public Job CreateNewJob(Job job)
        {
            SqlCommand command = new SqlCommand("insert PrTa_Jobs (Task_ID, StartDate, Duration, Job_Description) output inserted.id values ("
                + SelectedTask.TaskId + ", '" + job.StartDate.ToString() + "', " + job.Duration + ", '" + job.Description + "');", conn);
            try
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        job.Id = (int)reader[0];
                        return job;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Error CreateNewJob(): " + e);
                return null;
            }
        }
        public ObservableCollection<MenuObject> GetDefaultMenuItems()
        {
            ObservableCollection<MenuObject> menuItems = new ObservableCollection<MenuObject>
            {
                new MenuObject() { Name = "Профиль", IconPath = "../Assets/img_home.png" },
                new MenuObject() { Name = "Команды", IconPath = "../Assets/img_contact.png" },
                new MenuObject() { Name = "Приглашения", IconPath = "../Assets/img_message.png" },
                new MenuObject() { Name = "Задачи", IconPath = "../Assets/img_map.png" },
                new MenuObject() { Name = "Настройки", IconPath = "../Assets/img_setting.png" },
                new MenuObject() { Name = "Выйти", IconPath = "../Assets/img_signout.png" }
            };
            return menuItems;
        }
        public ObservableCollection<MenuObject> GetLoginMenuItems()
        {
            ObservableCollection<MenuObject> menuItems = new ObservableCollection<MenuObject>
            {
                new MenuObject() { Name = "Вход", IconPath = "../Assets/img_home.png" },
                new MenuObject() { Name = "Регистрация", IconPath = "../Assets/img_signout.png" }
            };
            return menuItems;
        }
        public void UpdateJobInfo(Job job)
        {
            SqlCommand command = new SqlCommand(@"update PrTa_Jobs set Duration = @duration, Job_Description = @description where ID = @id;", conn);
            command.Parameters.AddWithValue("@duration", job.Duration);
            command.Parameters.AddWithValue("@description", job.Description);
            command.Parameters.AddWithValue("@id", job.Id);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.WriteLine("Error UpdateJobInfo(): " + e);
            }
        }
        public User Authenticate(string un, string pw)
        {
            SqlCommand command = new SqlCommand("SELECT * FROM Users WHERE LOWER(Username) = '" + un.ToLower() + "' and Password = '" + pw + "'", conn);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new User((int)reader["Id"], (string)reader["Username"], (string)reader["Password"]);
                }
            }
            return null;
        }
        public async Task<User> AuthenticateAsync(string un, string pw)
        {
            SqlCommand command = new SqlCommand("SELECT Id, Username FROM Users WHERE LOWER(Username) = '" + un.ToLower() + "' and Password = '" + pw + "'", conn);

            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                if (reader.Read())
                {
                    return new User((int)reader["Id"], (string)reader["Username"]);
                }
            }
            return null;
        }
        public List<int> GetOtherTeamsIds(User user)
        {
            List<int> ids = new List<int>();
            if (conn == null || conn.State != ConnectionState.Open)
            {
                conn = new SqlConnection();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["ConnString"].ToString();
                conn.Open();
            }
            SqlCommand command = new SqlCommand("select distinct t.id ID from Teams t left" +
                " join Participants p on t.ID = p.Team_ID left join Pa_Users pa_u on p.ID = pa_u.Parts_id left join Users u" +
                " on pa_u.User_ID = u.Id where u.Id = " + user.Id, conn);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    ids.Add((int)reader["ID"]);
                }
            }
            return ids;
        }
        public List<int> GetMyTeamsIds(User user)
        {
            List<int> ids = new List<int>();
            SqlCommand command = new SqlCommand("select distinct t.id ID from Teams t where t.creator_id = " + user.Id, conn);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    ids.Add((int)reader["ID"]);
                }
            }
            return ids;
        }
        public List<MyTask> GetTasks(int projectId)
        {
            List<MyTask> tasks = new List<MyTask>();
            SqlCommand command = new SqlCommand("select t.id, task_name, creator_id, c.username, performer_id, p.username, isActive, time_spent" +
                " from Pr_Tasks t left join Users c on c.Id = t.creator_id left join Users p on p.Id = t.performer_id" +
                " where Proj_ID = " + projectId, conn);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    tasks.Add(new MyTask(reader.GetInt32(0), reader.GetString(1), new User(reader.GetInt32(2), reader.GetString(3)),
                        new User(reader.GetInt32(4), reader.GetString(5)), reader.GetBoolean(6), reader.GetInt32(7)));
                }
            }
            return tasks;
        }
        public List<Job> GetJobs(int taskId)
        {
            List<Job> jobs = new List<Job>();
            SqlCommand command = new SqlCommand("select id, StartDate, duration, Job_Description from PrTa_Jobs where Task_ID = " + taskId, conn);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    jobs.Add(new Job(reader.GetInt32(0), reader.GetDateTime(1), reader.GetInt32(2), reader.IsDBNull(3) ? "" : reader.GetString(3)));
                }
            }
            return jobs;
        }
        public List<Job> GetJobsWithScreens(int taskId)
        {
            List<Job> jobs = new List<Job>();
            SqlCommand command = new SqlCommand("select j.id, StartDate, duration, Job_Description, s.ID, s.Screen from PrTa_Jobs j" +
                " left join Screenshots s on j.ID = s.Job_ID where Task_ID = " + taskId + " order by j.id", conn);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                List<Screenshot> Screens = new List<Screenshot>();
                while (reader.Read())
                {
                    var JobId = reader.GetInt32(0);
                    var StartDate = reader.GetDateTime(1);
                    var Duration = reader.GetInt32(2);
                    var Description = reader.IsDBNull(3) ? "" : reader.GetString(3);
                    int ScrId = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                    BitmapSource bmpSource = null;
                    Bitmap image;
                    //var ScrId = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                    byte[] outbyte = new byte[100];
                    if (reader.IsDBNull(5))
                    {
                        //Console.WriteLine("empty screen");
                    }
                    else
                    {
                        outbyte = (byte[])reader[5];
                        using (MemoryStream ms = new MemoryStream(outbyte))
                        {
                            image = new Bitmap(ms);
                            bmpSource = ToWpfBitmap(image);
                        }
                    }

                    var job = jobs.Where(j => j.Id == JobId).FirstOrDefault();
                    if (job == null)
                    {
                        Screens = new List<Screenshot>();
                        if (ScrId != 0)
                            Screens.Add(new Screenshot(ScrId, bmpSource));
                        job = new Job(JobId, StartDate, Duration, Description, Screens);
                        jobs.Add(job);
                    }
                    else
                    {
                        Screens.Add(new Screenshot(ScrId, bmpSource));
                    }
                }
            }
            return jobs;
        }
        public List<Team> GetTeams(List<int> teamsIds)
        {
            if (teamsIds == null || teamsIds.Count == 0)
            {
                return null;
            }
            List<Team> teams = new List<Team>();
            string ids = "(";
            foreach (int a in teamsIds)
            {
                ids += a + ",";
            }
            ids = ids.Substring(0, ids.Length - 1) + ")";
            SqlCommand command = new SqlCommand("select t.Team_Name TeamName, t.ID TeamId, u.Id PartUserId, u.Username PartUserName," +
                " p.Role_Name PartRoleName, p.Is_Admin PartIsAdmin, pr.Id ProjectId, pr.Project_Name ProjectName, pr.IsActive ProjectIsActive," +
                " u2.Id PrRoUserId, u2.Username PrRoUserName, prr.Role_Name PrRolesRoleName, prr.Is_Admin PrRolesIsAdmin from Teams t left" +
                " join Participants p on t.ID = p.Team_ID left join Pa_Users pa_u on p.ID = pa_u.Parts_id left join Users u" +
                " on pa_u.User_ID = u.Id left join Projects pr on t.ID = pr.Team_ID left join Pr_Roles prr on pr.ID = prr.Proj_ID left join" +
                " PrRo_Users pru on prr.ID = pru.PrRoles_ID left join Users u2 on pru.User_ID = u2.Id where t.id in " + ids, conn);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                List<User> partUsers = new List<User>();
                List<User> prRoUsers = new List<User>();
                List<Role> projectRoles = new List<Role>();
                User partUser;
                User prRoUser;
                Participant participant;
                Project project;
                Role projectRole;
                while (reader.Read())
                {
                    var TeamName = (string)reader["TeamName"];
                    var TeamId = (int)reader["TeamId"];
                    var PartUserId = (int)reader["PartUserId"];
                    var PartUserName = (string)reader["PartUserName"];
                    var PartRoleName = (string)reader["PartRoleName"];
                    var PartIsAdmin = (bool)reader["PartIsAdmin"];
                    var ProjectId = (int)reader["ProjectId"];
                    var ProjectName = (string)reader["ProjectName"];
                    var ProjectIsActive = (bool)reader["ProjectIsActive"];
                    var PrRoUserId = (int)reader["PrRoUserId"];
                    var PrRoUserName = (string)reader["PrRoUserName"];
                    var PrRolesRoleName = (string)reader["PrRolesRoleName"];
                    var PrRolesIsAdmin = (bool)reader["PrRolesIsAdmin"];

                    var team = teams.Where(t => t.Id == TeamId).FirstOrDefault();
                    if (team == null)
                    {
                        team = new Team();
                        team.Name = TeamName;
                        team.Id = TeamId;
                        team.Participants = new List<Participant>();
                        team.Projects = new List<Project>();

                        partUser = new User(PartUserId, PartUserName);
                        partUsers = new List<User>();
                        partUsers.Add(partUser);
                        participant = new Participant(partUsers, PartRoleName, PartIsAdmin);
                        team.Participants.Add(participant);

                        prRoUser = new User(PrRoUserId, PrRoUserName);
                        prRoUsers = new List<User>();
                        prRoUsers.Add(prRoUser);
                        projectRole = new Role(prRoUsers, PrRolesRoleName, PrRolesIsAdmin);
                        projectRoles = new List<Role>();
                        projectRoles.Add(projectRole);
                        project = new Project(ProjectId, ProjectName, projectRoles, ProjectIsActive);
                        team.Projects.Add(project);

                        teams.Add(team);
                    }
                    else
                    {
                        project = team.Projects.Where(p => p.Id == ProjectId).FirstOrDefault();
                        if (project == null)
                        {
                            prRoUser = new User(PrRoUserId, PrRoUserName);
                            prRoUsers = new List<User>();
                            prRoUsers.Add(prRoUser);
                            projectRole = new Role(prRoUsers, PrRolesRoleName, PrRolesIsAdmin);
                            projectRoles = new List<Role>();
                            projectRoles.Add(projectRole);
                            project = new Project(ProjectId,  ProjectName, projectRoles, ProjectIsActive);
                            team.Projects.Add(project);
                        }
                        else
                        {
                            projectRole = project.ProjectRoles.Where(r => r.RoleName == PrRolesRoleName).FirstOrDefault();
                            if (projectRole == null)
                            {
                                prRoUser = new User(PrRoUserId, PrRoUserName);
                                prRoUsers = new List<User>();
                                prRoUsers.Add(prRoUser);
                                projectRole = new Role(prRoUsers, PrRolesRoleName, PrRolesIsAdmin);
                                projectRoles.Add(projectRole);
                            }
                            else
                            {
                                prRoUser = projectRole.Users.Where(u => u.Id == PrRoUserId).FirstOrDefault();
                                if (prRoUser == null)
                                {
                                    prRoUser = new User(PrRoUserId, PrRoUserName);
                                    prRoUsers.Add(prRoUser);
                                }
                                else
                                {
                                    participant = team.Participants.Where(part => part.RoleName == PartRoleName).FirstOrDefault();
                                    if (participant == null)
                                    {
                                        partUser = new User(PartUserId, PartUserName);
                                        partUsers = new List<User>();
                                        partUsers.Add(partUser);
                                        participant = new Participant(partUsers, PartRoleName, PartIsAdmin);
                                        team.Participants.Add(participant);
                                    }
                                    else
                                    {
                                        partUser = new User(PartUserId, PartUserName);
                                        partUsers.Add(partUser);
                                    }

                                }
                            }
                        }
                    }
                }
            }
            if (teams != null)
            {
                foreach(Team t in teams)
                {
                    t.ActiveProjects = new List<Project>();
                    t.CompletedProjects = new List<Project>();
                    foreach(Project p in t.Projects)
                    {
                        if (p.IsActive == true)
                        {
                            t.ActiveProjects.Add(p);
                        }
                        else
                        {
                            t.CompletedProjects.Add(p);
                        }
                    }
                }
            }
            return teams;
        }
        public bool DuplicateUsernameCheckExist(string UserName)
        {
            SqlCommand command = new SqlCommand(@" IF(EXISTS(SELECT * FROM Users WHERE LOWER(Username) = LOWER(@username)))
                                                        BEGIN
                                                            SELECT 'TRUE'
                                                        END
                                                        ELSE
                                                        BEGIN
                                                            SELECT 'FALSE'
                                                        END", conn);
            command.Parameters.AddWithValue("@username", UserName);
            return Convert.ToBoolean(command.ExecuteScalar());
        }
        public async Task<bool> AsyncDuplicateUsernameCheckExist(string UserName)
        {
            SqlCommand command = new SqlCommand(@" IF(EXISTS(SELECT * FROM Users WHERE LOWER(Username) = LOWER(@username)))
                                                        BEGIN
                                                            SELECT 'TRUE'
                                                        END
                                                        ELSE
                                                        BEGIN
                                                            SELECT 'FALSE'
                                                        END", conn);
            command.Parameters.AddWithValue("@username", UserName);
            return Convert.ToBoolean(await command.ExecuteScalarAsync());
        }
        public int InsertNewUser(User user)
        {
            SqlCommand command = new SqlCommand(@"insert into Users (Username, Password) output inserted.id values (@username, @password)", conn);
            command.Parameters.AddWithValue("@username", user.UserName);
            command.Parameters.AddWithValue("@password", user.Password);
            try
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (int)reader[0];
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Error CreateNewJob(): " + e);
                return 0;
            }
        }
        public static BitmapSource ToWpfBitmap(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }
    }
}
