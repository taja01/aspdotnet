namespace LotteryApp.ResponseDto
{
    public class DrawResponse
    {
        public List<byte> WinnerNumbers { get; set; }
        public int Seven { get; set; }
        public int Six { get; set; }
        public int Five { get; set; }
        public int Four { get; set; }
        public int Three { get; set; }
        public int Two { get; set; }
        public int One { get; set; }
        public List<DrawAnalysis> DrawAnalyses { get; set; }
    }
}
