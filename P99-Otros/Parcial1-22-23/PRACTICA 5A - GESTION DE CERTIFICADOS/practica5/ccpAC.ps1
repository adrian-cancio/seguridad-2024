$cert = New-SelfSignedCertificate -Type Custom `
-Subject "CN=zpAC" `
-KeyAlgorithm RSA -KeyLength 2048 -KeySpec Signature -KeyExportPolicy Exportable `
-KeyUsageProperty Sign -KeyUsage CertSign `
-HashAlgorithm sha256 `
-CertStoreLocation "Cert:\CurrentUser\My"