using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MSPOC.CrossCutting
{
    public static class Extensions
    {
        public static IConfiguration GetConfiguration(this IServiceProvider serviceProvider)
            => serviceProvider.GetService<IConfiguration>();

        public static T GetSection<T>(this IConfiguration configuration)
            => configuration.GetSection(typeof(T).Name).Get<T>();

        public static T As<T>(this ActionResult<T> action)
            where T : class
        {
            return ((action.Result as CreatedAtActionResult).Value as T);
        }

        public static void GuardIsNotNull<T>(T config) where T : class
        {
            if (config is null)
                throw new ArgumentNullException($"{typeof(T).Name}");
        }
    }
}