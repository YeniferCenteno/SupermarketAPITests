using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SupermarketAPI.DTOs;
using SupermarketAPI.Services.Products;
using SupermarketAPI.Services.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SupermarketAPI.Endpoints
{
    public static class UserEndpoints
    {
        public static void Add(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/users").WithTags("Users");

            //Obtener todos los usuarios
            group.MapGet("/", async (IUserServices userServices) =>
            {
                var users = await userServices.GetUsers();
                //200 OK: la solicitud se realizo correctamente y devuelve la lista de usuarios
                return Results.Ok(users);

            }).WithOpenApi(o => new OpenApiOperation(o)
            {
                Summary = "Obtener Usuarios",
                Description = "Muestra una lista de todos los Usuarios."
            }).RequireAuthorization();

            //Obtener por ID
            group.MapGet("/{id}", async (int id, IUserServices userServices) => {
                var users = await userServices.GetUser(id);
                if (users == null)
                    return Results.NotFound(); //404 Not Found: El recurso solicitado no existe
                else
                    return Results.Ok(users); //200 ok: la solicitud se realizo correctamente y Devuelve el objeto buscado
            }).WithOpenApi(o => new OpenApiOperation(o)
            {
                Summary = "Obtener usuario",
                Description = "Busca un usuario por id."
            }).RequireAuthorization();

            //Guardar
            group.MapPost("/", async (UserRequest user, IUserServices userServices) => {
                if (user == null)
                    Results.BadRequest();// 400 Bad Request: La solicitud no se pudo procesar, error de formato

                try
                {
                    var id = await userServices.PostUser(user);

                    //201 Created: El recurso se creo con exito, se devuelve la ubicacion del recurso creado
                    return Results.Created($"api/users/{id}", user);
                }
                catch (Exception)
                {
                    //409 Conflict
                    return Results.Conflict("El nombre de usuario ya esta en uso.");
                }

            }).WithOpenApi(o => new OpenApiOperation(o)
            {
                Summary = "Crear Usuario",
                Description = "Crear un nuevo usuario."
            }).RequireAuthorization();

            //Modificar
            group.MapPut("/{id}", async (int id, UserRequest user, IUserServices userServices) => {

                var result = await userServices.PutUser(id, user);
                if (result == -1)
                    return Results.NotFound(); //404 Not Found: El recurso solicitado no existe
                else
                    return Results.Ok(result); //200 ok: la solicitud se realizo correctamente


            }).WithOpenApi(o => new OpenApiOperation(o)
            {
                Summary = "Modificar Usuario",
                Description = "Actualiza un usuario existente."
            }).RequireAuthorization();

            //Eliminar
            group.MapDelete("/{id}", async (int id, IUserServices userServices) => {

                var result = await userServices.DeleteUser(id);
                if (result == -1)
                    return Results.NotFound(); //404 Not Found: El recurso solicitado no existe
                else
                    return Results.NoContent(); //204 No content: Recurso Eliminado
            }).WithOpenApi(o => new OpenApiOperation(o)
            {
                Summary = "Eliminar Usuario",
                Description = "Eliminar un usuario existente."
            }).RequireAuthorization();

            //Login
            group.MapPost("/login", async (UserRequest user, IUserServices userService, IConfiguration config) => {

                var login = await userService.Login(user);

                if (login is null)
                    return Results.Unauthorized(); // Retorna el estado 401: Unauthorized
                else {
                    var jwtSetting = config.GetSection("JwtSetting");
                    var secretKey = jwtSetting.GetValue<string>("SecretKey");
                    var issuer = jwtSetting.GetValue<string>("Issuer");
                    var audience = jwtSetting.GetValue<string>("Audience");

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(secretKey);

                    var tokenDescriptor = new SecurityTokenDescriptor {
                        Subject = new ClaimsIdentity(new[] {
                            new Claim(ClaimTypes.Name, login.Username),
                            new Claim(ClaimTypes.Role, login.UserRole)
                        }),
                        Expires = DateTime.UtcNow.AddHours(1),
                        Issuer = issuer,
                        Audience = audience,
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    //Crear Tocken, usando parametros definidos
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    //Convertit token en cadena
                    var jwt = tokenHandler.WriteToken(token);

                    return Results.Ok(jwt);
                }
            }).WithOpenApi(o => new OpenApiOperation(o)
            {
                Summary = "Login Usuario",
                Description = "Generar Token para inicio de sesion."
            }); ;
        }
    }
}
