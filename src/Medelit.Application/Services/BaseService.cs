using Medelit.Common;
using Medelit.Infra.CrossCutting.Identity.Data;
using Medelit.Infra.CrossCutting.Identity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Medelit.Application
{
    public  class BaseService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public BaseService(ApplicationDbContext context, IHttpContextAccessor contextAccessor, IConfiguration config)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _config = config;

        }
        public string GetAssignedUser(string assignedToId)
        {
            if (string.IsNullOrEmpty(assignedToId))
                return assignedToId;

            return _context.Users.Find(assignedToId).FullName;
        }

        public IEnumerable<AuthUser> GetUsers()
        {
            var users =  _context.Users.ToList();
            var commonUser = new List<AuthUser>();
            foreach (var user in users)
            {
                commonUser.Add(new AuthUser {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
                });
            }
            return commonUser;
        }

        internal string AwsKey
        {
            get
            {
                return _config.GetValue<string>("AWS:AccessKey");
            }
        }
        internal string AwsSecretKey
        {
            get
            {
                return _config.GetValue<string>("AWS:SecretKey");
            }
        }

        internal string BucketName
        {
            get
            {
                return _config.GetValue<string>("AWS:BucketName");
            }
        }

    }
}
