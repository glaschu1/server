using GraphQL.Builders;
using GraphQL.Types;
using System.Collections.Generic;

namespace GraphQL.Server.Authorization.AspNetCore
{
    /// <summary>
    /// Extension methods to configure authorization requirements for GraphQL elements: types, fields, schema.
    /// </summary>
    public static class AuthorizationMetadataExtensions
    {
        /// <summary>
        /// Metadata key name for storing authorization policy names. Value of this key
        /// is a simple list of strings.
        /// </summary>
        public const string POLICY_KEY = "Authorization__Policies";

        /// <summary>
        /// Gets a list of authorization policy names for the specified metadata provider.
        /// </summary>
        /// <param name="provider">
        /// Metadata provider. This can be an instance of <see cref="GraphType"/>,
        /// <see cref="FieldType"/>, <see cref="Schema"/> or others.
        /// </param>
        /// <returns> List of authorization policy names applied to this metadata provider. </returns>
        public static List<string> GetPolicies(this IProvideMetadata provider) => provider.GetMetadata<List<string>>(POLICY_KEY);

        /// <summary>
        /// Gets a boolean value that determines whether any authorization policy is applied to this metadata provider.
        /// </summary>
        /// <param name="provider">
        /// Metadata provider. This can be an instance of <see cref="GraphType"/>,
        /// <see cref="FieldType"/>, <see cref="Schema"/> or others.
        /// </param>
        /// <returns> <c>true</c> if any authorization policy is applied, otherwise <c>false</c>. </returns>
        public static bool RequiresAuthorization(this IProvideMetadata provider)
        {
            var policies = GetPolicies(provider);
            return policies != null && policies.Count > 0;
        }

        /// <summary>
        /// Adds authorization policy to the specified metadata provider. If the provider already contains
        /// a policy with the same name, then it will not be added twice.
        /// </summary>
        /// <typeparam name="TMetadataProvider"> The type of metadata provider. Generics are used here to
        /// let compiler infer the returning type to allow methods chaining.
        /// </typeparam>
        /// <param name="provider">
        /// Metadata provider. This can be an instance of <see cref="GraphType"/>,
        /// <see cref="FieldType"/>, <see cref="Schema"/> or others.
        /// </param>
        /// <param name="policy"> Authorization policy name. </param>
        /// <returns> The reference to the specified <paramref name="provider"/>. </returns>
        public static TMetadataProvider AuthorizeWith<TMetadataProvider>(this TMetadataProvider provider, string policy)
            where TMetadataProvider : IProvideMetadata
        {
            var list = GetPolicies(provider) ?? new List<string>();

            if (!list.Contains(policy))
                list.Add(policy);

            provider.Metadata[POLICY_KEY] = list;
            return provider;
        }

        [System.Obsolete("This method will be removed in v5. Use TMetadataProvider AuthorizeWith<TMetadataProvider>(this TMetadataProvider provider, string policy) instead.")]
        public static void AuthorizeWith(this IProvideMetadata type, string policy) // TODO: remove in v5
        {
            var list = GetPolicies(type) ?? new List<string>();

            if (!list.Contains(policy))
                list.Add(policy);

            type.Metadata[POLICY_KEY] = list;
        }

        /// <summary>
        /// Adds authorization policy to the specified field builder. If the underlying field already contains
        /// a policy with the same name, then it will not be added twice.
        /// </summary>
        /// <typeparam name="TSourceType"></typeparam>
        /// <typeparam name="TReturnType"></typeparam>
        /// <param name="builder"></param>
        /// <param name="policy"> Authorization policy name. </param>
        /// <returns> The reference to the specified <paramref name="builder"/>. </returns>
        public static FieldBuilder<TSourceType, TReturnType> AuthorizeWith<TSourceType, TReturnType>(
            this FieldBuilder<TSourceType, TReturnType> builder, string policy)
        {
            builder.FieldType.AuthorizeWith(policy);
            return builder;
        }

        /// <summary>
        /// Adds authorization policy to the specified connection builder. If the underlying field already
        /// contains a policy with the same name, then it will not be added twice.
        /// </summary>
        /// <typeparam name="TSourceType"></typeparam>
        /// <param name="builder"></param>
        /// <param name="policy"> Authorization policy name. </param>
        /// <returns> The reference to the specified <paramref name="builder"/>. </returns>
        public static ConnectionBuilder<TSourceType> AuthorizeWith<TSourceType>(
            this ConnectionBuilder<TSourceType> builder, string policy)
        {
            builder.FieldType.AuthorizeWith(policy);
            return builder;
        }
    }
}
