﻿using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Tinja.Interception.Generators.Extensions
{
    public static class PropertyInfoBuilderExtensions
    {
        public static PropertyBuilder SetCustomAttributes(this PropertyBuilder builder, PropertyInfo propertyInfo)
        {
            if (builder == null || propertyInfo == null)
            {
                return builder;
            }

            foreach (var customAttriute in propertyInfo
                .CustomAttributes
                .Where(item => item.AttributeType != typeof(InjectAttribute) && item.AttributeType != typeof(InterceptorAttribute)))
            {
                var attributeBuilder = GeneratorUtility.CreateCustomAttribute(customAttriute);
                if (attributeBuilder != null)
                {
                    builder.SetCustomAttribute(attributeBuilder);
                }
            }

            return builder;
        }
    }
}
