using KMS.Shared.DTOs.TrainingProgram;

namespace KMS.Shared.DTOs.TrainingProgram
{
    public class TrainingSections
    {
        public List<DetailSection> DetailSection { get; set; } = new();
        public List<ListSubject> ListSubject { get; set; } = new();
    }

    public class DetailSection
    {
        public string? code { get; set; }
        public string? name { get; set; }
        public string? department_id { get; set; }
        public int? no_of_cert { get; set; }
    }
    public class ListSubject
    {
        public string? id { get; set; }
        public string? title { get; set; }
        public string? bib_author { get; set; }
        public string? bib_publisher { get; set; }
        public string? bib_publisplace{ get; set; }
        public string? bib_yearpub { get; set; }
        public List<ListSubjectLib> ListSubjectLib { get; set; } = new();
    }

    public class ListSubjectLib
    {
        public int bib_id { get; set; }
        public string? title { get; set; }
        public string? bib_author { get; set; }
        public string? bib_publisher { get; set; }
        public string? bib_publisplace { get; set; }
        public string? bib_yearpub { get; set; }
        public bool is_mandatory_ref { get; set; }
        public bool is_optional_ref { get; set; }
        public bool is_included_ref { get; set; }
        public string? slug { get; set; }
    }
}
