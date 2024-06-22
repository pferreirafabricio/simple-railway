using SimpleRailway.Core.Services;

var service = new JsonPlaceholderService();

Console.WriteLine("Calling GetDataAsync");

var result = await service.GetDataAsync("https://jsonplaceholder.typicode.com/todos/1");

if (!result.IsSuccess)
{
    Console.WriteLine($"Error: {result.Error.Code} - {result.Error.Description}");
    return;
}

Console.WriteLine($"User ID: {result.Value.UserId}");
Console.WriteLine($"Title: {result.Value.Title}");
Console.WriteLine($"Completed: {result.Value.Completed}");
