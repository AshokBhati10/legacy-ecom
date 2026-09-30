using System.Collections.Generic;

namespace Ecommerce.Core.Common
{
    /// <summary>
    /// Operation outcome returned by services instead of throwing for
    /// expected business-rule failures.
    /// </summary>
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IList<string> Errors { get; set; }

        public ServiceResult()
        {
            Errors = new List<string>();
        }

        public static ServiceResult Ok(string message = null)
        {
            return new ServiceResult { Success = true, Message = message };
        }

        public static ServiceResult Fail(string message)
        {
            var r = new ServiceResult { Success = false, Message = message };
            r.Errors.Add(message);
            return r;
        }
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T Data { get; set; }

        public static ServiceResult<T> Ok(T data, string message = null)
        {
            return new ServiceResult<T> { Success = true, Data = data, Message = message };
        }

        public static new ServiceResult<T> Fail(string message)
        {
            var r = new ServiceResult<T> { Success = false, Message = message };
            r.Errors.Add(message);
            return r;
        }
    }
}
