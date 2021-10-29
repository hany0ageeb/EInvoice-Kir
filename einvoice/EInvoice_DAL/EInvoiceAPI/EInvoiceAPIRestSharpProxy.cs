using System;
using System.Collections.Generic;
using System.Text;
using EInvoice.Model;
using RestSharp;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Net;
using EInvoice.DAL.APIErrors;

namespace EInvoice.DAL.EInvoiceAPI
{
    public class EInvoiceAPIRestSharpProxy : IEInvoiceAPIProxy
    {
        private readonly APIEnvironment _environment;
        private bool isLoggedIn = false;
        private Token logInToken = null;
        private RestClient client;
        private readonly string _client_secret;
        private readonly string _client_id;
        private readonly Timer _timer;


        public DocumentExtended GetDocument(string uuid)
        {
            LogIn(_client_id, _client_secret,false);
            
            IRestRequest request = new RestRequest("/api/v1.0/documents/{UUID}/raw", Method.GET);
            request.AddUrlSegment("UUID", uuid);
            request.AddHeader("Accept", "application/json");
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                DocumentExtended temp = JsonConvert.DeserializeObject<DocumentExtended>(response.Content);
                temp.taxAuthorityDocument = JsonConvert.DeserializeObject<Document>(temp.document);
                return temp;
            }
            else
            {
                PortalError error = null;
                if (response.ContentType == "application/json")
                    error = JsonConvert.DeserializeObject<PortalError>(response.Content);
                else
                    error = new PortalError() { Error = new Error() { Code = response.StatusCode.ToString(), Message = response.StatusDescription??response.ErrorMessage } };
                if (!string.IsNullOrEmpty(error.Error.Message))
                    throw new WebAPIException(error.Error.Message);
                else
                    throw new WebAPIException(error.Error.Details[0]?.Message);
            }
        }
        public APIEnvironment Environment
        {
            get => _environment;
        }
        public void CancelDocument(string uuid)
        {
            LogIn(_client_id, _client_secret,false);
            IRestRequest request = new RestRequest("/api/v1.0/documents/{UUID}/state", Method.PUT);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddUrlSegment("UUID", uuid);
            IRestResponse response = client.Execute(request);
            if (!response.IsSuccessful)
            {
                PortalError error = JsonConvert.DeserializeObject<PortalError>(response.Content);
                if (!string.IsNullOrEmpty(error.Error?.Message))
                    throw new WebAPIException(error.Error?.Message);
                else
                    throw new WebAPIException(error.Error?.Details[0]?.Message);
            }
        }
        public SubmissionResult SubmitDocuments(string jsonText)
        {
            LogIn(_client_id, _client_secret);
            IRestRequest request = new RestRequest("/api/v1.0/documentsubmissions",Method.POST);
            request.AddHeader("Accept","application/json");
            request.AddParameter("Application/Json", jsonText, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<SubmissionResult>(response.Content);
            }
            else
            {
                Error error = SimpleJson.DeserializeObject<Error>(response.Content);
                throw new WebAPIException(error.Message);
            }
        }
        public EInvoiceAPIRestSharpProxy(APIEnvironment environment,string client_id, string client_secret)
        {
            _environment = environment;
            _client_id = client_id;
            _client_secret = client_secret;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            client = new RestClient(environment.BaseUri);
            _timer = new Timer();
            _timer.Elapsed += _timer_Elapsed;
        }

