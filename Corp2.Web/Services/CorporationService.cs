using Corp2.Web.Models;
using System.Collections.Generic;
using System.Runtime.Caching;
using Corp2.Lib;
using System.Linq;
using System.Threading.Tasks;

namespace Corp2.Web.Services
{
    public class CorporationService : ClassBase
    {
        private Corp2.Lib.CorporationLib _corporationLibrary;
        private UserService _userService;

        public CorporationService()
        {
            _corporationLibrary = new CorporationLib();
            _userService = new UserService();
        }
        private List<CorporationModel> CorporationList
        {
            get {
                var list = (List<CorporationModel>)MemoryCache.Default["CorporationList"];
                if (null == list)
                {
                    list = new List<CorporationModel>();
                    MemoryCache.Default["CorporationList"] = list;
                }
                return list;
            }

            set
            {
                MemoryCache.Default["CorporationList"] = value;
            }
        }

        public static CorporationService Default => new CorporationService();

        public IList<CorporationModel> GetCorporationList()
        {
            return CorporationList;
        }
        public CorporationModel GetCorporation(string id)
        {
            return CorporationList.FirstOrDefault(c=>c.CorporationId==id);
        }

        public async Task<bool> AddCorporation(CorporationModel corp)
        {
            if (Validate(corp))
            {
                if (CorporationList.Exists(c => c.CorporationId == corp.CorporationId))
                {
                    ErrorDescription = "Corporation Id already exists";
                    return false;
                }
                CorporationList.Add(corp);
                await _corporationLibrary.AddCorporation(corp);
                return true;
            }
            return false;
        }

        public bool UpdateCorporation(CorporationModel corp)
        {
            if (Validate(corp))
            {
                var pos = CorporationList.FindIndex(c => c.CorporationId == corp.CorporationId);
                if (pos >= 0) CorporationList.RemoveAt(pos);
                CorporationList.Add(corp);
                return true;
            }
            return false;
        }

        public bool DeleteCorporation(string corporationId)
        {
            bool retVal = false;
            var pos = CorporationList.FindIndex(c => c.CorporationId == corporationId);
            if (pos >= 0)
            {
                if (_userService.GetUserByCorporation(corporationId))
                {
                    ErrorDescription = "This corporation has users assigned. Unable to delete.";
                }
                else
                {
                    CorporationList.RemoveAt(pos);
                    retVal = true;
                }
            }
            else
            {
                ErrorDescription = "Corporation not found";
            }
            return retVal;
        }

        private bool Validate(CorporationModel corp)
        {
            if (string.IsNullOrEmpty(corp.CorporationId))
            {
                ErrorDescription = "Corporation Id is required";
                return false;
            }
            if (string.IsNullOrEmpty(corp.Name))
            {
                ErrorDescription = "Corporation Name is required";
                return false;
            }
            return true;
        }
    }
}