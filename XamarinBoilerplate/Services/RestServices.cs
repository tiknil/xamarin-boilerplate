using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using XamarinBoilerplate.Models;

namespace XamarinBoilerplate.Services
{

    public enum RestMethod
    {
        GET = 0,
        POST = 1,
        PUT = 2,
        PATCH = 3,
        DELETE = 4
    }

    public enum ApiEnvironment
    {
        Production,
        Staging,
        Development,
    }

    public partial class RestServices
    {

        #region Constant Fields

        protected const int defaultTimeout = 60;

        public static readonly string DateTimeFormat = "dd/MM/yyyy H.mm";
        public static readonly IsoDateTimeConverter DateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = DateTimeFormat };

        #endregion

        #region Fields & Properties

        // Client http
        protected readonly HttpClient _client;

        // Questa proprietà identifica l'environment su cui saranno eseguite le chiamate API
        private ApiEnvironment _environment;
        public ApiEnvironment Environment
        {
            get { return _environment; }
            set
            {
                _environment = value;
            }
        }

        // Url di base delle api di RealTime. Varia a seconda dell'environment
        protected string _baseUrl
        {
            get
            {
                switch (Environment)
                {
                    case ApiEnvironment.Production:
                    case ApiEnvironment.Staging:
                        return "http://prod.app/api/";
                    case ApiEnvironment.Development:
                        return "http://dev.app/api/";
                }
                return "";
            }
        }

        #endregion

        #region Constructors

        public RestServices(ICacheServices cache = null)
        {
            // Istanzio il client http e imposto i parametri di default
            _client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(defaultTimeout)
            };

            // Imposto l'environment di default
#if DEBUG
            Environment = ApiEnvironment.Development;
#else
            Environment = ApiEnvironment.Production;
#endif

        }

        #endregion

        #region Destructors
        #endregion

        #region Delegates
        #endregion

        #region Events
        #endregion

        #region Enums
        #endregion

        #region Public Methods

        protected async Task<T> PerformRequest<T>(RestMethod method, string baseUrl, string endpoint, HttpContent httpContent = null) where T : BaseModel
        {

            var uri = baseUrl + endpoint;
            Debug.WriteLine("RestServices <{0}:{1}> REQUEST [{2}]", method.ToString(), uri, (httpContent != null) ? httpContent.ReadAsStringAsync().ToString() : "");

            // Imposto la risposta generica
            T response = (T)Activator.CreateInstance(typeof(T), null);
            try
            {

                // Effettuo la request
                var request = new HttpRequestMessage(new HttpMethod(method.ToString()), uri) { Content = httpContent };

                HttpResponseMessage res = await _client.SendAsync(request);

                // Parso il contenuto della risposta
                if (res.IsSuccessStatusCode)
                {
                    string content = await res.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(content))
                    {
                        response = JsonConvert.DeserializeObject<T>(content, DateTimeConverter);
#if DEBUG
                        string dynamicResponse = JsonConvert.DeserializeObject<dynamic>(content, DateTimeConverter).ToString();

#pragma warning disable CS1974 // Dynamically dispatched call may fail at runtime because one or more applicable overloads are conditional methods
                        Debug.WriteLine("RestServices <{0}:{1}> RESPONSE [{2}]: {3}", method.ToString(), uri, res.StatusCode, dynamicResponse);
#pragma warning restore CS1974 // Dynamically dispatched call may fail at runtime because one or more applicable overloads are conditional methods
#endif
                    }
                    else
                    {
                        Debug.WriteLine("RestServices <{0}:{1}> RESPONSE [{2}]", method.ToString(), uri, res.StatusCode);
                    }

                }
                else
                {
                    // Assegno l'errore alla risposta
                    response.Error = new Error
                    {
                        Code = ((int)res.StatusCode).ToString(),
                        Description = res.ReasonPhrase,
                    };
                    Debug.WriteLine("RestService<{0}> ERROR: {1}", uri, response.Error.Description);
                }
            }
            catch (Exception ex)
            {
                // gestione delle eccezioni sollevate dalla chiamata
                int errorCode = (int)ErrorCodes.ServerError;

                Exception mvEx = ex;
                if (ex.InnerException is WebException)
                {
                    mvEx = ex.InnerException;
                    errorCode = (int)ErrorCodes.NetworkUnreachable;
                }
                else if (ex is TaskCanceledException)
                {
                    errorCode = (int)ErrorCodes.NetworkTimeout;
                }

                // Se c'è un errore di Http lo inserisco nella risposta
                response.Error = new Error
                {
                    Code = errorCode.ToString(),
                    Description = mvEx.Message,
                };
                Debug.WriteLine("RestService<{0}> ERROR: {1}", uri, ex);
            }
            return response;
        }

        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        #endregion

        #region Structs
        #endregion

        #region Inner Classes
        #endregion
    }

    public partial class RestServices : IRestServices
    {

        #region Constant Fields
        #endregion

        #region Fields & Properties
        #endregion

        #region Constructors
        #endregion

        #region Destructors
        #endregion

        #region Delegates
        #endregion

        #region Events
        #endregion

        #region Enums
        #endregion

        #region Public Methods
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        #endregion

        #region Structs
        #endregion

        #region Inner Classes
        #endregion
    }
}
