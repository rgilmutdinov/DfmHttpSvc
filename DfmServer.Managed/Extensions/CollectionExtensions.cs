using System.Collections.Generic;
using System.Linq;
using DfmServer.Managed.Collections;

namespace DfmServer.Managed.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Determines whether the collection is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type.</typeparam>
        /// <param name="enumerable">The enumerable, which may be null or empty.</param>
        /// <returns>
        ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }

            if (enumerable is ICollection<T> collection)
            {
                return collection.Count < 1;
            }

            return !enumerable.Any();
        }

        /// <summary>
        /// Determines whether the collection contains the only null item.
        /// </summary>
        /// <typeparam name="T">The ICollection type.</typeparam>
        /// <param name="collection">The collection, which may be null</param>
        /// <returns>
        ///     <c>true</c> if the ICollection contains the only null item; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasSingleNull<T>(this ICollection<T> collection)
        {
            if (collection != null)
            {
                return collection.Count == 1 && collection.ElementAt(0) == null;
            }

            return false;
        }

        /// <summary>
        /// Sanitizes the collection.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="collection">The collection, which may be null of empty</param>
        /// <returns>
        ///   Empty list if ICollection is null or empty or contains the only null item; otherwise, the list of the same items original collection has.
        /// </returns>
        public static List<T> Sanitize<T>(this ICollection<T> collection)
        {
            if (IsNullOrEmpty(collection) || HasSingleNull(collection))
            {
                return Lists.Empty<T>();
            }

            return new List<T>(collection);
        }
    }
}
