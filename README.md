# How to run 
1. clone the repo 
2. go to the solution directory and use the following command to create an image 

`docker build -f TradeArt.Api\Dockerfile --build-arg PORT=5003:5004 -t trade_art_api .`
3. to run the container use the following command 
	`docker run -d -p 5006:5004 --name trade_art_api_container_1 trade_art_api`

4. browse endpoints using the following address.
http://localhost:5006/swagger/index.html
