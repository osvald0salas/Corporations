using Corp2.Web.Models;
using System.Collections.Generic;
using System.Runtime.Caching;
using Corp2.Lib;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Corp2.Web.Services
{
    public class UserService: ClassBase
    {
        private List<UserModel> UserList
        {
            get {
                var list = (List<UserModel>)MemoryCache.Default["UserList"];
                if (null == list)
                {
                    list = new List<UserModel>();
                    MemoryCache.Default["UserList"] = list;
                }
                return list;
            }

            set
            {
                MemoryCache.Default["UserList"] = value;
            }
        }
        public IList<UserModel> GetUserList()
        {
            return UserList;
        }
        public IList<UserModel> GetUserListByCorporation(string corporationId)
        {
            return (IList<UserModel>)UserList.Where(s=> s.CorporationId==corporationId).ToList();
        }
        public UserModel GetUser(string id)
        {
            return UserList.FirstOrDefault(c => c.UserId == id);
        }
        public bool GetUserByCorporation(string corporationId)
        {
            var retVal = UserList.Exists(c => c.CorporationId == corporationId);
            return retVal;
        }

        public bool AddUser(UserModel user)
        {
            if (Validate(user))
            {
                if (UserList.Exists(c => c.UserId == user.UserId))
                {
                    ErrorDescription = "User Id already exists";
                    return false;
                }
                UserList.Add(user);
                if (new EmailAddressAttribute().IsValid(user.Email))
                {
                    GoogleMail.Default.SendMail(user.Email,"User added", $"The user {user.UserName} has been added to the Corporation {user.CorporationName}");
                }
                return true;
            }
            return false;
        }

        public bool UpdateUser(UserModel user)
        {
            if (Validate(user))
            {
                var pos = UserList.FindIndex(c => c.UserId == user.UserId);
                if (pos >= 0) UserList.RemoveAt(pos);
                UserList.Add(user);
                if (new EmailAddressAttribute().IsValid(user.Email))
                {
                    GoogleMail.Default.SendMail(user.Email,"User updated", $"The user {user.UserName} has been updated");
                }
                return true;
            }
            return false;
        }

        public bool DeleteUser(string UserId)
        {
            var pos = UserList.FindIndex(c => c.UserId == UserId);
            if (pos >= 0)
            {
                UserList.RemoveAt(pos);
                return true;
            }
            else
            {
                ErrorDescription = "User not found";
                return false;
            }
        }

        private bool Validate(UserModel user)
        {
            if (string.IsNullOrEmpty(user.UserId))
            {
                ErrorDescription = "User Id is required";
                return false;
            }
            if (string.IsNullOrEmpty(user.UserName))
            {
                ErrorDescription = "User Name is required";
                return false;
            }
            return true;
        }
    }
}