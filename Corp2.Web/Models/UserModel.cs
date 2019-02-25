using Corp2.Web.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Corp2.Web.Models
{
    public class UserModel
    {
        [DisplayName("User Id")]
        public string UserId { get; set; }

        [DisplayName("User Name")]
        public string UserName { get; set; }

        public string Password { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [DisplayName("Corporation Id")]
        public string CorporationId { get; set; }

        public string CorporationName
        {
            get
            {
                if (!string.IsNullOrEmpty(CorporationId))
                {
                    var corp = CorporationService.Default.GetCorporation(CorporationId);
                    return (corp == null) ? string.Empty : corp.Name;
                }
                else return string.Empty;
            }
        }
    }
}