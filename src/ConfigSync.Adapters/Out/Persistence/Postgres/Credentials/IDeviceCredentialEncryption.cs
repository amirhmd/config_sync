namespace ConfigSync.Adapters.Out.Persistence.Postgres.Credentials;

internal interface IDeviceCredentialEncryption
{
    byte[] EncryptPassword(string password);

    byte[] EncryptPrivateKey(string privateKey);

    string DecryptPassword(byte[] encryptedPassword);

    string DecryptPrivateKey(byte[] encryptedPrivateKey);
}