using System.Collections.Generic;
using System.Collections.ObjectModel;
using com.organo.x4ever.Extensions;

namespace com.organo.x4ever.Models.Local
{
    public class Grouping<T> : ObservableCollection<T>
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see
        ///         cref="Grouping{T}" />
        ///     class.
        /// </summary>
        /// <param name="items">
        ///     A collection of items of type T
        /// </param>
        public Grouping(IEnumerable<T> items)
        {
            Items.AddRange(items);
        }
    }
}