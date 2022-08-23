using System;
using System.Globalization;
using System.Text;
using System.Data;
using System.Collections;
using Oracle.ManagedDataAccess.Client;

using System.Collections.Generic;

namespace CNO.BPA.SendEmail.DataHandler
{
    public class DataAccess : IDisposable
    {
        #region Variables
        private CustomParameters _parmsCustom = null;
        private Framework.Cryptography crypto = new
           Framework.Cryptography();
        private OracleConnection _connection = null;
        private string _connectionString = null;
        private OracleTransaction _transaction = null;
        private string _DSN = string.Empty;
        private string _DBUser = string.Empty;
        private string _DBPass = string.Empty;

        #endregion

        #region Constructors
        public DataAccess(ref CustomParameters ParmsCustom)
        {
            _parmsCustom = ParmsCustom;

            //check to see that we have values for the db info
            if (_parmsCustom.DSN.Length != 0 && _parmsCustom.UserName.Length != 0 &&
                _parmsCustom.Password.Length != 0)
            {
                _DSN = _parmsCustom.DSN;
                _DBUser = crypto.Decrypt(_parmsCustom.UserName);
                _DBPass = crypto.Decrypt(_parmsCustom.Password);
                //build the connection string
                _connectionString = "Data Source=" + _DSN + ";Persist Security Info=True;User ID="
                   + _DBUser + ";Password=" + _DBPass + "";
            }
            else
            {
                throw new ArgumentNullException("-266007825; Database connection information was "
                   + "not found.");
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Connects and logs in to the database, and begins a transaction.
        /// </summary>
        public void Connect()
        {
            _connection = new OracleConnection();
            _connection.ConnectionString = _connectionString;
            try
            {
                _connection.Open();
                _transaction = _connection.BeginTransaction();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while connecting to the database.", ex);
            }
        }
        /// <summary>
        /// Commits the current transaction and disconnects from the database.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (null != _connection)
                {
                    _transaction.Commit();
                    _connection.Close();
                    _connection = null;
                    _transaction = null;
                }
            }
            catch { } // ignore an error here
        }
        /// <summary>
        /// Commits all of the data changes to the database.
        /// </summary>
        internal void Commit()
        {
            _transaction.Commit();
        }
        /// <summary>
        /// Cancels the transaction and voids any changes to the database.
        /// </summary>
        public void Cancel()
        {
            _transaction.Rollback();
            _connection.Close();
            _connection = null;
            _transaction = null;
        }
        /// <summary>
        /// Generates the command object and associates it with the current transaction object
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        internal OracleCommand GenerateCommand(string commandText, System.Data.CommandType commandType)
        {
            OracleCommand cmd = new OracleCommand(commandText, _connection);
            cmd.Transaction = _transaction;
            cmd.CommandType = commandType;
            return cmd;
        }
        #endregion

        #region Public Methods
        public string getFISID(string batchItemID)
        {
            try
            {
                string fisID = string.Empty;
                Connect();
                OracleCommand cmd = GenerateCommand("BPA_APPS.PKG_FRONT_OFFICE.SEL_FISDOCID", CommandType.StoredProcedure);
                DBUtilities.CreateAndAddParameter("P_IN_ITEM_ID",
                   batchItemID, OracleDbType.Varchar2, ParameterDirection.Input, cmd);
                cmd.BindByName = true;//01/08/2022: 21.4 upgradation

                //Create OUT parameter
                DBUtilities.CreateAndAddParameter("P_OUTPUT",
                    OracleDbType.RefCursor, ParameterDirection.Output, cmd);
                DBUtilities.CreateAndAddParameter("P_OUT_RESULT",
                   OracleDbType.Varchar2, ParameterDirection.Output, 255, cmd);
                DBUtilities.CreateAndAddParameter("p_out_error_message",
                   OracleDbType.Varchar2, ParameterDirection.Output, 4000, cmd);

                //Execute reader on stored procedure
                //**Start**01/08/2022: 21.4 upgradation
                //OracleDataReader oDR = cmd.ExecuteReader();
                //oDR.Read();
                //**End**01/08/2022: 21.4 upgradation

                using (OracleDataReader oDR = cmd.ExecuteReader())

                {
                    oDR.Read();
                    if (cmd.Parameters["P_OUT_RESULT"].Value.ToString().ToUpper() != "SUCCESSFUL")
                    {
                        throw new Exception("-9999; Procedure Error: " +
                        cmd.Parameters["P_OUT_RESULT"].Value.ToString() +
                        "; Oracle Error: " + cmd.Parameters["P_OUT_ERROR_MESSAGE"].Value.ToString());
                    }
                    else
                    {
                        using (OracleDataReader datareader = cmd.ExecuteReader())
                        {
                            string FISDOCID = (datareader.GetOrdinal("FISDOCID")).ToString();
                            //string FISDOCID = datareader.GetValue(datareader.GetOrdinal("FISDOCID")).ToString();
                            //string FISDOCID = cmd.GetValue(cmd.GetOrdinal("FISDOCID")).ToString();
                            return FISDOCID;
                        }
                    }
                }
            }
            //}
            catch (Exception e)
            {
                throw new Exception("DataHandler.getFISID Error " + e.Message);
            }
        }
        public string getDocTypeDescription(string docType)
        {
            try
            {
                string docTypeDescription = string.Empty;
                Connect();
                OracleCommand cmd = GenerateCommand("BPA_APPS.PKG_FRONT_OFFICE.SEL_FRONT_OFFICE_BARCODE",
                    CommandType.StoredProcedure);
                DBUtilities.CreateAndAddParameter("P_IN_BFO_BARCODE_DOCUMENT_TYPE",
                   docType, OracleDbType.Varchar2, ParameterDirection.Input, cmd);

                //Create OUT parameter
                DBUtilities.CreateAndAddParameter("P_OUTPUT",
                    OracleDbType.RefCursor, ParameterDirection.Output, cmd);
                DBUtilities.CreateAndAddParameter("P_OUT_RESULT",
                   OracleDbType.Varchar2, ParameterDirection.Output, 255, cmd);
                DBUtilities.CreateAndAddParameter("p_out_error_message",
                   OracleDbType.Varchar2, ParameterDirection.Output, 4000, cmd);

                //Execute reader on stored procedure
                OracleDataReader oDR = cmd.ExecuteReader();
                oDR.Read();

                if (cmd.Parameters["P_OUT_RESULT"].Value.ToString().ToUpper() != "SUCCESSFUL")
                {
                    throw new Exception("-9999; Procedure Error: " +
                    cmd.Parameters["P_OUT_RESULT"].Value.ToString() +
                    "; Oracle Error: " + cmd.Parameters["P_OUT_ERROR_MESSAGE"].Value.ToString());
                }
                else
                {
                    docTypeDescription = oDR.GetValue(oDR.GetOrdinal("DOC_TYPE_DESCRIPTION")).ToString();
                    return docTypeDescription;
                }
            }
            catch (Exception e)
            {
                throw new Exception("DataHandler.getDocTypeDescription Error " + e.Message);
            }
        }

