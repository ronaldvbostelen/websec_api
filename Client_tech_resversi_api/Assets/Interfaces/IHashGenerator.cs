namespace Client_tech_resversi_api.Assets.Interfaces
{
    public interface IHashGenerator
    {
        string GenerateHash(string seed);
        string GenerateURlSaveHash(string seed);
    }
}