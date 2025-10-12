namespace KMS.Api.Common
{
    public static class GenerateSqlQuery
    {
        public static string GenerateInsertQuery<T>(string tableName, params string[] excludedFields)
        {
            var properties = typeof(T).GetProperties()
                                    .Where(p => !excludedFields.Contains(p.Name));

            var columns = string.Join(", ", properties.Select(p => p.Name));
            var values = string.Join(", ", properties.Select(p => "@" + p.Name));

            return $"INSERT INTO {tableName} ({columns}) VALUES ({values});";   
        }

        public static string GenerateUpdateQuery<T>(string tableName, string keyColumn = "id", params string[] excludedFields)
        {
            var props = typeof(T).GetProperties()
                                .Where(p => p.Name != keyColumn && !excludedFields.Contains(p.Name));

            var setClause = string.Join(", ", props.Select(p => $"{p.Name} = @{p.Name}"));

            return $"UPDATE {tableName} SET {setClause} WHERE {keyColumn} = @{keyColumn};";
        }
    }
}