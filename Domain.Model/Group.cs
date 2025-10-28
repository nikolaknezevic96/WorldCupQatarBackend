namespace Domain.Model
{
    public class Group
    {
        public int Id { get; set; }
        public string GroupName { get; set; } = null!;
        public ICollection<Team> Teams { get; set; }   // Navigation property for teams in a group
    }
}