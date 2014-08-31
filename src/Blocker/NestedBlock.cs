using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace Blocker
{
    public class NestedBlock : Block
    {
        private readonly NestedBlockOptions _options;

        private readonly List<Block> _childs = new List<Block>(10);

        public NestedBlock(uint start, uint end, NestedBlockOptions options)
            : base(start, end)
        {
            Contract.Requires(options != null);

            _options = options;
        }

        public ReadOnlyCollection<Block> Childs
        {
            get
            {
                return new ReadOnlyCollection<Block>(_childs);
            }
        }

        private bool IsOnCorrectBoundary(Block block)
        {
            var result = block.Start % _options.Boundary == 0;
            return result;
        }

        public void AddChild(Block child)
        {
            _childs.Add(child);
        }

        public bool Validate()
        {
            foreach (var child in _childs)
            {
                if (!IsOnCorrectBoundary(child))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