        public DocumentTypeVersion GetDocumentTypeVersion(int id,int vid)
        {
            LogIn(_client_id, _client_secret);
            IRestRequest request = new RestRequest("/api/v1.0/documenttypes/{id}/versions/{vid}",Method.GET);
            request.AddUrlSegment("id", id);
            request.AddUrlSegment("vid", vid);
            request.AddHeader("Accept", "application/json");
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return SimpleJson.DeserializeObject<DocumentTypeVersion>(response.Content);
            }
            else
            {
                Error error = JsonConvert.DeserializeObject<Error>(response.Content);
                throw new WebAPIException(error.Message);
            }
        }
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            isLoggedIn = false;
            _timer.Stop();
            _timer.Interval = 1;
        }

        private void LogIn(string clinet_id,string client_secret, bool forceLogIn = false)
        {
            if(!isLoggedIn || forceLogIn)
            {
                IRestRequest request = new RestRequest(_environment.LogInUri, Method.POST);
                request.AddHeader("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes(clinet_id+":"+client_secret))}");
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("grant_type", "client_credentials");
                request.AddParameter("scope", "InvoicingAPI");
                IRestResponse response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    logInToken = JsonConvert.DeserializeObject<Token>(response.Content);
                    client.Authenticator = new RestSharp.Authenticators.OAuth2AuthorizationRequestHeaderAuthenticator(logInToken.access_token, logInToken.token_type);
                    isLoggedIn = true;
                    _timer.Stop();
                    _timer.Interval = logInToken.expires_in * 1000 - 20;
                    _timer.Start();
                }
                else
                {
                    Error error;
                    if (!string.IsNullOrEmpty(response.Content) && response.ContentType == "Application/json")
                        error = SimpleJson.DeserializeObject<Error>(response.Content);
                    else
                        error = new Error()
                        {
                            Code = response.StatusCode.ToString(),
                            Message = response.StatusDescription??response.ErrorMessage,
                            Target = ""
                        };
                    throw new WebAPIException($"Error While Connecting To Portal: {error.Message}");
                }
            }
        }
        public RecentDocumentsResult GetRecentDocuments(int pageNo, int pageSize)
        {
            LogIn(_client_id, _client_secret, false);
            IRestRequest request = new RestRequest("/api/v1.0/documents/recent", Method.GET);
            request.AddHeader("Accept-Language", "en");
            request.AddHeader("Accept", "application/json");
            request.AddQueryParameter("pageNo", pageNo.ToString());
            request.AddQueryParameter("pageSize", pageSize.ToString());
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<RecentDocumentsResult>(response.Content);
            }
            else
            {
                PortalError error = null;
                if (response.ContentType == "application/json")
                    error = SimpleJson.DeserializeObject<PortalError>(response.Content);
                else if (response.Content != "")
                    error = new PortalError() { Error = new Error() { Message = response.Content } };
                else
                    error = new PortalError() { Error = new Error() { Message = response.ErrorMessage ?? response.StatusDescription } };
                throw new WebAPIException($"{error?.Error?.Message??"Unable To Connect."}");
            }
        }
        public DocumentPackageRequestResult GetPackageRequests(int pageNo,int pageSize)
        {
            LogIn(_client_id, _client_secret, false);
            IRestRequest request = new RestRequest("/api/v1.0/documentpackages/requests", Method.GET);
            request.AddQueryParameter("pageNo", pageNo.ToString());
            request.AddQueryParameter("pageSize", pageSize.ToString());
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Accept-Language", "en");
            request.AddHeader("Content-type", "application/json");
            var response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<DocumentPackageRequestResult>(response.Content);
            }
            else
            {
                if(response.ContentType == "application/json")
                {
                    Error err = JsonConvert.DeserializeObject<Error>(response.Content);
                    throw new WebAPIException(err.Message);
                }
                else
                {
                    throw new WebAPIException(response.StatusDescription);
                }
            }
        }
        public string RequestDocumentPackage(DocumentPackageRequest packageRequest)
        {
            LogIn(_client_id, _client_secret, false);
            IRestRequest request = new RestRequest("/api/v1.0/documentpackages/requests",Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Accept-Language", "en");
            request.AddHeader("Content-type", "application/json");
            string jsonRequest = JsonConvert.SerializeObject(packageRequest);
            request.AddParameter("application/json", jsonRequest, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                JObject obj = JObject.Parse(response.Content);
                return obj.GetValue("requestId").Value<string>();
            }
            else
            {
                PortalError error;
                if (!string.IsNullOrEmpty(response.Content) && response.ContentType == "Application/json")
                    error = JsonConvert.DeserializeObject<PortalError>(response.Content);
                else
                    error = new PortalError()
                    {
                        Error = new Error()
                        {
                            Message = response.StatusDescription
                        }
                    };
                throw new WebAPIException($"{error?.Error?.Message}");
            }
        }
        public byte[] GetDocumentPrintOut(string uuid)
        {
            LogIn(_client_id, _client_secret, false);
            IRestRequest request = new RestRequest("/api/v1.0/documents/{uuid}/pdf", Method.GET);
            request.AddUrlSegment("uuid", uuid);
            request.AddHeader("Accept-Language", "ar");
            var response = client.Execute(request);
            if (response.IsSuccessful)
                return response.RawBytes;
            else
            {
                if (response.ContentType == "application/json")
                {
                    PortalError err = JsonConvert.DeserializeObject<PortalError>(response.Content);
                    throw new WebAPIException(err.Error?.Message??err.Error?.Details[0].Message);
                }
                else
                {
                    throw new WebAPIException(response.ErrorMessage);
                }
            }
        }
        public byte[] GetDocumentPackage(string requestId)
        {
            LogIn(_client_id, _client_secret, false);
            IRestRequest request = new RestRequest("/api/v1.0/documentpackages/{rid}", Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Accept-Language", "en");
            request.AddHeader("Content-type", "application/json");
            request.AddUrlSegment("rid", requestId);
            var response = client.Execute(request);
            if(response.IsSuccessful)
                return response.RawBytes;
            else
            {
                if(response.ContentType == "application/json")
                {
                    PortalError err = JsonConvert.DeserializeObject<PortalError>(response.Content);
                    if (err.Error!=null && err.Error.Message!=null)
                        throw new WebAPIException(err.Error.Message);
                    else if (err.Error.Details!=null && err.Error.Details.Count > 0)
                        throw new WebAPIException(err.Error.Details[0].Message);
                    else
                        throw new WebAPIException(response.Content);
                }
                else
                {
                    throw new WebAPIException("Invalid Request Id Or Package not yet ready.");
                }
            }
        }
    }
   
}
