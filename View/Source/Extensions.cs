namespace View.Utils
{
    internal static class Extensions
    {
        public static IEnumerable<TResult> ForceCast<TResult>(this IEnumerable<object> source)
        {
            if (source is IEnumerable<TResult> result)
            {
                return result;
            }
            if (source == null)
            {
                throw new ArgumentNullException();
            }
            return ForceCastIterator<TResult>(source);
        }

        private static IEnumerable<TResult> ForceCastIterator<TResult>(IEnumerable<object> source)
        {
            foreach (var item in source)
            {
                yield return ForceCastSingle<TResult>((object)item);
            }
        }

        private static unsafe TResult ForceCastSingle<TResult>(object obj)
        {
            return *(TResult*)&obj;
        }
    }
}
