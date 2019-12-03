namespace Medelit.Application
{
    public class UserViewModel : AuthBaseViewModel
    {
        public string UserId { get; set; }
        public string Fields { get; set; }
        public bool IncludeFields { get; set; } = true;
    }
}
