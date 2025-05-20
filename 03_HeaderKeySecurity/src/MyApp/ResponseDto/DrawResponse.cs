namespace LotteryApp.ResponseDto
{
    public class DrawResponse
    {
        public List<byte> WinnerNumbers { get; set; }
        public List<DrawAnalysis> DrawAnalyses { get; set; }
    }
}
