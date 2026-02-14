using System.Net.Http.Headers; using System.Text; using Newtonsoft.Json; using MultiServices.Store.Models; using MultiServices.Store.Services.Storage;
namespace MultiServices.Store.Services.Api;
public class ApiService
{
    private readonly HttpClient _httpClient; private readonly ISecureStorageService _secureStorage;
    public ApiService(HttpClient httpClient, ISecureStorageService secureStorage) { _httpClient = httpClient; _secureStorage = secureStorage; _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); }
    private async Task SetAuthHeaderAsync() { var token = await _secureStorage.GetAsync("auth_token"); if (!string.IsNullOrEmpty(token)) _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token); }
    public async Task<ApiResponse<T>> GetAsync<T>(string ep, Dictionary<string, string>? qp = null)
    { try { await SetAuthHeaderAsync(); var url = qp == null || qp.Count == 0 ? ep : ep + "?" + string.Join("&", qp.Where(k => !string.IsNullOrEmpty(k.Value)).Select(k => $"{Uri.EscapeDataString(k.Key)}={Uri.EscapeDataString(k.Value)}")); return await HandleResponse<T>(await _httpClient.GetAsync(url)); } catch (Exception ex) { return new ApiResponse<T> { Success = false, Message = ex.Message }; } }
    public async Task<ApiResponse<T>> PostAsync<T>(string ep, object? data = null)
    { try { await SetAuthHeaderAsync(); var c = data != null ? new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json") : null; return await HandleResponse<T>(await _httpClient.PostAsync(ep, c)); } catch (Exception ex) { return new ApiResponse<T> { Success = false, Message = ex.Message }; } }
    public async Task<ApiResponse<T>> PutAsync<T>(string ep, object data)
    { try { await SetAuthHeaderAsync(); return await HandleResponse<T>(await _httpClient.PutAsync(ep, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"))); } catch (Exception ex) { return new ApiResponse<T> { Success = false, Message = ex.Message }; } }
    public async Task<ApiResponse<T>> DeleteAsync<T>(string ep)
    { try { await SetAuthHeaderAsync(); return await HandleResponse<T>(await _httpClient.DeleteAsync(ep)); } catch (Exception ex) { return new ApiResponse<T> { Success = false, Message = ex.Message }; } }
    public async Task<ApiResponse<T>> UploadAsync<T>(string ep, Stream stream, string fileName)
    { try { await SetAuthHeaderAsync(); using var f = new MultipartFormDataContent(); f.Add(new StreamContent(stream), "file", fileName); return await HandleResponse<T>(await _httpClient.PostAsync(ep, f)); } catch (Exception ex) { return new ApiResponse<T> { Success = false, Message = ex.Message }; } }
    public async Task<ApiResponse<T>> UploadMultipleAsync<T>(string ep, List<(Stream Stream, string Name)> files)
    { try { await SetAuthHeaderAsync(); using var f = new MultipartFormDataContent(); foreach (var file in files) f.Add(new StreamContent(file.Stream), "files", file.Name); return await HandleResponse<T>(await _httpClient.PostAsync(ep, f)); } catch (Exception ex) { return new ApiResponse<T> { Success = false, Message = ex.Message }; } }
    private async Task<ApiResponse<T>> HandleResponse<T>(HttpResponseMessage r)
    { var json = await r.Content.ReadAsStringAsync(); if (r.IsSuccessStatusCode) return JsonConvert.DeserializeObject<ApiResponse<T>>(json) ?? new ApiResponse<T> { Success = true }; if (r.StatusCode == System.Net.HttpStatusCode.Unauthorized) { await Shell.Current.GoToAsync("//login"); return new ApiResponse<T> { Success = false, Message = "Session expiree" }; } return new ApiResponse<T> { Success = false, Message = $"Erreur {(int)r.StatusCode}" }; }
}