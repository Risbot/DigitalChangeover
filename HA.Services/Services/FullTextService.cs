using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Permissions;
using System.ServiceModel;

namespace HA.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class FullTextService : IFullTextService
    {
        public List<Objects.FullTextWork> FindWork(Parameters parameters)
        {
            List<Objects.FullTextWork> list = null;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Database"].ConnectionString))
                {
                    con.Open();
                    string comandStr = "SELECT Works.*, Vehicles.VehicleNumber, Vehicles.VehicleDescription, Dates.DateDate, Dates.DateIsNight, Dates.DateDescription, WorkTypes.WorkTypeName, WorkTypes.WorkTypeDescription FROM Dates INNER JOIN Works ON Dates.DateID = Works.WorkDateID INNER JOIN Vehicles ON Works.WorkVehicleID = Vehicles.VehicleID INNER JOIN WorkTypes ON Works.WorkWorkTypeID = WorkTypes.WorkTypeID WHERE (Dates.DateDate >= @from) AND (Dates.DateDate <= @to) AND (Dates.DateDetachmentID = @detachmentID)";
                    using (SqlCommand comand = new SqlCommand())
                    {
                        comand.Connection = con;
                        comand.Parameters.Add(new SqlParameter("from", parameters.From.Date));
                        comand.Parameters.Add(new SqlParameter("to", parameters.To.Date));
                        comand.Parameters.Add(new SqlParameter("detachmentID", parameters.DetachmentID));
                        if (parameters.TypeID != 0)
                        {
                            comandStr += "AND (Works.WorkWorkTypeID = @type)";
                            comand.Parameters.Add(new SqlParameter("type", parameters.TypeID));
                        }

                        if (parameters.VehicleID != 0)
                        {
                            comandStr += "AND (Works.WorkVehicleID = @vehicle)";
                            comand.Parameters.Add(new SqlParameter("vehicle", parameters.VehicleID));
                        }

                        if (!String.IsNullOrEmpty(parameters.SearchKey))
                        {
                            comandStr += "AND FREETEXT(Works.WorkFaultDescription, @searchState)";
                            comand.Parameters.Add(new SqlParameter("searchState", parameters.SearchKey));
                        }

                        comand.CommandText = comandStr;
                        using (SqlDataReader reader = comand.ExecuteReader())
                        {
                            if (reader.HasRows)
                                list = new List<Objects.FullTextWork>();
                            while (reader.Read())
                            {
                                list.Add(new Objects.FullTextWork()
                                {
                                    ID = (Int64)reader["WorkID"],
                                    FaultDescription = (string)reader["WorkFaultDescription"],
                                    CauseDescription = reader["WorkCauseDescription"] == DBNull.Value ? null : (string)reader["WorkCauseDescription"],
                                    DateID = (Int64)reader["WorkDateID"],
                                    DateContent = (DateTime)reader["DateDate"],
                                    DateIsNight = (bool)reader["DateIsNight"],
                                    DateDescriptor = reader["DateDescription"] == DBNull.Value ? null : (string)reader["DateDescription"],
                                    VehicleNumber = (string)reader["VehicleNumber"],
                                    VehicleDescription = reader["VehicleDescription"] == DBNull.Value ? null : (string)reader["VehicleDescription"],
                                    WorkTypeName = (string)reader["WorkTypeName"],
                                    WorkTypeDescription = reader["WorkTypeDescription"] == DBNull.Value ? null : (string)reader["WorkTypeDescription"]
                                });
                            }
                            reader.Close();
                        }
                    }
                    con.Close();
                }
                return list;
            }
            catch (Exception e)
            {
                throw new FaultException<WcfException>(ExceptionProvider.CreateFaultContract(e));
            } 
        }
    }
}