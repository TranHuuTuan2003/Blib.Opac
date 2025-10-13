namespace KMS.Api.Core
{
    public static class ConstQuery
    {
        public static readonly string SelectHomeCollectionQuery = "SELECT oc.id, oc.title, oc.db_type, oc.target_url, oc.order_index, COALESCE(oc.total_bib,0) AS docs_count";
        public static readonly string SelectHomeBanner = "SELECT cover_photo, has_title, title_color, title, subtitle, year";
        public static readonly string SelectNewDocumentQuery = "SELECT oi.id, oi.title, oi.cover_photo, oi.mfn, oi.item_ext, oi.slug";
        public static readonly string SelectDocumentViewQuery = "SELECT oi.id, oi.bib_type, oi.title, oi.year_pub, oi.cover_photo, oi.view, oi.download, oi.db_type, oi.mfn, oi.did, oi.item_ext, oi.is_attachment, oi.slug, oi.tenant_code";
        public static readonly string SelectDocumentDetailQuery = "SELECT oi.id, oi.bib_type, oi.title, oi.year_pub, oi.cover_photo, oi.view, oi.download, oi.db_type, oi.mfn, oi.did, oi.item_ext, oi.is_attachment, oi.slug, oid2.marc_field_value, oid2.marc, oid2.dublin_core";
        public static readonly string SelectRelatedDocumentQuery = SelectNewDocumentQuery;
        public static readonly string SelectMarc21Query = "SELECT oid2.marc_field_value, oid2.marc";
        public static readonly string SelectDublinCoreQuery = "SELECT oid2.dublin_core";
        public static readonly string SelectDigitalFileQuery = "SELECT od.id, oi.title, od.name, od.file_path, od.total_view, od.total_download, od.ext, od.item_id";
        public static readonly string SelectFacetFilter = "SELECT t.value  AS value, t.value AS label, COUNT(*) AS count";
        public static readonly string SelectCollectionTree = "SELECT id AS id, id AS value, title as text, parent_id, COALESCE(total_bib, 0) AS total_bib, order_index, ismobile as is_mobile, ishome";
        public static readonly string SelectFlatCollectionQuery = "SELECT id, title";
        public static readonly string SelectRBorrowingDocument = "SELECT id, mfn, did, title, bib_type, cover_photo, item_ext";
        public static readonly string SelectBibTypeQuery = "SELECT DISTINCT value key, value";
        public static readonly string SelectIntroduceBookQuery = "SELECT id, mfn, title, bib_type, year_pub, cover_photo, slug, item_ext";
		public static readonly string SelectCollectionTreeEn = "SELECT id AS id, id AS value, title_en as text, parent_id, COALESCE(total_bib, 0) AS total_bib, order_index, ismobile as is_mobile, ishome";
	}
}

// Item_ext gồm:

// Class_Name
// Summary
// Publish_Info
// Physical_Info
// Author
// Keyword
// Subject
// Regstr
// Isbd
// Cutter
// Language
// Barcode