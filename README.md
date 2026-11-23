# Pony-Client

The client for an online multiplayer fashion game, written from scratch in C# without a game engine. It connects to [Pony-Server](https://github.com/Bread-Buddies-Studio/Pony-Server) over UDP, using my own networking library, [Pony-Network](https://github.com/Bread-Buddies-Studio/Pony-Network).

## Status

In development.

- **Working:** logging in, walking around, dressing up, chat, and syncing player positions across players
- **In progress:** syncing clothing across players

## Features

- Log in to a server and see other players in the same world
- Walk around and see other players' positions update in real time
- Dress up your character (only works by adding the clothing to local player in the code)
- Chat with other players

## Tech
- C# on .NET 10
- Avalonia UI
- UDP networking, using [Pony-Network](https://github.com/Bread-Buddies-Studio/Pony-Network)
- Built in Visual Studio, with no game engine

## Project structure

- `Core/`: Handles basic utility classes that will be used everywhere.
- `Network/`: Handles remote connections between clients and servers.
- `Rendering/`: Handles drawing to the screen.
- `Audio/`: Handles audio (Currently Supported but Unused)

## Getting started

**Requirements**

- Visual Studio 2026 with the .NET 10 SDK
- A running [Pony-Server](https://github.com/Bread-Buddies-Studio/Pony-Server)

**Run the client**

1. Clone this repository.
2. Open `Pony-Client.sln` in Visual Studio.
4. Press **Run** (F5).

## Related repositories

- [Pony-Server](https://github.com/Bread-Buddies-Studio/Pony-Server): the game server
- [Pony-Network](https://github.com/Bread-Buddies-Studio/Pony-Network): the UDP networking library both use

## Notes

This is an unofficial, non-commercial fan project and is not affiliated with or endorsed by Hasbro.

## Author

Nicholas Ranin ([kersplash-wav](https://github.com/kersplash-wav))
