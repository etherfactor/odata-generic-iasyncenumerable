using Asp.Versioning;
using Asp.Versioning.OData;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;

namespace ODataGenericIAsyncEnumerable.Models;

public class Entity
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}

public class EntityConfiguration : IModelConfiguration
{
    public void Apply(
        ODataModelBuilder builder,
        ApiVersion apiVersion,
        string? routePrefix)
    {
        builder.EntitySet<Entity>("entities");
    }
}
