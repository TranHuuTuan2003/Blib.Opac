using System.Reflection;

namespace KMS.Shared.Helpers
{
    public static class ModelMapper
    {
        public static void MapProperties<TSource, TDestination>(TSource source, TDestination destination)
        {
            if (source == null || destination == null) return;

            var sourceProps = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var destProps = typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var sourceProp in sourceProps)
            {
                var targetProp = destProps.FirstOrDefault(p =>
                    p.Name == sourceProp.Name &&
                    p.PropertyType == sourceProp.PropertyType &&
                    p.CanWrite);

                if (targetProp != null)
                {
                    var value = sourceProp.GetValue(source);
                    targetProp.SetValue(destination, value);
                }
            }
        }
    }
}
