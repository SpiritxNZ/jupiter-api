using Jupiter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
//using System.Web.Http.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Jupiter.Controllers
{
    [ApiController]
    public abstract class BasicController : ControllerBase

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
                var propValue = prop.GetValue(model);
                try
                {
                    var propValueInt = Convert.ToInt32(propValue);
                    // the value of property is 200, then set null to this field
                    if (piInstance != null && propValueInt == 200 && prop.Name == "SpecialOrder")
                    {
                        piInstance.SetValue(tableRow, null);
                    }
                    else
                    {
                        throw new Exception();
                    }

                }
                catch (Exception e)
                // if propValue can not converted into int
                {
                    if (piInstance != null && propValue != null)
                    {
                        piInstance.SetValue(tableRow,propValue);
                    }
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

        protected async Task<bool> StoreImage(string folder, string newFileName, IFormFile file)
        {
            try
            {
                var folderName = Path.Combine("wwwroot", "Images", folder);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var path = Path.Combine(pathToSave, newFileName);
                var stream = new FileStream(path, FileMode.Create);
                await file.CopyToAsync(stream);
                stream.Close();
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
        protected void DeleteImage(string imagePath)
        {
            var pathNeedToBeDeleted = Path.Combine("wwwroot", imagePath);
            FileInfo fileNeedToBeDeleted = new FileInfo(pathNeedToBeDeleted);
            fileNeedToBeDeleted.Delete();
        }
        protected string RemoveWhitespace(string name)
        {
            return Regex.Replace(name, @"\s+", "");
        }


        protected DateTime toNZTimezone(DateTime utc)
        {
            DateTime nzTime = new DateTime();
            try
            {
                TimeZoneInfo nztZone = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
                nzTime = TimeZoneInfo.ConvertTimeFromUtc(utc, nztZone);
                return nzTime;
            }
            catch (TimeZoneNotFoundException)
            {
                TimeZoneInfo nztZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific/Auckland");
                nzTime = TimeZoneInfo.ConvertTimeFromUtc(utc, nztZone);
                return nzTime;
            }
            catch (InvalidTimeZoneException)
            {
                TimeZoneInfo nztZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific/Auckland");
                nzTime = TimeZoneInfo.ConvertTimeFromUtc(utc, nztZone);
                return nzTime;
            }
        }

    }
}
