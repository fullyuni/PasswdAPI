using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace PasswdAPI.Controllers
{

    [Route("api")]
    [ApiController]
    public class APIController : ControllerBase
    {
        //landing page on startup
        [HttpGet]
        public ActionResult<string> Get()
        {
            return @"Please make one of the following API calls: 
/users                                                                                      Return a list of all users on the system, as defined in the /etc/passwd file.
/users/query[?name=<nq>][&uid=<uq>][&gid=<gq>][&comment=<cq>][&home=<hq>][&shell=<sq>]      Return a list of users matching all of the specified query fields.
/users/<uid>                                                                                Return a single user with <uid>.
/users/<uid>/groups                                                                         Return all the groups for a given user.
/groups                                                                                     Return a list of all groups on the system, a defined by /etc/group.
/groups/query[?name=<nq>][&gid=<gq>][&member=<mq1>[&member=<mq2>][&...]                     Return a list of groups matching all of the specified query fields.
/groups/<gid>                                                                               Return a single group with <gid>.";
        }

        [HttpGet("users/")]
        public ActionResult<string> GetUsers()
        {
            return APIMethods.GetUsers(Program.dataSet.Tables["UserTable"]);
        }

        [HttpGet("users/{id}")]
        public ActionResult<string> GetUser(int id)
        {
            string result = APIMethods.GetUser(Program.dataSet.Tables["UserTable"], id);
            if (result == null)
            {
                Response.StatusCode = 404;
                JObject jObject = new JObject();

                jObject.Add("error", $"User with id {id} not found.");
                jObject.Add("error code", Response.StatusCode);
            }
            return result;
        }

        [HttpGet("users/query")]
        public ActionResult<string> GetUsersByQuery()
        {
            return APIMethods.GetUsers(Program.dataSet.Tables["UserTable"], Request.QueryString.ToString());
        }

        /* The test explanation was not clear on the relationship between user and group
         * so I included both possible relationships: User gids -> Group gid
         * and User name -> Group members
        */
        [HttpGet("users/{id}/groups")]
        public ActionResult<string> GetUserGroups(int id)
        {
            string results = @"Groups found using user's gid: 
" + APIMethods.GetGroupsByUser(Program.dataSet, id) + @"
Groups found using user's name as a member: 
" + APIMethods.GetGroupsByMember(Program.dataSet, id);

            return results;
        }

        [HttpGet("groups/")]
        public ActionResult<string> GetGroups()
        {
            return APIMethods.GetGroups(Program.dataSet.Tables["GroupTable"]);
        }

        [HttpGet("groups/query")]
        public ActionResult<string> GetGroupsByQuery()
        {
            return APIMethods.GetGroups(Program.dataSet.Tables["GroupTable"], Request.QueryString.ToString());
        }

        [HttpGet("groups/{id}")]
        public ActionResult<string> GetGroup(int id)
        {
            string result = APIMethods.GetGroup(Program.dataSet.Tables["GroupTable"], id);
            if (result == null)
            {
                Response.StatusCode = 404;
                JObject jObject = new JObject();

                jObject.Add("error", $"User with id {id} not found.");
                jObject.Add("error code", Response.StatusCode);

                result = jObject.ToString();
            }
            return result;
        }
    }
}
