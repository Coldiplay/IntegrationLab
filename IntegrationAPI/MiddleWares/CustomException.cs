namespace IntegrationAPI.MiddleWares
{
    public class CustomException : Exception
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; } = "Неизвестная ошибка";
    }
}
