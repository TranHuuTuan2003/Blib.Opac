namespace KMS.Web.Middlewares
{
    public class SessionSetupMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionSetupMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Session.Keys.Contains("SessionId"))
            {
                context.Session.SetString("SessionId", Guid.NewGuid().ToString());
            }

            await _next(context);
        }
    }
}