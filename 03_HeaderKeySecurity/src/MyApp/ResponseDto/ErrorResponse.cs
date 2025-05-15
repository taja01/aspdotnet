namespace LotteryApp.ResponseDto
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; }
    }
}
