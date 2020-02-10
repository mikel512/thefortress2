using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.SqlDataAccess
{
    public class CommentTree : DataAccess
    {
        
        public List<CommentModel> GetEventComments(int eventId)
        {
            // get root comments into one list, get root comments procedure
            var rootList = QueryComments("GetRootEventComments", Pairing.Of("@eventId", eventId));

            // for each root comment, recursively build m-ary tree; get child comments procedure
            foreach (var item in rootList)
            {
                MakeTree(item, GetCommentChildren(item.CommentId), 0);
                OrderTree(item);
            }
            
            return rootList;
        }

        private CommentModel MakeTree(CommentModel root, List<CommentModel> children, int height)
        {
            // base case
            root.Height = height;
            if (children == null) return root;

            height++;
            foreach (var item in children)
            {
                // initialize list if null
                if (root.Children == null)
                {
                    root.Children = new List<CommentModel>();
                }

                item.Height = height;
                root.Children.Add(item);
                MakeTree(item, GetCommentChildren(item.CommentId), height);
            }

            return root;
        }

        private void OrderTree(CommentModel root)
        {
            if (root.Children == null || root.Children.Count == 0) return;

            for(int i = 0; i < root.Children.Count; i++)
            {
                if(root.Children[i].Children == null) continue;
                
                if (root.Children[i].Children.Count > 0) 
                {
                    CommentModel temp = root.Children[0];
                    root.Children[0] = root.Children[i];
                    root.Children[i] = temp;
                    // OrderTree(root.Children[i]);
                }
            }
        }

        private List<CommentModel> GetCommentChildren(int? parentId)
        {
            return QueryComments("GetChildEventComments", 
                Pairing.Of("@commentID", parentId));
        }

        private List<CommentModel> QueryComments(string procedure, params KeyValuePair<string, object>[] pairs)
        {
            var list = new List<CommentModel>();
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var command = new SqlCommand(procedure, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                foreach (var keyValuePair in pairs)
                {
                    command.Parameters.Add(keyValuePair.Key, SqlDbType.Int).Value = keyValuePair.Value;
                }

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var check = reader["ParentCommentId"];
                    list.Add( new CommentModel
                    {
                        CommentId = Convert.ToInt32(reader["CommentId"]),
                        EventId = Convert.ToInt32(reader["EventId"]),
                        //Check for null ParentCommentId
                        ParentCommentId = (check.GetType() == typeof(DBNull)) ? (int?) null : Convert.ToInt32(check),
                        Content = reader["Content"].ToString(),
                        DateStamp = Convert.ToDateTime(reader["DateStamp"]),
                        Upvotes = Convert.ToInt32(reader["Upvotes"]),
                        UserId = reader["UserId"].ToString(),
                        UserName = reader["UserName"].ToString(),
                    });
                }
            }

            return list;
        }

        public CommentTree(ApplicationDbContext configuration) : base(configuration)
        {
        }
    }
}
