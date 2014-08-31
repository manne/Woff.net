namespace Blocker
{
    public class NestedBlockOptions
    {
        public NestedBlockOptions(uint maxPadding, uint boundary)
        {
            MaxPadding = maxPadding;
            Boundary = boundary;
        }

        public uint MaxPadding { get; private set; }

        public uint Boundary { get; private set; }
    }
}
