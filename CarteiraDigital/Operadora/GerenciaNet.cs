using CarteiraDigital.Domain;
using CarteiraDigital.Enum;
using CarteiraDigital.Utils;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CarteiraDigital.Operadora
{
    public class GerenciaNet
    {
        private const string URL_BASE = "https://api-pix.gerencianet.com.br/";
        private const string URL_TOKEN = "oauth/token";
        private const string URL_COB = "v2/cob/";
        private const string URL_PIX = "v2/pix/";
        private const string URL_DEVOLUCAO = "/devolucao/";
        private const string URL_LOC = "v2/loc/";
        private const string URL_QRCODE = "/qrcode";

        private Credenciais credenciais;

        public GerenciaNet(Credenciais credenciais)
        {
            this.credenciais = credenciais;
        }

        public X509CertificateCollection ObterCertificado()
        {
            X509Certificate2 uidCert = new X509Certificate2(this.credenciais.caminhoCertificado, "");
            return new X509CertificateCollection() { uidCert };
        }

        public GenericResult<Usuario> Token()
        {
            var result = new GenericResult<Usuario>();
            try
            {
                if(string.IsNullOrEmpty(this.credenciais.client_id))
                {
                    result.Message = "client_id não informado";
                    return result;
                }

                if (string.IsNullOrEmpty(this.credenciais.client_secret))
                {
                    result.Message = "client_secret não informado";
                    return result;
                }

                if (string.IsNullOrEmpty(this.credenciais.client_secret))
                {
                    result.Message = "client_secret não informado";
                    return result;
                }

                if (string.IsNullOrEmpty(this.credenciais.caminhoCertificado))
                {
                    result.Message = "Certificado não informado";
                    return result;
                }

                var credencials = new Dictionary<string, string>{
                    {"client_id", this.credenciais.client_id},
                    {"client_secret", this.credenciais.client_secret}
                };

                var authorization = Util.Base64Encode(credencials["client_id"] + ":" + credencials["client_secret"]);
                
                var client = new RestClient(URL_BASE + URL_TOKEN);
                var request = new RestRequest(Method.POST);
                client.ClientCertificates = ObterCertificado();

                request.AddHeader("Authorization", "Basic " + authorization);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\r\n    \"grant_type\": \"client_credentials\"\r\n}", ParameterType.RequestBody);

                IRestResponse restResponse = client.Execute(request);
                string responseContent = restResponse.Content;

                if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result.Result = JsonConvert.DeserializeObject<Usuario>(responseContent);
                    result.Success = true;
                }
                else
                {
                    result.Message = responseContent;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        public GenericResult<CobResponse> Cob(string token, Pagamento pagamento, string txtId)
        {
            var result = new GenericResult<CobResponse>();
            try
            {
                var jsonCob = JsonConvert.SerializeObject(pagamento);

                var client = new RestClient(URL_BASE + "v2/cob/" + txtId);
                var request = new RestRequest(Method.PUT);
                client.ClientCertificates = ObterCertificado();

                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", jsonCob, ParameterType.RequestBody);

                IRestResponse restResponse = client.Execute(request);
                string responseContent = restResponse.Content;
                if (restResponse.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var cobResponse = JsonConvert.DeserializeObject<CobResponse>(responseContent);
                    if (cobResponse.status == GerenciaNetStatus.ATIVA)
                    {
                        result.Result = cobResponse;
                        result.Success = true;
                    }
                    else
                    {
                        result.Message = cobResponse.status;
                    }
                }
                else
                {
                    result.Message = responseContent;
                }                
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        public GenericResult<QrCodeResponse> Loc(string token, CobResponse cob)
        {
            var result = new GenericResult<QrCodeResponse>();
            try
            {
                var client = new RestClient($"{URL_BASE}{URL_LOC}{cob.loc.id}{URL_QRCODE}");
                var request = new RestRequest(Method.GET);
                client.ClientCertificates = ObterCertificado();

                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("Content-Type", "application/json");                

                IRestResponse restResponse = client.Execute(request);
                string responseContent = restResponse.Content;
                if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result.Result = JsonConvert.DeserializeObject<QrCodeResponse>(responseContent);
                    result.Success = true;
                }
                else
                {
                    result.Message = responseContent;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        public GenericResult<CobResponse> CobStatus(string token, string txtId)
        {
            var result = new GenericResult<CobResponse>();
            try
            {
                var client = new RestClient($"{URL_BASE}{URL_COB}{txtId}");
                var request = new RestRequest(Method.GET);
                client.ClientCertificates = ObterCertificado();

                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("Content-Type", "application/json");

                IRestResponse restResponse = client.Execute(request);
                string responseContent = restResponse.Content;
                if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result.Result = JsonConvert.DeserializeObject<CobResponse>(responseContent);
                    result.Success = true;
                }
                else
                {
                    result.Message = responseContent;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        public GenericResult<DevResponse> Devolucao(string token, string txtId, string txtE2EId, decimal valor)
        {
            var result = new GenericResult<DevResponse>();
            try
            {
                var dados = new
                {
                    valor = Util.GetValorFormatado(valor)
                };

                var jsonCob = JsonConvert.SerializeObject(dados);

                var client = new RestSharp.RestClient($"{URL_BASE}{URL_PIX}{txtE2EId}{URL_DEVOLUCAO}{txtId}");
                client.ClientCertificates = ObterCertificado();

                var request = new RestRequest(Method.PUT);                
                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", jsonCob, ParameterType.RequestBody);

                IRestResponse restResponse = client.Execute(request);
                string responseContent = restResponse.Content;
                if (restResponse.StatusCode == System.Net.HttpStatusCode.Created)
                {                    
                    result.Result = JsonConvert.DeserializeObject<DevResponse>(responseContent);
                    result.Success = true;
                }
                else
                {
                    result.Message = responseContent;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }
    }
}
