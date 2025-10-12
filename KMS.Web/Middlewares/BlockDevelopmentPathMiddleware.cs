namespace KMS.Web.Middlewares
{
    public class BlockDevelopmentPathMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public BlockDevelopmentPathMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant();

            if (path != null && !_env.IsDevelopment())
            {
                string[] blockedPatterns =
                {
                    "/development", ".php", ".py", ".rb", ".yaml", ".yml", ".log", ".xml",
                    "swagger", ".env", "manage.py", "run.py", "config.py", "settings.py", "webpack",
                    "../", "./", ";", "assert(", "base64_decode", "md5(", ".bxss.me", "gethostbyname(", ".bat", ".ps1"
                };

                foreach (var pattern in blockedPatterns)
                {
                    if (path.EndsWith(pattern, StringComparison.OrdinalIgnoreCase) ||
                        path.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}