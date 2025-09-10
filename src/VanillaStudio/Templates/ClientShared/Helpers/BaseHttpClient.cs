using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using {{ProjectName}}.Framework;
using {{ProjectName}}.Framework.Utils;
using {{ProjectName}}.ServiceContracts.Models;

namespace {{ProjectName}}.Client.Shared;

public interface BaseHttpClient
{
    Task<T> GetFromJsonAsync<T>(string requestUri);
    Task<T> PostAsJsonAsync<T>(string requestUri, object formBusinessObject);
    Task<T> PutAsJsonAsync<T>(string requestUri, object formBusinessObject);
    Task<T> DeleteAsync<T>(string requestUri);
    public string BaseAddress { get; set; }
}

public class HttpTokenClient : BaseHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService localStorageService;
    public string BaseAddress { get; set; } = string.Empty;

    public HttpTokenClient(HttpClient client, ILocalStorageService localStorageService)
    {
        _httpClient = client;
        this.localStorageService = localStorageService;
    }

    public async Task<T> GetFromJsonAsync<T>(string requestUri)
    {
        _ = _httpClient ?? throw new ArgumentNullException($"{nameof(_httpClient)} is null in {GetType().Name}");
        var authToken = await localStorageService.GetValue("auth_token");
        if (!string.IsNullOrEmpty(authToken))
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");
        }

        var response = await _httpClient.GetAsync(requestUri);
        var responseString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var responseObject = JsonSerializer.Deserialize<T>(responseString, JsonSerializerOptions.Web);
            return responseObject ?? throw new Exception("response is null");
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException("Please login to continue");
        }

        throw new HttpRequestException($"Request failed: {response.StatusCode} - {responseString}");
    }

    public async Task<T> PostAsJsonAsync<T>(string requestUri, object formBusinessObject)
    {
        _ = _httpClient ?? throw new ArgumentNullException($"{nameof(_httpClient)} is null in {GetType().Name}");
        var authToken = await localStorageService.GetValue("auth_token");
        if (!string.IsNullOrEmpty(authToken))
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");
        }

        var response = await _httpClient.PostAsJsonAsync(requestUri, formBusinessObject);
        var responseString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            if (typeof(T) == typeof(string))
            {
                return (dynamic)responseString;
            }

            if (typeof(T) == typeof(int))
            {
                return (dynamic)SafeConvert.ToInt32(responseString);
            }

            if (typeof(T) == typeof(bool))
            {
                return (dynamic)Convert.ToBoolean(responseString);
            }

            if (typeof(T) == typeof(Guid))
            {
                return (dynamic)JsonSerializer.Deserialize<Guid>(responseString);
            }

            throw new InvalidCastException($"Conversion for {typeof(T)} is not defined in BaseHttpClient");
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException("Please login to continue");
        }

        throw new HttpRequestException($"Request failed: {response.StatusCode} - {responseString}");
    }

    public async Task<T> PutAsJsonAsync<T>(string requestUri, object formBusinessObject)
    {
        _ = _httpClient ?? throw new ArgumentNullException($"{nameof(_httpClient)} is null in {GetType().Name}");
        var authToken = await localStorageService.GetValue("auth_token");
        if (!string.IsNullOrEmpty(authToken))
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");
        }

        var response = await _httpClient.PutAsJsonAsync(requestUri, formBusinessObject);
        var responseString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            if (typeof(T) == typeof(string))
            {
                return (dynamic)responseString;
            }

            if (typeof(T) == typeof(int))
            {
                return (dynamic)SafeConvert.ToInt32(responseString);
            }

            if (typeof(T) == typeof(bool))
            {
                return (dynamic)Convert.ToBoolean(responseString);
            }

            if (typeof(T) == typeof(Guid))
            {
                return (dynamic)JsonSerializer.Deserialize<Guid>(responseString);
            }

            throw new InvalidCastException($"Conversion for {typeof(T)} is not defined in BaseHttpClient");
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException("Please login to continue");
        }

        throw new HttpRequestException($"Request failed: {response.StatusCode} - {responseString}");
    }

    public async Task<T> DeleteAsync<T>(string requestUri)
    {
        _ = _httpClient ?? throw new ArgumentNullException($"{nameof(_httpClient)} is null in {GetType().Name}");
        var authToken = await localStorageService.GetValue("auth_token");
        if (!string.IsNullOrEmpty(authToken))
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");
        }

        var response = await _httpClient.DeleteAsync(requestUri);
        var responseString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            if (typeof(T) == typeof(string))
            {
                return (dynamic)responseString;
            }

            if (typeof(T) == typeof(int))
            {
                return (dynamic)SafeConvert.ToInt32(responseString);
            }

            if (typeof(T) == typeof(bool))
            {
                return (dynamic)Convert.ToBoolean(responseString);
            }

            if (typeof(T) == typeof(Guid))
            {
                return (dynamic)JsonSerializer.Deserialize<Guid>(responseString);
            }

            throw new InvalidCastException($"Conversion for {typeof(T)} is not defined in BaseHttpClient");
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException("Please login to continue");
        }

        throw new HttpRequestException($"Request failed: {response.StatusCode} - {responseString}");
    }
}

