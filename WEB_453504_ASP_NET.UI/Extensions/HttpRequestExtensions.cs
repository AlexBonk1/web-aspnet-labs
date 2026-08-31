namespace WEB_453504_ASP_NET.UI.Extensions;


public static class HttpRequestExtensions
{
    public static bool IsAjaxRequest(this HttpRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }
}