# BoxTokenGenerator
Box token generator is a simple C# WPF application that leverages BoxV2 windows SDK to generate Auth tokens.

## BeforeYouCompile
You need to add two files under __Bin/Release__  or __Bin/Debug__ folder
1. config.xml
2. tokens.xml

## config.xml
In this file you will put ClientID, ClientSecret and RedirectURI of your application that is registered in Box

```
<?xml version="1.0" encoding="utf-8"?>
<Config>
  <ClientID>{your client id}</ClientID>
  <ClientSecret>{your client secret}</ClientSecret>
  <RedirectURI>{your redirect URI}</RedirectURI>
</Config>
```

## tokens.xml
In this file new tokens will be saved

```
<?xml version="1.0" encoding="utf-8"?>
<Tokens>
  <AccessToken>{accessToken}</AccessToken>
  <RefreshToken>{refreshToken}en}</RefreshToken>
  <ExpiresIn>3600</ExpiresIn>
</Tokens>
```

# Dependencies
You also need to install dependencies for BoxV2 SDK such as NewtonsoftJson, Nito.AsyncEx. 

Once these files are set and reference installed, you can run the application.