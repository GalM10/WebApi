using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Mdb.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Mdb.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly HttpClient _client;
    [Reactive] public string? FirstName { get; set; }
    [Reactive] public string? LastName { get; set; }
    [Reactive] public string? Age { get; set; }
    [Reactive] public string? SearchText { get; set; }
    public ObservableCollection<User> Users { get; } = [];
    public ObservableCollection<User> SearchResults { get; } = [];
    public User? SelectedUser { get; set; }
    public ReactiveCommand<Unit, Unit> AddUserCommand { get; }
    public ReactiveCommand<User, Unit> DeleteUserCommand { get; }
    public ReactiveCommand<Unit, Unit> RefreshUsersCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearSearchCommand { get; }


    public MainWindowViewModel(HttpClient client)
    {
        _client = client;
        AddUserCommand = ReactiveCommand.CreateFromTask(AddUserAsync);
        DeleteUserCommand = ReactiveCommand.CreateFromTask<User>(DeleteUserAsync);
        ClearSearchCommand = ReactiveCommand.Create(() =>
        {
            SearchText = string.Empty;
            SearchResults.Clear();
        });
        RefreshUsersCommand = ReactiveCommand.CreateFromTask(LoadUsersAsync);

        this.WhenAnyValue(x => x.SearchText)
            .DistinctUntilChanged()
            .SelectMany(s => Observable.FromAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    SearchResults.Clear();
                    return;
                }

                await SearchUsersAsync(s);
            }))
            .Subscribe();

        LoadUsersAsync().ConfigureAwait(false);
    }

    private async Task AddUserAsync()
    {
        if (string.IsNullOrWhiteSpace(FirstName)
            || string.IsNullOrWhiteSpace(LastName)
            || string.IsNullOrWhiteSpace(Age))
            return;
        var newUser = new User { FirstName = FirstName, LastName = LastName, Age = Age };
        var response = await _client.PostAsJsonAsync("/api/users", newUser);

        if (response.IsSuccessStatusCode)
        {
            Users.Add(newUser);
        }
        FirstName = string.Empty;
        LastName = string.Empty;
        Age = string.Empty;
    }

    private async Task DeleteUserAsync(User user)
    {
        var response = await _client.DeleteAsync($"/api/users/{user.Id}");
        if (response.IsSuccessStatusCode)
        {
            Users.Remove(user);
        }
    }

    private async Task LoadUsersAsync()
    {
        var users = _client.GetFromJsonAsAsyncEnumerable<User>("/api/users");
        Users.Clear();
        await foreach (var u in users)
        {
            if (u != null)
                Users.Add(u);
        }
    }

    private async Task SearchUsersAsync(string search)
    {
        var response = await _client.GetAsync($"/api/users/search/{search}");
        if (response.IsSuccessStatusCode)
        {
            var users = await response.Content.ReadFromJsonAsync<List<User>>();
            SearchResults.Clear();
            if (users != null)
            {
                foreach (var u in users)
                {
                    SearchResults.Add(u);
                }
            }
        }
    }
}