using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jupiter.Models
{
    public class Result<T>
    {
        //1--Success
        public bool IsSuccess { get; set; } =true;
        public bool IsFound { get; set; } = true;
        public string ErrorCode { get; set; } 
        public string ErrorMessage { get; set; }
        public T Data { get; set; }
    }
}