namespace Ironwall.Framework.Models.Communications.Events
{
    public interface IMalfunctionDetailModel
    {
        int FirstEnd { get; set; }
        int FirstStart { get; set; }
        int Reason { get; set; }
        int SecondEnd { get; set; }
        int SecondStart { get; set; }

        //void Insert(int reason, int fStart, int fEnd, int sStart, int sEnd);
    }
}