using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;
using Emc.InputAccel.QuickModule.ClientScriptingInterface;
using Emc.InputAccel.ScriptEngine.Scripting;
using log4net;
using log4net.Core;
using log4net.Appender;
using log4net.Layout;
using log4net.Config;
using log4net.Repository.Hierarchy;

//[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace CNO.BPA.SendEmail
{
    public class TaskEvents : ITaskEvents
    {
        
        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string LOG_PATTERN = "%d [%t] %-5p %m%n"; 

        [CustomParameterType(typeof(CustomParameters))]
        public void ExecuteTask(ITaskInformation taskInfo)
        {
            try
            {
                DataHandler.DataAccess _dbAccess = null;

                PatternLayout patternLayout = new PatternLayout();
                patternLayout.ConversionPattern = LOG_PATTERN;
                patternLayout.ActivateOptions();
                RollingFileAppender roller = new RollingFileAppender();
                roller.Layout = patternLayout;
                roller.AppendToFile = true;
                roller.RollingStyle = RollingFileAppender.RollingMode.Size;
                roller.MaxSizeRollBackups = 4;
                roller.MaximumFileSize = "10MB";
                roller.StaticLogFileName = true;
                roller.File = "D:\\InputAccel\\Client\\binnt\\SendEmail-log.txt";
                roller.ActivateOptions();
                BasicConfigurator.Configure(roller);

                CustomParameters parmsCustom;
                parmsCustom = (CustomParameters)taskInfo.CustomParameter.Value;

                //generate the batch number
                _dbAccess = new DataHandler.DataAccess(ref parmsCustom);

                #region DataTable
                DataTable dt = new DataTable();
                dt.Columns.Add("Reference Number", typeof(string));
                dt.Columns.Add("Reference Type", typeof(string));
                dt.Columns.Add("Page Sides", typeof(string));
                dt.Columns.Add("Document Type", typeof(string));
                dt.Columns.Add("Requirement ID", typeof(string));
                dt.Columns.Add("Document Index Number", typeof(string));
                dt.Columns.Add("Time and Date of Scan", typeof(string));
                #endregion

                log.Debug("SendEmail Loop Starting");

                foreach (IWorkflowStep wfStep in taskInfo.Task.Batch.WorkflowSteps)
                {
                    if (wfStep.Name.ToUpper() == "STANDARD_MDF")
                    {
                        log.Debug("Looping through Children in batch");
                        #region document loop - COMMENTED OUT
                        foreach (IBatchNode docNode in taskInfo.Task.TaskRoot.Children(1))
                        {
                            DocumentDetails docDetail = new DocumentDetails();

                            BatchDetail.officeNumber = docNode.Values(wfStep).GetString("D_OFFICE_NO", "");
                            BatchDetail.BatchNo = taskInfo.Task.Batch.Tree.Values(wfStep).GetString("BATCH_NO", "");

                            docDetail.ProductCategory = docNode.Values(wfStep).GetString("D_PRODUCT_CATEGORY", "");
                            docDetail.AppName = docNode.Values(wfStep).GetString("D_APP_NAME", "");
                            docDetail.CurrentStatus = docNode.Values(wfStep).GetString("D_CURRENT_STATUS", "");
                            docDetail.InputSource = docNode.Values(wfStep).GetString("D_INPUT_SOURCE", "");
                            docDetail.TransactionType = docNode.Values(wfStep).GetString("D_TRANSACTION_TYPE", "");
                            docDetail.ReplacementIndicator = docNode.Values(wfStep).GetString("D_REPLACEMENT_INDICATOR", "");
                            docDetail.LifeProIndicator = docNode.Values(wfStep).GetString("D_LIFEPRO_INDICATOR", "");
                            docDetail.ControlYear = docNode.Values(wfStep).GetString("D_CONTROL_YEAR", "");
                            docDetail.ControlNoMaster = docNode.Values(wfStep).GetString("D_CONTROL_NO_MASTER", "");
                            docDetail.ControlNo = docNode.Values(wfStep).GetString("D_CONTROL_NO", "");
                            docDetail.DocTypeOrig = docNode.Values(wfStep).GetString("D_DOC_TYPE_ORIG", "");
                            docDetail.DocType = docNode.Values(wfStep).GetString("D_DOCUMENT_TYPE", "");
                            docDetail.FormType = docNode.Values(wfStep).GetString("D_FORM_TYPE", "");
                            docDetail.BusinessArea = docNode.Values(wfStep).GetString("D_BUSINESS_AREA", "");
                            docDetail.PolicyNumber = docNode.Values(wfStep).GetString("D_POLICY_NO", "");
                            docDetail.RequirementID = docNode.Values(wfStep).GetString("D_REQUIREMENT_ID", "");
                            docDetail.WorkType = docNode.Values(wfStep).GetString("D_WORK_TYPE", "");
                            docDetail.AcordType = docNode.Values(wfStep).GetString("D_ACORD_TYPE", "");
                            docDetail.Pages = Convert.ToInt32(docNode.Values(wfStep).GetString("D_IMG_COUNT", ""));
                            DateTime recDate = Convert.ToDateTime(docNode.Values(wfStep).GetString("D_RECEIVED_DATE", ""));
                            docDetail.ReceivedDate = recDate.ToString("MM/dd/yyyy hh:mm:ss:tt");

                            log.Debug("Getting FISDOCID");

                            docDetail.FISDOCID = _dbAccess.getFISID(docNode.Values(wfStep).GetString("D_BATCH_ITEM_ID", ""));
                            try
                            {
                                log.Debug("Retrieving Doc Type Description");
                                docDetail.DocTypeDescription = _dbAccess.getDocTypeDescription(docNode.Values(wfStep).GetString("D_DOC_TYPE_ORIG", ""));
                            }
                            catch
                            {
                                log.Debug("Unable to retrieve Doc Type Description");
                                docDetail.DocTypeDescription = "";
                            }
                            if (docDetail.DocTypeDescription == "")
                            {
                                docDetail.DocTypeDescription = docDetail.DocTypeOrig;
                            }

                            if (docDetail.PolicyNumber != "")
                            {
                                docDetail.ReferenceNumber = docDetail.PolicyNumber;
                                docDetail.ReferenceType = "Policy";
                            }

                            if (docDetail.ControlNo != "")
                            {
                                docDetail.ReferenceNumber = docDetail.ControlNo;
                                docDetail.ReferenceType = "Control";
                            }

                            if (docDetail.RequirementID == "")
                            {
                                docDetail.RequirementID = "N/A";
                            }
                            if (docDetail.ReferenceNumber == null)
                            {
                                docDetail.ReferenceNumber = "UNREADABLE";
                                docDetail.ReferenceType = "UNKNOWN";
                            }

                            log.Debug("Building Email Row");
                            dt.Rows.Add(docDetail.ReferenceNumber,
                                docDetail.ReferenceType,
                                docDetail.Pages,
                                docDetail.DocTypeDescription,
                                docDetail.RequirementID,
                                docDetail.FISDOCID,
                                docDetail.ReceivedDate);
                        }

                        #region email section
                        log.Debug("Preparing to Send Email");
                        log.Debug("EMAIL BATCH NO: " + BatchDetail.BatchNo + " VALUE: " + taskInfo.Task.Batch.Tree.Values(wfStep).GetString("emailbatch", ""));
                        
                        if (taskInfo.Task.Batch.Tree.Values(wfStep).GetString("emailbatch", "") == "TRUE")
                        {
                            BatchDetail.emailTo = taskInfo.Task.Batch.Tree.Values(wfStep).GetString("EMAIL_TO", "");
                            
                            //use the to email address or concatenate based on office number
                            if (BatchDetail.emailTo.Contains("$BSO"))
                            {
                                char[] delimiter = { '@' };
                                string[] emailSplit = BatchDetail.emailTo.Split(delimiter);
                                BatchDetail.emailTo = "BSO" + BatchDetail.officeNumber + "@" + emailSplit[1];
                            }

                            BatchDetail.emailFrom = taskInfo.Task.Batch.Tree.Values(wfStep).GetString("EMAIL_FROM", "");
                            BatchDetail.helpEmailAddress = taskInfo.Task.Batch.Tree.Values(wfStep).GetString("HELP_EMAIL", "");
                            BatchDetail.helpPhoneNumber = taskInfo.Task.Batch.Tree.Values(wfStep).GetString("HELP_PHONE", "");
                            string body = createHTMLBody(dt);
                            SMTP smtp = new SMTP();
                            log.Info("Sending SMTP Mail: Office Number: " + BatchDetail.officeNumber + " Batch No:" + BatchDetail.BatchNo);
                            smtp.SendMail(BatchDetail.emailFrom, BatchDetail.emailTo, "Confirmation of documents received in the home office", body);
                            log.Info("Sending SMTP Mail Complete batch no: " + BatchDetail.BatchNo);
                        }

                        #endregion

                        break;
                        #endregion

                    }
                }
            }
            catch(Exception ex)
            {
                log.Error("ERROR: " + ex.Message,ex);
                log.Error("Inner Exception: " + ex.InnerException);
                throw new Exception("ExecuteTask Error: " + ex.Message + " Inner Exception: " + ex.InnerException);
            }
        }

        public string createHTMLBody(DataTable dt)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat(@"<caption> Documents from =");
            sb.AppendFormat(BatchDetail.officeNumber + "<p>THIS IS AN AUTOMATIC RECEIPT NOTIFICATION FOR THE FOLLOWING SCANNED ITEMS. DO NOT REPLY TO THIS EMAIL.<p>");
            sb.AppendFormat(@"  </caption>");

            sb.Append("<TABLE BORDER=1>");

            sb.Append("<TR ALIGN='CENTER'>");

            //first append the column names.
            foreach (DataColumn column in dt.Columns)
            {
                sb.Append("<TD><B>");
                sb.Append(column.ColumnName);
                sb.Append("</B></TD>");
            }

            sb.Append("</TR>");

            // next, the column values.
            foreach (DataRow row in dt.Rows)
            {
                sb.Append("<TR ALIGN='CENTER'>");

                foreach (DataColumn column in dt.Columns)
                {
                    sb.Append("<TD>");
                    if (row[column].ToString().Trim().Length > 0)
                        sb.Append(row[column]);
                    else
                        sb.Append("&nbsp;");
                    sb.Append("</TD>");
                }
                sb.Append("</TR>");
            }
            sb.Append("</TABLE>");

            sb.AppendFormat(@"<caption> <p> Should you find any discrepancies, contact the Help Desk at " + BatchDetail.helpPhoneNumber + " or email " + BatchDetail.helpEmailAddress + ". <p>REMINDER: If you do not receive a confirmation email for each batch of scanned items by the beginning of the next business day, immediately contact the Help Desk at " + BatchDetail.helpPhoneNumber + " or email " + BatchDetail.helpEmailAddress + ".");
            sb.AppendFormat(@"</caption>");

            return sb.ToString();
        }
    }
}
