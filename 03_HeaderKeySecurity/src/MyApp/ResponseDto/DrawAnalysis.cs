using LotteryApp.Models;

namespace LotteryApp.ResponseDto
{
    public class DrawAnalysis
    {
        public List<byte> YourNumbers { get; set; }
        public List<byte> Matches { get; set; }
        public LotteryResultTier ResultTier { get; set; }
        public byte MatchCount { get; internal set; }

        public DrawAnalysis()
        {
            YourNumbers = new List<byte>();
            Matches = new List<byte>();
        }
    }
}
