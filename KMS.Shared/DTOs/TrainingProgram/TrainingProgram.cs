using KMS.Shared.DTOs.TrainingProgram;

namespace KMS.Shared.DTOs.TrainingProgram
{
    public class TrainingPrograms
    {
        public List<DetailSystem> DetailSystem { get; set; } = new();
        public List<ListSection> ListSectionTrue { get; set; } = new();
        public List<ListSection> ListSectionFalse { get; set; } = new();
    }

    public class DetailSystem
    {
        public string? code { get; set; }
        public string? name { get; set; }
        public string? industry { get; set; }
        public string? description { get; set; }
    }
    public class ListSection
    {
        public string? code { get; set; }
        public string? name { get; set; }
        public int? no_of_cert { get; set; }
        public int? subject_lib_count { get; set; }
        public int? subject_count { get; set; }
    }
}
