using System.Net;
using BaseLibrary.Auth;
using BaseLibrary.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileSignalR.Tools;

namespace MobileSignalR.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController(LaravelRequestHandler laraClient, JwtTokenHandler tokenHandler) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<Response> Authorize(string login, string password)
        {
            var result = await laraClient.Post<UserAuth>("api/login", new { login, password });
            if (string.IsNullOrEmpty(result?.Token))
                return ToBadResponse("Неверная пара логин-пароль", HttpStatusCode.Unauthorized);
            var token = tokenHandler.GenerateToken(DateTime.UtcNow.AddMinutes(30));
            tokenHandler.AddTokenPair(token, result.Token);
            result.Token = token;
            return ToResponseWithData(result, "Успешная авторизация!");
        }
        
        
        private Response ToResponseWithData<T>(T? model = default, string? message = null,
            HttpStatusCode statusCode = HttpStatusCode.OK)
            where T : notnull
        {
            if (model is null)
            {
                return new Response
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = message ?? "Not found"
                };
            }


            var returnType = typeof(T);
            var typeName = returnType.IsGenericType
                ? returnType.GetGenericArguments()[0].Name
                :  returnType.Name;
            return new Response
            {
                StatusCode = statusCode,
                Data = model,
                DataTypeName = typeName,
                Message = message ?? $"Successful retrieved {typeName}"
            };
        }

        private Response ToBadResponse(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new Response
            {
                StatusCode = statusCode,
                Message = message
            };
        }
    }
}
