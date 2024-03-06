namespace Ironwall.Libraries.Sounds.Models
{
    public interface ISoundModel
    {
        int Id { get; set; }
        bool IsPlaying { get; set; }
        string File { get; set; }
        string Name { get; set; }
    }
}