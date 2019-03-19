using Jupiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
//using System.Web.Http.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Jupiter.Controllers
{
    [ApiController]
    //public abstract class BaseController : ApiController
    public abstract class BaseController : ControllerBase

    {
        private bool CanAccess(string currentControllerName, string currentActionName)
        {
           bool canAccess = true;
           return canAccess;
        }
        protected void SetSession()
        {
        }

        protected void UpdateTable(object model, Type type, object tableRow)
        {
            var properties = model.GetType().GetProperties();
            foreach (var prop in properties)
            {
                PropertyInfo piInstance = type.GetProperty(prop.Name);
                if (piInstance != null && prop.GetValue(model) != null)
                {
                    piInstance.SetValue(tableRow,prop.GetValue(model));
                }           
            }
        }

        protected Result<T> DataNotFound <T>(Result<T> tResult)
        {
            tResult.IsSuccess = false;
            tResult.IsFound = false;
            tResult.ErrorMessage = "Not Found";
            return tResult;
        }
        private bool SetCurrentUser()
        {
            return true;
        }
        private Uri GetAbsoluteUri()
        {
            //var request = HttpContext.Request;
            //UriBuilder uriBuilder = new UriBuilder
            //{
            //    Scheme = request.Scheme,
            //    Host = request.Host.ToString(),
            //    Path = request.Path.ToString(),
            //    Query = request.QueryString.ToString()
            //};
            UriBuilder uriBuilder = new UriBuilder();
            return uriBuilder.Uri;
        }
        protected string[] GetErrorMessages(ModelStateDictionary modelStates)
        {
            var errorMessages = new List<string>();
            foreach (var modelState in modelStates.Values)
            {
                foreach (var modelError in modelState.Errors)
                {
                    errorMessages.Add(modelError.ErrorMessage);
                }
            }
            return errorMessages.ToArray();
        }

    }
}
