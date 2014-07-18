using System;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Xml;
using Client.ServiceReference1;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var token =
                @"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJzZWxmIiwiYXVkIjoiaHR0cDovL3d3dy5leGFtcGxlLmNvbSIsIm5iZiI6MTQwNTY5NTA5MSwiZXhwIjoxNDA2Mjk5ODkxLCJ1bmlxdWVfbmFtZSI6IkpvaG4iLCJyb2xlIjoiQXV0aG9yIn0.ay-_EltjTKgZH7LWEgRJb8p0d43lGaVxgVnrvWb9660";

            //token = GetToken();
                
            CallRestService(token);
            CallSoapService(token);
        }

        private static string GetToken()
        {
            var client = new HttpClient();
            return client.GetStringAsync(new Uri("http://localhost:2727/api/identity")).Result;
        }

       
        private static void CallRestService(string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = client.GetStringAsync(new Uri("http://localhost:52854/service1.svc/getdrivers")).Result;
        }

        private static void CallSoapService(string token)
        {
            var client = new Service1Client();
            var channel = client.ChannelFactory.CreateChannelWithIssuedToken(GenerateXmlSecurityToken(token));
            var result = channel.GetDrivers();       
        }

        private static GenericXmlSecurityToken GenerateXmlSecurityToken(string token)
        {
            var document = new XmlDocument();
            XmlElement element = document.CreateElement("wsse", "BinarySecurityToken", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
            element.SetAttribute("ValueType", "urn:ietf:params:oauth:token-type:jwt");
            element.SetAttribute("EncodingType", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary");
            var encoding = new UTF8Encoding();
            element.InnerText = Convert.ToBase64String(encoding.GetBytes(token));

            return new GenericXmlSecurityToken(
                element,
                null,
                DateTime.Now,
                DateTime.Now.AddDays(1),
                null,
                null,
                null);
        }
    }
}
