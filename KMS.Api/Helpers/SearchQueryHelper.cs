using System.Text;

using Dapper;

using KMS.Api.Core;
using KMS.Shared.DTOs.Search;

using static KMS.Api.Common.Enums;

namespace KMS.Api.Helpers
{
    public static class SearchQueryHelper
    {
        private static string templateExactNonStrpos = " LOWER({0}) = LOWER({1})";
        private static string templateExactNonLowerStrpos = " {0} = {1}";
        private static string templateExact = " ((strpos({0}, 'w1w') > 0 AND strpos(LOWER(CONCAT({0},' w1w')), {1}) > 0) OR (strpos({0}, 'w1w') <= 0 AND strpos({0}, {1}) > 0))";
        private static string templateUnsign = " {0} LIKE {1}";
        private static string templateNot = " {0} NOT LIKE {1}";
        private static string templateExactNot = " {0} != {1}";
        private static string templateEGreater = "AND ( CASE WHEN TRIM({0}) ~ '^\\d{{4}}$' THEN CAST({0} AS INTEGER) ELSE NULL END ) >= {1}";
        private static string templateELess = "AND ( CASE WHEN TRIM({0}) ~ '^\\d{{4}}$' THEN CAST({0} AS INTEGER) ELSE NULL END ) <= {1}";

        public static string GetValueFromSearchBy(string[][] searchBy, int row, int column)
        {
            if (searchBy.Length > row && searchBy[row].Length > column)
            {
                return searchBy[row][column];
            }
            return string.Empty;
        }

        public static List<Tuple<string, string>> BuildSearchMap(string[][]? searchBy)
        {
            if (searchBy == null || searchBy.Length == 0)
                return new();

            var searchMap = new List<Tuple<string, string>>();

            for (int i = 0; i < searchBy.Length; i++)
            {
                string key = GetValueFromSearchBy(searchBy, i, 0);
                string value = GetValueFromSearchBy(searchBy, i, 1);

                if (!string.IsNullOrEmpty(key))
                {
                    searchMap.Add(new Tuple<string, string>(key, value));
                }
            }

            return searchMap;
        }

        public static List<Tuple<string, string>> BuildOperatorMap(string[][] searchBy)
        {
            var operatorMap = new List<Tuple<string, string>>();

            foreach (var row in searchBy)
            {
                if (row.Length < 2) continue;

                string key = row[0];
                string value = row[1].ToLower();

                if (!key.Contains("_operator_")) continue;
                if (string.IsNullOrEmpty(value)) continue;

                if (value != "and" && value != "or" && value != "not") continue;

                // Lấy property gốc (vd: title_0 từ title_operator_0)
                string prop = key.Replace("_operator_", "_");

                operatorMap.Add(new Tuple<string, string>(prop, value));
            }

            return operatorMap;
        }

        public static string GetOperator(List<Tuple<string, string>> operatorMap, string keyWithIndex)
        {
            var op = operatorMap.FirstOrDefault(x => x.Item1 == keyWithIndex);
            return op?.Item2 ?? string.Empty;
        }

