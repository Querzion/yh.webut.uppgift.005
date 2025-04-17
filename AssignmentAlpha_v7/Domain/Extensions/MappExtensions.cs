using System.Reflection;

namespace Domain.Extensions;

// public static class MappExtensions
// {
//     public static TDestination MapTo<TDestination>(this object source)
//     {
//         ArgumentNullException.ThrowIfNull(source, nameof(source));
//         
//         TDestination destination = Activator.CreateInstance<TDestination>()!;
//         
//         var sourceProperties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
//         var destinationProperties = destination.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
//
//         foreach (var destinationProperty in destinationProperties)
//         {
//             var sourceProperty = sourceProperties.FirstOrDefault(p => p.Name == destinationProperty.Name && p.PropertyType == destinationProperty.PropertyType);
//             if (sourceProperty != null && destinationProperty.CanWrite)
//             {
//                 var value = sourceProperty.GetValue(source);
//                 destinationProperty.SetValue(destination, value);
//             }
//         }
//
//         return destination;
//     }
// }

public static class MappExtensions
{
    public static TDestination MapTo<TDestination>(this object source)
    {
        ArgumentNullException.ThrowIfNull(source);

        TDestination destination = Activator.CreateInstance<TDestination>()!;
        return source.MapTo(destination);
    }

    public static TDestination MapTo<TDestination>(this object source, TDestination destination)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        var sourceProperties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationProperties = typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var destProp in destinationProperties)
        {
            var sourceProp = sourceProperties.FirstOrDefault(p =>
                p.Name == destProp.Name &&
                p.PropertyType == destProp.PropertyType);

            if (sourceProp != null && destProp.CanWrite)
            {
                var value = sourceProp.GetValue(source);
                destProp.SetValue(destination, value);
            }
        }

        return destination;
    }
}
