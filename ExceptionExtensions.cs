namespace StationeersWebApi
{
    using System;

    /// <summary>
    /// Extensions for the <see cref="Exception"/> class.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Gets the innermost exception of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of exception to get.</typeparam>
        /// <param name="exception">The exception.</param>
        /// <returns>The innermost exception of the given type, or null.</returns>
        public static T GetInnerException<T>(this Exception exception)
            where T : Exception
        {
            T lastFound = null;
            while (exception != null)
            {
                if (exception is AggregateException aggregate)
                {
                    foreach (var inner in aggregate.InnerExceptions)
                    {
                        var found = inner.GetInnerException<T>();
                        if (found != null)
                        {
                            return found;
                        }
                    }
                }

                if (exception is T asT)
                {
                    lastFound = asT;
                }

                exception = exception.InnerException;
            }

            return lastFound;
        }
    }
}
