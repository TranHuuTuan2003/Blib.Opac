using System.ComponentModel;

namespace KMS.Shared.Enums
{
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
}