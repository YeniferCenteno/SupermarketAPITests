using Microsoft.AspNetCore.Mvc.Testing;
using SupermarketAPI.DTOs;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Net;


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

        /// <summary>
        /// Agregar token de autorizacion a la cabecera del cliente HTTP
        /// </summary>
        [TestInitialize]
        public void AgregarTokenALaCabecera()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        [TestMethod]
        public async Task ObtenerUsuarios_ConTokenValido_RetornaListaDeUsuarios()
        {
            //Arrange: pasar autorizacion a la cabecera 
            AgregarTokenALaCabecera();
            //Act: Realizar solicitud para obtener los usuarios 
            var users = await _httpClient.GetFromJsonAsync<List<UserResponse>>("api/users/");
            //Assert: Verificar que la lista de usuario no sea nula y que tenga elementos
            Assert.IsNotNull(users, "La lista de usuarios no deberia ser nula.");
            Assert.IsTrue(users.Count > 0, "La lista de usuarios deberia contener al menos un elemneto.");
        }

        [TestMethod]
        public async Task ObtenerUsuarioPorId_UsuarioExistente_RetornaUsuario()
        {
            //Arrange: pasar autorizacion a la cabecera y estables ID de usuario exixtente
            AgregarTokenALaCabecera();
            var userId = 1;
            //Act: Realizar solicitud para obtener  usuarios por ID
            var user = await _httpClient.GetFromJsonAsync<UserResponse>($"api/users/{userId}");
            //Assert: Verificar que el usuario no sea nulo y que tenga el ID correcto 
            Assert.IsNotNull(user, "El usuario no deberia ser nulo.");
            Assert.AreEqual(userId, user.UserId, "El ID del usuario devuelto no coincide.");
        }

        [TestMethod]
        public async Task GuardarUsuario_ConDatosValidos_RetornaCreated() {
            //Arrange: pasar autorizacion a la cabecera y preparar el nuevo usuario
            AgregarTokenALaCabecera();
            var newUser = new UserRequest { Username = "fany", UserPassword = "123", UserRole = "Verdedor" };
            //Act: Realizar solicitud para guardar el usuario 
            var response = await _httpClient.PostAsJsonAsync("api/users", newUser);
            //Assert: Verificar el codigo de estado Created
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "El usuario no se creo correctamente");
        }

        [TestMethod]
        public async Task GuardarUsuario_UsernameDuplicado_RetornaConflict()
        {
            //Arrange: pasar autorizacion a la cabecera y preparar el nuevo usuario duplicado
            AgregarTokenALaCabecera();
            var newUser = new UserRequest { Username = "contreras", UserPassword = "123", UserRole = "Verdedor" };
            //Act: Realizar solicitud para guardar el usuario con nombre de usuario duplicado
            var response = await _httpClient.PostAsJsonAsync("api/users", newUser);
            //Assert: Verificar el codigo de estado Conflict
            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode, "Se esperaba un conflicto al intentar crear usuario duplicado.");
        }

        [TestMethod]
        public async Task ModificarUsuario_UsuarioExistente_RetornaOk() {
            //Arrange: pasar autorizacion a la cabecera y preparar el nuevo usuario modificado, pasando un ID
            AgregarTokenALaCabecera();
            var existingUser = new UserRequest { Username = "yen", UserPassword = "0000", UserRole = "Administrador"};
            var userId = 12;
            //Act: Realizar solicitud para modificar usuario existente 
            var response = await _httpClient.PutAsJsonAsync($"api/users/{userId}", existingUser);
            //Assert: Verifica que la respuesta se OK
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "El usuario no se modifico correctamente");
        }

        [TestMethod]
        public async Task EliminarUsuario_UsuarioExistente_RetornaNoContent() {
            //Arrange: pasar autorizacion a la cabecera, pasando un ID
            AgregarTokenALaCabecera();
            var userId = 13;
            //Act: Realizar solicitud para eliminar usuario existente 
            var response = await _httpClient.DeleteAsync($"api/users/{userId}");
            //Assert: Verifica que la respuesta se NoContent
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode, "El usuario no se elimino correctamente");
        }

        [TestMethod]
        public async Task EliminarUsuario_UsuarioNoExistente_RetornaNotFound()
        {
            //Arrange: pasar autorizacion a la cabecera, pasando un ID
            AgregarTokenALaCabecera();
            var userId = 11;
            //Act: Realizar solicitud para eliminar usuario existente 
            var response = await _httpClient.DeleteAsync($"api/users/{userId}");
            //Assert: Verifica que la respuesta se NotFound
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, 
                "Se esperaba un 404 NotFound al intentar eliminar un usuario inexistente.");

        }
    }
}
