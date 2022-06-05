using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UtilityTypes.Trees
{
    public class TreeNode<GIndex, GValue>
    {

        public GValue Value { get; set; }

        private Dictionary<GIndex, TreeNode<GIndex, GValue>> _children = new Dictionary<GIndex, TreeNode<GIndex, GValue>>();

        public TreeNode<GIndex, GValue> GotoChildOrCreate(GIndex index)
        {
            if (_children.ContainsKey(index))
            {
                return _children[index];
            }
            TreeNode<GIndex, GValue> node = new TreeNode<GIndex, GValue>();
            _children.Add(index, node);
            return node;
        }

    }
}
