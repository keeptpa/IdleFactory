using Microsoft.AspNetCore.Components;

public class NavigationService
{
    private readonly NavigationManager _navigationManager;

    public NavigationService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    public void NavigateTo(string route)
    {
        _navigationManager.NavigateTo(route);
    }
}