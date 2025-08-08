// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Meziantou.Framework.Win32;

namespace NativeAOTDependencyHelper.Core;

/// <summary>
/// CredentialManager helper class copied from the Microsoft Store CLI project.
/// </summary>
public class CredentialManager
{
    internal string ApplicationName { get; set; } = "NativeAOTDependencyHelper";

    private string defaultUserName = "default";

    public bool HasCredential { get; private set; } = false;

    private string GetCredentialName() => $"{ApplicationName}:user={defaultUserName}";

    public CredentialManager()
    {
        HasCredential = !string.IsNullOrEmpty(ReadCredential());
    }

    public string? ReadCredential()
    {
        var clientCredential = Meziantou.Framework.Win32.CredentialManager.ReadCredential(GetCredentialName());

        if (clientCredential != null)
        {
            return clientCredential.Password;
        }

        return string.Empty;
    }

    public void WriteCredential(string secret)
    {
        Meziantou.Framework.Win32.CredentialManager.WriteCredential(GetCredentialName(), defaultUserName, secret, CredentialPersistence.LocalMachine);
        HasCredential = !String.IsNullOrEmpty(secret);
    }

    public void ClearCredentials(string userName)
    {
        try
        {
            var credentialName = GetCredentialName();
            if (Meziantou.Framework.Win32.CredentialManager.EnumerateCredentials(credentialName).Any())
            {
                Meziantou.Framework.Win32.CredentialManager.DeleteCredential(credentialName);
            }
            HasCredential = false;
        }
        catch
        {
        }
    }
}
