# NerdBot-Core

This is a GroupMe bot that lets people in chat query Magic: The Gathering card information through the ScryFall API.

This is written using ASP.NET Core 2.2 using NancyFX for the routing. This is basicaloly a copy of my other project [NerdBot](https://github.com/jpann/NerdBot), but I trimmed down the plugins/commands a bit and removed the dependency on a MongoDB database containing card and set information.

### Usage

#### Steps

1. Go to [GroupMe's bot site](https://dev.groupme.com/bots) and create a new bot and assign it to your GroupMe group.
2.  Make note of the 'Bot ID' for the bot you created and provide use it in the Docker environment variable 'BOTID' when creating the docker container below.
3. Create a [Bitly account](https://dev.bitly.com/get_started.html) and get the username and key to provide to the Docker environment variables 'BITLY_USER' and 'BITLY_KEY' when creating the docker container.
4. Create the container using the instructions below.
4. Set your GroupMe bot's 'Callback URL' to the external IP address for the host running the container followed by '/bot/<TOKEN>'. For example, https://myhost.com/bot/TOKEN

#### Running with docker

Clone the repository:
```
    $ git clone https://github.com/jpann/nerdbot-core.git
```

Create docker image from Dockerfile:
```
    $ cd ~/nerdbot-core
    $ sudo docker build -t nerdbot-core:1.0.0 .
```

Run it:
```
    $ sudo docker run -d -p 5000:3579 --name nerdbot \
	-v /path/to/logs:/app/logs \
	-e TOKEN="<BOT TOKEN>" \
	-e BOTID="<GROUP ME BOT ID" \
	-e BOTNAME="<GROUP ME BOT NAME> \
	-e BITLY_USER=<YOUR BITLEY USER> \
	-e BITLY_KEY=<YOUR BITLEY KEY> \
	nerdbot-core:1.0.0
```




