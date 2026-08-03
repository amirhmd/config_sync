namespace ConfigSync.Infrastructure.Credentials;

public interface IDeviceCredentialEncryption
{
    byte[] EncryptPassword(string password);
    byte[] EncryptPrivateKey(string privateKey);
    string DecryptPassword(byte[] encryptedPassword);
    string DecryptPrivateKey(byte[] encryptedPrivateKey);
}