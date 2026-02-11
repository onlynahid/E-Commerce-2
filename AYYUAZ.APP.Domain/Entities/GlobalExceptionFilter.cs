using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Domain.Entities
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
           if(context.Exception is KeyNotFoundException)
            {
                context.Result = new NotFoundObjectResult(new
                {
                    message = "Tapılmadı"
                });

            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                context.Result = new ObjectResult(new
                {
                    message = "İcazəniz yoxdur"
                })
                {
                    StatusCode = 401
                };
              }
            else
            {
                context.Result = new ObjectResult(new
                {
                    message = "Server xətası baş verdi"
                })
                {
                    StatusCode = 500
                };
            }
           context.ExceptionHandled = true;
        }
    }
}
