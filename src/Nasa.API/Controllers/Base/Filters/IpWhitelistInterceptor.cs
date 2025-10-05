using Grpc.Core;
using Grpc.Core.Interceptors;

public class IpWhitelistInterceptor : Interceptor
{
    private readonly string[] _allowedIps = { "127.0.0.1", "192.168.1.100" };
    
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var ip = context.().Connection.RemoteIpAddress?.ToString();
        
        if (!_allowedIps.Contains(ip))
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied, "IP não autorizado"));
        }
        
        return await continuation(request, context);
    }
}