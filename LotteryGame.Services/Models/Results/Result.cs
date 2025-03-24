namespace LotteryGame.Services.Models.Results
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }

        public string? Message { get; set; }

        public T? Data { get; set; }

        public static Result<T> Error(string? message = null)
            => new()
            {
                IsSuccess = false,
                Message = message,
                Data = default,
            };

        public static Result<T> Success(T data, string? message = null)
            => new()
            {
                IsSuccess = true,
                Message = message,
                Data = data,
            };
    }
}
