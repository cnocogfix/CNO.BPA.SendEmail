using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CNO.BPA.SendEmail
{
    public class DocumentDetails
    {
        private string productCategory;
        private string appName;
        private string currentStatus;
        private string inputSource;
        private string transactionType;
        private string replacementIndicator;
        private string lifeProIndicator;
        private string controlYear;
        private string controlNo;
        private string controlNoMaster;
        private string docTypeOrig;
        private string docType;
        private string formType;
        private string businessArea;
        private string policyNumber;
        private string launchType2;
        private string workType;
        private string acordType;
        private string templateCode;
        private string requirementID;
        private string referenceType;
        private string receivedDate;
        private string fisDocID;
        private string docTypeDescription;
        private string referenceNumber;

        private int docNumber;
        private int pages;

        public int Pages
        {
            get { return pages; }
            set { pages = value; }
        }

        public string ReferenceNumber
        {
            get { return referenceNumber; }
            set { referenceNumber = value; }
        }

        public  string TemplateCode
        {
            get { return templateCode; }
            set { templateCode = value; }
        }

        public int DocNumber
        {
            get { return docNumber; }
            set { docNumber = value; }
        }


        public string ProductCategory
        {
            get { return productCategory; }
            set { productCategory = value; }
        }

        public string AppName
        {
            get { return appName; }
            set { appName = value; }
        }

        public string CurrentStatus
        {
            get { return currentStatus; }
            set { currentStatus = value; }
        }

        public string InputSource
        {
            get { return inputSource; }
            set { inputSource = value; }
        }

        public string TransactionType
        {
            get { return transactionType; }
            set { transactionType = value; }
        }

        public string ReplacementIndicator
        {
            get { return replacementIndicator; }
            set { replacementIndicator = value; }
        }

        public string LifeProIndicator
        {
            get { return lifeProIndicator; }
            set { lifeProIndicator = value; }
        }

        public string ControlYear
        {
            get { return controlYear; }
            set { controlYear = value; }
        }

        public string ControlNo
        {
            get { return controlNo; }
            set { controlNo = value; }
        }

        public string ControlNoMaster
        {
            get { return controlNoMaster; }
            set { controlNoMaster = value; }
        }

        public string DocTypeOrig
        {
            get { return docTypeOrig; }
            set { docTypeOrig = value; }
        }

        public string DocType
        {
            get { return docType; }
            set { docType = value; }
        }

        public string FormType
        {
            get { return formType; }
            set { formType = value; }
        }

        public string BusinessArea
        {
            get { return businessArea; }
            set { businessArea = value; }
        }

        public string PolicyNumber
        {
            get { return policyNumber; }
            set { policyNumber = value; }
        }

        public string LaunchType2
        {
            get { return launchType2; }
            set { launchType2 = value; }
        }

        public string WorkType
        {
            get { return workType; }
            set { workType = value; }
        }

        public string AcordType
        {
            get { return acordType; }
            set { acordType = value; }
        }

        public string DocTypeDescription
        {
            get { return docTypeDescription; }
            set { docTypeDescription = value; }
        }

        public string FISDOCID
        {
            get { return fisDocID; }
            set { fisDocID = value; }
        }

        public string ReceivedDate
        {
            get { return receivedDate; }
            set { receivedDate = value; }
        }

        public string ReferenceType
        {
            get { return referenceType; }
            set { referenceType = value; }
        }
        public string RequirementID
        {
            get { return requirementID; }
            set { requirementID = value; }
        }
        //public static void Clear()
        //{
        //    productCategory = "";
        //    appName = "";
        //    currentStatus = "";
        //    inputSource = "";
        //    transactionType = "";
        //    replacementIndicator = "";
        //    lifeProIndicator = "";
        //    controlYear = "";
        //    controlNo = "";
        //    controlNoMaster = "";
        //    docTypeOrig = "";
        //    docType = "";
        //    formType = "";
        //    businessArea = "";
        //    policyNumber = "";
        //    launchType2 = "";
        //    workType = "";
        //    acordType = "";
        //    templateCode = "";

        //    referenceType = "";
        //    receivedDate = "";
        //    fisDocID = "";
        //    docTypeDescription = "";
        //    docNumber = 0;
        //}

    }
}
