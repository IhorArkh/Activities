# Activities
Web API .NET Core app with Clean Architecture using the CQRS + Mediator pattern

## Stack
- .NET, C#, Entity Framework, SignalR, Postgres
- ASP.NET Identity, AutoMapper, MediatR, FluentValidation

## Used 3rd party APIs
- Facebook login
- Cloudinary

## Use cases
- Registration and login
  - New users have the ability to register using email and username, or via Facebook.
  - Registered users have the ability to log in using their credentials or via Facebook.
  - Logged users can sign out.
  
- User profile
  - Registered users have a profile with the ability to make changes to their profile.
  - The profile includes a photo, bio, display name, followers, following, and attended activities.

- View activities
  - Users can view all existing activities and details about attendees' profiles on the home page.

- Activity details
  - Every activity has a details page with an attendees list, title, date, description, city, venue.

- Filtering activities
  - Users can filter activities by date.
  - Users can filter activities by filters "All activities," "I am going," "I am hosting."

- Joining and canceling participation in activity
  - Logged users have the ability to attend any activity.
  - Users can cancel their participation at any time.

- Managing activities
  - Registered users have the ability to create new activities.
  - Hosts have the ability to edit or cancel activities.

- Following feature
  - Registered users can follow other users.

- Chat about event (comments)
  - Registered users can chat about activities.
