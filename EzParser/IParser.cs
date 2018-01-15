namespace EzParser
{
    public interface IParser
    {
        AstNode Parse(Slice input, Context ctx);
    }
}
