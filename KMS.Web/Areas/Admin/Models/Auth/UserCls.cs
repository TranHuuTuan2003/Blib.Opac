using System.Data;

namespace KMS.Web.Areas.Admin.Models.Auth
{
    public class UserCls
    {
        public string SiteId { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Full_Name { get; set; } = string.Empty;
        public string Login_Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RetypePassword { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Group_Id { get; set; } = string.Empty;
        public string Group_Code { get; set; } = string.Empty;
        public string Group_Name { get; set; } = string.Empty;
        public string Role_Id { get; set; } = string.Empty;
        public string Role_Code { get; set; } = string.Empty;
        public string Role_Name { get; set; } = string.Empty;
        public string Role_Type { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
        public string OrganizationId { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public int CurrentDBYear { get; set; } = 0;
        public int Is_Active { get; set; } = 1;
        public int Is_Domain { get; set; } = 0;
        public int Is_MainSite { get; set; } = 0;
        public int Is_System { get; set; } = 0;

        public string? Site_Id { get; set; }
        public string Created_Date { get; set; } = string.Empty;
        public string Activated_Date { get; set; } = string.Empty;

        public DataTable dtFunctionItem = new DataTable();
        public string LogoutConfig { get; set; } = string.Empty;

        public string SessionId { get; set; } = string.Empty;
        public UserCls()
        {
        }

        public UserCls(
            //string _Site_Id,
            string _UserId,
        string _FullName,
        string _LoginName,
        string _Password,
        string _GroupId,
        string _GroupCode,
        string _GroupName,
        string _RoleId,
        string _RoleCode,
        string _RoleName,
        string _RoleType,
        string _Note,
        string _OrganizationName,
        string _OrganizationId,
            string _Avatar,
            string _Site_Id,
        int _CurrentDBYear
            )
        {
            UserId = _UserId;
            Full_Name = _FullName;
            Login_Name = _LoginName;
            Password = _Password;
            Group_Id = _GroupId;
            Group_Code = _GroupCode;
            Group_Name = _GroupName;
            Role_Id = _RoleId;
            Role_Code = _RoleCode;
            Role_Name = _RoleName;
            Role_Type = _RoleType;
            Note = _Note;
            OrganizationName = _OrganizationName;
            OrganizationId = _OrganizationId;
            Avatar = _Avatar;
            Site_Id = _Site_Id;
            CurrentDBYear = _CurrentDBYear;
        }


    }
}