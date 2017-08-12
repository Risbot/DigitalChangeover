using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace UserCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            var user = new User()
            {
                Name= "admin",
                Password = "admin",
                DetachmentID = 1
            };
            CreateDetacment("Zkusebna");
            CreateRoles();
            CreateUser(user);
        }

        private static void CreateDetacment(string name)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Database"].ConnectionString))
            {
                con.Open();
                using (SqlCommand comand = new SqlCommand())
                {
                    comand.Connection = con; 
                    comand.CommandText = "INSERT INTO [dbo].[Detachments]([DetachmentName]) VALUES ( '"+name+"')";
                    comand.ExecuteNonQuery();
                }
                con.Close();
            }
        }

        private static void CreateRoles()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Database"].ConnectionString))
            {
                con.Open();
                using (SqlCommand comand = new SqlCommand())
                {                  
                    comand.Connection = con;
                    comand.CommandText = "INSERT INTO [dbo].[Roles]([RoleName]) VALUES ('Write')";
                    comand.ExecuteNonQuery();
                    comand.CommandText = "INSERT INTO [dbo].[Roles]([RoleName]) VALUES ('Master')";
                    comand.ExecuteNonQuery();
                    comand.CommandText = "INSERT INTO [dbo].[Roles]([RoleName]) VALUES ('Admin')";
                    comand.ExecuteNonQuery();
                }
                con.Close();
            }
        }

        public static void CreateUser(User user)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Database"].ConnectionString))
            {
                con.Open();
           
                string comandStr = "INSERT INTO [dbo].[Users]([UserDetachmentID],[UserName],[UserPassword]) VALUES ( @detachmentID, @name,@password)";
                using (SqlCommand comand = new SqlCommand())
                {
                    Password pg = new Password(user.Name, user.Password);
                    comand.Connection = con;
                    comand.Parameters.Add(new SqlParameter("detachmentID", user.DetachmentID));
                    comand.Parameters.Add(new SqlParameter("name", user.Name));
                    comand.Parameters.Add(new SqlParameter("password", pg.Hash));
                    comand.CommandText = comandStr;
                    comand.ExecuteNonQuery();
                    comand.CommandText = "INSERT INTO [dbo].[UserInRole]([UserInRoleRoleID],[UserInRoleUserID])VALUES(1,1)";
                    comand.ExecuteNonQuery();
                    comand.CommandText = "INSERT INTO [dbo].[UserInRole]([UserInRoleRoleID],[UserInRoleUserID])VALUES(2,1)";
                    comand.ExecuteNonQuery();
                    comand.CommandText = "INSERT INTO [dbo].[UserInRole]([UserInRoleRoleID],[UserInRoleUserID])VALUES(3,1)";
                    comand.ExecuteNonQuery();
                }
                con.Close();
            }
        }
    }
}
