using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Fitness_App_Notification.Grpc;
using Grpc.Core;

public class GrpcAuthorizeAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
        var token = authHeader?.Replace("Bearer ", "")?.Trim();

        if (string.IsNullOrEmpty(token))
        {
            context.Result = new UnauthorizedObjectResult("Missing token");
            return;
}


        var grpcClient = httpContext.RequestServices.GetService<UserService.UserServiceClient>();

        if (grpcClient == null)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return;
        }

        try
        {
            var response = await grpcClient.ValidateTokenAsync(new TokenRequest { AccessToken = token });
            httpContext.Items["User"] = response;

            await next();
        }
        catch (RpcException ex)
        {
            context.Result = new UnauthorizedObjectResult("Invalid token: " + ex.Status.Detail);
        }
    }
}

