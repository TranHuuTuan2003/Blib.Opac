using System.ComponentModel;

namespace KMS.Api.Common
{
    public class Enums
    {
        public enum SearchType
        {
            Init,
            Quick,
            Basic,
            Advance
        }
        public enum SearchDocBy
        {
            [Description("quick search")]
            qs,
            [Description("title")]
            ti,
            [Description("author")]
            au,
            [Description("subject")]
            su,
            [Description("publisher")]
            pb,
            [Description("oclc number")]
            no,
            [Description("keyword")]
            kw,
            [Description("journal name")]
            so,
            [Description("isbn/issn")]
            bn,
            [Description("barcode")]
            bc,
            [Description("year")]
            yr,
            [Description("language")]
            lg,
            [Description("bibtype")]
            bt,
            [Description("class")]
            cl,
            [Description("cip nlv")]
            ci,
            [Description("all")]
            uc,

            //▪	qs: title. isbn/issn. cip 
            //▪	ti: title
            //▪	au: author
            //▪	su: subject
            //▪	pb: publisher
            //▪	no: OCLC number
            //▪	kw: keyword
            //▪	so: journal name
            //▪	bn: isbn/issn
            //▪	bc: barcode
            //▪	yr: year
            //▪	lg: languague
            //▪	bt: bibtype
            //▪	cl: class
            //▪	ci: cip nlv
            //▪	uc: all

        }
        public enum OItemEnum
        {
            id,
            item_detail,
            quicksearch,
            quicksearch_uns,
            keyword,
            keywords_uns,
            subject,
            subjects_uns,
            cover_photo,
            view,
            download,
            is_attachment,
            created_date,
            updated_date,
            source_id,
            db_type,
            mfn,
            is_lock,
            class_name,
            bib_type,
            title,
            title_sort,
            titles_uns,
            author,
            author_sort,
            authors,
            authors_uns,
            year_pub,
            year_pub_no,
            publish_info,
            origin_id,
            did,
            source,
            keywords,
            isbn,
            reg_str,
            subjects,
            titles,
            keyword_info,
            physic_info,
            titl,
            auth,
            keyword_uns,
            subject_uns,
            is_new
        }
    }
}
