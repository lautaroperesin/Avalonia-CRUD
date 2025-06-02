using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using AvaloniaCRUD.Class;
using AvaloniaCRUD.Interfaces;

namespace AvaloniaCRUD.Services;

public class GenericService<T> : IGenericService<T> where T : class
{
    protected readonly HttpClient client;
    protected readonly JsonSerializerOptions options;
    protected readonly string _endpoint;

    public GenericService()
    {
        this.client = new HttpClient();
        this.options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        string urlApi = Resources.urlApi;

        this._endpoint = urlApi + ApiEndpoints.GetEndpoint(typeof(T).Name);

    }

    public async Task<List<T>?> GetAllAsync()
    {
        var response = await client.GetAsync(_endpoint);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException(content?.ToString());
        }

        return JsonSerializer.Deserialize<List<T>>(content, options);
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        var response = await client.GetAsync($"{_endpoint}/{id}");
        var content = await response.Content.ReadAsStreamAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException(content?.ToString());
        }

        return JsonSerializer.Deserialize<T>(content, options);
    }

    public async Task<T?> AddAsync(T? entity)
    {
        var response = await client.PostAsJsonAsync(_endpoint, entity);
        var content = await response.Content.ReadAsStreamAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException(content?.ToString());
        }

        return JsonSerializer.Deserialize<T>(content, options);
    }

    public async Task<bool> UpdateAsync(T? entity)
    {
        var idValue = entity.GetType().GetProperty("Id").GetValue(entity);

        var response = await client.PutAsJsonAsync($"{_endpoint}/{idValue}", entity);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException(response?.ToString());
        }
        
        return response.IsSuccessStatusCode;
    }

    public async Task DeleteAsync(int id)
    {
        var response = await client.DeleteAsync($"{_endpoint}/{id}");
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException(response.ToString());
        }
    }
}