        public static string GetFirstPartQueryOfCollection(SearchBody model)
        {
            StringBuilder sql = new StringBuilder();
            List<Tuple<string, string>> filterMap = BuildSearchMap(model.filterBy);
            var collectionId = "";
            foreach (var item in filterMap)
            {
                string filterKey = item.Item1;
                string filterValue = item.Item2;
                filterValue = filterValue.Trim();
                if (filterKey == "collection")
                {
                    if (!string.IsNullOrEmpty(filterValue))
                    {
                        collectionId = filterValue;
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(collectionId))
            {
                throw new Exception("Collection id empty!");
            }
            sql.AppendLine("WITH RECURSIVE collection_tree AS (");
            sql.AppendLine("    SELECT id");
            sql.AppendLine("    FROM o_collection");
            sql.AppendLine($"    WHERE id = '{collectionId}'");
            sql.AppendLine("    UNION ALL");
            sql.AppendLine("    SELECT c.id");
            sql.AppendLine("    FROM o_collection c");
            sql.AppendLine("    INNER JOIN collection_tree ct ON c.parent_id = ct.id");
            sql.AppendLine(")");
            return sql.ToString();
        }

        public static bool IsNotSpecialColumn(string column_prop)
        {
            return column_prop != OItemEnum.bib_type.ToString() &&
                    column_prop != OItemEnum.year_pub.ToString() &&
                    column_prop != OItemEnum.db_type.ToString() &&
                    column_prop != OItemEnum.isbn.ToString() &&
                    column_prop != OItemEnum.reg_str.ToString();
        }

        public static bool IsExceptionalOfSpecialColumn(string column_prop)
        {
            return column_prop == OItemEnum.reg_str.ToString() || column_prop == OItemEnum.isbn.ToString();
        }

        public static string BuildCollectionQueryFilter(List<Tuple<string, string>> filterMap)
        {
            StringBuilder sql = new StringBuilder();

            foreach (var item in filterMap)
            {
                string filterKey = item.Item1;
                string filterValue = item.Item2;
                filterValue = filterValue.Trim();

                if (string.IsNullOrEmpty(filterKey) || string.IsNullOrEmpty(filterValue)) continue;

                if (filterKey == "type")
                {
                    if (filterValue == "ddoc")
                    {
                        sql.AppendLine("AND (oi.db_type = 'ddoc'");
                        sql.AppendLine("OR oi.db_type = 'adoc')");
                    }
                    else if (filterValue == "pdoc")
                    {
                        sql.AppendLine("AND (oi.db_type = 'pdoc'");
                        sql.AppendLine("OR oi.db_type = 'adoc')");
                    }
                }
            }

            return sql.ToString();
        }

        public static List<string> GetTenantFromFilterBy(string[][]? filterBy)
        {
            var filterByMap = BuildSearchMap(filterBy);
            return filterByMap.Where(t => t.Item1 == "tenant" && !string.IsNullOrWhiteSpace(t.Item2))
                .Select(t => t.Item2.Trim())
                .Distinct()
                .ToList();
        }

        public static string BuildTenantQueryFilter()
        {
            return "AND oi.tenant_code = ANY(@_tenantCodes)";
        }

        public static string BuildQueryOrderAndOffset(int page, int pageSize, SearchBody model)
        {
            int offset = (page - 1) * pageSize;
            StringBuilder sql = new StringBuilder();
            var option = string.IsNullOrEmpty(model?.sortBy?[0][0]) ? "year_pub_no" : "year_pub_no";
            var sort = string.IsNullOrEmpty(model?.sortBy?[0][1]) ? "desc" : (model.sortBy[0][1] == "asc" ? "asc" : "desc");
            sql.AppendLine($"ORDER BY oi.{option} {sort}");
            sql.AppendLine($"LIMIT {pageSize} OFFSET {offset}");
            return sql.ToString();
        }

        public static (string, DynamicParameters) BuildQueryFilter(SearchBody model)
        {
            StringBuilder sql = new StringBuilder();
            DynamicParameters parameters = new();
            List<string> bibTypeFilterValues = new();
            List<string> authorFilterValues = new();
            List<string> keywordFilterValues = new();
            List<string> pubYearFilterValues = new();
            List<string> subjectFilterValues = new();

            if (model.filterBy != null && model.filterBy.Length > 0)
            {
                for (int i = 0; i < model.filterBy.Length; i++)
                {
                    string filterType = model.filterBy[i][0];
                    if (filterType == SearchDocBy.bt.ToString())
                    {
                        for (int j = 1; j < model.filterBy[i].Length; j++)
                        {
                            bibTypeFilterValues.Add(model.filterBy[i][j]);
                        }
                    }
                    else if (filterType == SearchDocBy.au.ToString())
                    {
                        for (int j = 1; j < model.filterBy[i].Length; j++)
                        {
                            authorFilterValues.Add(model.filterBy[i][j]);
                        }
                    }
                    else if (filterType == SearchDocBy.kw.ToString())
                    {
                        for (int j = 1; j < model.filterBy[i].Length; j++)
                        {
                            keywordFilterValues.Add(model.filterBy[i][j]);
                        }
                    }
                    else if (filterType == SearchDocBy.yr.ToString())
                    {
                        for (int j = 1; j < model.filterBy[i].Length; j++)
                        {
                            pubYearFilterValues.Add(model.filterBy[i][j]);
                        }
                    }
                    else if (filterType == SearchDocBy.su.ToString())
                    {
                        for (int j = 1; j < model.filterBy[i].Length; j++)
                        {
                            subjectFilterValues.Add(model.filterBy[i][j]);
                        }
                    }
                }

                if (bibTypeFilterValues.Count > 0)
                {
                    sql.AppendLine("AND EXISTS (");
                    sql.AppendLine("SELECT 1 FROM o_bibtype bt");
                    sql.AppendLine($"WHERE bt.item_id = oi.id AND bt.tenant_code = ANY(@_tenantCodes)");
                    sql.AppendLine($"AND bt.value = ANY(@bibTypeFilterValues))");
                    parameters.Add("bibTypeFilterValues", bibTypeFilterValues);
                }

                if (authorFilterValues.Count > 0)
                {
                    sql.AppendLine("AND EXISTS (");
                    sql.AppendLine("SELECT 1 FROM o_author at");
                    sql.AppendLine($"WHERE at.item_id = oi.id AND at.tenant_code = ANY(@_tenantCodes)");
                    sql.AppendLine($"AND at.value = ANY(@authorFilterValues))");
                    parameters.Add("authorFilterValues", authorFilterValues);
                }

                if (keywordFilterValues.Count > 0)
                {
                    sql.AppendLine("AND EXISTS (");
                    sql.AppendLine("SELECT 1 FROM o_keyword kw");
                    sql.AppendLine($"WHERE kw.item_id = oi.id AND kw.tenant_code = ANY(@_tenantCodes)");
                    sql.AppendLine($"AND kw.value = ANY(@keywordFilterValues))");
                    parameters.Add("keywordFilterValues", keywordFilterValues);
                }

                if (pubYearFilterValues.Count > 0)
                {
                    sql.AppendLine("AND EXISTS (");
                    sql.AppendLine("SELECT 1 FROM o_pubyear py");
                    sql.AppendLine($"WHERE py.item_id = oi.id AND py.tenant_code = ANY(@_tenantCodes)");
                    sql.AppendLine($"AND py.value = ANY(@pubYearFilterValues))");
                    parameters.Add("pubYearFilterValues", pubYearFilterValues);
                }

                if (subjectFilterValues.Count > 0)
                {
                    sql.AppendLine("AND EXISTS (");
                    sql.AppendLine("SELECT 1 FROM o_subject sj");
                    sql.AppendLine($"WHERE sj.item_id = oi.id AND sj.tenant_code = ANY(@_tenantCodes)");
                    sql.AppendLine($"AND sj.value = ANY(@subjectFilterValues))");
                    parameters.Add("subjectFilterValues", subjectFilterValues);
                }

                return (sql.ToString(), parameters);
            }

            return (string.Empty, parameters);
        }

        public static (string, DynamicParameters) BuildInitQuerySearch(int lastId, SearchBody model, IReadOnlyList<string> _tenantCodes)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(ConstQuery.SelectDocumentViewQuery);
            query.AppendLine("FROM o_item oi");
            query.AppendLine("WHERE oi.title IS NOT NULL AND oi.title <> '' AND TRIM(oi.title) <> '' AND LENGTH(TRIM(oi.title)) > 5");
            query.AppendLine($"AND is_lock = false");
            query.AppendLine($"AND id between @lastId - 99 and @lastId");
            query.AppendLine(BuildTenantQueryFilter());
            var parameters = new DynamicParameters();
            parameters.Add("lastId", lastId);
            var tenantCodes = GetTenantFromFilterBy(model.filterBy);
            if (tenantCodes.Any())
            {
                parameters.Add("_tenantCodes", tenantCodes);
            }
            else
            {
                parameters.Add("_tenantCodes", _tenantCodes);
            }

            return (query.ToString(), parameters);
        }

        public static (string, DynamicParameters) BuildQuickQuerySearch(SearchBody model, IReadOnlyList<string> _tenantCodes, string type = "search")
        {
            string option = model?.searchBy?[0][0].ToLower() == "option" ? model.searchBy[0][1] : "";
            string keyword = model?.searchBy?[1][0].ToLower() == "keyword" ? model.searchBy[1][1] : "";
            List<Tuple<string, string>> filterMap = new List<Tuple<string, string>>();

            StringBuilder sql = new StringBuilder();
            var parameters = new DynamicParameters();

            if (type == "collection")
            {
                sql.AppendLine(GetFirstPartQueryOfCollection(model ?? new()));
            }

            sql.AppendLine(ConstQuery.SelectDocumentViewQuery);

            if (type == "search")
            {
                sql.AppendLine("FROM o_item oi");
                sql.AppendLine($"WHERE is_lock = false");
            }
            else if (type == "collection")
            {
                sql.AppendLine("FROM o_item oi");
                sql.AppendLine("INNER JOIN o_collection_item oci ON oi.id = oci.item_id");
                sql.AppendLine($"WHERE is_lock = false");
                sql.AppendLine($"AND oci.collection_id IN (SELECT id FROM collection_tree)");
            }

            if (string.IsNullOrEmpty(option) || string.IsNullOrEmpty(option))
            {
                throw new Exception("Query search is not valid!");
            }

            keyword = keyword.ToLower().Trim();

            if (!string.IsNullOrEmpty(model?.advanceWhereClause))
            {
                var conditions = AdvanceWhereClause.ParseAdvancedSearch(model.advanceWhereClause);

                foreach (var cond in conditions)
                {
                    if (string.IsNullOrEmpty(cond.Field) || string.IsNullOrEmpty(cond.Value)) continue;
                    var searchOption = AdvanceWhereClause.MapFieldCodeToOption(cond.Field);
                    string paramName = $"@{cond.Field}_{Guid.NewGuid().ToString("N").Substring(0, 6)}";
                    string logicOp = cond.Operator?.ToUpper() switch
                    {
                        "AND" => "AND",
                        "OR" => "OR",
                        "NOT" => "AND NOT", // NOT được xử lý như phủ định bằng AND NOT
                        _ => "AND"
                    };

                    sql.AppendLine($"{logicOp}");

                    var subSql = new StringBuilder();
                    AdvanceWhereClause.AppendConditionByOption(subSql, searchOption);
                    sql.Append(subSql.ToString().Replace("@keyword", paramName));
                    var value = cond.Value?.ToLower()?.Trim() ?? string.Empty;
                    if (!value.Contains(" "))
                        value += " ";
                    parameters.Add(paramName, value.ToLikeParam());
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    if (option == SearchDocBy.ti.ToString())
                    {
                        if (!keyword.Contains(" "))
                        {
                            keyword += " ";
                        }
                        sql.AppendLine("and oi.titles like @keyword");
                        parameters.Add("keyword", keyword.ToLikeParam());
                    }
                    else if (option == SearchDocBy.au.ToString())
                    {
                        if (!keyword.Contains(" "))
                        {
                            keyword += " ";
                        }
                        sql.AppendLine("and oi.authors like @keyword");
                        parameters.Add("keyword", keyword.ToLikeParam());
                    }
                    else if (option == SearchDocBy.kw.ToString())
                    {
                        if (!keyword.Contains(" "))
                        {
                            keyword += " ";
                        }
                        sql.AppendLine("and oi.keywords like @keyword");
                        parameters.Add("keyword", keyword.ToLikeParam());
                    }
                    else if (option == SearchDocBy.su.ToString())
                    {
                        if (!keyword.Contains(" "))
                        {
                            keyword += " ";
                        }
                        sql.AppendLine("and oi.subjects like @keyword");
                        parameters.Add("keyword", keyword.ToLikeParam());
                    }
                    else if (option == SearchDocBy.bt.ToString())
                    {
                        sql.AppendLine("and lower(oi.bib_type) = @keyword");
                        parameters.Add("keyword", keyword);
                    }
                    else if (option == SearchDocBy.yr.ToString())
                    {
                        sql.AppendLine("and oi.year_pub = @keyword");
                        parameters.Add("keyword", keyword);
                    }
                    else if (option == SearchDocBy.bn.ToString())
                    {
                        sql.AppendLine("and oi.isbn like @keyword");
                        parameters.Add("keyword", keyword.ToLikeParam());
                    }
                    else if (option == SearchDocBy.bc.ToString())
                    {
                        sql.AppendLine("and oi.reg_str like @keyword");
                        parameters.Add("keyword", keyword.ToLikeParam());
                    }
                    else
                    {
                        if (!keyword.Contains(" "))
                        {
                            keyword += " ";
                        }
                        sql.AppendLine("and (((quicksearch is not null and quicksearch @@ plainto_tsquery(lower(@keyword)))");
                        sql.AppendLine("or (quicksearch_uns is not null and quicksearch_uns @@ plainto_tsquery(lower(@keyword)))))");
                        parameters.Add("keyword", keyword);
                    }
                }
                else
                {
                    if (type == "search")
                        throw new Exception("Keyword can not be empty!");
                }
            }
            if (type == "collection")
            {
                sql.AppendLine(BuildCollectionQueryFilter(filterMap));
            }
            var tenantCodes = GetTenantFromFilterBy(model?.filterBy);
            sql.AppendLine(BuildTenantQueryFilter());
            if (tenantCodes.Any())
            {
                parameters.Add("_tenantCodes", tenantCodes);
            }
            else
            {
                parameters.Add("_tenantCodes", _tenantCodes);
            }

            return (sql.ToString(), parameters);
        }

        public static (string, DynamicParameters) BuildBasicQuerySearch(SearchBody model, IReadOnlyList<string> _tenantCodes, string type = "search")
        {
            var searchMap = BuildSearchMap(model.searchBy);
            List<Tuple<string, string>> filterMap = new List<Tuple<string, string>>();

            StringBuilder sql = new StringBuilder();
            var parameters = new DynamicParameters();

            if (type == "collection")
            {
                sql.AppendLine(GetFirstPartQueryOfCollection(model));
            }

            sql.AppendLine(ConstQuery.SelectDocumentViewQuery);

            if (type == "search")
            {
                sql.AppendLine("FROM o_item oi");
                sql.AppendLine($"WHERE is_lock = false");
            }
            else if (type == "collection")
            {
                filterMap = BuildSearchMap(model.filterBy);
                sql.AppendLine("FROM o_item oi");
                sql.AppendLine("INNER JOIN o_collection_item oci ON oi.id = oci.item_id");
                sql.AppendLine($"WHERE is_lock = false");
                sql.AppendLine($"AND oci.collection_id IN (SELECT id FROM collection_tree)");
            }

            bool is_unsign = false;
            bool is_exact = false;

            var searchingConditionFlag = 0;
            string templateBuild = string.Empty;
            string column_prop = string.Empty;

            foreach (var item in searchMap)
            {
                string key = item.Item1;
                string value = item.Item2;

                value = value.ToLower().Trim();

                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                if (searchingConditionFlag == 2) break;

                if (key == "is_unsign")
                {
                    is_unsign = string.IsNullOrEmpty(value) ? false : (value == "true" ? true : false);
                    searchingConditionFlag++;
                    continue;
                }
                if (key == "is_exact")
                {
                    is_exact = string.IsNullOrEmpty(value) ? false : (value == "true" ? true : false);
                    searchingConditionFlag++;
                    continue;
                }
            }

            foreach (var item in searchMap)
            {
                templateBuild = templateUnsign;
                string key = item.Item1;
                string value = item.Item2;
                bool isAssigned = false;
                value = value.Trim().ToLower();

                if (key == "is_unsign")
                {
                    continue;
                }
                if (key == "is_exact")
                {
                    continue;
                }
                if (key == OItemEnum.db_type.ToString())
                {
                    if (value == "all") continue;

                    column_prop = OItemEnum.db_type.ToString();
                    templateBuild = templateExactNonLowerStrpos;
                    isAssigned = true;
                }
                if (key == SearchDocBy.bt.ToString())
                {
                    column_prop = OItemEnum.bib_type.ToString();
                    templateBuild = templateExactNonStrpos;
                    isAssigned = true;
                }
                if (key == SearchDocBy.ti.ToString())
                {
                    if (!value.Contains(" "))
                    {
                        value += " ";
                    }
                    column_prop = OItemEnum.title.ToString() + "s";
                }
                else if (key == SearchDocBy.bn.ToString())
                {
                    column_prop = OItemEnum.isbn.ToString();
                }
                else if (key == SearchDocBy.au.ToString())
                {
                    if (!value.Contains(" "))
                    {
                        value += " ";
                    }
                    column_prop = OItemEnum.author.ToString() + "s";
                }
                else if (key == SearchDocBy.su.ToString())
                {
                    if (!value.Contains(" "))
                    {
                        value += " ";
                    }
                    column_prop = OItemEnum.subject.ToString() + "s";
                }
                else if (key == SearchDocBy.kw.ToString())
                {
                    if (!value.Contains(" "))
                    {
                        value += " ";
                    }
                    column_prop = OItemEnum.keyword.ToString() + "s";
                }
                else if (key == $"s_{SearchDocBy.yr}")
                {
                    column_prop = OItemEnum.year_pub.ToString();
                    templateBuild = templateEGreater;
                    isAssigned = true;
                }
                else if (key == $"e_{SearchDocBy.yr}")
                {
                    column_prop = OItemEnum.year_pub.ToString();
                    templateBuild = templateELess;
                    isAssigned = true;
                }
                else if (key == SearchDocBy.bc.ToString())
                {
                    column_prop = OItemEnum.reg_str.ToString();
                }

                if (!isAssigned)
                {
                    // Nếu tìm kiếm chính xác
                    if (!is_unsign && is_exact)
                    {
                        if (IsNotSpecialColumn(column_prop))
                        {
                            templateBuild = templateExact;
                        }
                        else
                        {
                            // Khi không phải column đặc biệt, và là reg_str thì ko nên tìm kiếm tuyệt đối
                            if (IsExceptionalOfSpecialColumn(column_prop))
                            {
                                templateBuild = templateUnsign;
                                value = value.ToLikeParam();
                            }
                            else
                            {
                                templateBuild = templateExactNonLowerStrpos;
                            }
                        }
                    }
                    // Nếu tìm kiếm không dấu thì chỉ áp dụng nên các cột có _uns
                    else if (is_unsign && !is_exact)
                    {
                        if (IsNotSpecialColumn(column_prop))
                        {
                            column_prop += "_uns";
                            templateBuild = templateUnsign;
                            value = value.ToLikeParam();
                        }
                    }
                    // Tìm kiếm thông thường (không phải không dấu, không chính xác)
                    else
                    {
                        if (IsNotSpecialColumn(column_prop))
                        {
                            templateBuild = templateUnsign;
                            value = value.ToLikeParam();
                        }
                        else
                        {
                            if (IsExceptionalOfSpecialColumn(column_prop))
                            {
                                templateBuild = templateUnsign;
                                value = value.ToLikeParam();
                            }
                            else
                            {
                                templateBuild = templateExactNonLowerStrpos;
                            }
                        }
                    }
                }

                var new_param = "keyword_" + Guid.NewGuid().ToString("N")[..6];
                if (templateBuild.ToLower().Contains("and"))
                {
                    sql.AppendLine(string.Format(templateBuild, $"oi.{column_prop}", "@" + new_param));
                }
                else
                {
                    sql.AppendLine("AND " + string.Format(templateBuild, $"oi.{column_prop}", "@" + new_param));
                }
                if (key == $"s_{SearchDocBy.yr}" || key == $"e_{SearchDocBy.yr}")
                {
                    if (int.TryParse(value, out int year))
                    {
                        parameters.Add(new_param, year);
                    }
                }
                else
                {
                    parameters.Add(new_param, value);
                }
            }

            if (type == "collection")
            {
                sql.AppendLine(BuildCollectionQueryFilter(filterMap));
            }

            var tenantCodes = GetTenantFromFilterBy(model.filterBy);
            sql.AppendLine(BuildTenantQueryFilter());
            if (tenantCodes.Any())
            {
                parameters.Add("_tenantCodes", tenantCodes);
            }
            else
            {
                parameters.Add("_tenantCodes", _tenantCodes);
            }

            return (sql.ToString(), parameters);
        }

        public static (string, DynamicParameters) BuildAdvanceQuerySearch(SearchBody searchRequest, IReadOnlyList<string> _tenantCodes, string type = "search")
        {
            var searchMap = BuildSearchMap(searchRequest.searchBy);
            List<Tuple<string, string>> filterMap = new List<Tuple<string, string>>();

            StringBuilder sql = new();
            var parameters = new DynamicParameters();

            if (type == "collection")
                sql.AppendLine(GetFirstPartQueryOfCollection(searchRequest));

            sql.AppendLine(ConstQuery.SelectDocumentViewQuery);
            sql.AppendLine("FROM o_item oi");

            if (type == "search")
            {
                sql.AppendLine("WHERE is_lock = false");
            }
            else if (type == "collection")
            {
                filterMap = BuildSearchMap(searchRequest.filterBy);
                sql.AppendLine("INNER JOIN o_collection_item oci ON oi.id = oci.item_id");
                sql.AppendLine("WHERE is_lock = false");
                sql.AppendLine("AND oci.collection_id IN (SELECT id FROM collection_tree)");
            }

            // Xử lý cờ tìm kiếm
            bool is_unsign = false;
            bool is_exact = false;

            foreach (var item in searchMap)
            {
                string key = item.Item1;
                string value = item.Item2.Trim().ToLower();

                if (string.IsNullOrEmpty(value)) continue;

                if (key == "is_unsign")
                {
                    is_unsign = value == "true";
                    continue;
                }
                if (key == "is_exact")
                {
                    is_exact = value == "true";
                    continue;
                }
            }

            List<Tuple<string, string>> operatorMap = BuildOperatorMap(searchRequest.searchBy ?? Array.Empty<string[]>());
            List<string> notConditions = new();

            // Gom điều kiện theo thứ tự
            List<(string op, string cond)> allConditions = new();

            foreach (var item in searchMap)
            {
                string key = item.Item1;
                string value = item.Item2.Trim();
                if (string.IsNullOrEmpty(value) || key.Contains("_operator"))
                    continue;

                // Bỏ qua cờ tìm kiếm
                if (key == "is_unsign" || key == "is_exact")
                    continue;

                // Xử lý db_type = all thì bỏ qua
                if (key == "db_type" && value.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Xác định field chính
                string groupKey;
                int lastUnderscore = key.LastIndexOf('_');
                var repeatableFields = new HashSet<string> { "ti", "bn", "au", "bc", "kw", "yr", "bt", "su" };

                if (lastUnderscore > 0)
                {
                    string maybeProp = key[..lastUnderscore];
                    string maybeIndex = key[(lastUnderscore + 1)..];
                    if (repeatableFields.Contains(maybeProp) && int.TryParse(maybeIndex, out _))
                        groupKey = key;
                    else
                        groupKey = key;
                }
                else
                {
                    groupKey = key;
                }

                string prop = groupKey == "db_type" ? groupKey : (groupKey.Contains("_") ? groupKey[..groupKey.LastIndexOf("_")] : groupKey);

                string column_prop = prop switch
                {
                    "ti" => "titles",
                    "bn" => "isbn",
                    "au" => "authors",
                    "bc" => "reg_str",
                    "kw" => "keywords",
                    "yr" => "year_pub",
                    "bt" => "bib_type",
                    "su" => "subjects",
                    "db_type" => "db_type",
                    _ => prop
                };

                // Chỉ tìm toán tử nếu không phải db_type/is_unsign/is_exact
                string pOperator = (prop == "db_type") ? "and" : GetOperator(operatorMap, groupKey);

                // Xác định template
                string templateBuild = templateUnsign;

                if (!is_unsign && is_exact)
                {
                    if (IsNotSpecialColumn(column_prop))
                    {
                        templateBuild = templateExact;
                    }
                    else
                    {
                        templateBuild = IsExceptionalOfSpecialColumn(column_prop) ? templateUnsign : templateExactNonLowerStrpos;
                        if (IsExceptionalOfSpecialColumn(column_prop))
                            value = value.ToLikeParam();
                    }
                }
                else if (is_unsign && !is_exact)
                {
                    if (IsNotSpecialColumn(column_prop))
                    {
                        column_prop += "_uns";
                        templateBuild = templateUnsign;
                        value = value.ToLikeParam();
                    }
                }
                else
                {
                    if (column_prop == "db_type")
                    {
                        templateBuild = templateExactNonLowerStrpos;
                    }
                    else if (IsNotSpecialColumn(column_prop))
                    {
                        templateBuild = templateUnsign;
                        value = value.ToLikeParam();
                    }
                    else
                    {
                        if (IsExceptionalOfSpecialColumn(column_prop))
                        {
                            templateBuild = templateUnsign;
                            value = value.ToLikeParam();
                        }
                        else
                            templateBuild = templateExactNonLowerStrpos;
                    }
                }

                var new_param = "keyword_" + Guid.NewGuid().ToString("N")[..6];
                string condition;

                if (pOperator == "not")
                {
                    if (IsNotSpecialColumn(column_prop) || IsExceptionalOfSpecialColumn(column_prop))
                    {
                        condition = string.Format(templateNot, $"oi.{column_prop}", "@" + new_param);
                        value = value.ToLikeParam();
                    }
                    else
                    {
                        condition = string.Format(templateExactNot, $"oi.{column_prop}", "@" + new_param);
                    }
                    notConditions.Add(condition);
                }
                else
                {
                    condition = string.Format(templateBuild, $"oi.{column_prop}", "@" + new_param);
                    allConditions.Add((pOperator, condition));
                }

                parameters.Add(new_param, value);
            }

            // Build expression theo đúng logic OR/AND
            List<string> finalConditions = new();

            if (allConditions.Count > 0)
            {
                // Tách các điều kiện AND và OR
                List<string> andConditions = new();
                List<string> orConditions = new();

                for (int i = 0; i < allConditions.Count; i++)
                {
                    var (op, cond) = allConditions[i];
                    if (op == "and")
                    {
                        andConditions.Add(cond);
                    }
                    else // or
                    {
                        orConditions.Add(cond);
                    }
                }

                // Xây dựng biểu thức cuối cùng
                string expr = "";

                // Xử lý các điều kiện AND (bắt buộc)
                if (andConditions.Count > 0)
                {
                    expr = string.Join(" AND ", andConditions);
                }

                // Xử lý các điều kiện OR (optional)
                if (orConditions.Count > 0)
                {
                    string orExpr = string.Join(" OR ", orConditions);
                    if (!string.IsNullOrEmpty(expr))
                    {
                        expr = $"({expr} AND ({orExpr} OR 1 = 1))";
                    }
                    else
                    {
                        expr = $"({orExpr} OR 1 = 1)";
                    }
                }

                // Nếu chỉ có điều kiện AND
                if (!string.IsNullOrEmpty(expr) && orConditions.Count == 0)
                {
                    expr = $"({expr})";
                }

                finalConditions.Add(expr);
            }

            // Gộp tất cả AND/NOT lại
            List<string> allFinalConditions = [.. finalConditions, .. notConditions];

            if (allFinalConditions.Count > 0)
                sql.AppendLine("AND " + string.Join(" AND ", allFinalConditions));

            // Collection filter
            if (type == "collection")
                sql.AppendLine(BuildCollectionQueryFilter(filterMap));

            // Tenant filter
            var tenantCodes = GetTenantFromFilterBy(searchRequest.filterBy);
            sql.AppendLine(BuildTenantQueryFilter());
            parameters.Add("_tenantCodes", tenantCodes.Any() ? tenantCodes : _tenantCodes);

            return (sql.ToString(), parameters);
        }
    }
}