using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace IVISMSWindowsService
{
    public class Database
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SqlConnection dbConn = null;
        private SqlDataReader reader = null;
        private SqlCommand cmnd = null;
        private SqlDataAdapter da = null;

        private string connStr = "";

        private List<SMS> smslist = null;

        public Database()
        {
            this.connStr = Configuration.Instance.ConnectionString;
            log.Info("connStr=" + this.connStr);
        }

        public List<SMS> GetRecords()
        {
            log.Info("-->");
            SqlParameter prm = null;
            DataTable dt = null;

            string spName = "sms_GetAbandonedRecords";
            log.Info("spName:" + spName);

            if (dbConn == null) dbConn = new SqlConnection(connStr);
            if (dbConn.State == ConnectionState.Closed) dbConn.Open();

            cmnd = new SqlCommand(spName, dbConn);
            cmnd.CommandType = CommandType.StoredProcedure;

            prm = new SqlParameter("@hostid", SqlDbType.NVarChar, 50);
            prm.Value = Environment.MachineName;
            prm.Direction = ParameterDirection.Input;
            cmnd.Parameters.Add(prm);

            prm = new SqlParameter("@outputCode", SqlDbType.Int);
            prm.Direction = ParameterDirection.Output;
            cmnd.Parameters.Add(prm);

            //  cmd = new SqlCommand(_sql, connDB);
            da = new SqlDataAdapter(cmnd);
            dt = new DataTable();

            try
            {

                da.Fill(dt);

                string OutputCode = cmnd.Parameters["@OutputCode"].Value.ToString();

                log.Info("rows :" + dt.Rows.Count);
                log.Info("OutputCode:" + OutputCode);

                if (dt != null && dt.Rows.Count > 0)
                {
                    SMS sms = new SMS();
                    smslist = new List<SMS>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        sms = FillSmsRecord(dr);

                        smslist.Add(sms);
                    }
                }
            }
            catch (Exception exc)
            {
                log.Error(exc.Message);
                log.Error(exc.StackTrace);
            }
            finally
            {
                Disconnect();
                log.Info("<--");
            }

            return smslist;
        }

        private SMS FillSmsRecord(DataRow dr)
        {
            log.Info("-->");
            SMS sms = new SMS(); ;

            sms.phonenumber = dr["PhoneNumber"].ToString();
            sms.ucid = dr["UCID"].ToString();
            sms.vdn = dr["VDN"].ToString();
            sms.from = Configuration.Instance.From;
            sms.user = Configuration.Instance.User;
            sms.password = Configuration.Instance.Password;
            sms.content = Configuration.Instance.Content;
            sms.emailaddress = Configuration.Instance.Emailaddress;
            sms.sender = Configuration.Instance.Sender;
 
            string json = JsonConvert.SerializeObject(sms, Formatting.Indented);
            log.Info(json);

            log.Info("<--");
            return sms;
        }

        public int UpdateResult(
                            SMS sms,
                            int result,
                            string xmlresponse
                            )
        {
            try
            {
                log.Info("-->");
                SqlParameter prm = null;
                /*
                        stored procedure : UpdateResul 
                                    @UCID varchar(50),
                                    @sender varchar(50),
                                    @phonenumber varchar(50),
                                    @result nvarchar(50),
                                    @outputMsg nvarchar(2000) out,
                                    @outputCode int out
                 */
                if (dbConn == null) dbConn = new SqlConnection(connStr);
                if (dbConn.State == ConnectionState.Closed) dbConn.Open();

                string spName = "sms_UpdateResult";
                log.Info("spName:" + spName);
                cmnd = new SqlCommand(spName, dbConn);
                cmnd.CommandType = CommandType.StoredProcedure;


                prm = new SqlParameter("@UCID", SqlDbType.VarChar, 50);
                prm.Value = sms.ucid;
                prm.Direction = ParameterDirection.Input;
                cmnd.Parameters.Add(prm);

                prm = new SqlParameter("@sender", SqlDbType.VarChar, 50);
                prm.Value = sms.sender;
                prm.Direction = ParameterDirection.Input;
                cmnd.Parameters.Add(prm);

                prm = new SqlParameter("@phonenumber", SqlDbType.VarChar, 50);
                prm.Value = sms.phonenumber;
                prm.Direction = ParameterDirection.Input;
                cmnd.Parameters.Add(prm);

                prm = new SqlParameter("@result", SqlDbType.Int);
                prm.Value = result;
                prm.Direction = ParameterDirection.Input;
                cmnd.Parameters.Add(prm);

                prm = new SqlParameter("@xmlresponse", SqlDbType.NVarChar, -1);
                prm.Value = xmlresponse;
                prm.Direction = ParameterDirection.Input;
                cmnd.Parameters.Add(prm);

                prm = new SqlParameter("@hostid", SqlDbType.NVarChar, 50);
                prm.Value = Environment.MachineName;
                prm.Direction = ParameterDirection.Input;
                cmnd.Parameters.Add(prm);

                prm = new SqlParameter("@outputMsg", SqlDbType.NVarChar, 2000);
                prm.Direction = ParameterDirection.Output;
                cmnd.Parameters.Add(prm);

                prm = new SqlParameter("@outputCode", SqlDbType.Int);
                prm.Direction = ParameterDirection.Output;
                cmnd.Parameters.Add(prm);

                prm = new SqlParameter("@return", SqlDbType.Int);
                prm.Direction = ParameterDirection.ReturnValue;
                cmnd.Parameters.Add(prm);

                // execute the command
                int rows = 0;
                cmnd.ExecuteNonQuery();

                rows = (int)cmnd.Parameters["@return"].Value;

                string OutputMsg = cmnd.Parameters["@OutputMsg"].Value.ToString();
                string OutputCode = cmnd.Parameters["@OutputMsg"].Value.ToString();
                string ReturnCode = cmnd.Parameters["@return"].Value.ToString();
                log.Info("rows updated after store procedure:" + rows);
                log.Info("OutputMsg:" + OutputMsg);
                log.Info("OutputCode:" + OutputCode);
                log.Info("ReturnCode:" + ReturnCode);

                return rows;
            }
            catch (Exception exc)
            {
                log.Info(exc.Message);
                log.Info(exc.StackTrace);
                return -99;
            }
            finally
            {
                Disconnect();
                log.Info("<--");

            }
        }

        public void Disconnect()
        {
            if (reader != null && !reader.IsClosed)
                reader.Close();
            reader = null;
            if (dbConn != null && dbConn.State == System.Data.ConnectionState.Open)
                dbConn.Close();
            cmnd = null;
        }

    }

}

