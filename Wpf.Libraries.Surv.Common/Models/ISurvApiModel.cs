namespace Wpf.Libraries.Surv.Common.Models
{
    public interface ISurvApiModel : ISurvBaseModel
    {
        string ApiAddress { get; set; }
        uint ApiPort { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
    }
}