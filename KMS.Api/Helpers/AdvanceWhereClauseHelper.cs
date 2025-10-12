using System.Text;
using System.Text.RegularExpressions;

using static KMS.Api.Common.Enums;

namespace KMS.Api.Helpers
{
    public class SearchCondition
    {
        public string? Operator { get; set; }
        public string Field { get; set; } = "";
        public string Value { get; set; } = "";
    }

    public class AdvanceWhereClause
    {
        public static List<SearchCondition> ParseAdvancedSearch(string input)
        {
            var fields = "qs|ti|au|su|pb|no|kw|so|bn|bc|yr|lg|bt|cl|ci|uc";
            var pattern = $@"(?:(?<op>\bAND\b|\bOR\b|\bNOT\b)\s+)?(?<field>{fields}):(?<value>(?:(?!\s+\b(?:AND|OR|NOT)\b\s+(?:{fields}):).)+)";
            var matches = Regex.Matches(input, pattern, RegexOptions.IgnoreCase);

            var list = new List<SearchCondition>();
            foreach (Match match in matches)
            {
                list.Add(new SearchCondition
                {
                    Operator = match.Groups["op"].Success ? match.Groups["op"].Value.ToUpper() : null,
                    Field = match.Groups["field"].Value.ToLower(),
                    Value = match.Groups["value"].Value.Trim()
                });
            }
            return list;
        }

        public static SearchDocBy MapFieldCodeToOption(string fieldCode)
        {
            return fieldCode.ToLower() switch
            {
                "qs" => SearchDocBy.qs,
                "ti" => SearchDocBy.ti,
                "au" => SearchDocBy.au,
                "su" => SearchDocBy.su,
                "pb" => SearchDocBy.pb,
                "no" => SearchDocBy.no,
                "kw" => SearchDocBy.kw,
                "so" => SearchDocBy.so,
                "bn" => SearchDocBy.bn,
                "bc" => SearchDocBy.bc,
                "yr" => SearchDocBy.yr,
                "lg" => SearchDocBy.lg,
                "bt" => SearchDocBy.bt,
                "cl" => SearchDocBy.cl,
                "ci" => SearchDocBy.ci,
                "uc" => SearchDocBy.uc,
                _ => throw new Exception($"Unknown field code: {fieldCode}")
            };
        }

        public static void AppendConditionByOption(StringBuilder sql, SearchDocBy option)
        {
            switch (option)
            {
                case SearchDocBy.qs:
                case SearchDocBy.cl:
                case SearchDocBy.ci:
                case SearchDocBy.no:
                case SearchDocBy.so:
                    sql.AppendLine(" ((quicksearch IS NOT NULL AND quicksearch @@ plainto_tsquery(lower(@keyword)))");
                    sql.AppendLine(" OR (quicksearch_uns IS NOT NULL AND quicksearch_uns @@ plainto_tsquery(lower(@keyword))))");
                    break;

                case SearchDocBy.ti:
                    sql.AppendLine(" oi.titles LIKE '%' || @keyword || '%'");
                    break;

                case SearchDocBy.au:
                    sql.AppendLine(" oi.authors LIKE '%' || @keyword || '%'");
                    break;

                case SearchDocBy.kw:
                    sql.AppendLine(" oi.keywords LIKE '%' || @keyword || '%'");
                    break;

                case SearchDocBy.yr:
                    sql.AppendLine(" oi.year_pub = @keyword");
                    break;

                case SearchDocBy.su:
                    sql.AppendLine(" oi.subjects LIKE '%' || @keyword || '%'");
                    break;

                case SearchDocBy.pb:
                    sql.AppendLine(" oi.publisher LIKE '%' || @keyword || '%'");
                    break;

                case SearchDocBy.lg:
                    sql.AppendLine(" oi.language LIKE '%' || @keyword || '%'");
                    break;

                case SearchDocBy.bt:
                    sql.AppendLine(" oi.bibtype LIKE '%' || @keyword || '%'");
                    break;

                case SearchDocBy.bc:
                    sql.AppendLine(" oi.reg_str LIKE '%' || @keyword || '%'");
                    break;

                case SearchDocBy.bn:
                    sql.AppendLine(" oi.isbn LIKE '%' || @keyword || '%'");
                    break;

                default:
                    throw new Exception($"Unhandled search option: {option}");
            }
        }
    }
}