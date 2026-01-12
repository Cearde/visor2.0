using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;

namespace VisorDoc.Services
{
    public class SharePointService
    {

        public string documentID { get; set; }

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _configuration;

        public SharePointService(IHttpClientFactory httpClientFactory, TokenService tokenService, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        public async Task<JsonElement> GetSharePointSiteInfoAsync(string id)
        {
            var accessToken = await _tokenService.GetApplicationTokenAsync();
            
            var client = _httpClientFactory.CreateClient(); 
            var apiEndpoint = _configuration.GetSection("PowerAutomate")["ApiEndpoint"];
            var powerAutomateUrl = _configuration.GetSection("PowerAutomate")["ApiEndpoint"];;
 
            var request = new HttpRequestMessage(HttpMethod.Post, powerAutomateUrl); 
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Add("Accept", "application/json;odata=verbose");


            //var payload = new { name = "cesar", email = "cesar.rodriguez@softlivesystem.com", subject = "TEST", message = id }; // Use the id in the payload
            var payload = new { documentID = id}; // Use the id in the payload
            
            request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // Si la llamada no fue exitosa, lanzamos una excepción con el contenido que nos dio SharePoint (que probablemente sea HTML).
                throw new HttpRequestException($"La llamada a Power Automate falló con el código de estado {response.StatusCode}. Respuesta recibida: {responseContent}");
            }

            try
            {
                // Si la llamada fue exitosa, intentamos interpretar la respuesta como JSON.
                var jsonDocument = JsonDocument.Parse(responseContent);
                return jsonDocument.RootElement;
            }
            catch (JsonException ex)
            {
                // Si falla la interpretación de JSON, lanzamos una excepción con el contenido que nos dio SharePoint.
                throw new JsonException($"SharePoint devolvió un código de éxito, pero la respuesta no es un JSON válido. Error: {ex.Message}. Respuesta recibida: {responseContent}");
            }
        }
    }
}
