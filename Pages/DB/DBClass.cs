using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using static Lab2_Johnson_Imlay_Freeman.Pages.Admin.Projects.AddProjectModel;

namespace Lab2_Johnson_Imlay_Freeman.Pages.DB;

public class DBClass
{
    private static readonly string Lab2DBConnString = "Server=Localhost;Database=Lab2;Trusted_Connection=True";
    private static readonly string? AuthConnString = "Server=Localhost;Database=AUTH;Trusted_Connection=True";

    public static bool ValidateUser(string username, string password, HttpContext context)
    {
        using (SqlConnection conn = new SqlConnection(AuthConnString))
        {
            conn.Open();
            string query = "SELECT UserID, PasswordHash, PasswordSalt FROM Users WHERE Username = @Username";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        byte[] storedHash = Convert.FromBase64String(reader["PasswordHash"].ToString());
                        byte[] storedSalt = Convert.FromBase64String(reader["PasswordSalt"].ToString());

                        if (VerifyPassword(password, storedHash, storedSalt))
                        {
                            context.Session.SetString("Username", username);
                            context.Session.SetInt32("UserID", Convert.ToInt32(reader["UserID"]));
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public static bool RegisterUser(string username, string password)
    {
        using (SqlConnection conn = new SqlConnection(AuthConnString))
        {
            conn.Open();
            byte[] salt = GenerateSalt();
            byte[] hash = HashPassword(password, salt);

            string query = "INSERT INTO Users (Username, PasswordHash, PasswordSalt) VALUES (@Username, @PasswordHash, @PasswordSalt)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PasswordHash", Convert.ToBase64String(hash));
                cmd.Parameters.AddWithValue("@PasswordSalt", Convert.ToBase64String(salt));
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

    private static byte[] GenerateSalt()
    {
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    private static byte[] HashPassword(string password, byte[] salt)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
        {
            return pbkdf2.GetBytes(32);
        }
    }

    private static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
    {
        byte[] computedHash = HashPassword(password, storedSalt);
        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
    }

    public static void Logout(HttpContext context)
    {
        context.Session.Clear();
    }
}

    
    // ================================
    // BEGIN: Griffin Section
    // ================================
    #region GriffinLand
    public static bool AddUser(string username, string password, string? email, string firstName, string lastName, string userType, string? department, string? adminType, int? businessPartnerID)
    {
        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            string query = @"INSERT INTO [User] 
                        (Username, Password, Email, FirstName, LastName, UserType, Department, AdminType, BusinessPartnerID) 
                        VALUES (@Username, @Password, @Email, @FirstName, @LastName, @UserType, @Department, @AdminType, @BusinessPartnerID)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);  // Should be hashed
                cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(email) ? DBNull.Value : email);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@UserType", userType);
                cmd.Parameters.AddWithValue("@Department", string.IsNullOrEmpty(department) ? DBNull.Value : department);
                cmd.Parameters.AddWithValue("@AdminType", string.IsNullOrEmpty(adminType) ? DBNull.Value : adminType);
                cmd.Parameters.AddWithValue("@BusinessPartnerID", businessPartnerID.HasValue ? businessPartnerID.Value : DBNull.Value);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

    public static bool EditUser(int userID, string username, string? email, string firstName, string lastName, string userType, string? department, string? adminType, int? businessPartnerID)
    {
        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            string query = @"UPDATE [User] 
                        SET Username = @Username, Email = @Email, FirstName = @FirstName, LastName = @LastName, 
                            UserType = @UserType, Department = @Department, AdminType = @AdminType, BusinessPartnerID = @BusinessPartnerID
                        WHERE UserID = @UserID";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(email) ? DBNull.Value : email);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@UserType", userType);
                cmd.Parameters.AddWithValue("@Department", string.IsNullOrEmpty(department) ? DBNull.Value : department);
                cmd.Parameters.AddWithValue("@AdminType", string.IsNullOrEmpty(adminType) ? DBNull.Value : adminType);
                cmd.Parameters.AddWithValue("@BusinessPartnerID", businessPartnerID.HasValue ? businessPartnerID.Value : DBNull.Value);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

    public static List<UserModel> LoadUsers()
    {
        List<UserModel> users = new();

        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(@"
            SELECT u.UserID, u.Username, u.Email, u.FirstName, u.LastName, u.UserType, 
                   u.Department, u.AdminType, b.Name AS BusinessPartnerName
            FROM [User] u
            LEFT JOIN BusinessPartner b ON u.BusinessPartnerID = b.BusinessPartnerID", conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    users.Add(new UserModel
                    {
                        UserID = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Email = reader.IsDBNull(2) ? "N/A" : reader.GetString(2),
                        FirstName = reader.GetString(3),
                        LastName = reader.GetString(4),
                        UserType = reader.GetString(5),
                        Department = reader.IsDBNull(6) ? "N/A" : reader.GetString(6),
                        AdminType = reader.IsDBNull(7) ? "N/A" : reader.GetString(7),
                        BusinessPartnerID = reader.IsDBNull(8) ? "N/A" : reader.GetString(8)
                    });
                }
            }
        }

