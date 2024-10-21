using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Identity.Client;
using SupermarketAPI.DTOs;
using System.Net.Http;
using System.Net.Http.Json;


namespace SupermarketAPI.IntegrationTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            //Crear Instancia de la Aplicación en Memoria
            using var application = new WebApplicationFactory<Program>();

            //Crear Cliente HTTP para Enviar Solicitudes
            using var _httpClient = application.CreateClient();

            var userSession = new UserRequest { Username = "fer", UserPassword = "1234" };
            var response = await _httpClient.PostAsJsonAsync("api/users/login", userSession);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<string>();
            }
        }
    }
}