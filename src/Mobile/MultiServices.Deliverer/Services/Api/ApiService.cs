using System.Net.Http.Headers;
using System.Text;
using MultiServices.Deliverer.Helpers;
using MultiServices.Deliverer.Services.Storage;
using Newtonsoft.Json;
namespace MultiServices.Deliverer.Services.Api;
public class ApiService
{
    private readonly HttpClient _http; private readonly ISecureStorageService _storage;
    public ApiService(HttpClient http, ISecureStorageService storage) { _http = http; _storage = storage; }
    private async Task SetAuth() { var t = await _storage.GetAsync(AppConstants.TokenKey); if (!string.IsNullOrEmpty(t)) _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", t); }
    public async Task<T?> GetAsync<T>(string ep) { try { await SetAuth(); var r = await _http.GetAsync(ep); if (r.StatusCode == System.Net.HttpStatusCode.Unauthorized) { if (await RefreshAsync()) { await SetAuth(); r = await _http.GetAsync(ep); } else { await Redir(); return default; } } r.EnsureSuccessStatusCode(); return JsonConvert.DeserializeObject<T>(await r.Content.ReadAsStringAsync()); } catch { return default; } }
    public async Task<T?> PostAsync<T>(string ep, object data) { try { await SetAuth(); var c = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"); var r = await _http.PostAsync(ep, c); if (r.StatusCode == System.Net.HttpStatusCode.Unauthorized) { if (await RefreshAsync()) { await SetAuth(); r = await _http.PostAsync(ep, c); } else { await Redir(); return default; } } r.EnsureSuccessStatusCode(); return JsonConvert.DeserializeObject<T>(await r.Content.ReadAsStringAsync()); } catch { return default; } }
    public async Task<bool> PostAsync(string ep, object? data = null) { try { await SetAuth(); var c = data != null ? new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json") : null; var r = await _http.PostAsync(ep, c); return r.IsSuccessStatusCode; } catch { return false; } }
    public async Task<bool> PutAsync(string ep, object data) { try { await SetAuth(); var c = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"); return (await _http.PutAsync(ep, c)).IsSuccessStatusCode; } catch { return false; } }
    public async Task<T?> UploadAsync<T>(string ep, Stream stream, string fileName, string paramName = "file", Dictionary<string, string>? extra = null) { try { await SetAuth(); using var fd = new MultipartFormDataContent(); var sc = new StreamContent(stream); sc.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream"); fd.Add(sc, paramName, fileName); if (extra != null) foreach (var kv in extra) fd.Add(new StringContent(kv.Value), kv.Key); var r = await _http.PostAsync(ep, fd); r.EnsureSuccessStatusCode(); return JsonConvert.DeserializeObject<T>(await r.Content.ReadAsStringAsync()); } catch { return default; } }
    private async Task<bool> RefreshAsync() { try { var rt = await _storage.GetAsync(AppConstants.RefreshTokenKey); if (string.IsNullOrEmpty(rt)) return false; var c = new StringContent(JsonConvert.SerializeObject(new { RefreshToken = rt }), Encoding.UTF8, "application/json"); var r = await _http.PostAsync(AppConstants.RefreshTokenEndpoint, c); if (!r.IsSuccessStatusCode) return false; var res = JsonConvert.DeserializeObject<Models.LoginResponse>(await r.Content.ReadAsStringAsync()); if (res == null) return false; await _storage.SetAsync(AppConstants.TokenKey, res.Token); await _storage.SetAsync(AppConstants.RefreshTokenKey, res.RefreshToken); return true; } catch { return false; } }
    private async Task Redir() { await _storage.ClearAllAsync(); await Shell.Current.GoToAsync("//login"); }
}
