using Microsoft.AspNetCore.Http;
using System;

namespace AYYUAZ.APP.Application.Exceptions.AppException
{
    public abstract class AppException : Exception
    {
        public string ErrorCode { get; }
        public int StatusCode { get; }

        protected AppException(string errorCode, int statusCode) : base()
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }
    }

    public sealed class NotFoundException : AppException
    {
        public object? ResourceId { get; }

        public NotFoundException() : base("not_found", StatusCodes.Status404NotFound)
        {
        }
       public NotFoundException(string errorcode) : base(errorcode, StatusCodes.Status404NotFound)
        {
        }
        public NotFoundException(string errorCode, object resourceId)
      : base(errorCode, StatusCodes.Status404NotFound)
        {
            ResourceId = resourceId;
        }
    }

    public sealed class BadRequestException : AppException
    {
        public BadRequestException() : base("validation_error", StatusCodes.Status400BadRequest)
        {
        }
        public BadRequestException(string errorcode) : base("validation_error", StatusCodes.Status400BadRequest)
        {
        }
    }

    public sealed class ForbiddenException : AppException
    {
        public ForbiddenException() : base("forbidden", StatusCodes.Status403Forbidden)
        {
        }
    }

    public sealed class UnauthorizedException : AppException
    {
        public UnauthorizedException() : base("unauthorized", StatusCodes.Status401Unauthorized)
        {
        }
        public UnauthorizedException(string errorcode) : base("unauthorized", StatusCodes.Status401Unauthorized)
        {
        }
    }
    public sealed class KeyNotFoundException : AppException
    {

        public KeyNotFoundException() : base("key_not_found", StatusCodes.Status404NotFound)
        {
        }
        public KeyNotFoundException(string errorcode) : base(errorcode, StatusCodes.Status404NotFound)
        {
        }
    }
    public sealed class ConflictException : AppException
    {

        public ConflictException() : base("conflict", StatusCodes.Status409Conflict)
        {
        }
        public ConflictException(string errorcode) : base(errorcode,StatusCodes.Status409Conflict)
        {
        }
    }
}
