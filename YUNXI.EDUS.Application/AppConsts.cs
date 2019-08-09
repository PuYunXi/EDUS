namespace YUNXI.EDUS
{
    public class AppConsts
    {
        /// <summary>
        ///     Length of a code unit between dots.
        /// </summary>
        public const int CodeUnitLength = 5;

        /// <summary>
        ///     Default page size for paged requests.
        /// </summary>
        public const int DefaultPageSize = 10;

        public const int MaxCodeLength = (MaxDepth * (CodeUnitLength + 1)) - 1;

        /// <summary>
        ///     Maximum depth of an UO hierarchy.
        /// </summary>
        public const int MaxDepth = 16;

        /// <summary>
        ///     Maximum allowed page size for paged requests.
        /// </summary>
        public const int MaxPageSize = 1000;
    }
}
