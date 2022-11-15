namespace ManageEmployee.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public double IdentityNumber { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
        public string? PlaceOrigin { get; set; }
        public string? PlaceResidence { get; set; }
    }
}
