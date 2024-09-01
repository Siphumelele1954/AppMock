namespace TugwellApp.Middleware
{
    public class RedirectAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirectAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/GuestLanding" && context.Session.GetString("FirebaseToken") != null)
            {
                context.Response.Redirect("/Home/Index");
                return;
            }

            await _next(context);
        }
    }
}
