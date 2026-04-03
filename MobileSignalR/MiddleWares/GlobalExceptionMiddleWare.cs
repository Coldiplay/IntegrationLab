using System.ComponentModel.DataAnnotations;

namespace MobileSignalR.MiddleWares
{
    /// <summary>
    /// Класс для отловли ошибок происходящих в запросах
    /// </summary>
    /// <param name="request">Делегат запроса</param>
    /// <param name="environment">Окружение запроса</param>
    /// <param name="logger">Логгер для логгирования в консоль</param>
    public class GlobalExceptionMiddleWare(RequestDelegate request, IWebHostEnvironment environment, ILogger<GlobalExceptionMiddleWare> logger) : IExceptionMiddleWare
    {
        private readonly RequestDelegate _requestDelegate = request;
        private readonly IWebHostEnvironment _environment = environment;
        private readonly ILogger<GlobalExceptionMiddleWare> _logger = logger;

        public static int GetErrorCode(Exception exception) => exception switch
        {
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            ArgumentException or ValidationException => StatusCodes.Status400BadRequest,
            CustomException ex => ex.ErrorCode,
            _ => StatusCodes.Status500InternalServerError
        };

        public static string GetErrorMessage(Exception exception) => exception switch
        {
            UnauthorizedAccessException => "Доступ запрещён: требуется аутентификация.",
            KeyNotFoundException => "Запрашиваемый ресурс не найден.",
            ArgumentException => "Некорректные входные данные.",
            ValidationException => "Ошибка валидации данных.",
            CustomException ex => ex.ErrorMessage, 
            _ => "Произошла внутренняя ошибка сервера."
        };

        public static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception, bool isDevelopment)
        {
            int errorCode = GetErrorCode(exception);
            string errorMessage = GetErrorMessage(exception);
            //Задаём ответу статус код
            httpContext.Response.StatusCode = errorCode;

            //Создаём анонимный класс для превращения его в json для тела ответа
            var response = new
            {
                errorCode,
                errorMessage,
                details = isDevelopment ? exception.Message : null
            };
            
            //Записываем ошибку в тело ответа
            await httpContext.Response.WriteAsJsonAsync(response);
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // Пробуем запустить запрос в контексте
                await _requestDelegate.Invoke(httpContext);
            }
            //Отлавливаем возможную ошибку
            catch (Exception ex)
            {

                string message = ex switch
                {
                    UnauthorizedAccessException => "Произошёл запрос от неаутентифицированного пользователя",
                    KeyNotFoundException => "Пользователь не нашёл запрашиваемый ресурс",
                    ArgumentException => "Пользователь ввёл некорректные входные данные",
                    ValidationException => "Данные пользователя не прошли валидацию",
                    CustomException e => e.ErrorMessage, // Пока так 
                    _ => $"Произошла непредвиденная ошибка, трассировка: {ex}"
                };

                var severity = GetSeverityOfError(ex);
                //Логгируем ошибку в консоль
                _logger.Log(severity, "MiddleWare поймал ошибку в выполнении запроса пользователя {0} в методе {1} {2}, ошибка: {3}", httpContext.Connection.RemoteIpAddress, httpContext.Request.Method, httpContext.Request.Path, message);
                //Запускаем метод для отправки ошибки клиенту
                await HandleExceptionAsync(httpContext, ex, _environment.IsDevelopment());
            }
        }

        private static LogLevel GetSeverityOfError(Exception exception) => exception switch
        {
            UnauthorizedAccessException 
                or KeyNotFoundException
                or ArgumentException
                or ValidationException => LogLevel.Information,
            CustomException ex => ex.ErrorCode >= 500 ? LogLevel.Error : LogLevel.Information,
            _ => LogLevel.Error
        };
    }
}