        public ArrayList getFrontOfficeRequestData()
        {
            ArrayList documentList = new ArrayList();

            try
            {
                string docTypeDescription = string.Empty;
                Connect();
                OracleCommand cmd = GenerateCommand("BPA_APPS.PKG_FRONT_OFFICE.SEL_FRONT_OFFICE_REQUEST",
                    CommandType.StoredProcedure);
                DBUtilities.CreateAndAddParameter("P_IN_BATCH_NO",
                   BatchDetail.BatchNo, OracleDbType.Varchar2, ParameterDirection.Input, cmd);

                //Create OUT parameter
                DBUtilities.CreateAndAddParameter("P_OUTPUT",
                    OracleDbType.RefCursor, ParameterDirection.Output, cmd);
                DBUtilities.CreateAndAddParameter("P_OUT_RESULT",
                   OracleDbType.Varchar2, ParameterDirection.Output, 255, cmd);
                DBUtilities.CreateAndAddParameter("p_out_error_message",
                   OracleDbType.Varchar2, ParameterDirection.Output, 4000, cmd);

                //Execute reader on stored procedure
                OracleDataReader oDR = cmd.ExecuteReader();
                while (oDR.Read())
                {

                    if (cmd.Parameters["P_OUT_RESULT"].Value.ToString().ToUpper() != "SUCCESSFUL")
                    {
                        throw new Exception("-9999; Procedure Error: " +
                        cmd.Parameters["P_OUT_RESULT"].Value.ToString() +
                        "; Oracle Error: " + cmd.Parameters["P_OUT_ERROR_MESSAGE"].Value.ToString());
                    }
                    else
                    {
                        DocumentDetails docDetails = new DocumentDetails();

                        //pull back the data from the table
                        docDetails.ControlNo = oDR.GetValue(oDR.GetOrdinal("CONTROL_NO")).ToString(); ;
                        docDetails.PolicyNumber = oDR.GetValue(oDR.GetOrdinal("POLICY_NO")).ToString(); ;
                        if (docDetails.ControlNo != "")
                        {
                            docDetails.ReferenceType = "Control";
                        }
                        if (docDetails.PolicyNumber != "")
                        {
                            docDetails.ReferenceType = "APP";
                        }
                        docDetails.DocTypeOrig = oDR.GetValue(oDR.GetOrdinal("DOCUMENT_TYPE_ORIG")).ToString(); 
                        docDetails.DocTypeDescription = getDocTypeDescription(docDetails.DocTypeOrig);
                        docDetails.RequirementID = oDR.GetValue(oDR.GetOrdinal("REQUIREMENT_ID")).ToString(); 
                        docDetails.FISDOCID = oDR.GetValue(oDR.GetOrdinal("EXTERNAL_ITEM_ID")).ToString(); 
                        docDetails.ReceivedDate = oDR.GetValue(oDR.GetOrdinal("RECEIVED_DATE")).ToString(); 
                        
                        documentList.Add(docDetails);
                        //need page sides
                    }
                }
                return documentList;
            }
            catch (Exception e)
            {
                throw new Exception("DataHandler.getDocTypeDescription Error " + e.Message);
            }
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            crypto = null;
            _parmsCustom = null;
            _connection = null;
            _connectionString = null;
            _transaction = null;
            _DSN = string.Empty;
            _DBUser = string.Empty;
            _DBPass = string.Empty;
        }

        #endregion


    }

}
