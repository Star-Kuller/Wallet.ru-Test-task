using System.Net;
using System.Net.Http.Json;
using TestTask.Client.Infrastructure.Exceptions;

namespace TestTask.Client.Models;

public abstract class BaseHttpService
{
    protected static async Task<T?> ExecuteRequestAsync<T>(Func<Task<HttpResponseMessage>> requestFunc, CancellationToken token)
    {
        try
        {
            var response = await requestFunc();

            if (response.IsSuccessStatusCode)
                return typeof(T) == typeof(string)
                    ? (T)(object) await response.Content.ReadAsStringAsync(token)
                    : await response.Content.ReadFromJsonAsync<T>(cancellationToken: token);
            
            
            Console.WriteLine($"Ошибка HTTP {response.StatusCode}");
            Console.WriteLine(response);
            
            var content = await response.Content.ReadAsStringAsync(token);
            throw response.StatusCode switch
            {
                HttpStatusCode.BadRequest => new BadRequestHttpException(content),
                HttpStatusCode.InternalServerError => new InternalServerErrorHttpException(content),
                _ => new HttpRequestException(content, null, response.StatusCode)
            };
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode is not null)
                throw;
            const string noConnectMessage = "Ошибка HTTP: нет соединения с сервером";
            Console.WriteLine(noConnectMessage);
            throw new NoConnectHttpException(noConnectMessage, e);
        }
        catch (Exception e)
        {
            Console.WriteLine("Необработанная ошибка HTTP");
            Console.WriteLine(e);
            throw;
        }
    }
    
    protected static string CreateQuery(Dictionary<string, string> keyValue) =>
        string.Join("&", keyValue.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
}