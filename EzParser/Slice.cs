namespace EzParser
{
    public struct Slice
    {
        public string Base { get; }
        public int StartIndex { get; }
        public int EndIndex { get; }
        public int Length => EndIndex - StartIndex;

        private string _value;
        public string Value => _value ?? (_value = Base.Substring(StartIndex, Length));

        public Slice(string value)
        {
            Base = value;
            StartIndex = 0;
            EndIndex = Base.Length;
            _value = null;
        }

        private Slice(ref Slice value, int from, int length)
        {
            Base = value.Base;
            StartIndex = value.StartIndex + from;
            EndIndex = value.StartIndex + from + length;
            _value = null;
        }

        public Slice Between(int from, int to)
        {
            return this[from, to - from];
        }

        public Slice From(int from)
        {
            return Between(from, Length);
        }

        public Slice To(int to)
        {
            return Between(0, to);
        }

        public Slice this[int from, int length] => new Slice(ref this, from, length);

        public char this[int index] => Base[StartIndex + index];

        public static implicit operator Slice(string value)
        {
            return new Slice(value);
        }
    }
}
