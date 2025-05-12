
# Carton Caps Referral API

This API is meant to support a new referral feature for the Carton Caps app. It contains endpoints to support viewing referral code and active/in progress referrals, custom launch data based off login status and referral code presence, and an activation code feature meant to mitigate feature abuse.

## How to open

![Screenshot 2025-05-12 181729](https://github.com/user-attachments/assets/b5ca3860-173d-4c1c-b68b-faa99e6fe2d7)

- Download using any of the methods on the GitHub page
- In Visual Studio
    - Navigate to File -> Open -> Projects/Solutions
    - Open CartonCapsAPI.sln in file location you downloaded the project

## Usage

- Launch local application directly through Visual Studio

- Use the Swagger interface or your desired testing to suite to test the API endpoints at:
    
    
        http://localhost:5037/api/{Controller}/{Action}
        (will redirect to https version if running as https)
    
    (The API has been provided testing mock services to provide the kind of data the application can expect to receive for integration testing purposes)

## Test

### Integration testing

The projects Swagger UI documents the API's specs, alongside its possible responses

![Screenshot 2025-05-12 185702](https://github.com/user-attachments/assets/e8aa5ec7-4571-4d7a-a6dd-eaa187d57127)

![Screenshot 2025-05-12 185734](https://github.com/user-attachments/assets/2ae860c0-991c-4ee9-88ac-49457325932c)


The following contains some specs, alongside values handled in the mock services

- Account Controller
    - `GetApplicationLaunchData/{isLoggedIn<bool>}?referralCode={referralCode}` - [GET]
        - `true` or `false` for `isLoggedIn` parameter
        - For the `referralCode` query parameter, leave it empty for no referral code, string of length 6 for valid referral code, any other length for invalid referral code

- Referral Controller
    - `GetReferralInformation/{userId<int>}` - [GET]
        - Following are populated user id's, any other number will return NotFound Status code
            - 1 - User with active account and 2 users referred
            - 2 - User with active account and no users referred
            - 3 - User with inactive account
            - 4 - User with inactive account

- User Controller
    - `SendSmsConfirmation?userId={userId<int>}` - [POST]
        - Following are populated user id's, any other number will return NotFound Status code
            - 1 - User with already active account
            - 2 - User with already active account
            - 3 - User with account pending activation
            - 4 - User with account pending activation
    - `ActivateAccount?userId={userId<int>}&activationToken={activationToken<string>}` - [POST]
        - Following are populated user id's and their given activation token (if applicable), any other integer will return NotFound Status code
            - 1 - User with already active account
            - 2 - User with already active account
            - 3 - User with account pending activation
                - NULL activation token
            - 4 - User with account pending activation
                - "abc" activation token

### Unit testing

The solution contains tests for the 3 controllers that are expected to test every path and return type of the API controllers.

How to run:

- Open Test Explorer

![Screenshot 2025-05-12 190246](https://github.com/user-attachments/assets/0617e744-3a62-4ddb-ba12-7831300d2f61)

- Run tests as desired

![Screenshot 2025-05-12 190403](https://github.com/user-attachments/assets/86cabd28-bb65-4dce-a65c-d2f50d3f8c94)

## LiveFront notes

#### My expectations are that, with the addition of the new referral feature, that:

- Referral codes likely won't be generated for existing users, so I have the endpoint check for and generate referral code, if necessary.

- We would like some way to mitigate abuse. I don't think it's perfect, but requiring a phone number activation makes it more difficult for the layman to create infinite referrals for themself.

- Phone numbers would need to be added to existing accounts to unlock the referral feature, so there would need to be a route to updating their phone number.

- We would need some sort of unique validation for phone numbers, I thought about creating one for this demo, but it gets a little interesting when considering countries with different presentation on phone numbers, and whether country codes are included or not.

#### My concerns are:

I ran into a roadblock with the considerations at the end of the project instructions. Concerning the deferred deep linking, the solutions I researched to handle the deferred deep linking seemed to be handled client-side in the mobile app.

From my understanding the mobile app would install the sdk and from there you would use the 3rd party service to handle the link generation and handling.

This is the 3rd party service I looked into for deferred deep links: https://help.branch.io/developers-hub/docs/apis-overview


## Conclusion

Thank you for the opportunity to participate in this coding challenge. I hope that my submission has included all expected requirements. Please let me know if there is anything I am missing, or feel free to respond with any feedback; I would love the opportunity to learn something new!
