// <copyright file="TypeExt.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Linq.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Provides extension methods to System.Type to provide simple
    /// and efficient access to delegates representing reflection
    /// operations.
    /// </summary>
    public static class TypeExt
    {

        private static ConstructorInfo GetConstructor(Type type, params Type[] argumentTypes)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(argumentTypes);

            ConstructorInfo? ci = type.GetConstructor(argumentTypes);
            return ci ?? throw new InvalidOperationException($"{type.Name} has no ctor({string.Join(",", (IEnumerable<Type>)argumentTypes)})");
        }
        /// <summary>
        /// Obtains a delegate to invoke a parameterless constructor
        /// </summary>
        /// <typeparam name="TResult">The base/interface type to yield as the
        /// new value; often object except for factory pattern implementations</typeparam>
        /// <param name="type">The Type to be created</param>
        /// <returns>A delegate to the constructor if found, else null</returns>
        public static Func<TResult> Ctor<TResult>(this Type type)
        {
            return Expression.Lambda<Func<TResult>>(Expression.New(GetConstructor(type, Type.EmptyTypes))).Compile();
        }
        /// <summary>
        /// Obtains a delegate to invoke a constructor which takes a parameter
        /// </summary>
        /// <typeparam name="TArg1">The type of the constructor parameter</typeparam>
        /// <typeparam name="TResult">The base/interface type to yield as the
        /// new value; often object except for factory pattern implementations</typeparam>
        /// <param name="type">The Type to be created</param>
        /// <returns>A delegate to the constructor if found, else null</returns>
        public static Func<TArg1, TResult>
            Ctor<TArg1, TResult>(this Type type)
        {
            ParameterExpression param1 = Expression.Parameter(typeof(TArg1), "arg1");
            return Expression.Lambda<Func<TArg1, TResult>>(
                Expression.New(GetConstructor(type, typeof(TArg1)), param1), param1).Compile();
        }
        /// <summary>
        /// Obtains a delegate to invoke a constructor with multiple parameters
        /// </summary>
        /// <typeparam name="TArg1">The type of the first constructor parameter</typeparam>
        /// <typeparam name="TArg2">The type of the second constructor parameter</typeparam>
        /// <typeparam name="TResult">The base/interface type to yield as the
        /// new value; often object except for factory pattern implementations</typeparam>
        /// <param name="type">The Type to be created</param>
        /// <returns>A delegate to the constructor if found, else null</returns>
        public static Func<TArg1, TArg2, TResult>
            Ctor<TArg1, TArg2, TResult>(this Type type)
        {
            ParameterExpression param1 = Expression.Parameter(typeof(TArg1), "arg1");
            ParameterExpression param2 = Expression.Parameter(typeof(TArg2), "arg2");
            return Expression.Lambda<Func<TArg1, TArg2, TResult>>(
                Expression.New(GetConstructor(type, typeof(TArg1), typeof(TArg2)), param1, param2), param1, param2).Compile();
        }
        /// <summary>
        /// Obtains a delegate to invoke a constructor with multiple parameters
        /// </summary>
        /// <typeparam name="TArg1">The type of the first constructor parameter</typeparam>
        /// <typeparam name="TArg2">The type of the second constructor parameter</typeparam>
        /// <typeparam name="TArg3">The type of the third constructor parameter</typeparam>
        /// <typeparam name="TResult">The base/interface type to yield as the
        /// new value; often object except for factory pattern implementations</typeparam>
        /// <param name="type">The Type to be created</param>
        /// <returns>A delegate to the constructor if found, else null</returns>
        public static Func<TArg1, TArg2, TArg3, TResult>
            Ctor<TArg1, TArg2, TArg3, TResult>(this Type type)
        {
            ParameterExpression param1 = Expression.Parameter(typeof(TArg1), "arg1");
            ParameterExpression param2 = Expression.Parameter(typeof(TArg2), "arg2");
            ParameterExpression param3 = Expression.Parameter(typeof(TArg3), "arg3");
            return Expression.Lambda<Func<TArg1, TArg2, TArg3, TResult>>(
                Expression.New(GetConstructor(type, typeof(TArg1), typeof(TArg2), typeof(TArg3)), param1, param2, param3),
                    param1, param2, param3).Compile();
        }
        /// <summary>
        /// Obtains a delegate to invoke a constructor with multiple parameters
        /// </summary>
        /// <typeparam name="TArg1">The type of the first constructor parameter</typeparam>
        /// <typeparam name="TArg2">The type of the second constructor parameter</typeparam>
        /// <typeparam name="TArg3">The type of the third constructor parameter</typeparam>
        /// <typeparam name="TArg4">The type of the fourth constructor parameter</typeparam>
        /// <typeparam name="TResult">The base/interface type to yield as the
        /// new value; often object except for factory pattern implementations</typeparam>
        /// <param name="type">The Type to be created</param>
        /// <returns>A delegate to the constructor if found, else null</returns>
        public static Func<TArg1, TArg2, TArg3, TArg4, TResult>
            Ctor<TArg1, TArg2, TArg3, TArg4, TResult>(this Type type)
        {
            ParameterExpression param1 = Expression.Parameter(typeof(TArg1), "arg1");
            ParameterExpression param2 = Expression.Parameter(typeof(TArg2), "arg2");
            ParameterExpression param3 = Expression.Parameter(typeof(TArg3), "arg3");
            ParameterExpression param4 = Expression.Parameter(typeof(TArg4), "arg4");
            return Expression.Lambda<Func<TArg1, TArg2, TArg3, TArg4, TResult>>(
                Expression.New(GetConstructor(type, typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4)), param1, param2, param3, param4),
                    param1, param2, param3, param4).Compile();
        }

    }
}
