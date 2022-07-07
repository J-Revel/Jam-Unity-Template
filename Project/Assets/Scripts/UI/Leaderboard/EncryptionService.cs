using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.IO;
using System.Text;


public struct EncryptionResult
{
    public byte[] data;
    public byte[] iv;
}

public class EncryptionService : MonoBehaviour
{
    public static string encryptionKey;
    public static string projectId;

    public void Start()
    {
        
    }

    public static EncryptionResult Encrypt(byte[] data)
    {
        TextAsset configAsset = Resources.Load<TextAsset>("config");
        SimpleJSON.JSONNode configRoot = SimpleJSON.JSON.Parse(configAsset.text);
        encryptionKey = configRoot["encryption_key"];
        projectId = configRoot["project_uid"];
        EncryptionResult result = new EncryptionResult();
        using (RijndaelManaged rijndael = new RijndaelManaged())
        {
            rijndael.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            rijndael.Mode = System.Security.Cryptography.CipherMode.CBC;
            rijndael.Key = Encoding.UTF8.GetBytes(encryptionKey);
            result.iv = rijndael.IV;
            // Debug.Log("to encrypt : " + System.BitConverter.ToString(data).Replace("-", ""));
        
            // Debug.Log("encryption key : " + System.BitConverter.ToString(rijndael.Key).Replace("-", ""));
            // Debug.Log("IV : " + System.BitConverter.ToString(rijndael.IV).Replace("-", ""));
            ICryptoTransform encryptor = rijndael.CreateEncryptor(rijndael.Key, rijndael.IV);
            
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            result.data = ms.ToArray();
            // Debug.Log("encrypted data : " + System.BitConverter.ToString(result.data).Replace("-", ""));
        }
        return result;
    }
}
