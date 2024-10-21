namespace SupermarketAPI.Endpoints
{
    public static class Startup
    {
        public static void UseEndpoints(this WebApplication app) {
            ProductEndpoints.Add(app);
            UserEndpoints.Add(app);
        }
    }
}