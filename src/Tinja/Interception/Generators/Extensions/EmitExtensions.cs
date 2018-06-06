﻿using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Tinja.Extensions
{
    internal static class EmitExtensions
    {
        internal static readonly MethodInfo MethodGetMethodFromHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) });

        internal static readonly MethodInfo GetTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle", new[] { typeof(RuntimeTypeHandle) });

        internal static readonly MethodInfo GetProperty = typeof(Type).GetMethod("GetProperty", new[] { typeof(string), typeof(BindingFlags) });

        internal static readonly MethodInfo MakeGenericMethod = typeof(MethodInfo).GetMethod("MakeGenericMethod", new[] { typeof(Type[]) });

        internal static ILGenerator Box(this ILGenerator il, Type boxType)
        {
            if (boxType.IsValueType)
            {
                il.Emit(OpCodes.Box, boxType);
            }

            return il;
        }

        internal static ILGenerator UnBoxAny(this ILGenerator ilGen, Type unBoxType)
        {
            if (unBoxType.IsByRef)
            {
                ilGen.Emit(OpCodes.Unbox_Any, unBoxType.GetElementType());
            }
            else
            {
                ilGen.Emit(OpCodes.Unbox_Any, unBoxType);
            }

            return ilGen;
        }

        internal static void LoadMethodInfo(this ILGenerator ilGen, MethodInfo methodInfo)
        {
            ilGen.Emit(OpCodes.Ldtoken, methodInfo);
            ilGen.Emit(OpCodes.Ldtoken, methodInfo.DeclaringType);
            ilGen.Emit(OpCodes.Call, MethodGetMethodFromHandle);
        }

        internal static void LoadPropertyInfo(this ILGenerator ilGen, PropertyInfo propertyInfo)
        {
            ilGen.Emit(OpCodes.Ldtoken, propertyInfo.DeclaringType);
            ilGen.Emit(OpCodes.Ldstr, propertyInfo.Name);
            ilGen.Emit(OpCodes.Ldc_I4, (int)(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic));
            ilGen.Emit(OpCodes.Call, GetProperty);
        }

        internal static ILGenerator LoadMethodGenericArguments(this ILGenerator ilGen, MethodInfo methodInfo)
        {
            if (!methodInfo.IsGenericMethod)
            {
                return ilGen;
            }

            var genericArguments = methodInfo.GetGenericArguments();
            if (genericArguments.Length == 0)
            {
                return ilGen;
            }

            ilGen.Emit(OpCodes.Ldc_I4, genericArguments.Length);
            ilGen.Emit(OpCodes.Newarr, typeof(Type));

            for (var i = 0; i < genericArguments.Length; i++)
            {
                ilGen.Emit(OpCodes.Dup);
                ilGen.Emit(OpCodes.Ldc_I4, i);
                ilGen.Emit(OpCodes.Ldtoken, genericArguments[i]);
                ilGen.Emit(OpCodes.Call, GetTypeFromHandle);     //泛型实参
                ilGen.Emit(OpCodes.Stelem_Ref);
            }

            return ilGen;
        }

        internal static ILGenerator LoadDefaultValue(this ILGenerator ilGen, Type valueType)
        {
            if (valueType == typeof(void))
            {
                return ilGen;
            }

            switch (Type.GetTypeCode(valueType))
            {
                case TypeCode.Decimal:
                    ilGen.Emit(OpCodes.Ldc_I4_0);
                    ilGen.Emit(OpCodes.Newobj, valueType.GetConstructor(new Type[] { typeof(int) }));
                    break;
                case TypeCode.Double:
                    ilGen.Emit(OpCodes.Ldc_R8, default(Double));
                    break;
                case TypeCode.DBNull:
                case TypeCode.Empty:
                case TypeCode.String:
                    ilGen.Emit(OpCodes.Ldnull);
                    break;
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                    ilGen.Emit(OpCodes.Ldc_I4_0);
                    break;
                case TypeCode.Single:
                    ilGen.Emit(OpCodes.Ldc_R4, default(Single));
                    break;

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    ilGen.Emit(OpCodes.Ldc_I8);
                    break;
                default:
                    if (valueType.IsValueType)
                    {
                        var localVar = ilGen.DeclareLocal(valueType);
                        ilGen.Emit(OpCodes.Ldloca, localVar);
                        ilGen.Emit(OpCodes.Initobj, valueType);
                        ilGen.Emit(OpCodes.Ldloc, localVar);
                        break;
                    }

                    ilGen.Emit(OpCodes.Ldnull);
                    break;
            }

            return ilGen;
        }

        internal static ILGenerator BuildDefaultMethodBody(this ILGenerator ilGen, Type methodReturnType)
        {
            if (methodReturnType != typeof(void))
            {
                ilGen.LoadDefaultValue(methodReturnType);
            }

            ilGen.Emit(OpCodes.Ret);

            return ilGen;
        }



        /// <summary>
        /// </summary>
        /// <returns></returns>
        internal static ILGenerator CastValueToObject(this ILGenerator ilGen, Type valueType)
        {
            if (ilGen == null)
            {
                throw new NullReferenceException(nameof(ilGen));
            }

            if (valueType == null)
            {
                throw new NullReferenceException(nameof(valueType));
            }

            if (!valueType.IsByRef)
            {
                return ilGen.Box(valueType);
            }

            var elementType = valueType.GetElementType();
            if (elementType == null)
            {
                return ilGen;
            }

            switch (Type.GetTypeCode(elementType))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                    ilGen.Emit(OpCodes.Ldind_U1);
                    break;
                case TypeCode.SByte:
                    ilGen.Emit(OpCodes.Ldind_I1);
                    break;
                case TypeCode.Int16:
                    ilGen.Emit(OpCodes.Ldind_I2);
                    break;
                case TypeCode.Char:
                case TypeCode.UInt16:
                    ilGen.Emit(OpCodes.Ldind_U2);
                    break;
                case TypeCode.Int32:
                    ilGen.Emit(OpCodes.Ldind_I4);
                    break;
                case TypeCode.UInt32:
                    ilGen.Emit(OpCodes.Ldind_U4);
                    break;
                case TypeCode.Int64:
                    ilGen.Emit(OpCodes.Ldind_I8);
                    break;
                case TypeCode.UInt64:
                    ilGen.Emit(OpCodes.Ldind_Ref);
                    break;
                case TypeCode.Single:
                    ilGen.Emit(OpCodes.Ldind_R4);
                    break;
                case TypeCode.Double:
                    ilGen.Emit(OpCodes.Ldind_R8);
                    break;
                default:
                    ilGen.Emit(OpCodes.Ldind_Ref);
                    break;
            }

            return ilGen.Box(elementType);
        }
    }
}