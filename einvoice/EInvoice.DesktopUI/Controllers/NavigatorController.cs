using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EInvoice.DAL.DAO;
using EInvoice.Model;
using EInvoice.Validation;
using EInvoice.Signature;
using System.IO;
using Newtonsoft.Json;
using EInvoice.DesktopUI.ViewModel;
using EInvoice.DAL.EInvoiceAPI;

namespace EInvoice.DesktopUI.Controllers
{
    
    public class NavigatorController
    {
        private readonly IDocumentDao _documentDao;
        private readonly IIssuerAPIAccessDetailsDao _accessDetailsDao;
        private readonly IReceiverDao _receiverDao;
        public NavigatorController(IDocumentDao documentDao, IIssuerAPIAccessDetailsDao accessDetailsDao,IReceiverDao receiverDao)
        {
            _documentDao = documentDao;
            _accessDetailsDao = accessDetailsDao;
            _receiverDao = receiverDao;
        }
        public void ViewDocumentDetails(Document doc)
        {
            DocumentForm form = new DocumentForm(new DocumentViewModel() { Document = doc,IsEditable = false});
            form.ShowDialog();
        }
        public DocumentSearchViewModel SearchDocuments(Issuer issuer,APIEnvironment env)
        {
            DocumentSearchViewModel model = new DocumentSearchViewModel();
            model.Issuer = issuer;
            model.Receivers =ObjectFactory.ReceiverDao.Find(issuer);
            model.Receivers.Add(new Receiver() { Id = "", Name = "الكل", InternalId = -1 });
            model.IssuanceDateFrom = DateTime.Now.AddMonths(-1);
            model.IssuanceDateTo = DateTime.Now;
            model.SubmissionDateFrom = DateTime.Now.AddMonths(-1);
            model.SubmissionDateTo = DateTime.Now;
            model.APIEnvironment = env;
            return model;
        }
        public DocumentSerachResultViewModel SearchDocuments(DocumentSearchViewModel model)
        {
            var result = _documentDao.FindDocumentSubmissions(model.IssuanceDateFrom, model.IssuanceDateTo, model.SubmissionDateFrom, model.SubmissionDateTo, model.Issuer, (int)model.APIEnvironment.Id, model.SelectedReceiver, model.InvoiceNumber, model.SelectedStatus);
            DocumentSerachResultViewModel resultModel = new DocumentSerachResultViewModel()
            {
                DocumentSearchViewModel = model,
                Issuer = model.Issuer,
                Environment = model.APIEnvironment
            };
            foreach (var doc in result)
            {
                resultModel.Lines.Add(new DocumentSearchResultLineViewModel()
                {
                    DateTimeIssued = doc.dateTimeIssued,
                    DateTimeReceived = doc.dateTimeReceived,
                    InvoiceNumber = doc.taxAuthorityDocument.ProformaInvoiceNumber,
                    ReceiverName = doc.receiverName,
                    Status = doc.status,
                    Total = doc.total,
                    UUID = doc.uuid,
                    InternalId = doc.internalId
                }) ;
            }
            return resultModel;
        }
        public SelectReportViewModel SelectReport(Issuer issuer,APIEnvironment environment)
        {
            var model = new SelectReportViewModel()
            {
                AvailableReports = ObjectFactory.ReportDefinitionDao.Find(),
                Issuer = issuer,
                APIEnvironment = environment
            };
            model.SelectReport = model.AvailableReports.FirstOrDefault();
            if (model.AvailableReports.Count > 0)
                model.EnableOkButton = true;
            else
                model.EnableOkButton = false;
            return model;
        }
        public void SelectReport(ReportDefinition report,Issuer issuer,APIEnvironment environment)
        {
            switch (report.Name)
            {
                case "Invoice Summary Report":
                    IList<Receiver> receivers = ObjectFactory.ReceiverDao.Find(issuer);
                    receivers.Add(new Receiver() { InternalId = null ,Name = "ALL",Id="ALL"});
                    InvoiceSummaryReportParametersViewModel invoiceSummaryReportParametersViewModel = new InvoiceSummaryReportParametersViewModel()
                    {
                        IssueDateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                        IssueDateTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                        Receivers = receivers,
                        ValidationResult = "",
                        Issuer = issuer,
                        APIEnvironment = environment
                    };
                    InvoiceSummaryReportParametersForm parametersForm = new InvoiceSummaryReportParametersForm(invoiceSummaryReportParametersViewModel,this);
                    parametersForm.ShowDialog();
                    break;
                case "Invoice Details Report":
                    break;
                default:
                    return;
            }
        }
        public IList<InvoiceSummaryView> ShowInvoiceSummaryReport(InvoiceSummaryReportParametersViewModel model)
        {
            if (model.IssueDateFrom > model.IssueDateTo)
            {
                model.ValidationResult = "Date From After Date To.";
                return null;
            }
            IList<InvoiceSummaryView> data = _documentDao.GetInvoiceSummary(model.APIEnvironment.Id.Value, model.Issuer.Id,model.SelectedReceiver.InternalId==null?"%":model.SelectedReceiver.Name,model.IssueDateFrom,model.IssueDateTo);
            return data;
        }
        public void DownloadPdfFile(string fileName,string uuid,APIEnvironment env,Issuer issuer)
        {
            IssuerAPIAccessDetails accessDetails = ObjectFactory.IssuerAPIAccessDetailsDao.Find(env, issuer);
            var _eInvoiceAPIProxy = new EInvoiceAPIRestSharpProxy(env, accessDetails.ClientId, accessDetails.ClientSecret);
            byte[] data = _eInvoiceAPIProxy.GetDocumentPrintOut(uuid);
            File.WriteAllBytes(fileName, data);
        }
        public SubmitDocumentFormViewModel SubmitDocuments(Issuer issuer,APIEnvironment env)
        {
            IList<Document> newDocs = _documentDao.GetNewDataFromOracle(issuer, env);
            IList<SubmitDocumentViewModel> submits = new List<SubmitDocumentViewModel>();
            IList<IValidator<Document>> validators = ValidatorFactory.CreateValidators(ObjectFactory.ActivityCodeDao.Find(), ObjectFactory.CountryCodeDao.Find(),ObjectFactory.TaxTypeDao.Find(),AppSettingsController.Settings.MaximumInvoiceTotalAmountWithoutNationalId,AppSettingsController.Settings.InvoiceSubmissionInHours);
            foreach(Document doc in newDocs)
            {
                int? custId = _receiverDao.FindReceiverId(doc.Receiver.Name);
                if (custId == null)
                {
                    _receiverDao.Insert(doc.Receiver);
                }
                else
                {
                    doc.Receiver.InternalId = custId;
                }
                SubmitDocumentViewModel model = new SubmitDocumentViewModel()
                {
                    InternalId = doc.InternalId,
                    IssueDate = doc.DateTimeIssued,
                    ProformaInvoiceNumber = doc.ProformaInvoiceNumber,
                    ReceiverName = doc.Receiver.Name,
                    SubmissionUUID = "",
                    UUID = "",
                    TotalAmount = Convert.ToDecimal(doc.TotalAmount),
                    LocalValidationResult = new ValidationResult() { ValidationState = ValidationState.Valid },
                    StatusOnPortal = "",
                    Submit = true,
                    Document = doc
                };
                foreach(IValidator<Document> validator in validators)
                {
                    var result = validator.IsValid(doc);
                    if (result.ValidationState == ValidationState.Invalid) {
                        model.LocalValidationResult.ValidationState = ValidationState.Invalid;
                        foreach (var err in result.Errors)
                            model.LocalValidationResult.Errors.Add(err);
                    }
                }
                submits.Add(model);
            }
            return new SubmitDocumentFormViewModel()
            {
                ProgressBarMin = 0,
                ProgressBarMax = submits.Count,
                ProgressBarVisible = false,
                ProgressBarValue = 0,
                Submits = new System.ComponentModel.BindingList<SubmitDocumentViewModel>(submits),
                MessageBoardText = new StringBuilder(),
                SubmitButtonEnabled = submits.Count > 0 ? true : false,
                Issuer = issuer,
                APIEnvironment = env
            };
        }
        public void UpdateDocumentStatus(Issuer issuer,APIEnvironment environment)
        {
            IssuerAPIAccessDetails accessDetails = _accessDetailsDao.Find(environment, issuer);
            IList<DocumentSubmission> submissions = _documentDao.FindDocumentSubmissionsByState(issuer, environment, "Submitted");
            var _eInvoiceAPIProxy = new EInvoiceAPIRestSharpProxy(environment, accessDetails.ClientId, accessDetails.ClientSecret);
            foreach (var submission in submissions)
            {
                var docExtended = _eInvoiceAPIProxy.GetDocument(submission.UUID);
                submission.Status = docExtended.status;
                _documentDao.SaveOrUpdateDocumentSubmission(submission);
            }
        }
        public void GenerateRejectedDocumentReport(IList<DocumentRejected> documentRejecteds,string file)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!DCOTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine($"<Title>Rejected Invoices</title>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine($"<h2>Rejected Documents {DateTime.Now.ToShortDateString()}</h2>");
            sb.AppendLine("<p>");
            sb.AppendLine("<table><tr><th>Internal Id</th><th>Error</th>");
            foreach(var rej in documentRejecteds)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{rej.internalId}</td><td>{rej.error?.Message}</td>");
                sb.AppendLine("</tr>");
            }
            sb.AppendLine ("</table>");
            sb.AppendLine("</p>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            File.WriteAllText(file, sb.ToString());
        }
        public IList<DocumentRejected> SubmitDocument(SubmitDocumentFormViewModel model)
        {
                IList<DocumentRejected> documentRejecteds = new List<DocumentRejected>();
                IList<SubmitDocumentViewModel> found = (from s in model.Submits where s.Submit select s).ToList();
                IList<DocumentSubmission> submissions = new List<DocumentSubmission>();
                IssuerAPIAccessDetails accessDetails = ObjectFactory.IssuerAPIAccessDetailsDao.Find(model.APIEnvironment, model.Issuer);
                model.ProgressBarVisible = true;
                model.SubmitButtonEnabled = false;
                DocumentSigner documentSigner = new DocumentSigner();
                IList<Document> docs = (from doc in found select doc.Document).ToList();
                var proxy = new EInvoiceAPIRestSharpProxy(model.APIEnvironment, accessDetails.ClientId, accessDetails.ClientSecret);
                foreach (var item in found)
                {
                    if(item.Document.DateTimeIssued != item.IssueDate)
                        item.Document.DateTimeIssued = item.IssueDate;
                }
                documentSigner.Sign(docs, accessDetails.SecurityToken, AppSettingsController.Settings.DLLPath, AppSettingsController.Settings.TokenIssuerName);
                foreach (var item in found)
                {
                    
                    string jsonText = JsonConvert.SerializeObject(item.Document , Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "yyyy-MM-ddTHH:mm:ssZ" });
                    if (AppSettingsController.Settings.EnableFileGeneration)
                    {
                        if (!Directory.Exists(Environment.CurrentDirectory + "\\" + AppSettingsController.Settings.SerializedFolderName))
                        {
                            Directory.CreateDirectory(Environment.CurrentDirectory + "\\" + AppSettingsController.Settings.SerializedFolderName);
                        }
                        if (!Directory.Exists(Environment.CurrentDirectory + "\\" + AppSettingsController.Settings.SignedInvoicesFolderName))
                        {
                            Directory.CreateDirectory(Environment.CurrentDirectory + "\\" + AppSettingsController.Settings.SignedInvoicesFolderName);
                        }
                        string serialized = documentSigner.Serialize(jsonText);
                        File.WriteAllText(Environment.CurrentDirectory + "\\" + AppSettingsController.Settings.SerializedFolderName + "\\" + item.Document.InternalId + ".json", serialized);
                        File.WriteAllText(Environment.CurrentDirectory + "\\" + AppSettingsController.Settings.SignedInvoicesFolderName + "\\" + item.Document.InternalId + ".json", jsonText);
                    }
                    //documentSigner.Sign(item.Document, accessDetails.SecurityToken, AppSettingsController.Settings.DLLPath, AppSettingsController.Settings.TokenIssuerName);
                    jsonText = JsonConvert.SerializeObject(new { documents = new List<Document>() { item.Document } }, Formatting.Indented, new JsonSerializerSettings() { DateFormatString = "yyyy-MM-ddTHH:mm:ssZ" });
                    var result = proxy.SubmitDocuments(jsonText);
                    foreach (DocumentRejected documentRejected in result.rejectedDocuments)
                    {
                        documentRejecteds.Add(documentRejected);
                        var submit = (from s in found where s.InternalId == documentRejected.internalId select s).FirstOrDefault();
                        submit.StatusOnPortal = "Rejected";
                        model.MessageBoardText.AppendLine("Document Rejected");
                        model.MessageBoardText.AppendLine($"Id: {documentRejected.internalId}");
                        model.MessageBoardText.AppendLine($"Error: {documentRejected.error.Message??documentRejected.error.Details[0].Message}");
                        model.MessageBoardText.AppendLine("==============================================================================================");
                        if (AppSettingsController.Settings.EnableFileGeneration)
                        {
                            if (!Directory.Exists(Environment.CurrentDirectory + "\\" + AppSettingsController.Settings.RejectedInvoicesFolderName))
                            {
                                Directory.CreateDirectory(Environment.CurrentDirectory + "\\" + AppSettingsController.Settings.RejectedInvoicesFolderName);
                            }
                            if (!File.Exists(Environment.CurrentDirectory + "\\" + AppSettingsController.Settings.RejectedInvoicesFolderName + "\\" + documentRejected.internalId + ".txt"))
                                File.WriteAllText(Environment.CurrentDirectory + "\\" + AppSettingsController.Settings.RejectedInvoicesFolderName + "\\" + documentRejected.internalId + ".txt", model.MessageBoardText.ToString());
                            else
                            {
                                using (var writer = File.AppendText(Environment.CurrentDirectory + "\\" + AppSettingsController.Settings.RejectedInvoicesFolderName + "\\" + documentRejected.internalId + ".txt"))
                                {
                                    writer.WriteLine(model.MessageBoardText);
                                }
                            }
                        }
                        model.SubmitButtonEnabled = true;
                    }
                    
                    foreach (DocumentAccepted documentAccepted in result.acceptedDocuments)
                    {
                        var submit = (from s in found where s.InternalId == documentAccepted.internalId select s).FirstOrDefault();
                        submit.SubmissionUUID = result.submissionId;
                        submit.UUID = documentAccepted.uuid;
                        _documentDao.Insert(submit.Document);
                        var submission = new DocumentSubmission() { APIEnvironment = proxy.Environment, Document = submit.Document, SubmissionDate = DateTime.Now, Status = "Submitted", SubmissionUUID = result.submissionId, UUID = documentAccepted.uuid };
                        _documentDao.InsertDocumentSubmission(submission);
                        submissions.Add(submission);
                    }
                    model.ProgressBarValue += 1;
                }
            model.ProgressBarValue = 0;
            model.ProgressBarMin = 0;
            model.ProgressBarMax = submissions.Count;
            System.Threading.Thread.Sleep(5000);
            foreach (DocumentSubmission documentSubmission in submissions)
            {
                var docExtended = proxy.GetDocument(documentSubmission.UUID);
                documentSubmission.Status = docExtended.status;
                _documentDao.SaveOrUpdateDocumentSubmission(documentSubmission);
                var temp = (from xyz in model.Submits where xyz.UUID == documentSubmission.UUID select xyz).FirstOrDefault();
                if (temp != null)
                {
                    temp.StatusOnPortal = docExtended.status;
                    if (temp.StatusOnPortal == "Invalid")
                        temp.PortalValidationResult = docExtended.validationResults;
                    if (docExtended.status == "Invalid")
                    {
                        foreach(var ttt in temp.PortalValidationResult.validationSteps)
                            if(ttt.status == "Invalid")
                                temp.StatusErrorOnPortal += ttt.error.Message+"\n";
                    }
                }
                model.ProgressBarValue += 1;
            }
            return documentRejecteds;
        }
    }
}
