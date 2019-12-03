namespace Medelit.Application
{
    public class UserRequestViewModel : AuthBaseViewModel
    {
        public string Fields { get; set; } = null;
        public bool? IncludeFields { get; set; } = null;
        public string Query { get; set; } = null;
        public string SearchEngine { get; set; } = null;
        public string Sort { get; set; } = null;

        public bool IncludeTotals { get; set; }
        public int PerPage { get; set; } = int.MaxValue;
        public int PageNo { get; set; }
    }
}
