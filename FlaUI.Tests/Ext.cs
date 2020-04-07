using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FlaUI.Tests
{
    public static class Ext
    {
        public static T retry<T>(Func<T> func, Func<T, bool> predicate, int retryCount, TimeSpan? retryInterval = null)
        {
            var retry = retryInterval ?? TimeSpan.FromMilliseconds(100);
            var exceptions = new List<Exception>();

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    var result = func();
                    if (predicate(result))
                        return result;
                    else new NullReferenceException();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
                finally
                {
                    Thread.Sleep(retry);
                }
            }

            throw new AggregateException(exceptions);
        }
    }

}
