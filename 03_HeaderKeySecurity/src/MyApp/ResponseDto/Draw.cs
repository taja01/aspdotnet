namespace LotteryApp.ResponseDto
{
    public class Draw
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public List<byte> Numbers { get; set; }
    }
}
