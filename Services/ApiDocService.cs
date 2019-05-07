using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using UrlMetadata.Attributes;
using UrlMetadata.Dtos.ApiDoc;
using UrlMetadata.ExtensionMethods;
using UrlMetadata.Services.Interfaces;

namespace UrlMetadata.Services
{
    public class ApiDocService : IApiDocService
    {
        private readonly IDictionary<Type, TypeDto> _typeDictionary = new Dictionary<Type, TypeDto>();

        public IEnumerable<EndpointDto> ReadControllers(params Type[] controllerTypes)
        {
            return controllerTypes.SelectMany(ReadController).ToArray();
        }

        public IEnumerable<EndpointDto> ReadController(Type controllerType)
        {
            var controllerRoute = controllerType.GetCustomAttribute<RouteAttribute>();

            IEnumerable<MethodInfo> methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            methods = methods.Where(m => !m.IsDefined(typeof(NonActionAttribute)));

            return methods.Select(m =>
            {
                var info = GetMethodInfo(m);
                info.Route = controllerRoute.Template.UrlCombine(info.Route);
                return info;
            }).ToArray();
        }

        private EndpointDto GetMethodInfo(MethodInfo method)
        {
            var name = method.Name.RegexReplace(@"([a-z])([A-Z0-9])", "$1 $2");
            var routeAttribute = method.GetCustomAttribute<RouteAttribute>();
            var route = routeAttribute?.Template ?? "/";
            var routeDescription = (routeAttribute as IDescribed)?.Description;

            var httpMethods = method.GetCustomAttributes<HttpMethodAttribute>()
                .SelectMany(m => m.HttpMethods).Distinct().ToArray();

            var queryParams = method.GetParameters().Select(GetQueryParameterInfo).ToArray();

            var returnType = method.ReturnType;
            if (returnType.GetGenericTypeDefinition() == typeof(ActionResult<>))
            {
                returnType = returnType.GetGenericArguments()[0];
            }

            var returnTypeMeta = GetClassInfo(returnType);

            return new EndpointDto
            {
                Name = name,
                Route = route,
                Description = routeDescription,
                Methods = httpMethods,
                QueryParams = queryParams,
                ReturnType = returnTypeMeta
            };
        }

        private static QueryParameterDto GetQueryParameterInfo(ParameterInfo parameterInfo)
        {
            var attribute = parameterInfo.GetCustomAttribute<FromQueryAttribute>();
            var name = attribute?.Name ?? parameterInfo.Name;
            var description = (attribute as IDescribed)?.Description;
            var defaultValue = parameterInfo.HasDefaultValue ? parameterInfo.DefaultValue : null;
            var type = GetTypeName(parameterInfo.ParameterType);
            return new QueryParameterDto
            {
                Name = name,
                Description = description,
                DefaultValue = defaultValue,
                Type = type,
                Mandatory = !parameterInfo.HasDefaultValue
            };
        }

        private TypeDto GetClassInfo(Type clazz)
        {
            if (_typeDictionary.ContainsKey(clazz))
            {
                return _typeDictionary[clazz];
            }

            var returnValue = new TypeDto
            {
                Name = GetTypeName(clazz)
            };
            _typeDictionary[clazz] = returnValue;

            returnValue.Fields = GetFields(clazz);
             
            return returnValue;
        }

        private FieldDto[] GetFields(Type clazz)
        {
            if (clazz.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(clazz.GetGenericTypeDefinition()))
            {
                clazz = clazz.GetGenericArguments()[0];
            }

            return clazz.Namespace.StartsWith("UrlMetadata") ? clazz.GetProperties()
                .Select(GetPropertyInfo).ToArray() : new FieldDto[0];
        }

        private FieldDto GetPropertyInfo(PropertyInfo field)
        {
            var name = char.ToLowerInvariant(field.Name[0]) + (field.Name.Length > 1 ? field.Name.Substring(1) : "");

            var jsonInfo = field.GetCustomAttribute<JsonPropertyAttribute>();
            if (jsonInfo != null)
            {
                if (!string.IsNullOrWhiteSpace(jsonInfo.PropertyName))
                {
                    name = jsonInfo.PropertyName;
                }

                if (jsonInfo.NullValueHandling == NullValueHandling.Ignore)
                {
                    name += '?';
                }
            }

            var description = field.GetCustomAttribute<DescriptionAttribute>()?.Description;
            return new FieldDto
            {
                Name = name,
                Description = description,
                Type = GetClassInfo(field.PropertyType)
            };
        }

        private static string GetTypeName(Type type)
        {
            string name;

            if (type.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                name = type.GetGenericArguments()[0].Name;
                if (name.ToLower().EndsWith("dto"))
                    name = name.Substring(0, name.Length - 3);
                name += "[]";
            }
            else
            {
                name = type.Name;
                if (name.ToLower().EndsWith("dto"))
                    name = name.Substring(0, name.Length - 3);
            }

            return name;
        }
    }
}
