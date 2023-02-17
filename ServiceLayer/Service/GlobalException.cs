using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Service
{
    [Serializable]
    public class GlobalException: Exception
    {
        public HttpStatusCode Status { get; private set; }
        public GlobalException()
        {

        }

        public GlobalException(HttpStatusCode status)
        {
            Status = status;    
        }

        public GlobalException(HttpStatusCode status, string message) : base(message)
        {
            Status = status; 
        }

        public GlobalException(string message, Exception inner): base(message, inner)
        {

        }
    }
}