public class HttpCookieClient : BaseHttpClient
{
    private readonly HttpClient _httpClient;
    public string BaseAddress { get; set; } = string.Empty;

    public HttpCookieClient(HttpClient client)
    {
        _httpClient = client;
    }

    public async Task<T> GetFromJsonAsync<T>(string requestUri)
    {
        _ = _httpClient ?? throw new ArgumentNullException($"{nameof(_httpClient)} is null in {GetType().Name}");

        var response = await _httpClient.GetAsync(requestUri);
        var responseString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var responseObject = JsonSerializer.Deserialize<T>(responseString, JsonSerializerOptions.Web);
            return responseObject ?? throw new Exception("response is null");
        }

        throw new HttpRequestException($"Request failed: {response.StatusCode} - {responseString}");
    }

    public async Task<T> PostAsJsonAsync<T>(string requestUri, object formBusinessObject)
    {
        _ = _httpClient ?? throw new ArgumentNullException($"{nameof(_httpClient)} is null in {GetType().Name}");

        var response = await _httpClient.PostAsJsonAsync(requestUri, formBusinessObject);
        var responseString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            if (typeof(T) == typeof(string))
            {
                return (dynamic)responseString;
            }

            if (typeof(T) == typeof(int))
            {
                return (dynamic)SafeConvert.ToInt32(responseString);
            }

            if (typeof(T) == typeof(bool))
            {
                return (dynamic)Convert.ToBoolean(responseString);
            }

            if (typeof(T) == typeof(Guid))
            {
                return (dynamic)JsonSerializer.Deserialize<Guid>(responseString);
            }

            throw new InvalidCastException($"Conversion for {typeof(T)} is not defined in BaseHttpClient");
        }

        throw new HttpRequestException($"Request failed: {response.StatusCode} - {responseString}");
    }

    public async Task<T> PutAsJsonAsync<T>(string requestUri, object formBusinessObject)
    {
        _ = _httpClient ?? throw new ArgumentNullException($"{nameof(_httpClient)} is null in {GetType().Name}");

        var response = await _httpClient.PutAsJsonAsync(requestUri, formBusinessObject);
        var responseString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            if (typeof(T) == typeof(string))
            {
                return (dynamic)responseString;
            }

            if (typeof(T) == typeof(int))
            {
                return (dynamic)SafeConvert.ToInt32(responseString);
            }

            if (typeof(T) == typeof(bool))
            {
                return (dynamic)Convert.ToBoolean(responseString);
            }

            if (typeof(T) == typeof(Guid))
            {
                return (dynamic)JsonSerializer.Deserialize<Guid>(responseString);
            }

            throw new InvalidCastException($"Conversion for {typeof(T)} is not defined in BaseHttpClient");
        }

        throw new HttpRequestException($"Request failed: {response.StatusCode} - {responseString}");
    }

    public async Task<T> DeleteAsync<T>(string requestUri)
    {
        _ = _httpClient ?? throw new ArgumentNullException($"{nameof(_httpClient)} is null in {GetType().Name}");

        var response = await _httpClient.DeleteAsync(requestUri);
        var responseString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            if (typeof(T) == typeof(string))
            {
                return (dynamic)responseString;
            }

            if (typeof(T) == typeof(int))
            {
                return (dynamic)SafeConvert.ToInt32(responseString);
            }

            if (typeof(T) == typeof(bool))
            {
                return (dynamic)Convert.ToBoolean(responseString);
            }

            if (typeof(T) == typeof(Guid))
            {
                return (dynamic)JsonSerializer.Deserialize<Guid>(responseString);
            }

            throw new InvalidCastException($"Conversion for {typeof(T)} is not defined in BaseHttpClient");
        }

        throw new HttpRequestException($"Request failed: {response.StatusCode} - {responseString}");
    }
}
