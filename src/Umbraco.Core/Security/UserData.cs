using System;
using System.Runtime.Serialization;

namespace Umbraco.Core.Security
{
    /// <summary>
    /// Data structure used to store information in the authentication cookie
    /// </summary>
    [DataContract(Name = "userData", Namespace = "")]
    internal class UserData
    {
        public UserData()
        {
            AllowedApplications = new string[] {};
            Roles = new string[] {};
        }

        ///// <summary>
        ///// When their session is going to expire (in ticks)
        ///// </summary>
        //[DataMember(Name = "timeout")]
        //public long Timeout { get; set; }

        [DataMember(Name = "userContextId")]
        public string UserContextId { get; set; }

        [DataMember(Name = "id")]
        public object Id { get; set; }
        
        [DataMember(Name = "roles")]
        public string[] Roles { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "name")]
        public string RealName { get; set; }

        [DataMember(Name = "startContent")]
        public int StartContentNode { get; set; }

        [DataMember(Name = "startMedia")]
        public int StartMediaNode { get; set; }

        [DataMember(Name = "allowedApps")]
        public string[] AllowedApplications { get; set; }

        [DataMember(Name = "culture")]
        public string Culture { get; set; }
    }
}