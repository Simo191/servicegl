using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using MultiServices.Maui.Models;
using MultiServices.Maui.Services.Storage;

namespace MultiServices.Maui.Services.Api;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly ISecureStorageService _secureStorage;

    // FIX: Removed duplicate BaseAddress — already set in MauiProgram.cs via AddHttpClient
    public ApiService(HttpClient httpClient, ISecureStorageService secureStorage)
    {
        _httpClient = httpClient;
        _secureStorage = secureStorage;
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    private async Task SetAuthHeaderAsync()
    {
        var token = await _secureStorage.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint, Dictionary<string, string>? queryParams = null)
    {
        try
        {
            await SetAuthHeaderAsync();
            var url = BuildUrl(endpoint, queryParams);
            var response = await _httpClient.GetAsync(url);
            return await HandleResponse<T>(response, HttpMethod.Get, endpoint, queryParams: queryParams);
        }
        catch (Exception ex)
        {
            return new ApiResponse<T> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object? data = null)
    {
        try
        {
            await SetAuthHeaderAsync();
            var content = data != null
                ? new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
                : null;
            var response = await _httpClient.PostAsync(endpoint, content);
            return await HandleResponse<T>(response, HttpMethod.Post, endpoint, data);
        }
        catch (Exception ex)
        {
            return new ApiResponse<T> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data)
    {
        try
        {
            await SetAuthHeaderAsync();
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(endpoint, content);
            return await HandleResponse<T>(response, HttpMethod.Put, endpoint, data);
        }
        catch (Exception ex)
        {
            return new ApiResponse<T> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.DeleteAsync(endpoint);
            return await HandleResponse<T>(response, HttpMethod.Delete, endpoint);
        }
        catch (Exception ex)
        {
            return new ApiResponse<T> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<T>> UploadAsync<T>(string endpoint, Stream fileStream, string fileName)
    {
        try
        {
            await SetAuthHeaderAsync();
            using var formData = new MultipartFormDataContent();
            var streamContent = new StreamContent(fileStream);
            formData.Add(streamContent, "file", fileName);
            var response = await _httpClient.PostAsync(endpoint, formData);
            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            return new ApiResponse<T> { Success = false, Message = ex.Message };
        }
    }

    /// <summary>
    /// HandleResponse with optional retry after token refresh
    /// </summary>
    private async Task<ApiResponse<T>> HandleResponse<T>(HttpResponseMessage response,
        HttpMethod? method = null, string? endpoint = null, object? body = null,
        Dictionary<string, string>? queryParams = null, bool isRetry = false)
    {
        var json = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<ApiResponse<T>>(json);
                return result ?? new ApiResponse<T> { Success = true };
            }
            catch
            {
                // If the response isn't wrapped in ApiResponse, try direct deserialization
                try
                {
                    var data = JsonConvert.DeserializeObject<T>(json);
                    return new ApiResponse<T> { Success = true, Data = data };
                }
                catch
                {
                    return new ApiResponse<T> { Success = true };
                }
            }
        }

        // FIX: Retry after refresh (only once)
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && !isRetry)
        {
            var refreshed = await RefreshTokenAsync();
            if (refreshed && method != null && endpoint != null)
            {
                // Retry the original request
                await SetAuthHeaderAsync();
                HttpResponseMessage retryResponse;

                if (method == HttpMethod.Get)
                {
                    var url = BuildUrl(endpoint, queryParams);
                    retryResponse = await _httpClient.GetAsync(url);
                }
                else if (method == HttpMethod.Post)
                {
                    var content = body != null
                        ? new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
                        : null;
                    retryResponse = await _httpClient.PostAsync(endpoint, content);
                }
                else if (method == HttpMethod.Put)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                    retryResponse = await _httpClient.PutAsync(endpoint, content);
                }
                else if (method == HttpMethod.Delete)
                {
                    retryResponse = await _httpClient.DeleteAsync(endpoint);
                }
                else
                {
                    retryResponse = response;
                }

                return await HandleResponse<T>(retryResponse, isRetry: true);
            }

            // Refresh failed → redirect to login
            await Shell.Current.GoToAsync("//login");
            return new ApiResponse<T> { Success = false, Message = "Session expirée" };
        }

        return new ApiResponse<T>
        {
            Success = false,
            Message = $"Erreur {(int)response.StatusCode}: {response.ReasonPhrase}"
        };
    }

    /// <summary>
    /// FIX: Send both token and refreshToken as required by backend RefreshTokenDto
    /// Backend endpoint: /auth/refresh-token (not /auth/refresh)
    /// Backend expects: { token: "...", refreshToken: "..." }
    /// </summary>
    private async Task<bool> RefreshTokenAsync()
    {
        var accessToken = await _secureStorage.GetAsync("auth_token");
        var refreshToken = await _secureStorage.GetAsync("refresh_token");
        if (string.IsNullOrEmpty(refreshToken)) return false;

        try
        {
            var content = new StringContent(
                JsonConvert.SerializeObject(new
                {
                    token = accessToken ?? "",
                    refreshToken = refreshToken
                }),
                Encoding.UTF8, "application/json");

            // FIX: Correct endpoint
            var response = await _httpClient.PostAsync("/auth/refresh-token", content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<AuthResponse>>(json);
                if (result?.Data != null)
                {
                    var newToken = result.Data.GetToken();
                    await _secureStorage.SetAsync("auth_token", newToken);
                    await _secureStorage.SetAsync("refresh_token", result.Data.RefreshToken);
                    return true;
                }
            }
        }
        catch { }
        return false;
    }

    private string BuildUrl(string endpoint, Dictionary<string, string>? queryParams)
    {
        if (queryParams == null || queryParams.Count == 0) return endpoint;
        var query = string.Join("&", queryParams
            .Where(kv => !string.IsNullOrEmpty(kv.Value))
            .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
        return $"{endpoint}?{query}";
    }
}
