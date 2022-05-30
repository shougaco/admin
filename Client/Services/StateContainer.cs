using System.Net.Http.Json;
using Application.Shared.Models.Org;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Application.Client.Services;



public class StateContainer
{
    private string? savedString;
    private Company company;

    public string Property
    {
        get => savedString ?? string.Empty;
        set
        {
            savedString = value;
            NotifyStateChanged();
        }
    }

    public Company? Company
    {
        get => company ?? new Company();
        set
        {
            company = value;
            NotifyStateChanged();
        }
    }


    public async Task<string> GetFromSessionStorage(IJSRuntime _jsRuntime, string key)
    {
        string companyId = "";
        if(!String.IsNullOrEmpty(key)) {
            companyId = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", key);
        }

        return companyId;
        
    }

    public async Task SaveInSessionStorage(IJSRuntime _jsRuntime, string key, string value)
    {
        if(!String.IsNullOrEmpty(value)) {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", key, value);
        }
        
    }
 

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}