using Microsoft.AspNetCore.Mvc.Testing;
using SupermarketAPI.DTOs;
using System.Net.Http.Json;
using System.Net.Http.Headers;


namespace SupermarketAPI.IntegrationTests
{

    [TestClass]
    public class UserEndpointsTests
    {
        private static HttpClient _httpClient;
        private static WebApplicationFactory<Program> _factory;
        private static string _token;

        ///<summary>
        /// Configurar entorno de prueba inicializando la API y obteniendo el token JWT
        /// </summary>
        /// 

        [ClassInitialize]
        public static async Task ClassInit(TestContext context)
        {
            //Crear instancia de la aplicación en memoria
            _factory = new WebApplicationFactory<Program>();
            //Crear el cliente HTTP
            _httpClient = _factory.CreateClient();
            
            //Arrange: Preparar la carga util para el inicio de sesión
            var loginRequest = new UserRequest { Username = "fer", UserPassword = "1234" };

            //Act: Enviar la solicitud de inicio de sesión
            var loginResponse = await _httpClient.PostAsJsonAsync("api/users/login", loginRequest);
            
            //Assert: Verificar que el inicio de sesión sea exitoso
            loginResponse.EnsureSuccessStatusCode();

            _token = (await loginResponse.Content.ReadAsStringAsync()).Trim('"');
        }

        [TestInitialize]
        public void AgregarTokenALaCabecera()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }
    }
}
