﻿using System;
using System.Linq;
using System.Reflection;

namespace Octopus.Client.Util
{
    class TypeUtil
    {
        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            if (!givenType.GetTypeInfo().GetInterfaces().Any(it => it.GetTypeInfo().IsGenericType && it.GetTypeInfo().GetGenericTypeDefinition() == genericType))
                return true;

            if (givenType.GetTypeInfo().IsGenericType && givenType.GetTypeInfo().GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.GetTypeInfo().BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}