        return users;
    }

    public static string? LoadProjectTitle(int projectID)
    {
        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT Title FROM Project WHERE ProjectID = @ProjectID", conn))
            {
                cmd.Parameters.AddWithValue("@ProjectID", projectID);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetString(0);
                    }
                }
            }
        }
        return null;
    }

    public static bool AddTask(int projectID, string description, DateTime dueDate, string status)
    {
        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            string query = "INSERT INTO Task (ProjectID, Description, DueDate, Status) VALUES (@ProjectID, @Description, @DueDate, @Status)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ProjectID", projectID);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@DueDate", dueDate);
                cmd.Parameters.AddWithValue("@Status", status);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
    public static bool EditTask(int taskID, string description, DateTime dueDate, string status)
    {
        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            string query = "UPDATE Task SET Description = @Description, DueDate = @DueDate, Status = @Status WHERE TaskID = @TaskID";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@TaskID", taskID);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@DueDate", dueDate);
                cmd.Parameters.AddWithValue("@Status", status);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
    public class TaskModel
    {
        public int TaskID { get; set; }
        public int ProjectID { get; set; }
        public string Description { get; set; } = "";
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = "";
    }
    public static TaskModel? GetTask(int taskID)
    {
        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT ProjectID, Description, DueDate, Status FROM Task WHERE TaskID = @TaskID", conn))
            {
                cmd.Parameters.AddWithValue("@TaskID", taskID);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new TaskModel
                        {
                            TaskID = taskID,
                            ProjectID = reader.GetInt32(0),
                            Description = reader.GetString(1),
                            DueDate = reader.GetDateTime(2),
                            Status = reader.GetString(3)
                        };
                    }
                }
            }
        }
        return null;
    }
    //public class GrantModel { public int GrantID { get; set; } public string FundingSource { get; set; } = ""; public decimal Amount { get; set; } }
    //AddProject.cshtml.cs
    //public static List<UserModel> LoadUsers()
    //{
    //    List<UserModel> users = new();
    //    using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
    //    {
    //        conn.Open();
    //        using (SqlCommand cmd = new SqlCommand("SELECT UserID, FirstName, LastName FROM [User]", conn))
    //        using (SqlDataReader reader = cmd.ExecuteReader())
    //        {
    //            while (reader.Read())
    //            {
    //                users.Add(new UserModel
    //                {
    //                    UserID = reader.GetInt32(0),
    //                    FullName = reader.GetString(1) + " " + reader.GetString(2)
    //                });
    //            }
    //        }
    //    }
    //    return users;
    //}

    public static List<UserModel> LoadFacultyMembers()
    {
        List<UserModel> faculty = new();
        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT UserID, FirstName, LastName FROM [User] WHERE UserType = 'Faculty'", conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    faculty.Add(new UserModel
                    {
                        UserID = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2)
                    });
                }
            }
        }
        return faculty;
    }

    //public static List<BusinessPartner> LoadBusinessPartners()
    //{
    //    List<BusinessPartner> partners = new();
    //    using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
    //    {
    //        conn.Open();
    //        using (SqlCommand cmd = new SqlCommand("SELECT BusinessPartnerID, Name FROM BusinessPartner", conn))
    //        using (SqlDataReader reader = cmd.ExecuteReader())
    //        {
    //            while (reader.Read())
    //            {
    //                partners.Add(new BusinessPartner
    //                {
    //                    BusinessPartnerID = reader.GetInt32(0),
    //                    Name = reader.GetString(1)
    //                });
    //            }
    //        }
    //    }
    //    return partners;
    //}

    public static List<GrantModel> LoadGrants()
    {
        List<GrantModel> grants = new();
        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT GrantID, FundingSource, Amount FROM [Grant]", conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    grants.Add(new GrantModel
                    {
                        GrantID = reader.GetInt32(0),
                        FundingSource = reader.GetString(1),
                        Amount = reader.GetDecimal(2)
                    });
                }
            }
        }
        return grants;
    }
    public static int AddProject(string title, DateTime dueDate, int createdBy, int? businessPartnerID, int? grantID, int? assignedFacultyID)
    {
        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            string query = @"INSERT INTO Project 
                        (Title, DueDate, CreatedBy, BusinessPartnerID, GrantID) 
                        OUTPUT INSERTED.ProjectID
                        VALUES (@Title, @DueDate, @CreatedBy, @BusinessPartnerID, @GrantID)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@DueDate", dueDate);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                cmd.Parameters.AddWithValue("@BusinessPartnerID", businessPartnerID ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GrantID", grantID ?? (object)DBNull.Value);

                int projectID = (int)cmd.ExecuteScalar();

                if (assignedFacultyID.HasValue)
                {
                    string assignQuery = @"INSERT INTO ProjectAssignment 
                                       (ProjectID, UserID, Role) 
                                       VALUES (@ProjectID, @UserID, 'Faculty Member')";

                    using (SqlCommand assignCmd = new SqlCommand(assignQuery, conn))
                    {
                        assignCmd.Parameters.AddWithValue("@ProjectID", projectID);
                        assignCmd.Parameters.AddWithValue("@UserID", assignedFacultyID.Value);
                        assignCmd.ExecuteNonQuery();
                    }
                }

                return projectID;
            }
        }
    }
    public static int GetCurrentUserID()
    {
        // Example: Fetch logged-in UserID from session or database
        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT UserID FROM [User] WHERE /* YOUR LOGIN LOGIC HERE */", conn)) // TODO: LOGIN @ALL 
            {
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0; // Return 0 if no user found
            }
        }
    }



















































































































































    #endregion // SHould be at line 701.  This is the end of the GriffinLand region.
    // ================================
    // END: Griffin Section
    // ================================

    // ================================
    // BEGIN: Nicole Section (Faculty Classes)
    // ================================
    #region NicoleZone


    public static List<UserModel> GetFacultyMembers()
    {
        List<UserModel> facultyList = new List<UserModel>();

        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            string query = @"
            SELECT UserID, Username, Email, FirstName, LastName, UserType, 
                   Department, AdminType, BusinessPartnerID
            FROM [User] 
            WHERE UserType = 'Faculty'";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    facultyList.Add(new UserModel
                    {
                        UserID = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                        FirstName = reader.GetString(3),
                        LastName = reader.GetString(4),
                        UserType = reader.GetString(5),
                        Department = reader.IsDBNull(6) ? null : reader.GetString(6),
                        AdminType = reader.IsDBNull(7) ? null : reader.GetString(7)
                        // TODO: BusinessPartnerID assignment @Nicole -> Fix Me!
                    });
                }
            }
        }

        return facultyList;
    }

    public static List<ProjectModel> GetProjectsByFacultyID(int? facultyId)
    {
        List<ProjectModel> projectList = new List<ProjectModel>();

        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();

            string query = @"
                    SELECT 
                        P.ProjectID, 
                        P.Title, 
                        P.DueDate, 
                        U.FirstName + ' ' + U.LastName AS CreatedByName, 
                        BP.Name AS BusinessPartnerName, 
                        G.Category + ' (' + CAST(G.Amount AS NVARCHAR) + ')' AS GrantInfo
                    FROM Project P
                    INNER JOIN [User] U ON P.CreatedBy = U.UserID
                    LEFT JOIN BusinessPartner BP ON P.BusinessPartnerID = BP.BusinessPartnerID
                    LEFT JOIN [GrantApplication] G ON P.GrantID = G.GrantID";

            if (facultyId.HasValue)
            {
                query += " WHERE P.CreatedBy = @FacultyID";
            }

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (facultyId.HasValue)
                {
                    cmd.Parameters.AddWithValue("@FacultyID", facultyId.Value);
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        projectList.Add(new ProjectModel
                        {
                            ProjectID = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            DueDate = reader.GetDateTime(2),
                            CreatedByName = reader.GetString(3),
                            BusinessPartnerName = reader.IsDBNull(4) ? null : reader.GetString(4),
                            GrantInfo = reader.IsDBNull(5) ? null : reader.GetString(5)
                        });
                    }
                }
            }
        }

        return projectList;
    }
    //ProjectTaskManagement.cshtml.cs
    public static string? GetProjectTitle(int projectID)
    {
        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT Title FROM Project WHERE ProjectID = @ProjectID", conn))
            {
                cmd.Parameters.AddWithValue("@ProjectID", projectID);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetString(0);
                    }
                }
            }
        }
        return null;
    }
    public static List<TaskModel> GetTasksByProjectID(int projectID)
    {
        List<TaskModel> tasks = new();

        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(
                "SELECT TaskID, Description, DueDate, Status FROM Task WHERE ProjectID = @ProjectID", conn))
            {
                cmd.Parameters.AddWithValue("@ProjectID", projectID);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new TaskModel
                        {
                            TaskID = reader.GetInt32(0),
                            Description = reader.GetString(1),
                            DueDate = reader.GetDateTime(2),
                            Status = reader.GetString(3)
                        });
                    }
                }
            }
        }

        return tasks;
    }
    public static bool UpdateTaskStatus(int taskID, string newStatus)
    {
        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            string query = "UPDATE Task SET Status = @Status WHERE TaskID = @TaskID";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@TaskID", taskID);
                cmd.Parameters.AddWithValue("@Status", newStatus);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
    //ComposeMessage.cshtml.cs
    //public static List<UserModel> LoadUsers()
    //{
    //    List<UserModel> users = new();
    //    using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
    //    {
    //        conn.Open();
    //        using (SqlCommand cmd = new SqlCommand("SELECT UserID, FirstName + ' ' + LastName AS FullName FROM [User]", conn))
    //        using (SqlDataReader reader = cmd.ExecuteReader())
    //        {
    //            while (reader.Read())
    //            {
    //                users.Add(new UserModel
    //                {
    //                    UserID = reader.GetInt32(0),
    //                    FullName = reader.GetString(1)
    //                });
    //            }
    //        }
    //    }
    //    return users;
    //}
    public static (string Subject, string Body)? LoadReplyMessage(int messageID)
    {
        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(
                "SELECT Subject, Body FROM Message WHERE MessageID = @MessageID", conn))
            {
                cmd.Parameters.AddWithValue("@MessageID", messageID);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ("RE: " + reader.GetString(0), "\n\n----- Original Message -----\n" + reader.GetString(1));
                    }
                }
            }
        }
        return null;
    }
    public static bool SendMessage(int senderID, int recipientID, string subject, string body)
    {
        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            string query = "INSERT INTO Message (SenderID, RecipientID, Subject, Body, Timestamp) VALUES (@SenderID, @RecipientID, @Subject, @Body, GETDATE())";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SenderID", senderID);
                cmd.Parameters.AddWithValue("@RecipientID", recipientID);
                cmd.Parameters.AddWithValue("@Subject", subject);
                cmd.Parameters.AddWithValue("@Body", body);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
    //MessageList.cshtml.cs
    public static List<MessageModel> GetMessages(int recipientID, int? senderID = null)
    {
        List<MessageModel> messages = new();

        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();

            string query = @"
        SELECT m.MessageID, 
               u.FirstName + ' ' + u.LastName AS SenderName, 
               m.Subject, 
               m.Body,  
               m.Timestamp
        FROM Message m
        LEFT JOIN [User] u ON m.SenderID = u.UserID
        WHERE m.RecipientID = @RecipientID";

            if (senderID.HasValue)
            {
                query += " AND m.SenderID = @SenderID";
            }

            query += " ORDER BY m.Timestamp DESC";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@RecipientID", recipientID);

                if (senderID.HasValue)
                {
                    cmd.Parameters.AddWithValue("@SenderID", senderID.Value);
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        messages.Add(new MessageModel
                        {
                            MessageID = reader.GetInt32(0),
                            SenderName = reader.GetString(1),
                            Subject = reader.IsDBNull(2) ? "(No Subject)" : reader.GetString(2),
                            Timestamp = reader.GetDateTime(3)
                        });
                    }
                }
            }
        }

        return messages;
    }
    public class MessageModel
    {
        public int MessageID { get; set; }
        public string SenderName { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Body { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }
    public static List<UserModel> GetMessageSenders()
    {
        List<UserModel> senders = new();

        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(@"
                    SELECT DISTINCT u.UserID, u.FirstName, u.LastName
                    FROM [User] u
                    JOIN Message m ON u.UserID = m.SenderID", conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    senders.Add(new UserModel
                    {
                        UserID = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2)
                    });
                }
            }
        }

        return senders;
    }
    public static MessageModel? GetMessageByID(int messageID)
    {
        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(@"
                    SELECT m.MessageID, u.FirstName + ' ' + u.LastName AS SenderName, 
                    m.Subject, m.Body, m.Timestamp
                    FROM Message m
                    JOIN [User] u ON m.SenderID = u.UserID
                    WHERE m.MessageID = @MessageID", conn))
            {
                cmd.Parameters.AddWithValue("@MessageID", messageID);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new MessageModel
                        {
                            MessageID = reader.GetInt32(0),
                            SenderName = reader.GetString(1),
                            Subject = reader.IsDBNull(2) ? "(No Subject)" : reader.GetString(2),
                            Body = reader.GetString(3),
                            Timestamp = reader.GetDateTime(4)
                        };
                    }
                }
            }
        }
        return null;
    }

    //Method for Inserting data into the GrantApplication
    public static int InsertGrantApplication(string category, string grantName, string fundingSource, DateTime submissionDate,
        DateTime? awardDate, decimal amount, string leadFacultyID, string? businessPartnerID, string status)
    {
        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();

            string query = @"INSERT INTO [GrantApplication] 
                               (Category, GrantName, FundingSource, SubmissionDate, AwardDate, Amount, LeadFacultyID, BusinessPartnerID, Status)
                               VALUES (@Category, @GrantName, @FundingSource, @SubmissionDate, @AwardDate, @Amount, @LeadFacultyID, @BusinessPartnerID, @Status)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Category", category);
                cmd.Parameters.AddWithValue("@GrantName", grantName);
                cmd.Parameters.AddWithValue("@FundingSource", fundingSource);
                cmd.Parameters.AddWithValue("@SubmissionDate", submissionDate);
                cmd.Parameters.AddWithValue("@AwardDate", awardDate.HasValue ? (object)awardDate.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@LeadFacultyID", leadFacultyID);
                cmd.Parameters.AddWithValue("@BusinessPartnerID", string.IsNullOrWhiteSpace(businessPartnerID) ? DBNull.Value : businessPartnerID);
                cmd.Parameters.AddWithValue("@Status", status);

                return cmd.ExecuteNonQuery();
            }
        }
    }

    //Method for Getting the GrantDetails
    public static GrantModel? GetGrantDetails(int grantId)
    {
        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();

            string query = @"
                    SELECT GrantName, Category, FundingSource, SubmissionDate, AwardDate, Amount, LeadFacultyID, BusinessPartnerID, Status
                    FROM [Grant]
                    WHERE GrantID = @GrantID";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@GrantID", grantId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new GrantModel
                        {
                            GrantID = grantId,
                            GrantName = reader.GetString(0),
                            Category = reader.GetString(1),
                            FundingSource = reader.GetString(2),
                            SubmissionDate = reader.GetDateTime(3),
                            AwardDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                            Amount = reader.GetDecimal(5),
                            LeadFacultyID = reader.GetInt32(6),
                            BusinessPartnerID = reader.IsDBNull(7) ? null : reader.GetString(7),
                            Status = reader.GetString(8)
                        };
                    }
                }
            }
        }
        return null;
    }

    //Keeps all of GrantDetails in one place
    public class GrantModel
    {
        public int GrantID { get; set; }
        public string GrantName { get; set; } = "";
        public string Category { get; set; } = "";
        public string FundingSource { get; set; } = "";
        public DateTime SubmissionDate { get; set; }
        public DateTime? AwardDate { get; set; } // Nullable
        public decimal Amount { get; set; }
        public int LeadFacultyID { get; set; }
        public string? BusinessPartnerID { get; set; } // Nullable
        public string Status { get; set; } = "";
    }

    //Method to get list all Grants assigned to chosen Faculty Member
    public static List<GrantModel> GetGrantsByFaculty(int facultyId)
    {
        List<GrantModel> grants = new();

        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            string query = @"
                    SELECT GrantID, GrantName, Status
                    FROM [Grant]
                    WHERE @FacultyID IN (LeadFacultyID, BusinessPartnerID)
                    ORDER BY SubmissionDate DESC";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@FacultyID", facultyId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        grants.Add(new GrantModel
                        {
                            GrantID = reader.GetInt32(0),
                            GrantName = reader.GetString(1),
                            Status = reader.GetString(2)
                        });
                    }
                }
            }
        }

        return grants;
    }

    //Method to load the faculty list
    public static List<SelectListItem> LoadFacultyList()
    {
        List<SelectListItem> facultyList = new(); // Initialize list

        try
        {
            using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
            {
                conn.Open();
                string query = "SELECT UserID, FirstName + ' ' + LastName AS FullName FROM [User] WHERE UserType = 'Faculty'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        facultyList.Add(new SelectListItem
                        {
                            Value = reader.GetInt32(0).ToString(),
                            Text = reader.GetString(1)
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            //return an empty list if necessary
            return new List<SelectListItem>();
        }

        return facultyList; // return the list
    }






































































































































































































    #endregion //Should be at line 1001. End of NicoleZone region.

    // ================================
    // END: Nicole Section
    // ================================

    // ================================
    // BEGIN: Zach Section (BusinessPartner)
    // ================================
    #region ZachSection
    public static List<BusinessPartner> LoadBusinessPartners()
    {
        List<BusinessPartner> partners = new();
        using (SqlConnection conn = new SqlConnection(Lab2DBConnString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT BusinessPartnerID, Name FROM BusinessPartner", conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    partners.Add(new BusinessPartner
                    {
                        BusinessPartnerID = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }
        }
        return partners;
    }

    public class BusinessPartner
    {
        public int BusinessPartnerID { get; set; }
        public string Name { get; set; } = "";
    }




































































































































































    // ================================
    // END: Zach Section
    // ================================
    #endregion // Should end at 1205.
    // ================================
    // BEGIN: Common Section (Model Definitions & Connection Methods)
    // ================================

    public class UserModel
    {
        public int UserID { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string UserType { get; set; } = "";
        public string? Department { get; set; }
        public string? AdminType { get; set; }
        public string? BusinessPartnerID { get; set; }
    }

    public class ProjectModel
    {
        public int ProjectID { get; set; }
        public string Title { get; set; } = "";
        public DateTime DueDate { get; set; }
        public string CreatedByName { get; set; } = "";
        public string? BusinessPartnerName { get; set; }
        public string? GrantInfo { get; set; }
    }

    public static SqlDataReader UserReader()
    {
        SqlCommand cmdProductRead = new SqlCommand();
        cmdProductRead.Connection = Lab2DBConnection;
        cmdProductRead.Connection.ConnectionString = Lab2DBConnString;
        cmdProductRead.CommandText = "SELECT * FROM [User]";
        cmdProductRead.Connection.Open(); // Open connection here, close in Model!

        SqlDataReader tempReader = cmdProductRead.ExecuteReader();

        return tempReader;
    }

    public static SqlDataReader BusinessPartnerReader()
    {
        SqlCommand cmd = new SqlCommand("SELECT * FROM BusinessPartner", Lab2DBConnection);
        cmd.Connection.Open();
        SqlDataReader tempReader = cmd.ExecuteReader();
        return tempReader;
    }

    public static SqlDataReader GrantReader()
    {
        SqlCommand cmd = new SqlCommand("SELECT * FROM [Grant]", Lab2DBConnection);
        cmd.Connection.Open();
        SqlDataReader tempReader = cmd.ExecuteReader();
        return tempReader;
    }

    public static SqlDataReader GrantTeamReader()
    {
        SqlCommand cmd = new SqlCommand("SELECT * FROM GrantTeam", Lab2DBConnection);
        cmd.Connection.Open();
        SqlDataReader tempReader = cmd.ExecuteReader();
        return tempReader;
    }

    public static SqlDataReader ProjectReader()
    {
        SqlCommand cmd = new SqlCommand("SELECT * FROM Project", Lab2DBConnection);
        cmd.Connection.Open();
        SqlDataReader tempReader = cmd.ExecuteReader();
        return tempReader;
    }

    public static SqlDataReader ProjectAssignmentReader()
    {
        SqlCommand cmd = new SqlCommand("SELECT * FROM ProjectAssignment", Lab2DBConnection);
        cmd.Connection.Open();
        SqlDataReader tempReader = cmd.ExecuteReader();
        return tempReader;
    }

    public static SqlDataReader TaskReader()
    {
        SqlCommand cmd = new SqlCommand("SELECT * FROM Task", Lab2DBConnection);
        cmd.Connection.Open();
        SqlDataReader tempReader = cmd.ExecuteReader();
        return tempReader;
    }

    public static SqlDataReader MeetingMinuteReader()
    {
        SqlCommand cmd = new SqlCommand("SELECT * FROM MeetingMinute", Lab2DBConnection);
        cmd.Connection.Open();
        SqlDataReader tempReader = cmd.ExecuteReader();
        return tempReader;
    }

    public static SqlDataReader MessageReader()
    {
        SqlCommand cmd = new SqlCommand("SELECT * FROM Message", Lab2DBConnection);
        cmd.Connection.Open();
        SqlDataReader tempReader = cmd.ExecuteReader();
        return tempReader;
    }

    // ================================
    // END: Common Section
    // ================================
}
