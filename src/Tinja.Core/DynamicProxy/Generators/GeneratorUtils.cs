﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Tinja.Abstractions.DynamicProxy;
using Tinja.Core.DynamicProxy.Executors;

namespace Tinja.Core.DynamicProxy.Generators
{
    public static class GeneratorUtils
    {
        private const string AssemblyName = "Tinja.Interception.DynamicProxy";

        private const string ModuleName = "ProxyModules";

        internal static ModuleBuilder ModuleBuilder { get; }

        internal static AssemblyBuilder AssemblyBuilder { get; }

        internal static Dictionary<Type, int> ProxyIndexs { get; }

        internal static readonly MethodInfo GetOrCreateInterceptors = typeof(InterceptorAccessor).GetMethod("GetOrCreateInterceptors");

        internal static readonly ConstructorInfo NewMethodInvocation = typeof(MethodInvocation).GetConstructor(new[]
        {
            typeof(object),
            typeof(MethodInfo),
            typeof(Type[]),
            typeof(object[]),
            typeof(IInterceptor[])
        });

        internal static readonly ConstructorInfo NewPropertyMethodInvocation = typeof(MethodPropertyInvocation).GetConstructor(new[]
        {
            typeof(object),
            typeof(MethodInfo),
            typeof(Type[]),
            typeof(object[]),
            typeof(IInterceptor[]),
            typeof(PropertyInfo)
        });

        static GeneratorUtils()
        {
            AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(AssemblyName), AssemblyBuilderAccess.Run);
            ModuleBuilder = AssemblyBuilder.DefineDynamicModule(ModuleName);
            ProxyIndexs = new Dictionary<Type, int>();
        }

        public static string GetProxyTypeName(Type proxyTargetType)
        {
            lock (ProxyIndexs)
            {
                var order = ProxyIndexs.GetValueOrDefault(proxyTargetType);
                if (order == 0)
                {
                    ProxyIndexs[proxyTargetType] = 1;
                }
                else
                {
                    ProxyIndexs[proxyTargetType] = order + 1;
                }

                return proxyTargetType.FullName + "." + proxyTargetType.Name + "_proxy_" + order;
            }
        }

        public static CustomAttributeBuilder CreateCustomAttribute(CustomAttributeData customAttribute)
        {
            if (customAttribute.NamedArguments == null)
            {
                return new CustomAttributeBuilder(customAttribute.Constructor, customAttribute.ConstructorArguments.Select(c => c.Value).ToArray());
            }

            var args = new object[customAttribute.ConstructorArguments.Count];
            for (var i = 0; i < args.Length; i++)
            {
                if (typeof(IEnumerable).IsAssignableFrom(customAttribute.ConstructorArguments[i].ArgumentType))
                {
                    args[i] = (customAttribute.ConstructorArguments[i].Value as IEnumerable<CustomAttributeTypedArgument>)?.Select(x => x.Value).ToArray();
                    continue;
                }

                args[i] = customAttribute.ConstructorArguments[i].Value;
            }

            var namedProperties = customAttribute
                .NamedArguments
                .Where(n => !n.IsField)
                .Select(n => customAttribute.AttributeType.GetProperty(n.MemberName))
                .ToArray();

            var properties = customAttribute
                .NamedArguments
                .Where(n => !n.IsField)
                .Select(n => n.TypedValue.Value)
                .ToArray();

            var namedFields = customAttribute
                .NamedArguments
                .Where(n => n.IsField)
                .Select(n => customAttribute.AttributeType.GetField(n.MemberName))
                .ToArray();

            var fields = customAttribute
                .NamedArguments
                .Where(n => n.IsField)
                .Select(n => n.TypedValue.Value)
                .ToArray();

            return new CustomAttributeBuilder(customAttribute.Constructor, args
               , namedProperties
               , properties, namedFields, fields);
        }
    }
}