$cert = New-SelfSignedCertificate -Type Custom -DnsName zpUSU `
-Subject "CN=zpUSU" `
-KeyAlgorithm RSA -KeyLength 2048 -KeySpec Signature -KeyExportPolicy Exportable `
-HashAlgorithm sha256 `
-Signer $cert -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.2") `
-CertStoreLocation "Cert:\CurrentUser\My"