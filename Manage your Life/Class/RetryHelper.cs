using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Manage_your_Life
{
    public static class RetryHelper
    {
        private static Action<Exception> Throw
        {
            get { return new Action<Exception>(ex => ExceptionDispatchInfo.Capture(ex).Throw()); }
        }

        /// <summary>
        /// 発生した例外が指定条件に一致するとき、指定回数以内で再試行し、それ以外は指定の例外処理を行う。
        /// </summary>
        /// <param name="onAction"></param>
        /// <param name="onError"></param>
        /// <param name="retryCondition"></param>
        /// <param name="maxRetryCount"></param>
        public static void Retry(Action onAction, Action<Exception> onError, Func<Exception, bool> retryCondition, uint maxRetryCount = 10)
        {
            if (onAction == null) throw new ArgumentNullException("onAction");
            if (onError == null) throw new ArgumentNullException("onError");
            if (retryCondition == null) throw new ArgumentNullException("retryCondition");
            RetryCore(onAction, onError, retryCondition, maxRetryCount);
        }
        

        public static TResult Retry<TResult>(Func<TResult> onAction, Action<Exception> onError, Func<Exception, bool> retryCondition, uint maxRetryCount = 10)
        {
            if (onAction == null) throw new ArgumentNullException("onAction");
            if (onError == null) throw new ArgumentNullException("onError");
            if (retryCondition == null) throw new ArgumentNullException("retryCondition");
            return RetryCore(onAction, onError, retryCondition, maxRetryCount);
        }


        private static void RetryCore(Action onAction, Action<Exception> onError, Func<Exception, bool> retryCondition, uint retryCount)
        {
            if (onError == null) onError = (ex => { });
            if (retryCondition == null) retryCondition = (ex => true);
            uint count = 0;

        Retry:
            try
            {
                onAction();
            }
            catch (Exception ex)
            {
                if (retryCondition(ex))
                {
                    if (retryCount > count++) goto Retry;
                }
                onError(ex);
            }
        }


        private static TResult RetryCore<TResult>(Func<TResult> onAction, Action<Exception> onError, Func<Exception, bool> retryCondition, uint retryCount)
        {
            if (onError == null) onError = (ex => { });
            if (retryCondition == null) retryCondition = (ex => true);
            uint count = 0;

        Retry:
            try
            {
                return onAction();
            }
            catch (Exception ex)
            {
                if (retryCondition(ex))
                {
                    if (retryCount > count++) goto Retry;
                }
                onError(ex);
            }
            return default(TResult);
        }
    }
}
