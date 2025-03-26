using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using ODataGenericIAsyncEnumerable.Models;

namespace ODataGenericIAsyncEnumerable.Controllers;

public class EntitiesController : ODataController
{
    private const string BaseRoute = "api/v{version:apiVersion}/entities";

    private readonly ILogger _logger;

    private int _id = 0;
    private readonly Dictionary<int, Entity> _data = [];

    public EntitiesController(
        ILogger<EntitiesController> logger)
    {
        _logger = logger;

        _data.Add(++_id, new() { Id = _id, Name = "Entity 1", Description = "A description" });
        _data.Add(++_id, new() { Id = _id, Name = "Entity 2", Description = "A description" });
        _data.Add(++_id, new() { Id = _id, Name = "Entity 3", Description = "A description" });
    }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute)]
    public IActionResult Search(
        ODataQueryOptions<Entity> queryOptions,
        [FromQuery] Variant variant = Variant.None)
    {
        _ = new ODataOutputFormatter([]);

        var queryable = _data.Values.BuildMock();

        //Oddly enough, if left as-is, this will execute as an IEnumeralble, not an IAsyncEnumerable
        var asyncEnumerable = queryable.AsAsyncEnumerable();
        
        if (variant == Variant.Generic)
        {
            asyncEnumerable = GenericAsyncEnumerableWithDelay(asyncEnumerable, TimeSpan.FromSeconds(1));
        }
        else if (variant == Variant.Typed)
        {
            asyncEnumerable = TypedAsyncEnumerableWithDelay(asyncEnumerable, TimeSpan.FromSeconds(1));
        }

        var asyncEnumerableType = asyncEnumerable.GetType();
        _logger.LogInformation(
            "Attempting to return an IAsyncEnumerable of type {Type}, with generic type {GenericType}, and IAsyncEnumerable interface {IAsyncEnumerableType}",
            asyncEnumerableType,
            asyncEnumerableType.IsGenericType ? asyncEnumerableType.GetGenericTypeDefinition() : null,
            asyncEnumerableType.GetInterfaces().FirstOrDefault(e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)));

        return Ok(asyncEnumerable);
    }

    public enum Variant
    {
        None = 0,
        Typed = 1,
        Generic = 2,
    }

    private async IAsyncEnumerable<Entity> TypedAsyncEnumerableWithDelay(
        IAsyncEnumerable<Entity> asyncEnumerable,
        TimeSpan delay)
    {
        await foreach (var entity in asyncEnumerable)
        {
            await Task.Delay(delay);
            yield return entity;
        }
    }

    private async IAsyncEnumerable<TEntity> GenericAsyncEnumerableWithDelay<TEntity>(
        IAsyncEnumerable<TEntity> asyncEnumerable,
        TimeSpan delay)
    {
        await foreach (var entity in asyncEnumerable)
        {
            await Task.Delay(delay);
            yield return entity;
        }
    }
}
