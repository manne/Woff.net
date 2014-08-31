using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Blocker
{
    public class NestedBlock : Block
    {
        private readonly List<Block> _childs = new List<Block>(10);

        public NestedBlock(uint start, uint end)
            : base(start, end)
        {
            // empty
        }

        public ReadOnlyCollection<Block> Childs
        {
            get
            {
                return new ReadOnlyCollection<Block>(_childs);
            }
        }

        public void AddChild(Block child)
        {
            _childs.Add(child);
        }

        public bool Validate()
        {
            return true;
        }
    }
}
