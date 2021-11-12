# GithubApiOmada
Omada training
This a training for Omada. Just por learning purposes.

# Configuration
You just need to add appsettings connection string or user secrets. Should be configured on appsettings in integration test project as well.
```json
  "ConnectionStrings": {
    "Default": "YOUR_CONNECTION_STRING"
  }
```

It also needs any github pesonal token that you must generate in your private area. This token will be sent to this api endpoints.

This project was made using Vertical Slice Architecture which is used also on Microsoft guidelines and eShopOnWeb demo project. You can read further on [Duck Duck Go](https://jimmybogard.com/vertical-slice-architecture)

