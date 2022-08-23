using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using CNO.BPA.SendEmail.DataHandler;
using Emc.InputAccel.CaptureClient;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;

namespace CNO.BPA.SendEmail
{
	public class SendEmailModule : CustomCodeModule
	{
		private const string LOG_PATTERN = "%d [%t] %-5p %m%n";

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public override void ExecuteTask(IClientTask task, IBatchContext batchContext)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Expected O, but got Unknown
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			try
			{
				DataAccess dataAccess = null;
				PatternLayout val = new PatternLayout();
				val.ConversionPattern="%d [%t] %-5p %m%n";
				((LayoutSkeleton)val).ActivateOptions();
				RollingFileAppender val2 = new RollingFileAppender();
				((AppenderSkeleton)val2).Layout=((ILayout)val);
				((FileAppender)val2).AppendToFile=true;
				val2.RollingStyle=((RollingFileAppender.RollingMode)1);
				val2.MaxSizeRollBackups=4;
				val2.MaximumFileSize="10MB";
				val2.StaticLogFileName=true;
				((FileAppender)val2).File="D:\\InputAccel\\Client\\binnt\\SendEmail-log.txt";
				((AppenderSkeleton)val2).ActivateOptions();
				BasicConfigurator.Configure((IAppender)val2);
				CustomParameters ParmsCustom = new CustomParameters();
				ParmsCustom.DSN = task.BatchNode.StepData.StepConfiguration
                    .ReadString("_DSN", "");
				ParmsCustom.UserName = task.BatchNode.StepData.StepConfiguration
                    .ReadString("_USERNAME", "");
                ParmsCustom.Password = task.BatchNode.StepData.StepConfiguration
					.ReadString("_PASSWORD", "");
				dataAccess = new DataAccess(ref ParmsCustom);
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("Reference Number", typeof(string));
				dataTable.Columns.Add("Reference Type", typeof(string));
				dataTable.Columns.Add("Page Sides", typeof(string));
				dataTable.Columns.Add("Document Type", typeof(string));
				dataTable.Columns.Add("Requirement ID", typeof(string));
				dataTable.Columns.Add("Document Index Number", typeof(string));
				dataTable.Columns.Add("Time and Date of Scan", typeof(string));
				log.Debug("SendEmail Loop Starting");
				IBatchNode stepNode = batchContext.GetStepNode(task.BatchNode, "STANDARD_MDF");
				log.Debug("Looping through Children in batch");
				foreach (IBatchNode item in (IEnumerable<IBatchNode>)stepNode.GetDescendantNodes(1))
				{
					DocumentDetails documentDetails = new DocumentDetails();
					BatchDetail.officeNumber = ((IValueReader)item.NodeData.ValueSet).ReadString("D_OFFICE_NO", "");
					BatchDetail.BatchNo = ((IValueReader)batchContext.GetRoot("Standard_MDF").NodeData.ValueSet).ReadString("BATCH_NO", "");
					documentDetails.ProductCategory = ((IValueReader)item.NodeData.ValueSet).ReadString("D_PRODUCT_CATEGORY", "");
					documentDetails.AppName = ((IValueReader)item.NodeData.ValueSet).ReadString("D_APP_NAME", "");
					documentDetails.CurrentStatus = ((IValueReader)item.NodeData.ValueSet).ReadString("D_CURRENT_STATUS", "");
					documentDetails.InputSource = ((IValueReader)item.NodeData.ValueSet).ReadString("D_INPUT_SOURCE", "");
					documentDetails.TransactionType = ((IValueReader)item.NodeData.ValueSet).ReadString("D_TRANSACTION_TYPE", "");
					documentDetails.ReplacementIndicator = ((IValueReader)item.NodeData.ValueSet).ReadString("D_REPLACEMENT_INDICATOR", "");
					documentDetails.LifeProIndicator = ((IValueReader)item.NodeData.ValueSet).ReadString("D_LIFEPRO_INDICATOR", "");
					documentDetails.ControlYear = ((IValueReader)item.NodeData.ValueSet).ReadString("D_CONTROL_YEAR", "");
					documentDetails.ControlNoMaster = ((IValueReader)item.NodeData.ValueSet).ReadString("D_CONTROL_NO_MASTER", "");
					documentDetails.ControlNo = ((IValueReader)item.NodeData.ValueSet).ReadString("D_CONTROL_NO", "");
					documentDetails.DocTypeOrig = ((IValueReader)item.NodeData.ValueSet).ReadString("D_DOC_TYPE_ORIG", "");
					documentDetails.DocType = ((IValueReader)item.NodeData.ValueSet).ReadString("D_DOCUMENT_TYPE", "");
					documentDetails.FormType = ((IValueReader)item.NodeData.ValueSet).ReadString("D_FORM_TYPE", "");
					documentDetails.BusinessArea = ((IValueReader)item.NodeData.ValueSet).ReadString("D_BUSINESS_AREA", "");
					documentDetails.PolicyNumber = ((IValueReader)item.NodeData.ValueSet).ReadString("D_POLICY_NO", "");
					documentDetails.RequirementID = ((IValueReader)item.NodeData.ValueSet).ReadString("D_REQUIREMENT_ID", "");
					documentDetails.WorkType = ((IValueReader)item.NodeData.ValueSet).ReadString("D_WORK_TYPE", "");
					documentDetails.AcordType = ((IValueReader)item.NodeData.ValueSet).ReadString("D_ACORD_TYPE", "");
					documentDetails.Pages = Convert.ToInt32(((IValueReader)item.NodeData.ValueSet).ReadString("D_IMG_COUNT", ""));
					documentDetails.ReceivedDate = Convert.ToDateTime(((IValueReader)item.NodeData.ValueSet).ReadString("D_RECEIVED_DATE", "")).ToString("MM/dd/yyyy hh:mm:ss:tt");
					log.Debug("Getting FISDOCID");
					documentDetails.FISDOCID = dataAccess.getFISID(((IValueReader)item.NodeData.ValueSet).ReadString("D_BATCH_ITEM_ID", ""));
					try
					{
						log.Debug("Retrieving Doc Type Description");
						documentDetails.DocTypeDescription = dataAccess.getDocTypeDescription(((IValueReader)item.NodeData.ValueSet).ReadString("D_DOC_TYPE_ORIG", ""));
					}
					catch
					{
						log.Debug("Unable to retrieve Doc Type Description");
						documentDetails.DocTypeDescription = "";
					}
					if (documentDetails.DocTypeDescription == "")
					{
						documentDetails.DocTypeDescription = documentDetails.DocTypeOrig;
					}
					if (documentDetails.PolicyNumber != "")
					{
						documentDetails.ReferenceNumber = documentDetails.PolicyNumber;
						documentDetails.ReferenceType = "Policy";
					}
					if (documentDetails.ControlNo != "")
					{
						documentDetails.ReferenceNumber = documentDetails.ControlNo;
						documentDetails.ReferenceType = "Control";
					}
					if (documentDetails.RequirementID == "")
					{
						documentDetails.RequirementID = "N/A";
					}
					if (documentDetails.ReferenceNumber == null)
					{
						documentDetails.ReferenceNumber = "UNREADABLE";
						documentDetails.ReferenceType = "UNKNOWN";
					}
					log.Debug("Building Email Row");
					dataTable.Rows.Add(documentDetails.ReferenceNumber, documentDetails.ReferenceType, documentDetails.Pages, documentDetails.DocTypeDescription, documentDetails.RequirementID, documentDetails.FISDOCID, documentDetails.ReceivedDate);
				}
				log.Debug("Preparing to Send Email");
				log.Debug(("EMAIL BATCH NO: " + BatchDetail.BatchNo + " VALUE: " + ((IValueReader)batchContext.GetRoot("Standard_MDF").NodeData.ValueSet).ReadString("emailbatch", "")));
				if (((IValueReader)batchContext.GetRoot("Standard_MDF").NodeData.ValueSet).ReadString("emailbatch", "") == "TRUE")
				{
					BatchDetail.emailTo = ((IValueReader)batchContext.GetRoot("Standard_MDF").NodeData.ValueSet).ReadString("EMAIL_TO", "");
					if (BatchDetail.emailTo.Contains("$BSO"))
					{
						char[] separator = new char[1] { '@' };
						string[] array = BatchDetail.emailTo.Split(separator);
						BatchDetail.emailTo = "BSO" + BatchDetail.officeNumber + "@" + array[1];
					}
					BatchDetail.emailFrom = ((IValueReader)batchContext.GetRoot("Standard_MDF").NodeData.ValueSet).ReadString("EMAIL_FROM", "");
					BatchDetail.helpEmailAddress = ((IValueReader)batchContext.GetRoot("Standard_MDF").NodeData.ValueSet).ReadString("HELP_EMAIL", "");
					BatchDetail.helpPhoneNumber = ((IValueReader)batchContext.GetRoot("Standard_MDF").NodeData.ValueSet).ReadString("HELP_PHONE", "");
					string body = createHTMLBody(dataTable);
					SMTP sMTP = new SMTP();
					log.Info(("Sending SMTP Mail: Office Number: " + BatchDetail.officeNumber + " Batch No:" + BatchDetail.BatchNo));
					sMTP.SendMail(BatchDetail.emailFrom, BatchDetail.emailTo, "Confirmation of documents received in the home office", body);
					log.Info(("Sending SMTP Mail Complete batch no: " + BatchDetail.BatchNo));
				}
				task.CompleteTask();
			}
			catch (Exception ex)
			{
				log.Error(("ERROR: " + ex.Message), ex);
				log.Error(("Inner Exception: " + ex.InnerException));
				task.FailTask((FailTaskReasonCode)2, ex);
			}
		}

