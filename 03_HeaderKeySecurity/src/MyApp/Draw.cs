namespace LotteryApp
{
    public class Draw
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public List<byte> WinningNumbers { get; set; }
    }
}
