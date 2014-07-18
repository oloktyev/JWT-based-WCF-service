using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security.Tokens;

namespace RestService
{
    public class BearerTokenMessageInspector : IDispatchMessageInspector
    {
        public BearerTokenMessageInspector()
        {
        }
  
        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {
            object correlationState = null;
  
            HttpRequestMessageProperty requestMessage = request.Properties["httpRequest"] as HttpRequestMessageProperty;
            if (request == null)
            {
                throw new InvalidOperationException("Invalid request type.");
            }
            string authHeader = requestMessage.Headers["Authorization"];
  
            if (string.IsNullOrEmpty(authHeader) || !Authenicate(OperationContext.Current.IncomingMessageHeaders.To.AbsoluteUri, requestMessage.Method, authHeader))
            {
                WcfErrorResponseData error = new WcfErrorResponseData(HttpStatusCode.Forbidden);
                correlationState = error;
                request = null;
            }

            return correlationState;
        }
  
        private bool Authenicate(string resourceName, string action, string authHeader)
        {
            const string bearer = "Bearer "; 
            if (authHeader.StartsWith (bearer, StringComparison.InvariantCultureIgnoreCase) )
            {
                string tokenString = authHeader.Substring(bearer.Length);
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters()
                {
                    SigningToken = new BinarySecretSecurityToken(GetBytes("ThisIsAnImportantStringAndIHaveNoIdeaIfThisIsVerySecureOrNot!")),
                    ValidateIssuer = false,
                    AllowedAudience = "http://www.example.com"
                    
                };
    
           // from Token to ClaimsPrincipal - easy!
           var principal = tokenHandler.ValidateToken(tokenString, validationParameters);

                
                //HttpContext.Current.User = new ClaimsPrincipal(identity);
                return true;
            }
            return false;
        }

        private byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;

        }

        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            WcfErrorResponseData error = correlationState as WcfErrorResponseData;
            if (error != null)
            {
                HttpResponseMessageProperty responseProperty = new HttpResponseMessageProperty();
                reply.Properties["httpResponse"] = responseProperty;
                responseProperty.StatusCode = error.StatusCode;
  
                IList<KeyValuePair<string, string>> headers = error.Headers;
                if (headers != null)
                {
                    for (int i = 0; i < headers.Count; i++)
                    {
                        responseProperty.Headers.Add(headers[i].Key, headers[i].Value);
                    }
                }
            }
        }
    }
  
    public class BearerTokenServiceBehavior : IServiceBehavior
    {
        public BearerTokenServiceBehavior()
        {
  
        }
  
        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            // no-op
        }
  
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher chDisp in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher epDisp in chDisp.Endpoints)
                {
                    epDisp.DispatchRuntime.MessageInspectors.Add(new BearerTokenMessageInspector());
                }
            }
        }
  
        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            // no-op
        }
    }
  
    public class BearerTokenExtensionElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(BearerTokenServiceBehavior); }
        }
  
        protected override object CreateBehavior()
        {
            return new BearerTokenServiceBehavior();
        }
    }
  
    internal class WcfErrorResponseData
    {
        public WcfErrorResponseData(HttpStatusCode status)
            : this(status, string.Empty, null)
        {
        }
        public WcfErrorResponseData(HttpStatusCode status, string body)
            : this(status, body, null)
        {
        }
        public WcfErrorResponseData(HttpStatusCode status, string body, params KeyValuePair<string, string>[] headers)
        {
            StatusCode = status;
            Body = body;
            Headers = headers;
        }
  
  
        public HttpStatusCode StatusCode
        {
            private set;
            get;
        }
  
        public string Body
        {
            private set;
            get;
        }
  
        public IList<KeyValuePair<string, string>> Headers
        {
            private set;
            get;
        }
    }
}