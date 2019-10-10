using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PasswdAPI
{
    public class APIMethods
    {
        //gets collection of users based on input parameters
        public static string GetUsers(DataTable dataTable, string input = null)
        {
            DataRow[] results;

            //if blank, return all users, otherwise, query based on input params
            if (input == null || input.Length == 0) results = dataTable.Select();
            else
            {
                //cleanup query string
                input = input.Replace("?", "");

                //seperate parameters
                string[] parameters = input.Split("&");

                string query = ParameterCorrect(parameters[0], true);

                //build query stirng
                for (int i = 1; i < parameters.Count(); i++)
                {
                    query += $" AND {ParameterCorrect(parameters[i], true)}";
                }

                results = dataTable.Select(query);
            }

            //convert collection of users to JSON Array
            JArray users = new JArray();

            foreach (var row in results)
            {
                JObject user = new JObject();

                user.Add("name", row["name"].ToString());
                user.Add("uid", row["uid"].ToString());
                user.Add("gid", row["gids"].ToString().Replace("|", ""));
                user.Add("comment", row["comment"].ToString());
                user.Add("home", row["home"].ToString());
                user.Add("shell", row["shell"].ToString());

                users.Add(user);
            }

            return users.ToString();
        }

        //gets a user based on the unique uid
        public static string GetUser(DataTable dataTable, int uid)
        {
            
            DataRow[] results = dataTable.Select($"uid = {uid}");

            //should only ever return one result
            DataRow row = results[0];

            JObject user = new JObject();

            user.Add("name", row["name"].ToString());
            user.Add("uid", row["uid"].ToString());
            user.Add("gid", row["gids"].ToString().Replace("|", ""));
            user.Add("comment", row["comment"].ToString());
            user.Add("home", row["home"].ToString());
            user.Add("shell", row["shell"].ToString());

            return user.ToString();
        }

        //gets collection of groups based on input parameters
        public static string GetGroups(DataTable dataTable, string input = null)
        {
            DataRow[] results;

            if (input == null || input.Length == 0) results = dataTable.Select();
            else
            {
                input = input.Replace("?", "");


                string[] parameters = input.Split("&");

                string query = ParameterCorrect(parameters[0]);

                for (int i = 1; i < parameters.Count(); i++)
                {
                    query += $" AND {ParameterCorrect(parameters[i])}";
                }

                results = dataTable.Select(query);
            }

            JArray groups = new JArray();

            foreach (var row in results)
            {
                JObject group = new JObject();

                group.Add("name", row["name"].ToString());
                group.Add("gid", row["gid"].ToString());
                group.Add("members", row["members"].ToString().Replace("|", ""));

                groups.Add(group);
            }

            return groups.ToString();
        }

        //gets a group based on the unique gid
        public static string GetGroup(DataTable dataTable, int gid)
        {
            DataRow[] results = dataTable.Select($"gid = {gid}");

            DataRow row = results[0];

            JObject group = new JObject();

            group.Add("name", row["name"].ToString());
            group.Add("gid", row["gid"].ToString());
            group.Add("members", row["members"].ToString().Replace("|", ""));

            return group.ToString();
        }

        //gets all groups with the same gid as the selected user
        public static string GetGroupsByUser(DataSet dataSet, int uid)
        {
            DataRow[] results = dataSet.Tables["UserTable"].Select($"uid = {uid}");

            DataRow row = results[0];

            string output = "";

            if (row["gids"].ToString().Replace("|", "").Length > 0)
            {
                string[] gids = row["gids"].ToString().Replace("|", "").Split(",");
                string query = "?";

                foreach (var gid in gids)
                {
                    query += $"gid={gid}";
                }

                output = GetGroups(dataSet.Tables["GroupTable"], query);
            }
            else output = "No groups associated with user's gid";

            return output;
        }

        //gets all groups with the user's name as one of the members
        public static string GetGroupsByMember(DataSet dataSet, int uid)
        {
            DataRow[] results = dataSet.Tables["UserTable"].Select($"uid = {uid}");

            DataRow row = results[0];

            return GetGroups(dataSet.Tables["GroupTable"], $"member={row["name"].ToString()}");
        }

        //converts parameters recieved from controller to queryable code
        //isUser causes the function to treat gid's differently since a user may have multiple
        private static string ParameterCorrect(string input, bool isUser = false)
        {
            string[] parts = input.Split("=");
            string output = "";

            if (parts[0].Equals("member")) output = $"members like '*|{parts[1]}|*'";
            else if (parts[0].Equals("gid") && isUser) output = $"gids like '*|{parts[1]}|*'";
            else output = $"{parts[0]} = '{parts[1]}'";
            return output;
        }
    }
}
