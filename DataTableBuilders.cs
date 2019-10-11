using System;
using System.Data;
using System.IO;

/* This class builds the in-memory tables used in this solution.
 * See README.txt for a better explanation as to why I chose this method
 * vs. the traditional database model.
*/

namespace PasswdAPI
{
    public class DataTableBuilders
    {
        //Builds/updates the GroupTable based on the input filePath
        public static void UpdateGroupTable(DataSet dataSet, string filePath)
        {
            //init table and columns
            DataTable groupTable = new DataTable("GroupTable");
            DataColumn column;

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "name";
            column.AutoIncrement = false;
            column.ReadOnly = false;
            column.Unique = false;
            groupTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.Int32");
            column.ColumnName = "gid";
            column.AutoIncrement = false;
            column.ReadOnly = false;
            column.Unique = true;
            groupTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "members";
            column.AutoIncrement = false;
            column.ReadOnly = false;
            column.Unique = false;
            groupTable.Columns.Add(column);

            dataSet.Tables.Add(groupTable);

            try
            {
                //read in input file and build table rows based on data on each line
                using (StreamReader streamReader = new StreamReader(filePath))
                {
                    try
                    {
                        string record = streamReader.ReadLine();

                        while (record != null && record.Length > 0)
                        {
                            //skip header comments
                            if (!record.Contains("#"))
                            {
                                DataRow row = groupTable.NewRow();

                                string[] groupValues = record.Split(":");

                                row["name"] = groupValues[0];
                                //groupValues[1] is the password which is always "*" and therefore cam be ignored
                                row["gid"] = groupValues[2];

                                //beacuse a group may have multiple members, we handle this column
                                string memberString = "";
                                if (groupValues[3].Length > 0)
                                {
                                    string[] members = groupValues[3].Split(",");

                                    //by surrounding the individual members with "|", we can prevent the
                                    //chance of accidentally getting partial names when we later query
                                    //the table (i.e. "groot" would be returned when searching "root").
                                    memberString += $"|{members[0]}|";
                                    for (int i = 1; i < members.Length; i++)
                                    {
                                        memberString += $",|{members[i]}|";
                                    }
                                }

                                row["members"] = memberString;

                                groupTable.Rows.Add(row);

                            }

                            record = streamReader.ReadLine();
                        }
                    }
                    catch
                    {
                        Program.errorStatus = PasswdErrors.PasswdError.BadData;
                    }

                    streamReader.Close();
                }
            }
            catch
            {
                Program.errorStatus = PasswdErrors.PasswdError.FileNotFound;
            }
        }

        //Builds/updates the UserTable based on the input filePath
        public static void UpdateUserTable(DataSet dataSet, string filePath)
        {
            //init table and columns
            DataTable userTable = new DataTable("UserTable");
            DataColumn column;

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "name";
            column.AutoIncrement = false;
            column.ReadOnly = false;
            column.Unique = false;
            userTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.Int32");
            column.ColumnName = "uid";
            column.AutoIncrement = false;
            column.ReadOnly = false;
            column.Unique = true;
            userTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "gids";
            column.AutoIncrement = false;
            column.ReadOnly = false;
            column.Unique = false;
            userTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "comment";
            column.AutoIncrement = false;
            column.ReadOnly = false;
            column.Unique = false;
            userTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "home";
            column.AutoIncrement = false;
            column.ReadOnly = false;
            column.Unique = false;
            userTable.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "shell";
            column.AutoIncrement = false;
            column.ReadOnly = false;
            column.Unique = false;
            userTable.Columns.Add(column);

            dataSet.Tables.Add(userTable);

            try
            {
                //read in input file and build table rows based on data on each line
                using (StreamReader streamReader = new StreamReader(filePath))
                {
                    try
                    {
                        string record = streamReader.ReadLine();

                        while (record != null && record.Length > 0)
                        {
                            //skip header comments
                            if (!record.Contains("#"))
                            {
                                DataRow row = userTable.NewRow();

                                string[] userValues = record.Split(":");

                                row["name"] = userValues[0];
                                //userValues[1] is the password which is always "*" and therefore cam be ignored
                                row["uid"] = userValues[2];

                                //similar to the group members issue from above, we use this to handle
                                //multiple gids and prevent querying substrings
                                string gidString = "";
                                if (userValues[3].Length > 0)
                                {
                                    string[] gids = userValues[3].Split(",");

                                    gidString += $"|{gids[0]}|";

                                    for (int i = 1; i < gids.Length; i++)
                                    {
                                        gidString += $",|{gids[i]}|";
                                    }
                                }

                                row["gids"] = gidString;
                                row["comment"] = userValues[4];
                                row["home"] = userValues[5];
                                row["shell"] = userValues[6];

                                userTable.Rows.Add(row);
                            }

                            record = streamReader.ReadLine();
                        }

                        streamReader.Close();
                    }
                    catch
                    {
                        Program.errorStatus = PasswdErrors.PasswdError.BadData;
                    }
                }
                    
            }
            catch
            {
                Program.errorStatus = PasswdErrors.PasswdError.FileNotFound;
            }
            
        }

    }
}
