using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

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
            var result = true;
            _childs.Sort((block, block1) => block.Start.CompareTo(block1.Start));
            var enumerator = _childs.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;

                var aggregateException = CheckSingleBlock(current);
                if (!aggregateException)
                {
                    result = false;
                }

                if (OverlapsWithAnyBlock(current))
                {
                    result = false;
                }
            }

            Exceptions = _exceptions;
            return result;
        }

        private bool OverlapsWithAnyBlock(Block block)
        {
            var result = false;
            var otherBlocks = _childs.Except(new[] { block });
            foreach (var otherBlock in otherBlocks)
            {
                if (block.IsOverlapping(otherBlock))
                {
                    result = true;
                    _exceptions.Add(new BlockOverlappingException(string.Format(CultureInfo.InvariantCulture, "Blocks are overlapping, {0} and {1}.", block, otherBlock)));
                }
            }

            return result;
        }

        private bool CheckSingleBlock(Block block)
        {
            var result = true;

            if (block.Start > End)
            {
                result = false;
                _exceptions.Add(new BlockOverlappingException(string.Format(CultureInfo.InvariantCulture, "Block start is behind container. Block {0} Container {1}.", block, this)));
            }

            if (block.End > End)
            {
                result = false;
                _exceptions.Add(new BlockOverlappingException(string.Format(CultureInfo.InvariantCulture, "Block end is behind container. Block {0} Container {1}.", block, this)));
            }

            if (!IsOnCorrectBoundary(block))
            {
                result = false;
                _exceptions.Add(new BlockNotOnBoundaryException(string.Format(CultureInfo.InvariantCulture, "Block is not on a {0} boundary. It starts at {1}.", _options.Boundary, block.Start)));
            }

            return result;
        }

        public IEnumerable<Exception> Exceptions { get; private set; }
    }
}
