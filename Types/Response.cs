using System.Net;

namespace RestFire.Types;

public sealed record Response(
    bool IsSuccess,
    HttpStatusCode StatusCode,
    string ResponseText
)
{
    public static Response Create(
        bool isSuccess,
        HttpStatusCode statusCode,
        string responseText
    )
    {
        return new Response(isSuccess, statusCode, responseText);
    }

    public static Response Ok(
        HttpStatusCode statusCode,
        string responseText
    )
    {
        return new Response(true, statusCode, responseText);
    }

    public static Response Bad(
        HttpStatusCode statusCode,
        string responseText
    )
    {
        return new Response(false, statusCode, responseText);
    }
}