using Microsoft.AspNetCore.Mvc.Testing;
using SupermarketAPI.DTOs;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Net;


namespace SupermarketAPI.IntegrationTests
{

    [TestClass]
    public class ProductsEndpointsTests
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

        /// <summary>
        /// Agregar token de autorizacion a la cabecera del cliente HTTP
        /// </summary>
        [TestInitialize]
        public void AgregarTokenALaCabecera()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        [TestMethod]
        public async Task ObtenerProductos_ConTokenValido_RetornaListaDeProductos()
        {
            //Arrange: pasar autorizacion a la cabecera 
            AgregarTokenALaCabecera();
            //Act: Realizar solicitud para obtener los productos 
            var products = await _httpClient.GetFromJsonAsync<List<ProductResponse>>("api/products/");
            //Assert: Verificar que la lista de productos no sea nula y que tenga elementos
            Assert.IsNotNull(products, "La lista de productos no deberia ser nula.");
            Assert.IsTrue(products.Count > 0, "La lista de productos deberia contener al menos un elemneto.");
        }

        [TestMethod]
        public async Task ObtenerProductoPorId_ProductoExistente_RetornaProducto()
        {
            //Arrange: pasar autorizacion a la cabecera y estables ID de producto existente
            AgregarTokenALaCabecera();
            var productId = 1;
            //Act: Realizar solicitud para obtener  productos por ID
            var product = await _httpClient.GetFromJsonAsync<ProductResponse>($"api/products/{productId}");
            //Assert: Verificar que el producto no sea nulo y que tenga el ID correcto 
            Assert.IsNotNull(product, "El producto no deberia ser nulo.");
            Assert.AreEqual(productId, product.ProductId, "El ID del producto devuelto no coincide.");
        }
    }
}
