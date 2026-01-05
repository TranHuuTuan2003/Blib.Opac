using KMS.Shared.DTOs.DigitalFile;

namespace KMS.Shared.DTOs.Document
{
    public class Details : Result
    {
        public int? like { get; set; }
        public string? marc { get; set; } = string.Empty;
        public List<RegisteredCirculation>? register_circulation_place { get; set; }
        public List<MarcField>? marc_field_value_object { get; set; }
        public List<DublinCoreField>? dublin_core_object { get; set; }
        public List<ClassifiedFile> files { get; set; } = new();
    }

    public class RequestQueueDto
    {
        public Guid? Id { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string? ReaderId { get; set; }
        public string? Status { get; set; }
        public string? UpdatedBy { get; set; }
        public string? RegId { get; set; }
        public string? StoreId { get; set; }
        public string? CirPlaceId { get; set; }
        public DateTime? DateCreated { get; set; }
        public decimal? BibId { get; set; }
        public string? RegisterId { get; set; }
        public string? StatusId { get; set; }   
        public string? Active { get; set; }
        public DateTime? LastReceive { get; set; }
        public string? RefuseNotes { get; set; }
        public string? UserId { get; set; }
        public DateTime? RequestDate { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Tel { get; set; }
        public string? Type { get; set; }
        public string? CardNo { get; set; }
        public string? Sex { get; set; }
        public string? CCCD { get; set; }
        public DateTime? Dob { get; set; }
    }

}
