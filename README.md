# C#-Local-Chat-Server

Este es un proyecto final para la clase de Programacion I.

## ABOUT:
El proyecto consiste en una aplicación de consola que pueda host localmente un servidor que reciba conexiones y mensajes. Estos mensajes son, transmitidos a todas sus demás conexiones.

## How To Run:
### Before Running:
Es recomendado tener múltiples terminales abiertas. Ya que, este requiere una para correr el servidor y las demás para las conexiones al servidor.

### Command:
Para correr el programa en la terminal:
Run `dotnet run --project 'ChatApplication\ChatApplication.csproj'`

El Programa comienza con esta pregunta:
![Starting Question](https://raw.githubusercontent.com/Javijarp/C-Local-Chat-Server/main/Pictures/Starting%20Question.png)<br /> <br />
**Host**: Indica que el server comenzara en [LocalHost:6000](http://LocalHost:6000) (127.0.0.1:6000) <br /> <br />
![Host](https://raw.githubusercontent.com/Javijarp/C-Local-Chat-Server/main/Pictures/Host.png) <br /> <br />

**Join**: Revisa si hay un servidor abierto en [LocalHost:6000](http://LocalHost:6000) (127.0.0.1:6000) y si lo hay, se une.<br /> <br />
![Client Connection](https://raw.githubusercontent.com/Javijarp/C-Local-Chat-Server/main/Pictures/Client%20Connection.png) <br /> <br />

Se pueden unir tantos clientes como quieran y pueden tener el mismo nombre.

## TextFile Generation

Al comenzar un sevidor, se comenzara a generar un .txt file con la fecha y hora en la que comenzo el server.
![Creating Text File](https://raw.githubusercontent.com/Javijarp/C-Local-Chat-Server/main/Pictures/Creating%20Text%20File.png)<br /> <br />

Los Message Logs se encuentran dentro del proyecto en un folder llamados Logs. <br />
![File Created](https://raw.githubusercontent.com/Javijarp/C-Local-Chat-Server/main/Pictures/File%20Created.png)<br /> <br />
Aqui estaran todos los mensajes de esa session.
