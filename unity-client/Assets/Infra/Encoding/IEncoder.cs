namespace Infra.Encoding
{
    //todo: add crypt encoder
    public interface IEncoder
    {
        string Encode(string sourceData);

        string Decode(string sourceData);
    }
}