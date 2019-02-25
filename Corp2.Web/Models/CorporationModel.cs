using System.ComponentModel;

namespace Corp2.Web.Models
{
    public class CorporationModel: Lib.Corporation
    {
        public CorporationModel()
        {

        }
        [DisplayName("Corporation Id")]
        public override string CorporationId { get; set; }
    }
}
