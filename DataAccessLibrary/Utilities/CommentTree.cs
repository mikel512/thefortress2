using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Utilities
{
    public class CommentTree
    {
        private Dictionary<int?, List<CommentModel>> _commentDict;
        public CommentTree(Dictionary<int?, List<CommentModel>> commentList)
        {
            _commentDict = commentList;
        }
        
        public List<CommentModel> GetEventComments()
        {
            // for each root comment, recursively build m-ary tree
            foreach (var item in _commentDict[0])
            {
                // if comment has no replies
                if (!_commentDict.ContainsKey(item.CommentId)) continue;
                // else, make the tree
                MakeTree(item, _commentDict[item.CommentId], 0);
            }

            return _commentDict[0];
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
                // if this comment has no replies, continue
                if (!_commentDict.ContainsKey(item.CommentId)) continue;
                MakeTree(item, _commentDict[item.CommentId], height);
            }

            return root;
        }
    }
}
