Google Calendar API Backend

This backend application interacts with the Google Calendar API to retrieve 
events based on specified parameters.

1-Clone this repository to your local machine.
https://github.com/Aamr-mohamed/google-calendar-api.git

2-Navigate to the project directory.
cd google-calendar-api

3-Install required packages.
dotnet build

4-Create a project in the Google Developer Console.
https://developers.google.com/workspace/guides/create-project

5-Enable the Google Calendar API for your project.

Go to this link https://support.google.com/googleapi/answer/6158841?hl=en
provides guide on how to enable the google calendar api.

6-Configure OAuth 2.0 credentials, including the necessary redirect URIs

7-Set up Google Calendar API credentials.
Follow the steps in the Google Calendar API documentation to create a project
and get the client id and client secret and add them in the "appsettings.json"
under the name ClientId , ClientSecret.

Usage
1-Run the application.
use => dotnet watch --launch-profile https

To monitor the project for changes and automatically restart the application whenever changes are detected.
The --launch-profile https flag specifies the launch profile to use, and in this case, it specifies that the application should be launched using HTTPS.

2-Open your web browser and navigate to http://localhost:7100.
And click on the Auth button to redirect
The application will guide you through the authentication process. Follow the on-screen instructions to authorize the application to access your Google Calendar.

3-Once authenticated, you can use the API endpoints to retrieve calendar events. The API provides the following endpoints:

API Endpoints

GET /api/events : Retrieve all events, or get by name or time and add ur token u got from the authentication aswell to the query.
POST /api/events: To add an event given the body(summary,description,startime,endtime,token) calenderId and eventId is set by default.
DELETE /api/events: To delete a certain event given then the body id (which is eventId) and the refresh token u got from the authentication.

Contributing
If you'd like to contribute to this project, please follow the standard GitHub flow:

Fork the repository.
Create a branch: git checkout -b feature/my-feature.
Commit your changes: git commit -m 'Add some feature'.
Push to the branch: git push origin feature/my-feature.
Submit a pull request.