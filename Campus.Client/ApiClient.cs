using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Campus.Client;

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly Session _s;
    public ApiClient(HttpClient http, Session s) { _http = http; _s = s; }

    public bool LoggedIn => _s.Token != null;
    public string? Role => _s.Role;

    private void Auth() =>
        _http.DefaultRequestHeaders.Authorization =
            _s.Token == null ? null : new AuthenticationHeaderValue("Bearer", _s.Token);

    // Friendly messages only. Never show server details to the user.
    private static string? Check(HttpResponseMessage res) =>
        res.IsSuccessStatusCode ? null : (int)res.StatusCode switch
        {
            400 => "Invalid input.",
            401 => "Please log in again.",
            403 => "You are not allowed to do this.",
            404 => "Not found.",
            _ => "Server problem. Try again later."
        };

    public async Task RestoreAsync()
    {
        if (_s.Token != null) return;
        try
        {
            _s.Token = await SecureStorage.Default.GetAsync("jwt");
            _s.Role = await SecureStorage.Default.GetAsync("role");
        }
        catch { }
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var res = await _http.PostAsJsonAsync("api/auth/login", new { Email = email, Password = password });
        if (!res.IsSuccessStatusCode) return "Wrong email or password.";
        var r = (await res.Content.ReadFromJsonAsync<LoginResult>())!;
        _s.Token = r.Token;
        _s.Role = r.Role;
        try
        {
            await SecureStorage.Default.SetAsync("jwt", r.Token);
            await SecureStorage.Default.SetAsync("role", r.Role);
        }
        catch { }
        return null;
    }

    public async Task<string?> RegisterAsync(string email, string password)
    {
        var res = await _http.PostAsJsonAsync("api/auth/register", new { Email = email, Password = password });
        return res.IsSuccessStatusCode ? null : "Register failed. Password needs upper, lower, digit, symbol, 6+ chars.";
    }

    public Task LogoutAsync()
    {
        _s.Token = null;
        _s.Role = null;
        try { SecureStorage.Default.Remove("jwt"); SecureStorage.Default.Remove("role"); } catch { }
        return Task.CompletedTask;
    }

    public async Task<List<CategoryItem>> GetCategoriesAsync()
    {
        Auth();
        return await _http.GetFromJsonAsync<List<CategoryItem>>("api/categories") ?? new();
    }

    public async Task<List<RequestItem>> GetRequestsAsync()
    {
        Auth();
        return await _http.GetFromJsonAsync<List<RequestItem>>("api/requests") ?? new();
    }

    public async Task<string?> CreateAsync(string title, string description, int categoryId)
    {
        Auth();
        var res = await _http.PostAsJsonAsync("api/requests",
            new { Title = title, Description = description, CategoryId = categoryId });
        return Check(res);
    }

    public async Task<string?> AssignAsync(int id)
    {
        Auth();
        return Check(await _http.PutAsync($"api/requests/{id}/assign", null));
    }

    public async Task<string?> SetStatusAsync(int id, string status)
    {
        Auth();
        return Check(await _http.PutAsJsonAsync($"api/requests/{id}/status", new { NewStatus = status }));
    }
}
