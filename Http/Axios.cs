using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using RestFire.Types;

namespace RestFire.Http;

public sealed class Axios
{
    private readonly HttpClient _client = new();
    public HttpClient UseClient => _client;

    public Axios(string baseUrl)
    {
        _client.BaseAddress = new Uri(baseUrl);
        _client?.DefaultRequestHeaders.Accept.Clear();
        _client?.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public Axios UseBaseUrl(string baseAddress)
    {
        if (_client != null)
            _client.BaseAddress = new Uri(baseAddress);
        return this;
    }

    public Axios UseToken(string token, string tokenType = "Bearer")
    {
        if (_client != null)
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(tokenType, token);
        return this;
    }

    private static async Task<Response> MapToResponse(
        HttpResponseMessage response
    )
    {
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            // Check if the response content is "null"
            return responseContent.Trim() == "null"
                ? Response.Bad(response.StatusCode, "Not Found")
                : Response.Ok(response.StatusCode, responseContent);
        }

        // Handle non-success status code
        var errorContent = await response.Content.ReadAsStringAsync();
        return Response.Bad(response.StatusCode, errorContent);
    }

    private static async Task<Response> FromHttpResponseMessage(
        HttpResponseMessage httpResponseMessage
    )
    {
        try
        {
            httpResponseMessage.EnsureSuccessStatusCode();
            return await MapToResponse(httpResponseMessage);
        }
        catch (HttpRequestException e)
        {
            // Handle HttpRequestException
            return Response.Bad(
                HttpStatusCode.InternalServerError,
                "An error occurred while making the HTTP request.\n" + e.Message
            );
        }
        catch (Exception ex)
        {
            // Handle other unexpected exceptions
            return Response.Bad(
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred.\n" + ex.Message
            );
        }
    }

    public async Task<Response> GetAsync(string url)
    {
        var responseMessage = await _client.GetAsync(url);
        return await FromHttpResponseMessage(responseMessage);
    }


    public async Task<Response> PutAsync(string url, string json)
    {
        var response = await _client.PutAsync(
            url, new StringContent(json, Encoding.UTF8, "application/json")
        );
        return await FromHttpResponseMessage(response);
    }


    public async Task<bool> Put(string url, string json)
    {
        try
        {
            if (_client == null) return false;
            var response = await _client.PutAsync(
                url, new StringContent(json, Encoding.UTF8, "application/json")
            );

            response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Response> PostAsync(string url, string json)
    {
        var response = await _client.PostAsync(
            url, new StringContent(json, Encoding.UTF8, "application/json")
        );
        return await FromHttpResponseMessage(response);
    }

    public async Task<bool> Post(string url, string json)
    {
        try
        {
            if (_client == null) return false;
            var response = await _client.PostAsync(
                url, new StringContent(json, Encoding.UTF8, "application/json")
            );
            response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Response> DeleteAsync(string url)
    {
        try
        {
            var response = await _client.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                // Check if the response content is "null"
                return Response.Ok(response.StatusCode, responseContent);
            }

            // Handle non-success status code
            var errorContent = await response.Content.ReadAsStringAsync();
            return Response.Bad(response.StatusCode, errorContent);
        }
        catch (HttpRequestException e)
        {
            // Handle HttpRequestException
            return Response.Bad(
                HttpStatusCode.InternalServerError,
                "An error occurred while making the HTTP request.\n" + e.Message
            );
        }
        catch (Exception ex)
        {
            // Handle other unexpected exceptions
            return Response.Bad(
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred.\n" + ex.Message
            );
        }
    }

    public async Task<bool> Delete(string url)
    {
        try
        {
            if (_client == null) return false;
            var response = await _client.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> FileExist(string url)
    {
        if (_client == null) return false;
        var response = await _client.GetAsync(url);
        return response.IsSuccessStatusCode;
    }

    public async Task<string> ReadFile(string url)
    {
        if (_client == null) return null;
        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }

        return null;
    }
}