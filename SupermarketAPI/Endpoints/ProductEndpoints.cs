using Microsoft.OpenApi.Models;
using SupermarketAPI.DTOs;
using SupermarketAPI.Models;
using SupermarketAPI.Services.Products;

namespace SupermarketAPI.Endpoints
{
    public static class ProductEndpoints
    {
        public static void Add(this IEndpointRouteBuilder routes) {
            var group = routes.MapGroup("/api/products").WithTags("Products");

            //Obtener todos los productos
            group.MapGet("/", async (IProductServices productServices) =>
            {
                var products = await productServices.GetProducts();
                //200 OK: la solicitud se realizo correctamente y devuelve la lista
                return Results.Ok(products);
            }).WithOpenApi(o => new OpenApiOperation(o)
            {
                Summary = "Obtener Productos",
                Description = "Muestra una lista de todos los productos."
            }).RequireAuthorization();

            //Obtener por ID
            group.MapGet("/{id}", async (int id, IProductServices productServices) => { 
                var products = await productServices.GetProduct(id);
                if (products == null)
                    return Results.NotFound(); //404 Not Found: El recurso solicitado no existe
                else
                    return Results.Ok(products); //200 ok: la solicitud se realizo correctamente y Devuelve el objeto buscado
            }).WithOpenApi(o => new OpenApiOperation(o)
            {
                Summary = "Obtener Producto",
                Description = "Busca un producto por id."
            }).RequireAuthorization();

            //Guardar
            group.MapPost("/", async (ProductRequest product, IProductServices productServices) => { 
                if(product == null)
                    Results.BadRequest();// 400 Bad Request: La solicitud no se pudo procesar, error de formato

                var id = await productServices.PostProduct(product);
                
                //201 Created: El recurso se creo con exito, se devuelve la ubicacion del recurso creado
                return Results.Created($"api/products/{id}", product); 
            }).WithOpenApi(o => new OpenApiOperation(o)
            {
                Summary = "Crear Producto",
                Description = "Crear un nuevo producto."
            }).RequireAuthorization();

            //Modificar
            group.MapPut("/{id}", async (int id, ProductRequest product, IProductServices productServices) => {

                var result = await productServices.PutProduct(id, product);
                if (result == -1)
                    return Results.NotFound(); //404 Not Found: El recurso solicitado no existe
                else
                    return Results.Ok(result); //200 ok: la solicitud se realizo correctamente

               
            }).WithOpenApi(o => new OpenApiOperation(o)
            {
                Summary = "Crear Producto",
                Description = "Crear un nuevo producto."
            }).RequireAuthorization();

            //Eliminar
            group.MapDelete("/{id}", async (int id, IProductServices productServices) => {

                var result = await productServices.DeleteProduct(id);
                if (result == -1)
                    return Results.NotFound(); //404 Not Found: El recurso solicitado no existe
                else
                    return Results.NoContent(); //204 No content: Recurso Eliminado
            }).WithOpenApi(o => new OpenApiOperation(o)
            {
                Summary = "Eliminar Producto",
                Description = "Eliminar un producto existente."
            }).RequireAuthorization();
        }
    }
}
