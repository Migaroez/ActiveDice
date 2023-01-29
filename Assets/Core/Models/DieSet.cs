namespace Core.Models
{
    public struct DieSet
    {
        public int Value;
        public int Count;

        public DieSet(int value, int count)
        {
            Value = value;
            Count = count;
        }
    }
}
