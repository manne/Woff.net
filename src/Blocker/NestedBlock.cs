using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Globalization;

using Blocker.Exceptions;

namespace Blocker
{
    public class NestedBlock : Block
    {
        private readonly NestedBlockOptions _options;

        private readonly List<Exception> _exceptions = new List<Exception>(10);

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
                    _exceptions.Add(new BlockNotOnBoundaryException(string.Format(CultureInfo.InvariantCulture, "Block is not on a {0} boundary. It starts at {1}.", _options.Boundary, child.Start)));
                    Exceptions = _exceptions;
                    return false;
                }
            }

            return true;
        }

        public IEnumerable<Exception> Exceptions { get; private set; }
    }
}
