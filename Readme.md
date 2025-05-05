# CartonCaps

Carton Caps is a .Net 9 web API application that empowers consumers to raise money for the schools they care about, while buying the everyday products they love.

## Project Structure

```
├──CartonCaps
│   └── Controller
│       └── ReferralController.cs
│       └── SharedLinkController.cs
│   └── Data
│       └── FakeDataProvider.cs
│   └── Dtos
│       └── CartonCapsDto.cs
│   └── Enums
│       └── OsPlatform.cs
│       └── ReferralStatus.cs
│       └── SharingMedium.cs
│   └── Extionsions
│       └── ServiceRegistration.cs
│       └── UserExtionsion.cs
│   └── IData
│       └── IDataProvider.cs
├──CartonCaps.Test
│   └── Controllers
│       └── ReferralControllerTest.cs
│       └── SharedLinkControllerTest.cs
│    └── Services
│       └── ReferralServiceTest.cs
│       └── SharedLinkServiceTest.cs

```

## Setting Up

To setup this project, clone the git repo

```sh
$ git clone https://github.com/Shagragada/CartonCaps.git
$ cd CartonCaps
```

followed by

```sh
$ dotnet restore
```

To run the API project, go into the project

```sh
$ cd CartonCaps
```

and run the project

```sh
$ dotnet run
```

The application will be available on `http://localhost:5156`.
Go to `http://localhost:5156/swagger/index.html` to view swagger documentation

To run the test, go into the test project

```sh
$ cd CartonCaps.Test
```

and run the tests

```sh
$ dotnet test
```

To run the API project in docker

build the docker image

```sh
$ docker build -t cartoncaps-api -f CartonCaps/Dockerfile .
```

Run the container. You may expose a different port other than `5157`

```sh
docker run -p 5157:8080 cartoncaps-api
```

View swagger documentation at
`http://localhost:5157/swagger/index.html`
