using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using MultiServices.Maui.Models;

namespace MultiServices.Maui.Services.Api;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly ISecureStorageService _secureStorage;
    private const string BaseUrl = "https://api.multiservices.ma/api/v1";

    public ApiService(HttpClient httpClient, ISecureStorageService secureStorage)
    {
        _httpClient = httpClient;
        _secureStorage = secureStorage;
        _httpClient.BaseAddress = new Uri(BaseUrl);
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
            return await HandleResponse<T>(response);
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
            return await HandleResponse<T>(response);
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
            return await HandleResponse<T>(response);
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
            return await HandleResponse<T>(response);
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

    private async Task<ApiResponse<T>> HandleResponse<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            var result = JsonConvert.DeserializeObject<ApiResponse<T>>(json);
            return result ?? new ApiResponse<T> { Success = true };
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var refreshed = await RefreshTokenAsync();
            if (!refreshed)
            {
                await Shell.Current.GoToAsync("//login");
                return new ApiResponse<T> { Success = false, Message = "Session expirée" };
            }
        }

        return new ApiResponse<T>
        {
            Success = false,
            Message = $"Erreur {(int)response.StatusCode}: {response.ReasonPhrase}"
        };
    }

    private async Task<bool> RefreshTokenAsync()
    {
        var refreshToken = await _secureStorage.GetAsync("refresh_token");
        if (string.IsNullOrEmpty(refreshToken)) return false;

        try
        {
            var content = new StringContent(
                JsonConvert.SerializeObject(new { refreshToken }),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/auth/refresh", content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<AuthResponse>>(json);
                if (result?.Data != null)
                {
                    await _secureStorage.SetAsync("auth_token", result.Data.Token);
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
