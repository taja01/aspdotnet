using LotteryApp.Models;

namespace LotteryApp.ResponseDto
{
    public class DrawAnalysis
    {
        public List<byte> WinnerNumbers { get; set; }
        public List<byte> YourNumbers { get; set; }
        public List<byte> Matches { get; set; }
        public LotteryResultTier ResultTier { get; set; }

        public DrawAnalysis()
        {
            WinnerNumbers = new List<byte>();
            YourNumbers = new List<byte>();
            Matches = new List<byte>();
        }
    }
}
