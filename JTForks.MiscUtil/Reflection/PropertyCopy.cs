// <copyright file="PropertyCopy.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Generic class which copies to its target type from a source
    /// type specified in the Copy method. The types are specified
    /// separately to take advantage of type inference on generic
    /// method arguments.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    public static class PropertyCopy<TTarget>
        where TTarget : class, new()
    {
        /// <summary>
        /// Static class to efficiently store the compiled delegate which can
        /// do the copying. We need a bit of work to ensure that exceptions are
        /// appropriately propagated, as the exception is generated at type initialization
        /// time, but we wish it to be thrown as an ArgumentException.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        private static class PropertyCopier<TSource>
            where TSource : class
        {
            private static readonly Func<TSource, TTarget>? copier;
            private static readonly Exception? initializationException;

            internal static TTarget Copy(TSource source)
            {
                if (initializationException != null)
                {
                    throw initializationException;
                }

                ArgumentNullException.ThrowIfNull(source);
                return copier!(source);
            }

            static PropertyCopier()
            {
                try
                {
                    copier = BuildCopier();
                }
                catch (Exception e)
                {
                    initializationException = e;
                }
            }

            private static Func<TSource, TTarget> BuildCopier()
            {
                ParameterExpression sourceParameter = Expression.Parameter(typeof(TSource), "source");
                var bindings = new List<MemberBinding>();
                foreach (PropertyInfo sourceProperty in typeof(TSource).GetProperties())
                {
                    if (!sourceProperty.CanRead)
                    {
                        continue;
                    }
                    PropertyInfo? targetProperty = typeof(TTarget).GetProperty(sourceProperty.Name) ?? throw new ArgumentException($"Property {sourceProperty.Name} is not present and accessible in {typeof(TTarget).FullName}");
                    if (!targetProperty.CanWrite)
                    {
                        throw new ArgumentException($"Property {sourceProperty.Name} is not writable in {typeof(TTarget).FullName}");
                    }
                    if (!targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                    {
                        throw new ArgumentException($"Property {sourceProperty.Name} has an incompatible type in {typeof(TTarget).FullName}");
                    }
                    bindings.Add(Expression.Bind(targetProperty, Expression.Property(sourceParameter, sourceProperty)));
                }
                Expression initializer = Expression.MemberInit(Expression.New(typeof(TTarget)), bindings);
                return Expression.Lambda<Func<TSource, TTarget>>(initializer, sourceParameter).Compile();
            }
        }
    }
}