		public override void StartModule(ICodeModuleStartInfo startInfo)
		{
		}

		public override bool SetupCodeModule(Control parentWindow, IValueAccessor stepConfiguration)
		{
			CustomParameters ParmsCustom = new CustomParameters();
			ParmsCustom.DSN = ((IValueReader)stepConfiguration).ReadString("_DSN", "");
			ParmsCustom.UserName = ((IValueReader)stepConfiguration).ReadString("_USERNAME", "");
			ParmsCustom.Password = ((IValueReader)stepConfiguration).ReadString("_PASSWORD", "");
			CustomParameterEditor1 customParameterEditor = new CustomParameterEditor1(ref ParmsCustom);
			customParameterEditor.ShowDialog(parentWindow);
			if (customParameterEditor.DialogResult == DialogResult.OK)
			{
				stepConfiguration.WriteString("_DSN", ParmsCustom.DSN);
				stepConfiguration.WriteString("_USERNAME", ParmsCustom.UserName);
				stepConfiguration.WriteString("_PASSWORD", ParmsCustom.Password);
				return true;
			}
			return false;
		}

		public string createHTMLBody(DataTable dt)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("<caption> Documents from =");
			stringBuilder.AppendFormat(BatchDetail.officeNumber + "<p>THIS IS AN AUTOMATIC RECEIPT NOTIFICATION FOR THE FOLLOWING SCANNED ITEMS. DO NOT REPLY TO THIS EMAIL.<p>");
			stringBuilder.AppendFormat("  </caption>");
			stringBuilder.Append("<TABLE BORDER=1>");
			stringBuilder.Append("<TR ALIGN='CENTER'>");
			foreach (DataColumn column in dt.Columns)
			{
				stringBuilder.Append("<TD><B>");
				stringBuilder.Append(column.ColumnName);
				stringBuilder.Append("</B></TD>");
			}
			stringBuilder.Append("</TR>");
			foreach (DataRow row in dt.Rows)
			{
				stringBuilder.Append("<TR ALIGN='CENTER'>");
				foreach (DataColumn column2 in dt.Columns)
				{
					stringBuilder.Append("<TD>");
					if (row[column2].ToString().Trim().Length > 0)
					{
						stringBuilder.Append(row[column2]);
					}
					else
					{
						stringBuilder.Append("&nbsp;");
					}
					stringBuilder.Append("</TD>");
				}
				stringBuilder.Append("</TR>");
			}
			stringBuilder.Append("</TABLE>");
			stringBuilder.AppendFormat("<caption> <p> Should you find any discrepancies, contact the Help Desk at " + BatchDetail.helpPhoneNumber + " or email " + BatchDetail.helpEmailAddress + ". <p>REMINDER: If you do not receive a confirmation email for each batch of scanned items by the beginning of the next business day, immediately contact the Help Desk at " + BatchDetail.helpPhoneNumber + " or email " + BatchDetail.helpEmailAddress + ".");
			stringBuilder.AppendFormat("</caption>");
			return stringBuilder.ToString();
		}
	}
}
