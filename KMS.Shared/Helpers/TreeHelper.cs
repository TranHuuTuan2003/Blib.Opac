using KMS.Shared.DTOs.Tree;

namespace KMS.Shared.Helpers
{
    public static class TreeHelper
    {
        public static List<T> BuildTree<T>(List<T> flatList) where T : CustomTree<T>
        {
            if (flatList.Count == 1)
            {
                return flatList;
            }

            var lookup = flatList.ToLookup(x => string.IsNullOrEmpty(x.parent_id) ? null : x.parent_id);

            foreach (var node in flatList)
            {
                node.children = lookup[node.value].OrderBy(x => x.order_index).ToList();
            }

            return lookup[null].OrderBy(x => x.order_index).ToList();
        }
    }
}
