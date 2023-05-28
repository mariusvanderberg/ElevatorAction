# Elevator Action - A Console application built with Clean architecture

![example workflow](https://github.com/github/docs/actions/workflows/main.yml/badge.svg)

Elevator action simulates the movement of elevators and hides the complexity by utlizing the power of services that encapsulates the inner workings, so you can worry about maintaining your domain.

Need to swap out for a new elevator service? Sure, it's built modular for a reason. Need to use a different scheduling mechanism? Sure, let's replace the queueing system with Service Bus, if you wanted to.

Want to persist the configuration? There's an empty spot in infrastructure with your name on it, so you can decide if you want to go BIG(-data) fo multi-config or keep it simple in a mySQL database.

## Features

This project features the following capabilities.

- clean architecture, with no persistance layer, yet.
- multiple floors
- multiple elevators
- queueing to effectively and efficiently manage elevator requests
- a simulator aiding the console app with starting the elevator action. Can also be used with a different interface such as an api

## Optional Features
- Background service to process queues: Should be used only with multi-threaded presentation, such as an api

## Gegenral notes

The application is limited to a single input and process at a time, since a console application cannot handle multiple inputs at the same time.
This has lead to using a single while loop to process queues, while disabling the backround service to avoid unecessary problems, such as random message and input requests.
The emergency stop function is hotwired to work, but the best would be to utilize it with an api to properly manage cancellation tokens.

## Getting Started

Make sure ElevatorAction.ConsoleUI is your startup project (when using Visual Studio or VS Code) and run. The rest will be configured in the application.
Note that this could easily be read from config, which would be better for a production environment.
